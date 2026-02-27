using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemSlotViewModel : MonoBehaviour, INotifyPropertyChanged
{

    // 변경될 속성들
    // 아이템 갯수
    // 획득 여부

    [SerializeField] private StoreItem _model;

    public StoreItem Model
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
                OnPropertyChanged(nameof(_model.ItemId));
            }
        }
    }

    private int _itemCount;

    public int ItemCount
    {
        get => _model.ItemCount;
        set
        {
            _model.ItemCount = value;
            OnPropertyChanged(nameof(_model.ItemCount));
        }
    }

    private bool _isGained;

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

    public void SetModel(StoreItem model)
    {
        Model = model;
        ItemId = model.ItemId;
    }

    public void Reset()
    {
        Model = null;
        ItemId = -1;
    }


    public void SetPopupModel()
    {
        TradeManager.Instance.model = Model;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        //if(PropertyChanged != null) 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
