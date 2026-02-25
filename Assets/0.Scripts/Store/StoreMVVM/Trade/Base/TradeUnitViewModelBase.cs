using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class TradeUnitViewModelBase : MonoBehaviour, INotifyPropertyChanged, ITradeStategy
{
    [SerializeField] private StoreItem model;

    public StoreItem Model
    {
        get => model;
        set 
        { 
            model = value; 
            OnPropertyChanged(null); 
        }
    }

    int tradeCount;
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

    int totalPrice;

    public int TotalPrice
    {
        get => totalPrice;
        set
        {
            if (totalPrice != value)
            {
                totalPrice = value;
                OnPropertyChanged(nameof(totalPrice));
            }
        }
    }


    public abstract void Trade(int tradeCount);

    // 판매/구매 금액 불러오는 메서드
    public abstract int GetPrice();

    // 아이템 갯수 변경
    // max를 넘거나 0 이하로 못가게 해야 한다
    public void ChangeCount(int tradeCount)
    {
        if (TradeCount > 0 && TradeCount < model.MaxCount)
        {
            TradeCount += tradeCount;
        }
    }

    // 구매 후 보유 여부가 변경되어야 할 경우 변경
    public virtual void ChangeIsGained(bool isGained)
    {
        Model.IsGained = isGained;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
