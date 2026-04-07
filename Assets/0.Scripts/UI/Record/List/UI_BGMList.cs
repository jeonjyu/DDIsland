using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_BGMList : UI_RecordList<UI_BGMSlot>
{
    private readonly TitleComparer titleComparer = new TitleComparer();         // 제목 기준 정렬 클래스
    private readonly ArtistComparer artistComparer = new ArtistComparer();      // 아티스트 기준 정렬 클래스

    [Header("해금 팝업 창")]
    [SerializeField] private UI_RecordUnlock unlockPopup;

    [Header("현재 재생중인 음반 정보창")]
    [SerializeField] private UI_PlayRecordInfo playRecordInfo;
    [SerializeField] private UI_AlbumDetail albumDetail;

    [Header("정렬 / 필터 드랍다운")]
    [SerializeField] private TMP_Dropdown sortDropdown;
    [SerializeField] private TMP_Dropdown filterDropdown;

    [Header("플레이리스트 추가 팝업")]
    [SerializeField] private UI_AddPlaylist addPlaylist;
    [SerializeField] private UI_CurrentPlaylist currentPlaylist;

    [SerializeField] private EnvironmentPresenter environment;

    private List<RecordDataSO> recordList = new List<RecordDataSO>();

    public bool IsPlayRepeat { get; private set; }        // 한 곡 반복 재생
    public bool IsPlayShuffle { get; private set; }       // 셔플 재생

    protected override IEnumerator Start()
    {
        yield return base.Start();

        environment.Model.OnSeasonChanged += AddDefaultRecords;

        AddDefaultRecords(environment.Model.CurrentSeason);

        foreach (var slot in recordSlotList)
        {
            if (slot.Record != null)
                recordList.Add(slot.Record);
        }
    }

    /// <summary>
    /// 배경음 재생 메서드
    /// </summary>
    /// <param name="slot"> 재생할 배경음의 슬롯 </param>
    public void PlayBGM(UI_BGMSlot slot)
    {
        CurrentSlot = slot;

        SoundManager.Instance.PlayBGM(slot.Record.RecordSoundPath_AudioClip, ShowRecordInfo, slot.Record);
        DataManager.Instance.RecordDatabase.CurrentRecord = slot.Record;
    }

    public void PlayBGM(RecordDataSO record)
    {
        CurrentSlot = recordSlotList[recordSlotList.FindIndex(x => x.Record == record)];

        SoundManager.Instance.PlayBGM(record.RecordSoundPath_AudioClip, ShowRecordInfo, record);
        DataManager.Instance.RecordDatabase.CurrentRecord = record;
    }

    // 재생중인 음원 정보 설정
    public void ShowRecordInfo(RecordDataSO record)
    {
        if (!playRecordInfo.gameObject.activeSelf)
            playRecordInfo.gameObject.SetActive(true);

        playRecordInfo.SetRecordData(record);

        if(albumDetail != null)
        {
            albumDetail.SetRecordData(record);
        }

        if (currentPlaylist != null)
        {
            currentPlaylist.SetRecordData(record);
        }
    }

    public void UpdateFavoriteRecord(RecordDataSO data)
    {
        foreach(var slot in recordSlotList)
        {
            if(slot.Record.RecordID == data.RecordID)
            {
                slot.InitData(data, this);
            }
        }
    }

    public void AddDefaultRecords(Season season)
    {
        List<int> list = new List<int>();

        foreach (var record in DataManager.Instance.RecordDatabase.RecordInfoData.datas)
        {
            bool isSeason = false;

            switch(record.bgthemeType)
            {
                case BgTheme.Spring:
                    isSeason = season == Season.Spring;
                    break;
                case BgTheme.Summer:
                    isSeason = season == Season.Summer;
                    break;
                case BgTheme.Autumn:
                    isSeason = season == Season.Autumn;
                    break;
                case BgTheme.Winter:
                    isSeason = season == Season.Winter;
                    break;
            }

            if(isSeason && record.IsDefaultRecord)
            {
                list.Add(record.RecordID);
            }
        }

        DataManager.Instance.RecordDatabase.DefaultRecords = list;
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

        List<int> playList = new List<int>();

        if (PlayerPrefsDataManager.PlayDefaultRecord)
        {
            playList = DataManager.Instance.RecordDatabase.DefaultRecords;
        }
        else
        {
            playList = DataManager.Instance.RecordDatabase.CurrentPlayList;
        }

        if (playList.Count == 0)
        {
            return;
        }

        RecordDataSO playRecord = new RecordDataSO();

        if (IsPlayRepeat)
        {
            playRecord = CurrentSlot.Record;
        }
        else
        {
            if (IsPlayShuffle)
            {
                playRecord = CurrentSlot.Record;

                while (playRecord == CurrentSlot.Record)
                {
                    int nextIndex = Random.Range(0, playList.Count);
                    playRecord = DataManager.Instance.RecordDatabase.RecordInfoData[playList[nextIndex]];
                }
            }
            else
            {
                int nextIndex = (playList.IndexOf(DataManager.Instance.RecordDatabase.CurrentRecord.RecordID) + type + playList.Count) % playList.Count;
                playRecord = DataManager.Instance.RecordDatabase.RecordInfoData[playList[nextIndex]];
            }
        }

        CurrentSlot = recordSlotList[recordSlotList.FindIndex(x => x.Record == playRecord)];
        SoundManager.Instance.PlayBGM(playRecord.RecordSoundPath_AudioClip, ShowRecordInfo, playRecord);
        DataManager.Instance.RecordDatabase.CurrentRecord = playRecord;
    }

    // 곡이 끝나면 자동으로 다음 곡 재생
    public void PlayNextRecord()
    {
        List<int> playList = new List<int>();

        if(PlayerPrefsDataManager.PlayDefaultRecord)
        {
            playList = DataManager.Instance.RecordDatabase.DefaultRecords;
        }
        else
        {
            playList = DataManager.Instance.RecordDatabase.CurrentPlayList;
        }

        if (playList.Count == 0)
        {
            return;
        }

        RecordDataSO playRecord = new RecordDataSO();

        if(IsPlayRepeat)
        {
            playRecord = CurrentSlot.Record;
        }
        else
        {
            if(IsPlayShuffle)
            {
                playRecord = CurrentSlot.Record;

                while(playRecord == CurrentSlot.Record)
                {
                    int nextIndex = Random.Range(0, playList.Count);
                    playRecord = DataManager.Instance.RecordDatabase.RecordInfoData[playList[nextIndex]];
                }
            }
            else
            {
                int nextIndex = (playList.IndexOf(DataManager.Instance.RecordDatabase.CurrentRecord.RecordID) + 1) % playList.Count;
                playRecord = DataManager.Instance.RecordDatabase.RecordInfoData[playList[nextIndex]];
            }
        }

        CurrentSlot = recordSlotList[recordSlotList.FindIndex(x => x.Record == playRecord)];
        SoundManager.Instance.PlayBGM(playRecord.RecordSoundPath_AudioClip, ShowRecordInfo, playRecord);
        DataManager.Instance.RecordDatabase.CurrentRecord = playRecord;
    }

    public void PlayPlaylist(PlaylistData data)
    {
        SetPlaylist(data);

        if(data.RecordLists.Contains(CurrentSlot.Record.RecordID))
            PlayBGM(CurrentSlot.Record);
        else
            PlayBGM(DataManager.Instance.RecordDatabase.RecordInfoData[DataManager.Instance.RecordDatabase.CurrentPlayList[0]]);
    }

    public void ShufflePlaylist(PlaylistData data)
    {
        SetPlaylist(data);

        List<int> playList = DataManager.Instance.RecordDatabase.CurrentPlayList;

        RecordDataSO playRecord = new RecordDataSO();

        playRecord = CurrentSlot.Record;

        while (playRecord == CurrentSlot.Record)
        {
            int nextIndex = Random.Range(0, playList.Count);
            playRecord = DataManager.Instance.RecordDatabase.RecordInfoData[playList[nextIndex]];
        }

        CurrentSlot = recordSlotList[recordSlotList.FindIndex(x => x.Record == playRecord)];
        SoundManager.Instance.PlayBGM(playRecord.RecordSoundPath_AudioClip, ShowRecordInfo, playRecord);
        DataManager.Instance.RecordDatabase.CurrentRecord = playRecord;
    }

    private void SetPlaylist(PlaylistData data)
    {
        if(DataManager.Instance.RecordDatabase.CurrentPlaylistId != data.Id)
        {
            DataManager.Instance.RecordDatabase.CurrentPlaylistId = data.Id;
        }
    }

    public void OnClick_PlayRepeat()
    {
        IsPlayRepeat = !IsPlayRepeat;
    }

    public void OnValueChange_PlayShuffle()
    {
        IsPlayShuffle = !IsPlayShuffle;
    }

    public void AddPlaylist(RecordDataSO record)
    {
        addPlaylist.gameObject.SetActive(true);
        addPlaylist.CreatePlaylistSlot(record);
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
        }

        OnValueChanged_FilterDropdown();
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

    private void OnDestroy()
    {
        environment.Model.OnSeasonChanged -= AddDefaultRecords;
    }
}
