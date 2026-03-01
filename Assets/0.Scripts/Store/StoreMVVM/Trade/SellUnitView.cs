using UnityEngine;

[RequireComponent(typeof(SellStrategy))]
public class SellUnitView : TradeUnitViewBase
{
    SellStrategy strategy;

    void Awake()
    {
        strategy = GetComponent<SellStrategy>();
    }

    public override ITradeStrategy GetTradeStrategy()
    {
        return strategy;
    }
}
