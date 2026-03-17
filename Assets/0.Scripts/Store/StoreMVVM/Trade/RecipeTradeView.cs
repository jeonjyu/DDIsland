using UnityEngine;

public class RecipeTradeView : TradeViewBase
{
    public override void SetView()
    {
        itemName.text = viewModel.Model.ItemName;
        itemDesc.text = viewModel.Model.PurchasePrice.ToString();
    }
}
