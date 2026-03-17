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
        return strategy;
    }
   

    // 아이템 보유 개수 최대일 경우 > 구매 유닛 버튼들 비활성화
    // 구매시 : 플레이어 골드보다 총합 가격이 높을 경우 > max, + 버튼 비활성화
    public override void SetButton()
    {
        base.SetButton();
        //Debug.Log($"구매 유닛 | 유닛 초기화 | 아이템 개수 : {viewModel.ItemCount} ({viewModel.IsGained}) 거래 개수 : {viewModel.TradeCount}");


        // 이미 최대로 보유하고 있으므로 거래 불가능

        // Tradecount가 0개일 경우 
        if (viewModel.TradeCount == 0)
        {
            Debug.Log(this + " 구매 유닛 | 개수가 0");

            SetBtnInteractable(tradeBtn, false);
            SetBtnInteractable(countDecBtn, false);
            return;
        }

        if(viewModel.ItemCount >= GetTradeStrategy().GetMaxCount(viewModel.Model))
        {
            Debug.Log(this + " 구매 유닛 | 현재 보유 개수가 최대");

            SetAllButtonAvailablity(false);
            return;
        }

        if(viewModel.TradeCount >= GetTradeStrategy().GetMaxCount(viewModel.Model))
        {
            Debug.Log(this + " 구매 유닛 | 구매 개수가 최대");

            SetBtnInteractable(countIncBtn, false);
            SetBtnInteractable (countMaxBtn, false);
            return;
        }
    }
}
