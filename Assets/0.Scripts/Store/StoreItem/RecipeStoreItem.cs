using UnityEngine;

public class RecipeStoreItem : StoreItem<FoodDataSO>
{
    public RecipeStoreItem(FoodDataSO data) : base(data)
    {
    }

    public override int ID => _data.ID;

    public override int ObjectId => _data.ID;

    public override bool IsGained { get => _isGained; set => _isGained = value; }

    public override bool IsSaleable => false;
    public override bool IsDefault => _data.IsDefault;

    public override int MaxCount => 1;

    public override int ItemCount { get => _itemCount; set => _itemCount = value; }

    public override int PurchasePrice => _data.PurchasePrice;

    public override string ItemName => _data.FoodName_String;

    public override string ItemDesc => _data.FoodDesc_String;

    public override string MainIngName
    {
        get
        {
            if (_data.MainIngredient != 0)
                return DataManager.Instance.FishingDatabase.FishData[_data.MainIngredient].FishName_String;
            else
                return null;
        }
    }
    public override string SubIngName
    {
        get
        {
            if (_data.SubIngredient != 0)
                return DataManager.Instance.FishingDatabase.FishData[_data.SubIngredient].FishName_String;
            else
                return null;
        }
    }

    public override Sprite ImgSprite => _data.FoodImgPath_Sprite;

    public override Sprite MainIngSprite
    {
        get
        {
            if (_data.MainIngredient != 0)
                return DataManager.Instance.FishingDatabase.FishData[_data.MainIngredient].FishImgPath_Sprite;
            else
                return null;
        }
    }

    public override Sprite SubIngSprite
    { 
        get
        {
            if(_data.SubIngredient != 0)
                return DataManager.Instance.FishingDatabase.FishData[_data.SubIngredient].FishImgPath_Sprite;
            else 
                return null;
        } 
    }
        
}
