using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PurchaseStrategy))]
public class PurchaseUnitView : TradeUnitViewBase
{
    PurchaseStrategy strategy;
    SellUnitView sellUnitView;

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
        strategy = GetComponent<PurchaseStrategy>();
        sellUnitView = GetComponent<SellUnitView>();
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
        if (strategy == null) strategy = GetComponent<PurchaseStrategy>();
        return strategy;
    }
   


    public override void SetButton()
    {
        base.SetButton();
        //Debug.Log($"구매 유닛 | 유닛 초기화 | 아이템 개수 : {viewModel.ItemCount} ({viewModel.IsGained}) 거래 개수 : {viewModel.TradeCount}");

        bool isTradeable = false;
        bool canIncrease = false;
        bool canDecrease = false;
        int maxCount = GetTradeStrategy().GetMaxCount(viewModel.Model);

        if (viewModel.ItemCount >= maxCount)
        {
            SetAllButtonAvailablity(false);
            return;
        }

        canDecrease = viewModel.TradeCount > 0;
        canIncrease = viewModel.TradeCount < maxCount;
        isTradeable = viewModel.TradeCount > 0 && viewModel.TradeCount <= maxCount;

        SetBtnInteractable(tradeBtn, isTradeable);
        SetBtnInteractable(countDecBtn, canDecrease);
        SetBtnInteractable(countIncBtn, canIncrease);
        SetBtnInteractable(countMaxBtn, canIncrease);
    }
}
