using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AddPlaylistSlot : MonoBehaviour
{
    [SerializeField] private UI_Playlist ui_Playlist;

    [SerializeField] private Image iconImg;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text timeText;

    [SerializeField] private Image addImg;

    [SerializeField] private Sprite addSprite;
    [SerializeField] private Sprite checkSprite;

    private List<RecordDataSO> records = new List<RecordDataSO>();
    private RecordDataSO selectRecord;
    private PlaylistData playlist;
    private bool isAlreadyAdd;

    public void PlaylistSlotInit(PlaylistData data, RecordDataSO selectRecord, UI_Playlist uiPlaylist)
    {
        playlist = data;
        this.selectRecord = selectRecord;
        ui_Playlist = uiPlaylist;

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

        if(playlist.RecordLists.Contains(selectRecord.RecordID))
        {
            addImg.sprite = checkSprite;
            isAlreadyAdd = true;
        }
        else
        {
            addImg.sprite = addSprite;
            isAlreadyAdd = false;
        }

            nameText.text = data.Name;
        timeText.text = string.Format("{0:D2}:{1:D2}", Mathf.RoundToInt(totalTime) / 60, Mathf.RoundToInt(totalTime) % 60);
    }

    public void OnClick_AddPlaylist()
    {
        if (selectRecord == null) return;

        if(playlist.RecordLists.Contains(selectRecord.RecordID))
        {
            addImg.sprite = addSprite;
            isAlreadyAdd = false;
            playlist.RecordLists.RemoveAt(playlist.RecordLists.FindIndex(x => x ==  selectRecord.RecordID));
            records.Remove(selectRecord);
        }
        else
        {
            addImg.sprite = checkSprite;
            isAlreadyAdd = true;
            playlist.RecordLists.Add(selectRecord.RecordID);
            records.Add(selectRecord);
        }

        ui_Playlist.slotList[ui_Playlist.slotList.FindIndex(x => x.Playlist == playlist)].PlaylistSlotInit(playlist, ui_Playlist);
    }
}
