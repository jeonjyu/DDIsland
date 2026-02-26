using UnityEngine;

[RequireComponent(typeof(PurchaseStrategy))]
public class PurchaseUnitView : TradeUnitViewBase
{
    PurchaseStrategy strategy;

    protected override void Start()
    {
        base.Start();
        strategy = GetComponent<PurchaseStrategy>();
    }

    public override ITradeStrategy GetTradeStrategy()
    {
        return strategy;
    }
}
