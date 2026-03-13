using System;
using UnityEngine;

public class FishingStoreItem : StoreItem<FishingStoreDataSO>
{
    public FishingStoreItem(FishingStoreDataSO data) : base(data)
    {
    }

    public override int ID => _data.ID;

    public override int ObjectId => _data.FishingItemId;  // id가 코스튬 Id가 리턴되어 정상적으로 테스트 불가

    public override bool IsGained { get => _isGained; set => _isGained = value; }

    public override bool IsSaleable => _data.IsSaleable;

    public override bool IsDefault => true; // DataManager.Instance.FishingDatabase.FishingItemData[ObjectId].IsDefault;

    public override int MaxCount => _data.MaxCount;

    public override int ItemCount { get => _itemCount; set => _itemCount = value; }

    public override int PurchasePrice => _data.PurchasePrice;

    public override int SellPrice => _data.SellPrice;

    public override string ItemName => DataManager.Instance.FishingDatabase.FishingItemData[ObjectId].ItemName_String;

    public override string ItemDesc => DataManager.Instance.FishingDatabase.FishingItemData[ObjectId].ItemDesc_String;

    public override Sprite ImgSprite => _data.ItemImgPath_Sprite;

    public override Enum Filter => _data.fishingcategoryType;

}
