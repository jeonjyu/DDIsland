using UnityEngine;

public class UI_BGMList : UI_RecordList<UI_BGMSlot>
{
    [Header("해금 팝업 창")]
    [SerializeField] private UI_RecordUnlock unlockPopup;

    [Header("현재 재생중인 음반 정보창")]
    [SerializeField] private UI_PlayRecordInfo playRecordInfo;

    public void ShowUnlockPopup(UI_BGMSlot slot) => unlockPopup.ShowUnlockPopup(slot);          // 해금 팝업창

    // 재생중인 음원 정보 설정
    public void ShowRecordInfo(RecordDataSO record)
    {
        if (!playRecordInfo.gameObject.activeSelf)
            playRecordInfo.gameObject.SetActive(true);

        playRecordInfo.SetRecordData(record);
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
}
