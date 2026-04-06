using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BGMSlot : UI_RecordSlot
{
    [Header("음반 잠금 오브젝트")]
    [SerializeField] private GameObject lockedObj;

    [Header("음반 아티스트 텍스트")]
    [SerializeField] private TMP_Text artistText;

    [Header("음반 재생시간 텍스트")]
    [SerializeField] private TMP_Text playTimeText;

    [Header("즐겨찾기 토글")]
    [SerializeField] private Toggle favoriteToggle;

    [Header("플레이리스트 추가 버튼")]
    [SerializeField] private Button playListAddBtn;

    public bool IsFavorite { get; private set; }    // 즐겨찾기 여부

    private UI_BGMList bgmList;
    private bool isLocked;      // 음반이 해금된 상태인지 여부

    private bool isInitialize;

    #region 프로퍼티
    public bool IsLocked
    {
        get { return isLocked; }
        private set
        {
            isLocked = value;
            lockedObj.SetActive(value);

            // 잠금 상태일 때는 상호작용 X
            favoriteToggle.interactable = !value;
            playListAddBtn.interactable = !value;
        }
    }
    #endregion

    public override void InitData<T>(RecordDataSO record, UI_RecordList<T> recordList)
    {
        isInitialize = true;

        base.InitData(record, recordList);

        bgmList = recordList as UI_BGMList;
        playTimeText.text = record.RecordSoundPath_AudioClip.GetClipLength();

        favoriteToggle.isOn = DataManager.Instance.RecordDatabase.BookmarkRecords.Contains(Record.RecordID);
        IsFavorite = favoriteToggle.isOn;

        CheckUserData();
        InitTextData();

        isInitialize = false;
    }

    public override void InitTextData()
    {
        base.InitTextData();

        artistText.text = Record.RecordArtist_String;
    }

    public override void CheckUserData()
    {
        if(Record.IsDefaultRecord)
        {
            IsLocked = false;
            return;
        }

        if (DataManager.Instance.RecordDatabase.UnlockRecords.Contains(Record.RecordID))
            IsLocked = false;
        else
            IsLocked = true;
    }

    public void OnValueChanged_FavoriteToggle()
    {
        if (isInitialize) return;

        IsFavorite = favoriteToggle.isOn;

        HashSet<int> bookmarks = DataManager.Instance.RecordDatabase.BookmarkRecords;

        if (favoriteToggle.isOn)
        {
            if (!bookmarks.Contains(Record.RecordID))
            {
                bookmarks.Add(Record.RecordID);
            }
        }
        else
        {
            if (bookmarks.Contains(Record.RecordID))
            {
                bookmarks.Remove(Record.RecordID);
            }
        }
    }

    public void OnClick_AddPlaylist()
    {
        bgmList.AddPlaylist(Record);
    }

    public override void OnClick_Slot()
    {
        if (Record == null) return;

        if(isLocked)
        {
            bgmList.ShowUnlockPopup(this);
        }
        else
        {
            bgmList.PlayBGM(this);
        }
    }

    // 음반 해금 상태로 변경
    public void UnlockRecord()
    {
        IsLocked = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        CheckUserData();
    }
}
