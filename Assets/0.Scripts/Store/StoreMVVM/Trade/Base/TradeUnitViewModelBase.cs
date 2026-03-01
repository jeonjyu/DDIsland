using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

using System;

//[RequireComponent(typeof(TradeViewBase))]
public class TradeUnitViewModelBase : MonoBehaviour, INotifyPropertyChanged
{
    protected TradeUnitViewBase view;

    [SerializeField] protected GameObject TradeConfirmPanel;
    [SerializeField] GameObject GoldWarningPanel;


    public ITradeStrategy purchaseStrategy;
    public ITradeStrategy sellStrategy;

    private StoreItem model;

    public StoreItem Model
    {
        //get => StoreManager.Instance.tradeModel;
        get => model;
        set 
        {
            model = value;
            //StoreManager.Instance.tradeModel = model; 
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
        Model = StoreManager.Instance.tradeModel;
        TradeCount = 1;
        view.SetItemCount(1);
    }

    public void ExcuteTrade(ITradeStrategy tradeStrategy)
    {
        Debug.Log(tradeStrategy.ToString());

        if (tradeStrategy.Trade(TradeCount, Model))
        {
            TradeConfirmPanel.SetActive(true);
        }
        else
        {
            GoldWarningPanel.SetActive(false);
        }
        InitUnit(); 
    }

    // 아이템 갯수 변경
    // max를 넘거나 0 이하로 못가게 해야 한다
    public void IncreaseCount()
    {
        if(TradeCount >= view.GetTradeStrategy().GetMaxCount(Model))
        {
            Debug.LogWarning("[TradeUnitViewModelBase] 아이템 최대 범위 초과");
            return;
        }
        else
        {
            TradeCount++;
            return;
        }
    }

    public void DecreaseCount()
    {
        // 0보다 낮을 때
        if(TradeCount <= 1)
        {
            Debug.LogWarning("[TradeUnitViewModelBase] 아이템 최소 범위 미만");
            TradeCount = 1;
            return;
        }
        else
        {
            TradeCount--;
            return;
        }
    }


    public void SetMaxCount()
    {
        if(view.GetTradeStrategy() is PurchaseStrategy)
        {
            TradeCount = Math.Min(GameManager.Instance.PlayerGold / Model.PurchasePrice, Model.MaxCount - Model.ItemCount);
        }
        else
        {
            TradeCount = Model.ItemCount;
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
