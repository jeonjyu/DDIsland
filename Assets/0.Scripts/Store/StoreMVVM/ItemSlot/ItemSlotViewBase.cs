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


    [SerializeField] protected Image _slotBackground;
    //[SerializeField] private TMP_Text _itemPrice;
    //[SerializeField] private TMP_Text _itemName;
    //[SerializeField] private TMP_Text _itemCount;
    [SerializeField] protected Image _itemImage;

    protected EventTrigger eventTrigger;

    // property
    public ItemSlotViewModelBase ViewModel => viewModel;
    public IStoreItem ModelData => modelData;
    public Image SlotBackground => _slotBackground;
    //public TMP_Text ItemPrice => _itemPrice;
    //public TMP_Text ItemName => _itemName;
    //public TMP_Text ItemCount => _itemCount;
    public Image ItemImage => _itemImage;

    void Awake()
    {
        viewModel = GetComponent<ItemSlotViewModelBase>();
    }
    void Start()
    {
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
        //Debug.Log("[ItemSlotView] Init");
        modelData = viewModel.Model;
        int itemID = viewModel.ItemId;

        if (modelData is null)
        {
            //Debug.Log("model이 없음");
            ResetSlot();
            return;
        }
        UpdateSlotColor(modelData.IsGained);
        //_itemName.text = modelData.ItemName;
        //_itemPrice.text = modelData.PurchasePrice.ToString();
        //_itemCount.text = modelData.ItemCount.ToString();
        _itemImage.sprite = modelData.ImgSprite;
    }

    public virtual void ResetSlot()
    {
        UpdateSlotColor(false);
        //_itemName.text = "";
        //_itemPrice.text = "";
        //_itemCount.text = "0";
    }
    //public void UpdateItemCount(int count)
    //{
    //    _itemCount.text = count.ToString();
    //}

    public void UpdateSlotColor(bool isGained)
    {
        if (!isGained)
            _slotBackground.color = Color.grey;
        else
            _slotBackground.color = Color.white;
    }

    // 버튼 팝업 띄우고
    // 해당 뷰의 모델을 전달하는 메서드 
    public void OnPointerClick(PointerEventData eventData)
    {
        // 구매보다 카테고리 변경을 더 자주하니까 구매창 킬 때 카테고리에 따라 다른 구매창 열도록 설정
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
        if (string.IsNullOrEmpty(e.PropertyName))
        {
            //Debug.Log("[ItemSlotView] OnViewModelPropChanged | 모델 변경");
            Init();
            return;
        }
        //else
        //    Debug.Log(this.name + " 변경됨 " + e.PropertyName + " " + sender);
        
        switch (e.PropertyName)
        {
            //case null:
            //case "":
            //    Init();
            //    break;
            case "IsGained":
            case "ItemCount":
                UpdateSlotUI(modelData.ItemCount);
                UpdateSlotColor(modelData.IsGained);
                StoreManager.Instance.sortDropdown.ApplySortPriority();
                StoreManager.Instance.StoreListVM.LoadSlotList();
                break;
        }
    }
}
