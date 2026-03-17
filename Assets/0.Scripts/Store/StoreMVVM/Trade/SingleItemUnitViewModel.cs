using UnityEngine;

public class SingleItemUnitViewModel : TradeUnitViewModelBase
{
    public override void InitUnit()
    {
        SetTotalPrice();
        view.SetButton();
    }

    public override void SetTotalPrice()
    {
        view.SetTotalPriceText(view.GetTradeStrategy().GetPrice(Model));
    }
}
