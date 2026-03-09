using UnityEngine;

[RequireComponent(typeof(SellStrategy))]
public class SellUnitView : TradeUnitViewBase
{
    SellStrategy strategy;
    PurchaseUnitView purchaseUnitView;
    protected override void Awake()
    {
        base.Awake();
        strategy = GetComponent<SellStrategy>();
        purchaseUnitView = GetComponent<PurchaseUnitView>();
        //Debug.Log(this.name);
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
        //Debug.Log($"판매 유닛 | 유닛 초기화 | 아이템 개수 : {viewModel.ItemCount} ({viewModel.Model.IsGained}) 거래 개수 : {viewModel.TradeCount}");

        // 거래 개수가 0일 경우 
        // 보유하지 않은 아이템일 경우 
        if (viewModel.TradeCount == 0)
        {
            Debug.Log("판매 유닛 | 거래 개수가 0");

            SetBtnInteractable(CountDecBtn, false);
            SetBtnInteractable(tradeBtn, false);
            return;
        }
        if (viewModel.ItemCount == 0 && viewModel.Model.IsGained == false)
        {
            Debug.Log("판매 유닛 | 보유하지 않은 아이템 " + viewModel.Model.IsGained);
            
            SetAllButtonAvailablity(false);
            return;
        }

        if (viewModel.TradeCount == viewModel.Model.ItemCount)
        {
            Debug.Log("판매 유닛 | 보유한 아이템보다 더 많이 판매할 수 없음");

            SetBtnInteractable(CountIncBtn, false);
            SetBtnInteractable(CountMaxBtn, false);
            return;
        }
    }

}
