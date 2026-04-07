using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EditPlaylist : UI_BaseLocalizedText
{
    [SerializeField] private Image iconImg;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text createDateText;
    [SerializeField] private TMP_Text recordCountText;
    [SerializeField] private TMP_Text lengthText;

    [SerializeField] private string createDateKey;
    [SerializeField] private string recordCountKey;
    [SerializeField] private string lengthKey;

    [SerializeField] private UI_Playlist uI_Playlist;
    [SerializeField] private UI_BGMList ui_BgmList;

    public PlaylistData PlaylistData { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    public void SetEditInit(PlaylistData data)
    {
        if (data == null) return;

        PlaylistData = data;

        if (data.RecordLists.Count > 0)
        {
            iconImg.sprite = DataManager.Instance.RecordDatabase.RecordInfoData[data.RecordLists[0]].RecordImgPath_Sprite;
        }

        titleText.text = data.Name;
        SetText();
    }

    protected override void SetText()
    {
        createDateText.text = string.Format(DataManager.Instance.StringUIDatabase.StringUIInfoData[createDateKey].ID_String,
            DateTime.Parse(PlaylistData.CreateDate).ToString("yyyy.MM.dd"));

        recordCountText.text = string.Format(DataManager.Instance.StringUIDatabase.StringUIInfoData[recordCountKey].ID_String,
            PlaylistData.RecordLists.Count);

        float length = 0f;

        foreach (var record in PlaylistData.RecordLists)
        {
            length += DataManager.Instance.RecordDatabase.RecordInfoData[record].RecordSoundPath_AudioClip.length;
        }

        lengthText.text = string.Format(DataManager.Instance.StringUIDatabase.StringUIInfoData[lengthKey].ID_String,
            Mathf.RoundToInt(length / 60f), Mathf.RoundToInt(length % 60f));
    }

    public void OnClick_Play()
    {
        ui_BgmList.PlayPlaylist(PlaylistData);
    }

    public void OnClick_Shuffle()
    {
        ui_BgmList.ShufflePlaylist(PlaylistData);
    }

    public void OnClick_EditTitle()
    {
        uI_Playlist.ChangePlaylistName(PlaylistData);
    }

    public void OnClick_DeletePlaylist()
    {
        if(DataManager.Instance.RecordDatabase.CurrentPlaylistId == PlaylistData.Id)
        {
            DataManager.Instance.RecordDatabase.CurrentPlaylistId = 0;
        }

        UI_PlaylistSlot slot = uI_Playlist.slotList[uI_Playlist.slotList.FindIndex(x => x.Playlist == PlaylistData)];
        uI_Playlist.slotList.Remove(slot);
        Destroy(slot.gameObject);
        DataManager.Instance.RecordDatabase.PlaylistDatas.Remove(PlaylistData);
        PlaylistData = null;
    }
}
