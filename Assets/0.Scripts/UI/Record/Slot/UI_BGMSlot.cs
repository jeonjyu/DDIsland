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

    private UI_BGMList bgmList;
    private bool isLocked;      // 음반이 해금된 상태인지 여부

    #region 프로퍼티
    public bool IsLocked
    {
        get { return isLocked; }
        private set
        {
            isLocked = value;
            lockedObj.SetActive(value);
        }
    }
    #endregion

    public override void InitData<T>(RecordDataSO record, UI_RecordList<T> recordList)
    {
        base.InitData(record, recordList);

        bgmList = recordList as UI_BGMList;

        artistText.text = record.RecordArtist_String;
        playTimeText.text = record.RecordSoundPath_AudioClip.GetClipLength();

        IsLocked = !record.IsDefaultRecord;
    }

    public override void CheckUserData()
    {
        // todo: 파이어베이스 음반 데이터 저장 기능 추가 후 작성
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
            SoundManager.Instance.PlayBGM(Record.RecordSoundPath_AudioClip);
            bgmList.ShowRecordInfo(Record);
            DataManager.Instance.RecordDatabase.CurrentRecord = Record;

            if (!DataManager.Instance.RecordDatabase.CurrentPlayList.Contains(Record.RecordID))
            {
                DataManager.Instance.RecordDatabase.CurrentPlayList.Add(Record.RecordID);
            }
        }
    }

    // 음반 해금 상태로 변경
    public void UnlockRecord()
    {
        IsLocked = false;
    }
}
