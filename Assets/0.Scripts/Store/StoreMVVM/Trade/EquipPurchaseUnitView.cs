using UnityEngine;

public class EquipPurchaseUnitView : TradeUnitViewBase
{
    PurchaseStrategy strategy;
    public EquipTradeView _equipTradeView;

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
        SetBtnInteractable(tradeBtn, !viewModel.IsGained);
    }
}
