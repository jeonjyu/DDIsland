using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PurchaseStrategy))]
public class PurchaseUnitView : TradeUnitViewBase
{
    PurchaseStrategy strategy;
    SellUnitView sellUnitView;

    private void Start()
    {
        strategy = GetComponent<PurchaseStrategy>();
        sellUnitView = GetComponent<SellUnitView>();
        //Debug.Log(this.name);
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
        //Debug.Log($"구매 유닛 | 유닛 초기화 | 아이템 개수 : {viewModel.ItemCount} ({viewModel.Model.IsGained}) 거래 개수 : {viewModel.TradeCount}");


        // 이미 최대로 보유하고 있으므로 거래 불가능

        // Tradecount가 0개일 경우 
        if (viewModel.TradeCount == 0)
        {
            Debug.Log("구매 유닛 | 개수가 0");

            SetBtnInteractable(tradeBtn, false);
            SetBtnInteractable(countDecBtn, false);
            return;
        }

        if(viewModel.TradeCount == GetTradeStrategy().GetMaxCount(viewModel.Model))
        {
            Debug.Log("구매 유닛 | 구매 개수가 최대");

            SetBtnInteractable(countIncBtn, false);
            SetBtnInteractable (countMaxBtn, false);
            return;
        }
    }
}
