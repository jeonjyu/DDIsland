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
}
