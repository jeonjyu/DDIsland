using UnityEngine;

public class UI_Record : MonoBehaviour
{
    [SerializeField] private UI_BGMList bgmList;
    [SerializeField] private UI_AMBList ambList;

    [field: SerializeField] public UI_RecordUnlock recordUnlock;

    private void Start()
    {
        bgmList.PlayBGM(DataManager.Instance.RecordDatabase.RecordInfoData[DataManager.Instance.RecordDatabase.DefaultRecords[0]]);
        foreach(var id in DataManager.Instance.RecordDatabase.CurrentPlayList)
        {
            Debug.Log(id);
        }
    }
}
