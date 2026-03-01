using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

//[RequireComponent(typeof(TradeViewBase))]
public class TradeUnitViewModelBase : MonoBehaviour, INotifyPropertyChanged
{
    [SerializeField] private StoreItem model;
    protected TradeUnitViewBase view;

    [SerializeField] protected GameObject TradeConfirmPanel;
    [SerializeField] GameObject GoldWarningPanel;


    public ITradeStrategy purchaseStrategy;
    public ITradeStrategy sellStrategy;


    public StoreItem Model
    {
        get => StoreManager.Instance.tradeModel;
        set 
        {
            StoreManager.Instance.tradeModel = value; 
            OnPropertyChanged(null); 
        }
    }

    protected int tradeCount = 1;
    public int TradeCount
    {
        get => tradeCount;
        set
        {
            if (tradeCount != value) 
            { 
                tradeCount = value; 
                OnPropertyChanged(nameof(TradeCount)); 
            }
        }
    }

    //protected int totalPrice;

    //public int TotalPrice
    //{
    //    get => totalPrice;
    //    set
    //    {
    //        if (totalPrice != value)
    //        {
    //            totalPrice = value;
    //            OnPropertyChanged(nameof(totalPrice));
    //        }
    //    }
    //}

    void Awake()
    {
        //Debug.Log("[TradeUnitViewModelBase] Awake");
        view = GetComponent<TradeUnitViewBase>();
        purchaseStrategy = GetComponent<PurchaseStrategy>();
        sellStrategy = GetComponent<SellStrategy>();
    }

    private void OnEnable()
    {
        InitUnit();
    }

    // 팝업 껐다 켰다, 거래완료 팝업 끝나고 호출
    public void InitUnit()
    {
        TradeCount = 1;
        view.SetItemCount(1);
    }

    public void ExcuteTrade(ITradeStrategy tradeStrategy)
    {
        if (tradeStrategy.Trade(TradeCount, Model))
        {
            TradeConfirmPanel.SetActive(true);
        }
        else
        {
            GoldWarningPanel.SetActive(false);
        }
        // todo: 구매 완료/골드 부족 팝업 닫기 이후 실행하도록 이동
        InitUnit(); 
    }

    // 아이템 갯수 변경
    // max를 넘거나 0 이하로 못가게 해야 한다
    public void ChangeCount(int tradeCount)
    {
        if (tradeCount > 0 && tradeCount < view.GetTradeStrategy().GetMaxCount(Model))
        {
            TradeCount += tradeCount;
            return;
        }
        else if (TradeCount < 1)
        {
            Debug.LogWarning("[TradeUnitViewModelBase] 아이템 최소 범위 미만");
            TradeCount = 1;
            return;
        }
        else // 최대를 넘어가는 경우
        {
            Debug.LogWarning("[TradeUnitViewModelBase] 아이템 최대 범위 초과");
            TradeCount = view.GetTradeStrategy().GetMaxCount(Model);
            //TradeCount = tradeCount;
            return;
        }
    }

    public void SetTotalPrice()
    {
        view.SetTotalPriceText(TradeCount * view.GetTradeStrategy().GetPrice(Model));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
