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

    private void Start()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            if (DataManager.Instance.Hub.IsLoaded)
            {
                SyncLakeDecoDataLoad();
            }
            else
            {
                DataManager.Instance.Hub.OnDataLoaded += SyncLakeDecoDataLoad;
            }
        }
    }

    private void OnEnable()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
            DataManager.Instance.Hub.OnRequestSave += SyncLakeDecoDataSave;
    }

    private void OnDisable()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            
            DataManager.Instance.Hub.OnRequestSave -= SyncLakeDecoDataSave;
        }
    }

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

    private void SyncLakeDecoDataSave()
    {
        var box = DataManager.Instance.Hub._allUserData;

        box.Store._currentLakeThemeId = ThemeID;

    }

    private void SyncLakeDecoDataLoad()
    {
        DataManager.Instance.Hub.OnDataLoaded -= SyncLakeDecoDataLoad;

        var box = DataManager.Instance.Hub._allUserData;
        int savedThemeId = box.Store._currentLakeThemeId;

        if (savedThemeId <= 0) return;

        if (ItemManager.Instance.storeDatas.TryGetValue(StoreCat.lake, out var lakeDb))
        {
            var item = lakeDb.Items.Find(x => x.ID == savedThemeId);
            if (item != null)
            {
                ChangedLakeSlot(item);
            }
        }
    }
}
