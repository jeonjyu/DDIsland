using UnityEngine;

[RequireComponent(typeof(PurchaseStrategy))]
public class PurchaseUnitView : TradeUnitViewBase
{
    PurchaseStrategy strategy;

    void Awake()
    {
        strategy = GetComponent<PurchaseStrategy>();
    }

    public override ITradeStrategy GetTradeStrategy()
    {
        return strategy;
    }
}
