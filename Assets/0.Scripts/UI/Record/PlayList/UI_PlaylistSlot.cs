using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlaylistSlot : MonoBehaviour
{
    [Header("플레이리스트 UI들")]
    [SerializeField] private Image iconImg;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text recordLengthText;
    [SerializeField] private Image playmodeImg;

    [Header("재생모드 이미지")]
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;

    private int playlistId;
    private string playlistName;
    private List<RecordDataSO> records = new List<RecordDataSO>();
    public PlaylistData Playlist { get; private set; }

    private UI_Playlist ui_Playlist;

    public void PlaylistSlotInit(string playlistName, UI_Playlist playlist)
    {
        float totalTime = 0f;

        ui_Playlist = playlist;
        this.playlistName = playlistName;
        nameText.text = playlistName;
        recordLengthText.text = string.Format("{0:D2}:{1:D2}", Mathf.RoundToInt(totalTime) / 60, Mathf.RoundToInt(totalTime) % 60);

        if(playlistId == 0)
        {
            Playlist = new PlaylistData();
            playlistId = DataManager.Instance.RecordDatabase.PlaylistDatas.Count + 1;
            Playlist.Id = playlistId;
            Playlist.Name = playlistName;
            Playlist.CreateDate = DateTime.Now.ToString("yyyy-MM-dd");
            Playlist.RecordLists = new List<int>();
            DataManager.Instance.RecordDatabase.PlaylistDatas.Add(Playlist);
        }
    }

    public void PlaylistSlotInit(PlaylistData data, UI_Playlist playlist)
    {
        Playlist = data;
        ui_Playlist = playlist;

        float totalTime = 0f;

        if (data.RecordLists != null)
        {
            foreach (var record in data.RecordLists)
            {
                RecordDataSO recordData = DataManager.Instance.RecordDatabase.RecordInfoData[record];

                records.Add(recordData);
                totalTime += recordData.RecordSoundPath_AudioClip.length;
            }
        }

        playlistName = data.Name;
        nameText.text = playlistName;
        recordLengthText.text = string.Format("{0:D2}:{1:D2}", Mathf.RoundToInt(totalTime) / 60, Mathf.RoundToInt(totalTime) % 60);

        playlistId = data.Id;

        if(data.RecordLists.Count > 0)
        {
            iconImg.sprite = DataManager.Instance.RecordDatabase.RecordInfoData[data.RecordLists[0]].RecordImgPath_Sprite;
        }
    }

    public void OnClick_SelectPlaylist()
    {
        ui_Playlist.ShowCurrentPlaylist(Playlist);
        DataManager.Instance.RecordDatabase.CurrentPlaylistId = Playlist.Id;
    }

    public void OnClick_EditPlaylist()
    {
        ui_Playlist.OnClick_EditPlaylist(Playlist);
    }
}
