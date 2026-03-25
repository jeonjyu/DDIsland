using System;
using UnityEngine;

public class LakeStoreItem : StoreItem<LakeStoreDataSO>
{
    public LakeStoreItem(LakeStoreDataSO data) : base(data)
    {
    }

    public override int ID => _data.IDLakeStore;

    public int LakeFloorID => _data.InteriorId1;
    public int LakeOrnamentID => _data.InteriorId2;

    public int LakeInteriorItemGroup => _data.LakeThemeNumber;

    public override bool IsGained { get => _isGained; set => _isGained = value; }

    public override bool IsDefault => DataManager.Instance.DecorationDatabase.InteriorData[LakeFloorID].IsDefault;

    public override int PurchasePrice => _data.PurchasePrice;

    public override string ItemName => _data.LakeThemeName;

    public override string ItemDesc => _data.LakeThemeDesc;

    public override Sprite ImgSprite => _data.InteriorImgPath_Sprite;
}
