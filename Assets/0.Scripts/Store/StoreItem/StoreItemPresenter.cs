using UnityEngine;

public class StoreItemPresenter : MonoBehaviour
{
    private StoreItem _model;
    private IStoreItemView _view;

    public StoreItem Model => _model;

    void Awake()
    {
        _view = gameObject.GetComponent<IStoreItemView>();
    }

    public void Reset()
    {
        _model = null;
        //transform.parent = // 오브젝트풀로 반납, 지금은 임시 오브젝트풀로 반납
    }

    public void SetModel(StoreItem model) 
    {
        //_model = model.GetStoreItem();
        _model = model;
        //_view.Init(_model.PurchasePrice, _model.ItemName, _model.ItemCount, _model.IsGained, _model.ItemImage);
    }
}