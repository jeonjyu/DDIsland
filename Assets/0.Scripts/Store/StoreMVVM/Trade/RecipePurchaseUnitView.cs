using UnityEngine;

// 구매만 가능한 팝업 뷰

public class RecipePurchaseUnitView : TradeUnitViewBase
{
    PurchaseStrategy strategy;


    protected override void Awake()
    {
        base.Awake();
        strategy = GetComponent<PurchaseStrategy>();
    }

    public override ITradeStrategy GetTradeStrategy()
    {
        return strategy;
    }

    public override void SetButton()
    {
        base.SetButton();

        if(viewModel.ItemCount == 1 && viewModel.IsGained == true)
        {
            SetAllButtonAvailablity(false);
            //Debug.Log(this + " 레시피 구매 유닛 | 구매 가능");
        }
        else
        {
            SetAllButtonAvailablity(true);
            //Debug.Log(this + " 레시피 구매 유닛 | 구매 불가");
        }

    }

}
