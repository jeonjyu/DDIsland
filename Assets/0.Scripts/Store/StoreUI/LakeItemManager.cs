using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LakeItemManager : Singleton<LakeItemManager>, INotifyPropertyChanged
{
    public LakeDecoManagerV2 lakeDecoManager;
    public ThemeApplyPopup themeApplyPopup;

    private int _themeID;
    private int _floorID;
    private int _ornamentID;

    public int ThemeID
    {
        get => _themeID;
        set
        {
            if(_themeID != value)
            {
                _themeID = value;
                OnPropertyChanged(nameof(ThemeID));
            }
        }
    }

    [SerializeField] GameObject content;

    public event PropertyChangedEventHandler PropertyChanged;

    public void ChangedLakeSlot(IStoreItem item)
    {
        LakeStoreItem lakeStoreItem = (LakeStoreItem)item;
  
        _themeID = lakeStoreItem.ID;
        _floorID = lakeStoreItem.LakeFloorID;
        _ornamentID = lakeStoreItem.LakeOrnamentID;
        

        lakeDecoManager?.ChangeImage(0, _floorID);    
        lakeDecoManager?.ChangeImage(1, _ornamentID);    
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
