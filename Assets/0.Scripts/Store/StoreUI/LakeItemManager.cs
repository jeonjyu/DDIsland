using System;
using System.Collections.Generic;
using UnityEngine;

public class LakeItemManager : Singleton<LakeItemManager>
{
    public LakeDecoManagerV2 lakeDecoManager;
    public ThemeApplyPopup themeApplyPopup;

    public int floorID;
    public int ornamentID;

    [SerializeField] GameObject content;

    public void ChangedLakeSlot(IStoreItem item)
    {
        Debug.Log(item.ID);
        LakeStoreItem lakeStoreItem = (LakeStoreItem)item;
        floorID = lakeStoreItem.LakeFloorID;
        ornamentID = lakeStoreItem.LakeOrnamentID;
        lakeDecoManager?.ChangeImage(0, floorID);    
        lakeDecoManager?.ChangeImage(1, ornamentID);    
    }
}
