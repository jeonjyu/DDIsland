using System;
using System.Collections.Generic;
using UnityEngine;

public class LakeItemManager : Singleton<LakeItemManager>
{
    public LakeDecoManagerV2 lakeDecoManager;
    public ThemeApplyPopup themeApplyPopup;

    private int _themeID;
    private int _floorID;
    private int _ornamentID;

    public int ThemeID => _themeID;
    [SerializeField] GameObject content;

    public void ChangedLakeSlot(IStoreItem item)
    {
        LakeStoreItem lakeStoreItem = (LakeStoreItem)item;
  
        _themeID = lakeStoreItem.ID;
        _floorID = lakeStoreItem.LakeFloorID;
        _ornamentID = lakeStoreItem.LakeOrnamentID;
        

        lakeDecoManager?.ChangeImage(0, _floorID);    
        lakeDecoManager?.ChangeImage(1, _ornamentID);    
    }
}
