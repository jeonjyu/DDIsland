using UnityEngine;

public class SingleItemUnitViewModel : TradeUnitViewModelBase
{
    public override void InitUnit()
    {
        SetTotalPrice();
        view.SetButton();
    }
}
