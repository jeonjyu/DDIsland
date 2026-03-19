using System;
using UnityEngine;

public class LakeStoreItem : StoreItem<LakeStoreDataSO>
{
    public LakeStoreItem(LakeStoreDataSO data) : base(data)
    {
    }

    public override int ID => _data.IDLakeStore;

    public override int ObjectId => _data.InteriorId;

    public int LakeInteriorItemGroup => _data.LakeInteriorItemGroup;

    public override bool IsGained { get => _isGained; set => _isGained = value; }

    public override bool IsDefault => DataManager.Instance.DecorationDatabase.InteriorData[ObjectId].IsDefault;

    public override int PurchasePrice => _data.PurchasePrice;

    public override string ItemName => DataManager.Instance.DecorationDatabase.InteriorData[ObjectId].InteriorName_String;

    public override string ItemDesc => DataManager.Instance.DecorationDatabase.InteriorData[ObjectId].InteriorDesc_String;

    public override Sprite ImgSprite => _data.InteriorImgPath_Sprite;

    public override Enum Filter => _data.lakestore_itemType;

}
