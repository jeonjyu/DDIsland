using UnityEngine;

public class StoreItemPresenter : MonoBehaviour
{
    private DummyStoreItemSO _model;
    private IStoreItemView _view;

    public DummyStoreItemSO Model => _model;

    void Awake()
    {
        _view = gameObject.GetComponent<IStoreItemView>();
    }

    public void SetModel(DummyStoreItemSO model) 
    {
        //_model = model.GetStoreItem();
        _model = model;
        //_view.Init(_model.PurchasePrice, _model.ItemName, _model.ItemCount, _model.IsGained, _model.ItemImage);
    }
}