using UnityEngine;

[RequireComponent(typeof(SellStrategy))]
public class SellUnitView : TradeUnitViewBase
{
    SellStrategy strategy;

    protected override void Start()
    {
        base.Start();
        strategy = GetComponent<SellStrategy>();
    }

    public override ITradeStrategy GetTradeStrategy()
    {
        return strategy;
    }
}
