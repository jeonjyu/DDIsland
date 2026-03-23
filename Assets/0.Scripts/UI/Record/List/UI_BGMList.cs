using System.Collections.Generic;
using UnityEngine;

public class UI_BGMList : UI_RecordList<UI_BGMSlot>
{
    [Header("해금 팝업 창")]
    [SerializeField] private UI_RecordUnlock unlockPopup;

    [Header("현재 재생중인 음반 정보창")]
    [SerializeField] private UI_PlayRecordInfo playRecordInfo;

    /// <summary>
    /// 배경음 재생 메서드
    /// </summary>
    /// <param name="slot"> 재생할 배경음의 슬롯 </param>
    public void PlayBGM(UI_BGMSlot slot)
    {
        ShowRecordInfo(slot.Record);

        SoundManager.Instance.PlayBGM(slot.Record.RecordSoundPath_AudioClip);
        DataManager.Instance.RecordDatabase.CurrentRecord = slot.Record;

        if (!DataManager.Instance.RecordDatabase.CurrentPlayList.Contains(slot.Record.RecordID))
        {
            DataManager.Instance.RecordDatabase.CurrentPlayList.Add(slot.Record.RecordID);
        }

        CurrentSlot = slot;
    }

    public void PlayBGM(RecordDataSO record)
    {
        ShowRecordInfo(record);

        SoundManager.Instance.PlayBGM(record.RecordSoundPath_AudioClip);
        DataManager.Instance.RecordDatabase.CurrentRecord = record;

        if (!DataManager.Instance.RecordDatabase.CurrentPlayList.Contains(record.RecordID))
        {
            DataManager.Instance.RecordDatabase.CurrentPlayList.Add(record.RecordID);
        }
    }

    // 재생중인 음원 정보 설정
    public void ShowRecordInfo(RecordDataSO record)
    {
        if (!playRecordInfo.gameObject.activeSelf)
            playRecordInfo.gameObject.SetActive(true);

        playRecordInfo.SetRecordData(record);
    }

    // 해금창 팝업
    public void ShowUnlockPopup(UI_BGMSlot slot)
    {
        CurrentSlot = slot;
        unlockPopup.ShowUnlockPopup(slot);
    }

    public void Click_PlayButton(int type)
    {
        type = Mathf.Clamp(type, -1, 1);

        List<int> playList = DataManager.Instance.RecordDatabase.CurrentPlayList;
        int index = (playList.IndexOf(DataManager.Instance.RecordDatabase.CurrentRecord.RecordID) + type + playList.Count) % playList.Count;
        RecordDataSO playRecord = DataManager.Instance.RecordDatabase.RecordInfoData[playList[index]];
        PlayBGM(playRecord);
    }

    // 곡이 끝나면 자동으로 다음 곡 재생
    public void PlayNextRecord()
    {
        List<int> playList = DataManager.Instance.RecordDatabase.CurrentPlayList;

        Debug.Log("실행");

        if (playList.Count == 0 || !playList.Contains(DataManager.Instance.RecordDatabase.CurrentRecord.RecordID))
        {
            return;
        }

        int nextIndex = (playList.IndexOf(DataManager.Instance.RecordDatabase.CurrentRecord.RecordID) + 1) % playList.Count;

        RecordDataSO playRecord = DataManager.Instance.RecordDatabase.RecordInfoData[playList[nextIndex]];

        SoundManager.Instance.PlayBGM(playRecord.RecordSoundPath_AudioClip);
        ShowRecordInfo(playRecord);
        DataManager.Instance.RecordDatabase.CurrentRecord = playRecord;
    }

    // 재생 목록이 없을 때 우선순위를 통해 재생시키는 메서드
    public void AutoPlayRecord()
    {
        if (recordSlotList == null || recordSlotList.Count == 0) return;

        // 현재 계절에 맞는 노래 재생
        // todo: 추후에 우선순위 조건에 맞게 변경, 지금은 임시로 기본 노래 재생시켜둠
        foreach(var record in recordSlotList)
        {
            if (record.IsLocked) continue;

            if(record.Record.IsDefaultRecord)
            {
                ShowRecordInfo(record.Record);
                return;
            }    
        }
    }

    //private void OnEnable()
    //{
    //    if (SoundManager.Instance != null)
    //        SoundManager.Instance.OnBGMPlayDone += PlayNextRecord;
    //}

    //private void OnDisable()
    //{
    //    if (SoundManager.Instance != null)
    //        SoundManager.Instance.OnBGMPlayDone -= PlayNextRecord;
    //}
}
