using UnityEngine;

public class SubIngredientImage : RecipeSlotImg
{
    public void InitSubIngr(IStoreItem storeItem)
    {
        ingredientName = storeItem.SubIngName;
    }
}
