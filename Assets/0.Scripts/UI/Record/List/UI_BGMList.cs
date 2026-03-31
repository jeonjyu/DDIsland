using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_BGMList : UI_RecordList<UI_BGMSlot>
{
    private readonly TitleComparer titleComparer = new TitleComparer();         // 제목 기준 정렬 클래스
    private readonly ArtistComparer artistComparer = new ArtistComparer();      // 아티스트 기준 정렬 클래스

    [Header("해금 팝업 창")]
    [SerializeField] private UI_RecordUnlock unlockPopup;

    [Header("현재 재생중인 음반 정보창")]
    [SerializeField] private UI_PlayRecordInfo playRecordInfo;

    [Header("정렬 / 필터 드랍다운")]
    [SerializeField] private TMP_Dropdown sortDropdown;
    [SerializeField] private TMP_Dropdown filterDropdown;

    private List<RecordDataSO> recordList = new List<RecordDataSO>();

    protected override void Start()
    {
        base.Start();

        foreach(var slot in recordSlotList)
        {
            if(slot.Record != null)
                recordList.Add(slot.Record);
        }
    }

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

    // 정렬 드랍다운의 값을 변경했을 때 호출
    public void OnValueChanged_SortDropdown()
    {
        switch(sortDropdown.value)
        {
            case 0:     // RecordId 기준 정렬
                recordList.Sort((x, y) => x.RecordID.CompareTo(y.RecordID));
                break;
            case 1:     // 제목 기준 정렬
                recordList.Sort(titleComparer);
                break;
            case 2:     // 아티스트 기준 정렬
                recordList.Sort(artistComparer);
                break;
            default:
                recordList.Sort((x, y) => x.RecordID.CompareTo(y.RecordID));
                break;
        }

        for(int i = 0; i < recordSlotList.Count; i++)
        {
            if (recordList[i] != null)
                recordSlotList[i].InitData(recordList[i], this);

            Debug.Log(recordList[i].RecordName_String);
        }
    }

    public void OnValueChanged_FilterDropdown()
    {
        foreach (var slot in recordSlotList)
        {
            switch (filterDropdown.value)
            {
                case 0:     // 전체
                    slot.gameObject.SetActive(true);
                    break;
                case 1:     // 즐겨찾기
                    slot.gameObject.SetActive(slot.IsFavorite);
                    break;
                case 2:     // 일반
                    slot.gameObject.SetActive(slot.Record.bgthemeType == BgTheme.General);
                    break;
                case 3:     // 봄
                    slot.gameObject.SetActive(slot.Record.bgthemeType == BgTheme.Spring);
                    break;
                case 4:     // 여름
                    slot.gameObject.SetActive(slot.Record.bgthemeType == BgTheme.Summer);
                    break;
                case 5:     // 가을
                    slot.gameObject.SetActive(slot.Record.bgthemeType == BgTheme.Autumn);
                    break;
                case 6:     // 겨울
                    slot.gameObject.SetActive(slot.Record.bgthemeType == BgTheme.Winter);
                    break;
                case 7:     // 콜라보
                    slot.gameObject.SetActive(slot.Record.bgthemeType == BgTheme.Collaboration);
                    break;
            }
        }
    }

    private void OnEnable()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.OnBGMPlayDone += PlayNextRecord;
    }

    private void OnDisable()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.OnBGMPlayDone -= PlayNextRecord;
    }
}
