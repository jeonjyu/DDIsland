using UnityEngine;

public class UI_Record : MonoBehaviour
{
    [SerializeField] private UI_BGMList bgmList;
    [SerializeField] private UI_AMBList ambList;

    [field: SerializeField] public UI_RecordUnlock recordUnlock;

    private void Start()
    {
        bgmList.PlayBGM(DataManager.Instance.RecordDatabase.RecordInfoData[DataManager.Instance.RecordDatabase.DefaultRecords[0]]);
    }

    private void OnEnable()
    {
        if (SoundManager.Instance != null && bgmList != null)
            SoundManager.Instance.OnBGMPlayDone += bgmList.PlayNextRecord;
    }

    private void OnDisable()
    {
        if (SoundManager.Instance != null && bgmList != null)
            SoundManager.Instance.OnBGMPlayDone -= bgmList.PlayNextRecord;
    }
}
