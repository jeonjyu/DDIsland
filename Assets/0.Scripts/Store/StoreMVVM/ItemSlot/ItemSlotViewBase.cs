using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotViewBase : MonoBehaviour, IStoreItemView, IPointerClickHandler
{
//field
    protected ItemSlotViewModelBase viewModel;
    protected IStoreItem modelData;
    protected Image slotImage;

    protected Color gainedColor = new Color(0.93f, 0.87f, 0.75f, 1f);
    protected Color ungainedColor = new Color(0.99f, 0.97f, 0.91f, 1f);

    [SerializeField] protected Image _slotBackground;
    [SerializeField] protected Image _itemImage;


    protected EventTrigger eventTrigger;


    // property
    public ItemSlotViewModelBase ViewModel => viewModel;
    public IStoreItem ModelData => modelData;
    public Image SlotBackground => _slotBackground;
    public Image ItemImage => _itemImage;

    protected virtual void Awake()
    {
        viewModel = GetComponent<ItemSlotViewModelBase>();
        eventTrigger = GetComponent<EventTrigger>();
        Init();
    }

    void OnEnable()
    {
        if (viewModel != null)
            viewModel.PropertyChanged += OnViewModelPropChanged;

    }

    void OnDisable()
    {
        viewModel.PropertyChanged -= OnViewModelPropChanged;
    }

    // 뷰 초기화 및 업데이트 메서드
    public virtual void Init()
    {
        //Debug.Log("[ItemSlotViewBase] Init");
        modelData = viewModel.Model;
        int itemID = viewModel.ItemId;

        if (modelData is null)
        {
            //Debug.Log("model이 없음");
            ResetSlot();
            return;
        }

        UpdateSlotColor(modelData.IsGained);
        _itemImage.sprite = modelData.ImgSprite;
    }

    public virtual void ResetSlot()
    {
        UpdateSlotColor(false);
    }

    public void UpdateSlotColor(bool isGained)
    {
        _slotBackground.color = isGained ? gainedColor : ungainedColor;
    }

    // 버튼 팝업 띄우고
    // 해당 뷰의 모델을 전달하는 메서드 
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("슬롯 클릭됨");
        viewModel.SetPopupModel();
        StoreManager.Instance.TradeItemSlot = this.viewModel;
        if (StoreManager.Instance.BuyAndSellPanel != null)
            StoreManager.Instance.BuyAndSellPanel.SetActive(true);
        else
            Debug.LogError("패널이없어요");
        StoreManager.Instance.ChangeDropdownAvailability(false);
    }

    public virtual void UpdateSlotUI(int count){}

    private void OnViewModelPropChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case null:
            case "":
                Init();
                break;
            case "IsGained":
                UpdateSlotUI(modelData.ItemCount);
                UpdateSlotColor(modelData.IsGained);
                StoreManager.Instance.sortDropdown.ApplySortPriority();
                StoreManager.Instance.StoreListVM.LoadSlotList();
                break;
            case "ItemCount":
                UpdateSlotUI(modelData.ItemCount);
                StoreManager.Instance.sortDropdown.ApplySortPriority();
                StoreManager.Instance.StoreListVM.LoadSlotList();
                break;
        }
    }
}
