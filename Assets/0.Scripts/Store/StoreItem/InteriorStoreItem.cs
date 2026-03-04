using System;
using UnityEngine;

public class InteriorStoreItem : StoreItem<InteriorStoreDataSO>
{
    public InteriorStoreItem(InteriorStoreDataSO data) : base(data)
    {
    }

    public override int ID => _data.ID;

    public override int ObjectId => _data.InteriorId;

    public override bool IsGained { get => _isGained; set => _isGained = value; }

    public override bool IsSaleable => _data.IsSaleable;

    public override int MaxCount => _data.MaxCount;

    public override int ItemCount { get => _itemCount; set => _itemCount = value; }

    public override int PurchasePrice => _data.PurchasePrice;

    public override int SellPrice => _data.SellPrice;

    public override string ItemName => DataManager.Instance.DecorationDatabase.InteriorData[ObjectId].InteriorName_String;

    public override string ItemDesc => DataManager.Instance.DecorationDatabase.InteriorData[ObjectId].InteriorDesc_String;

    public override Sprite ImgSprite => _data.InteriorImgPath_Sprite;

    public override Enum Filter => _data.interiorstore_itemType;

}
