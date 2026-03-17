using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SellStrategy))]
public class SellUnitView : TradeUnitViewBase
{
    SellStrategy strategy;
    PurchaseUnitView purchaseUnitView;

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
        purchaseUnitView = GetComponent<PurchaseUnitView>();
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
        return strategy;
    }

    // 버튼이 변경되어야 할 경우
    // 아이템 보유 개수 최대일 경우 > 구매 유닛 버튼들 비활성화
    // 구매시 : 플레이어 골드보다 총합 가격이 높을 경우 > max, + 버튼 비활성화
    // 아이템 개수가 0일 경우 > - 버튼 비활성화
    public override void SetButton()
    {
        base.SetButton();
        //Debug.Log($"판매 유닛 | 유닛 초기화 | 아이템 개수 : {viewModel.ItemCount} ({viewModel.IsGained}) 거래 개수 : {viewModel.TradeCount}");

        // 거래 불가능한 아이템일 경우 판매 유닛 버튼 클릭 불가능
        if (viewModel.Model.IsSaleable == false)
        {
            Debug.Log(this + " 판매 유닛 | 판매 불가능");
            SetAllButtonAvailablity(false);
            return;
        }

        // 거래 개수가 0일 경우 
        // 보유하지 않은 아이템일 경우 
        if (viewModel.TradeCount == 0)
        {
            Debug.Log(this + " 판매 유닛 | 거래 개수가 0");

            SetBtnInteractable(CountDecBtn, false);
            SetBtnInteractable(tradeBtn, false);
            return;
        }

        if (viewModel.ItemCount == 0 && viewModel.IsGained == false)
        {
            Debug.Log(this + " 판매 유닛 | 보유하지 않은 아이템 " + viewModel.IsGained);
            
            SetAllButtonAvailablity(false);
            return;
        }

        if (viewModel.TradeCount == viewModel.Model.ItemCount)
        {
            Debug.Log(this + " 판매 유닛 | 보유한 아이템보다 더 많이 판매할 수 없음");

            SetBtnInteractable(CountIncBtn, false);
            SetBtnInteractable(CountMaxBtn, false);
        }

 
    }

}
