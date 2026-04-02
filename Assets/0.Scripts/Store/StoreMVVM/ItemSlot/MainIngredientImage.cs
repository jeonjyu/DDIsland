using UnityEngine;

public class MainIngredientImage : RecipeSlotImg
{
    public void InitMainIngr(IStoreItem storeItem)
    {
        ingredientName = storeItem.MainIngName;
    }
}
