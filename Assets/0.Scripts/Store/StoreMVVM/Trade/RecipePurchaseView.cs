using UnityEngine;

public class RecipePurchaseView : TradeUnitViewBase
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
    }
}
