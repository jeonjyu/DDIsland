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
        switch (item.Filter.ToString())
        {
            case "Floor":
                Debug.Log(item.ObjectId);
                floorID = item.ObjectId;
                lakeDecoManager?.ChangeImage(0, floorID);
                break;

            case "ornament":
                Debug.Log(item.ObjectId);
                ornamentID = item.ObjectId;
                lakeDecoManager?.ChangeImage(1, ornamentID);
                break;
        }
    }
}
