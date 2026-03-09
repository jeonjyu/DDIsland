using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemSlotViewModel : MonoBehaviour, INotifyPropertyChanged
{

    // 해당 속성이 변경되면 뷰가 수정되어야 한다
    // 모델, 아이템 아이디
    // 아이템 개수
    // 획득 여부


    [SerializeField] private IStoreItem _model;

    public IStoreItem Model
    {
        get => _model;
        set
        {
            _model = value;
            OnPropertyChanged(null);
        }
    }

    private int _itemId;

    public int ItemId
    {
        get => _itemId;
        set
        {
            if (_itemId != value || _itemId == 99999999)
            {
                _itemId = value;
                OnPropertyChanged(nameof(_model.ID));
            }
        }
    }

    public int ItemCount
    {
        get => _model.ItemCount;
        set
        {
            _model.ItemCount = value;
            OnPropertyChanged(nameof(_model.ItemCount));
        }
    }

    public bool IsGained
    {
        get => _model.IsGained;
        set
        {
            if (_model.IsGained != value)
            {
                _model.IsGained = value;
                OnPropertyChanged(nameof(_model.IsGained));
            }
        }
    }

    private string _itemName;

    public string ItemName
    {
        get => _model.ItemName;
        set
        {
            if (_model.ItemName != value)
            {
                OnPropertyChanged(nameof(_model.ItemName));
            }
        }
    }

    public void SetModel(IStoreItem model)
    {
        Model = model;
        ItemId = model.ID;
    }

    public void Reset()
    {
        Model = null;
        ItemId = -1;
    }


    public void SetPopupModel()
    {
        StoreManager.Instance.TradeModel = Model;
        //StoreManager.Instance.currentTradeItem = this;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        //if(PropertyChanged != null) 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
