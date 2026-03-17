using System.Collections.Generic;
using UnityEngine;

public class UI_Record : MonoBehaviour
{
    [SerializeField] private UI_BGMList bgmList;
    [SerializeField] private UI_AMBList ambList;
    private void Start()
    {
        Test();
    }

    // 임시로 시작 시에 기본 제공 노래 재생
    private void Test()
    {
        foreach(var record in DataManager.Instance.RecordDatabase.RecordInfoData.datas)
        {
            if (record.recordType != RecordType.Background) continue;

            if(record.IsDefaultRecord)
            {
                SoundManager.Instance.PlayBGM(record.RecordSoundPath_AudioClip);
                bgmList.ShowRecordInfo(record);
                DataManager.Instance.RecordDatabase.CurrentRecord = record;
            }
        }
    }

    private void PlayNextRecord()
    {
        List<int> playList = DataManager.Instance.RecordDatabase.CurrentPlayList;

        if (playList.Count == 0 || !playList.Contains(DataManager.Instance.RecordDatabase.CurrentRecord.RecordID))
        {
            Test();
            return;
        }

        int nextIndex = (playList.IndexOf(DataManager.Instance.RecordDatabase.CurrentRecord.RecordID) + 1) % playList.Count;

        RecordDataSO playRecord = DataManager.Instance.RecordDatabase.RecordInfoData[playList[nextIndex]];

        SoundManager.Instance.PlayBGM(playRecord.RecordSoundPath_AudioClip);
        bgmList.ShowRecordInfo(playRecord);
        DataManager.Instance.RecordDatabase.CurrentRecord = playRecord;
    }

    private void OnEnable()
    {
        if(SoundManager.Instance != null)
            SoundManager.Instance.OnBGMPlayDone += PlayNextRecord;
    }

    private void OnDisable()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.OnBGMPlayDone -= PlayNextRecord;
    }
}
