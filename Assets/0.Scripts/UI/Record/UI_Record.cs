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
            }
        }
    }
}
