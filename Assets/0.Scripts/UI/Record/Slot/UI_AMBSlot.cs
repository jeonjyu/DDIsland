using UnityEngine;

public class UI_AMBSlot : UI_RecordSlot
{
    private UI_AMBList ambList;

    public override void InitData<T>(RecordDataSO record, UI_RecordList<T> recordList)
    {
        base.InitData(record, recordList);

        ambList = recordList as UI_AMBList;
    }

    public override void CheckUserData()
    {
        
    }

    public override void OnClick_Slot()
    {
        if (Record == null) return;

        SoundManager.Instance.PlayBGS(Record.RecordSoundPath_AudioClip);
    }
}
