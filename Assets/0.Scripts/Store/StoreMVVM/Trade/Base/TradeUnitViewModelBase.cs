using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

//[RequireComponent(typeof(TradeViewBase))]
public class TradeUnitViewModelBase : MonoBehaviour, INotifyPropertyChanged
{
    protected TradeUnitViewBase view;
    [SerializeField] private TradeViewModelBase tradeVM;
    [SerializeField] protected GameObject TradeConfirmPanel;
    //[SerializeField] GameObject GoldWarningPanel;


    public ITradeStrategy purchaseStrategy;
    public ITradeStrategy sellStrategy;

    public IStoreItem Model 
    {
        get => StoreManager.Instance.TradeModel;
        private set => StoreManager.Instance.TradeModel = value;
    }

    //public IStoreItem Model
    //{
    //    get => model;
    //    set 
    //    {
    //        if (model != value)
    //        {
    //            model = value;
    //            OnPropertyChanged(null);
    //        }
    //    }
    //}

    protected int _tradeCount = 1;
    public int TradeCount
    {
        get => _tradeCount;
        set
        {
            if (_tradeCount != value) 
            { 
                _tradeCount = value; 
                OnPropertyChanged(nameof(TradeCount));  // UpdateTradeUnitUI
            }
        }
    }

    protected int _totalPrice;
    public int TotalPrice => _totalPrice;

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

    public int ItemCount
    {
        get => StoreManager.Instance.TradeItemCount;
    }

    public bool IsGained
    {
        get => StoreManager.Instance.TradeModel.IsGained;
    }
    public TradeViewModelBase TradeVM => tradeVM; 

    void Awake()
    {
        //Debug.Log("[TradeUnitViewModelBase] Awake");
        view = GetComponent<TradeUnitViewBase>();
        purchaseStrategy = GetComponent<PurchaseStrategy>();
        sellStrategy = GetComponent<SellStrategy>();
        tradeVM = GetComponent<TradeViewModelBase>();

        //TradeConfirmPanel = gameObject.GetComponent<TradePopupBase>().gameObject;

        // StoreManager의 TradeModel,TradeItemCount이 변경되면 갱신되도록 알림 받도록
        StoreManager.Instance.PropertyChanged += UpdateTradeModel;
    }

    void OnEnable()
    {
        //Debug.Log("UI 초기화");
        SetTotalPrice();
        view.SetTradeCountText(TradeCount);
        view.SetButton();
    }

    // Trademodel이 갱신될 때 실행시켜 줄 메서드
    // 거래 아이템이 변경될 때
    // 거래 완료 후 
    public void UpdateTradeModel(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            // 모델 변경
            case null:
            case "":
                _tradeCount = 1;
                InitUnit();
                OnPropertyChanged();
                break;
            // 보유중인 아이템 개수 변경
            case nameof(StoreManager.Instance.TradeItemCount):
                //Debug.Log("UpdateTradeModel |" + nameof(StoreManager.Instance.TradeItemCount));
                _tradeCount = 1;
                InitUnit();
                OnPropertyChanged(nameof(StoreManager.Instance.TradeItemCount));
                break;
        }
        //SetTotalPrice();
        //view.SetTradeCountText(TradeCount);
    }

    //private void OnEnable()
    //{
    //    Debug.Log("모델 설정");
    //    Model = StoreManager.Instance.tradeModel;

    //    //InitUnit();
    //}

    // 팝업 껐다 켰다, 거래완료 팝업 끝나고 호출
    /// <summary>
    /// 각 거래 유닛 초기화
    /// </summary>
    public virtual void InitUnit()
    {
        SetTotalPrice();
        view.SetTradeCountText(TradeCount);
        view.SetButton();
    }

    public void ExcuteTrade(ITradeStrategy tradeStrategy)
    {
        if (tradeStrategy.Trade(TradeCount, TotalPrice))
        {
            StoreManager.Instance.BuyAndSellPanel.gameObject.SetActive(true);
            TradeConfirmPanel.SetActive(true);
            tradeStrategy.PlayTradeSFX();
        }

        _ = DataManager.Instance.Hub.UploadAllData();
    }

    // 아이템 갯수 변경
    // max를 넘거나 0 이하로 못가게 해야 한다
    public void IncreaseCount()
    {
        if(TradeCount >= view.GetTradeStrategy().GetMaxCount(Model))
        {
            //Debug.LogWarning("[TradeUnitViewModelBase] 아이템 최대 범위 초과");
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
        if(TradeCount <= 0)
        {
            //Debug.LogWarning("[TradeUnitViewModelBase] 거래 개수가 0보다 작아질 수 없음");
            //TradeCount = 1;
            return;
        }
        else
        {
            TradeCount--;
            return;
        }
    }

    public virtual void SetMaxCount()
    {
        if(view.GetTradeStrategy() is PurchaseStrategy)
        {
            TradeCount = Math.Min(GameManager.Instance.PlayerGold / Model.PurchasePrice, Model.MaxCount - Model.ItemCount);
        }
        else // 판매할 때
        {
            if (Model.IsGained && !Model.IsDefault)
            {
                // 섬 인테리어가 아닌 경우 
                if (StoreManager.Instance.currentCat != StoreCat.interior)
                {
                    TradeCount = 1;
                }
                TradeCount = Model.ItemCount;
            }
            else
            {
                TradeCount = 0;

            }
        }
    }

    public virtual void SetTotalPrice()
    {
        if (TradeCount <= 0)
        {
            view.SetTotalPriceText(0);
            Debug.Log("SetTotalPrice 거래 가격이 0" );
        }
        else
        {
            _totalPrice = TradeCount * view.GetTradeStrategy().GetPrice(Model);
            view.SetTotalPriceText(_totalPrice);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


}
