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
                OnPropertyChanged(null); 
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

    void Start()
    {
        Debug.Log("[TradeUnitViewModelBase] Start");
        view = GetComponent<TradeUnitViewBase>();
        purchaseStrategy = GetComponent<PurchaseStrategy>();
        sellStrategy = GetComponent<SellStrategy>();
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
    }

    // 아이템 갯수 변경
    // max를 넘거나 0 이하로 못가게 해야 한다
    public void ChangeCount(int tradeCount)
    {
        if (TradeCount > 0 && TradeCount < view.GetTradeStrategy().GetMaxCount(Model))
        {
            TradeCount += tradeCount;
            return;
        }
        else
        {
            Debug.LogWarning("[TradeUnitViewModelBase] 아이템 갯수가 범위를 벗어남");
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
