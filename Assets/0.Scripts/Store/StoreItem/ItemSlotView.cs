using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotView : MonoBehaviour, IStoreItemView
{
    ItemSlotViewModel viewModel;
    StoreItem modelData;


    // 인테리어 타입에 따라 다른 UI 테두리 표시
    [SerializeField] private Image _slotBackground;
    [SerializeField] private TMP_Text _itemPrice;
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _itemCount;
    [SerializeField] private Image _itemImage;

    EventTrigger eventTrigger;

    public Image SlotBackground => _slotBackground;
    public TMP_Text ItemPrice => _itemPrice;
    public TMP_Text ItemName => _itemName;
    public TMP_Text ItemCount => _itemCount;
    public Image ItemImage => _itemImage;


    void Start()
    {
        Debug.Log("[ItemSlotView] Start");
        viewModel = GetComponent<ItemSlotViewModel>();
        //modelData = viewModel.Model;
        eventTrigger = gameObject.GetComponent<EventTrigger>();
        //eventTrigger.OnPointerClick(() => { }); 
        // 버튼 팝업 띄우고
        // 해당 뷰의 모델을 전달하는 메서드 

        viewModel.PropertyChanged += OnViewModelPropChanged;
        Init();
    }

    void OnEnable()
    {
        if (viewModel != null)
            viewModel.PropertyChanged += OnViewModelPropChanged;
    }

    void OnDisable()
    {
        //viewModel.PropertyChanged -= OnViewModelPropChanged;
    }

    public void Init()
    {
        //Debug.Log("[ItemSlotView] Init");
        modelData = viewModel.Model;
        int itemID = viewModel.ItemId;

        if (!modelData)
        {
            Debug.Log("mode이 없음");
            return;
        }
        UpdateSlotColor(modelData.IsGained);
        _itemName.text = modelData.ItemName;
        _itemPrice.text = modelData.PurchasePrice.ToString();
        _itemCount.text = modelData.ItemCount.ToString();
        //_itemImage.sprite = ;
    }

    public void UpdateItemCount(int count)
    {
        _itemCount.text = count.ToString();
    }

    public void UpdateSlotColor(bool isGained)
    {
        if (!isGained)
            _slotBackground.color = Color.grey;
        else
            _slotBackground.color = Color.white;
    }

    private void OnViewModelPropChanged(object sender, PropertyChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.PropertyName))
        {
            //Debug.Log("[ItemSlotView] OnViewModelPropChanged | 모델 변경");

            Init();
        }
        switch (e.PropertyName)
        {
            case "IsGained":
            case "ItemCount":
                UpdateItemCount(modelData.ItemCount);
                UpdateSlotColor(modelData.IsGained);
                break;
        }
    }
}
