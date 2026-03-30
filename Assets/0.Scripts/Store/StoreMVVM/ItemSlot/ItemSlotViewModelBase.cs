using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemSlotViewModelBase : MonoBehaviour, INotifyPropertyChanged
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
            if (_model == null) Debug.Log("_model이 null");
            //Debug.Log(value);
            if (_model.IsGained != value)
            {
                _model.IsGained = value;
                Debug.Log(this.name + " 아이템 보유 여부 변경 " + _model.IsGained);
                OnPropertyChanged(nameof(_model.IsGained));
            }
        }
    }

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

    public virtual void SetModel(IStoreItem model)
    {
        //Debug.Log(gameObject.name + " ItemSlotViewModelBase : SetModel " + model.ItemName);
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
        //Debug.Log("ItemSlotViewModelBase | SetPopupModel" + Model);
        StoreManager.Instance.TradeModel = Model;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
