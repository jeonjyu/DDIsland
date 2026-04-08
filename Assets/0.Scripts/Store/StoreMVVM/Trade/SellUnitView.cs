using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SellStrategy))]
public class SellUnitView : TradeUnitViewBase
{
    SellStrategy strategy;

    [SerializeField] protected Button countIncBtn;
    [SerializeField] protected Button countDecBtn;
    [SerializeField] protected Button countMaxBtn;
    [SerializeField] protected TMP_Text countTxt;

    public Button CountIncBtn => countIncBtn;
    public Button CountDecBtn => countDecBtn;
    public Button CountMaxBtn => countMaxBtn;

    protected override void Awake()
    {
        base.Awake();
        strategy = GetComponent<SellStrategy>();
        //Debug.Log(this.name);
    }

    protected override void SetEventListener()
    {
        base.SetEventListener();
        countIncBtn.onClick.AddListener(() => viewModel.IncreaseCount());
        countDecBtn.onClick.AddListener(() => viewModel.DecreaseCount());
        countMaxBtn.onClick.AddListener(() => viewModel.SetMaxCount());
    }

    protected override void UnsetEventListener()
    {
        base.UnsetEventListener();
        countIncBtn.onClick.RemoveAllListeners();
        countDecBtn.onClick.RemoveAllListeners();
    }

    public override void SetTradeCountText(int count)
    {
        countTxt.text = count.ToString();
    }

    public override void SetAllButtonAvailablity(bool isAvailable)
    {
        base.SetAllButtonAvailablity(isAvailable);
        countIncBtn.interactable = isAvailable;
        countDecBtn.interactable = isAvailable;
        countMaxBtn.interactable = isAvailable;
    }

    protected override void UpdateUI()
    {
        SetTradeCountText(viewModel.TradeCount);
        base.UpdateUI();
    }

    public override ITradeStrategy GetTradeStrategy()
    {
        if (strategy == null) strategy = GetComponent<SellStrategy>();
        return strategy;
    }

    public override void SetButton()
    {
        base.SetButton();
        //Debug.Log($"판매 유닛 | 유닛 초기화 | 아이템 개수 : {viewModel.ItemCount} ({viewModel.IsGained}) 거래 개수 : {viewModel.TradeCount}");

        // 거래 불가능한 아이템일 경우 판매 유닛 버튼 클릭 불가능
        if (viewModel == null)
        {
            viewModel = GetComponent<TradeUnitViewModelBase>();
        }

        bool isItemCountAvailable = false;
        bool isTradeable = false;
        bool canIncrease = false;
        bool canDecrease = false;

        int unPlacedItemCount = DecoInventoryManager.Instance.GetInvenCount(viewModel.Model.ObjectId);
        
        if(!viewModel.Model.IsSaleable)
        {
            Debug.Log(this + " 판매 유닛 | 판매 불가능");

            SetAllButtonAvailablity(isTradeable);
            return;
        }

        if (!viewModel.IsGained)
        {
            Debug.Log(this + " 판매 유닛 | 미보유 아이템");

            SetAllButtonAvailablity(isTradeable);
            return;
        }

        isItemCountAvailable = unPlacedItemCount >= viewModel.TradeCount;
        isTradeable = viewModel.TradeCount > 0 && isItemCountAvailable;
        canIncrease = unPlacedItemCount > viewModel.TradeCount;
        canDecrease = viewModel.ItemCount > 0 && viewModel.TradeCount >= viewModel.ItemCount;

        SetBtnInteractable(tradeBtn, isTradeable);
        SetBtnInteractable(countDecBtn, canDecrease);
        SetBtnInteractable(countIncBtn, canIncrease);
        SetBtnInteractable(countMaxBtn, canIncrease); 
    }

}
