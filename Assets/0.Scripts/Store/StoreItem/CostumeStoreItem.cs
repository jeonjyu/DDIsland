using System;
using UnityEngine;

public class CostumeStoreItem : StoreItem<CostumeStoreDataSO>
{
    public CostumeStoreItem(CostumeStoreDataSO data) : base(data)
    {
    }

    public override int ID => _data.ID;

    public override int ObjectId => _data.CostumeId;

    public override bool IsGained { get => _isGained; set => _isGained = value; }

    public override bool IsSaleable => _data.IsSaleable;

    public override int MaxCount => _data.MaxCount;

    public override int ItemCount { get => _itemCount; set => _itemCount = value; }

    public override int PurchasePrice => _data.PurchasePrice;

    public override int SellPrice => _data.SellPrice;

    public override string ItemName => DataManager.Instance.DecorationDatabase.CostumeData[ObjectId].CostumeName_String;

    public override string ItemDesc => DataManager.Instance.DecorationDatabase.CostumeData[ObjectId].CostumeDesc_String;

    public override Sprite ImgSprite => _data.CostumeImgPath_Sprite;

    public override Enum Filter => _data.costumecategoryType;

}
