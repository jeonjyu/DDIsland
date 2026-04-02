using UnityEngine;
using UnityEngine.UI;

public class UI_AMBSlot : UI_RecordSlot
{
    [Header("환경음 타입 텍스트")]
    [SerializeField] private UI_AmbSourceText ambSourceText;

    [Header("재생모드 관련 Image / Sprite")]
    [SerializeField] private Image playModeImg;

    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;

    private UI_AMBList ambList;

    public void SetPlayModeImg(bool isPlay) => playModeImg.sprite = isPlay ? pauseSprite : playSprite;

    public override void InitData<T>(RecordDataSO record, UI_RecordList<T> recordList)
    {
        base.InitData(record, recordList);

        ambSourceText.SetSlot(this);
        ambList = recordList as UI_AMBList;

        base.InitTextData();
    }

    public override void CheckUserData()
    {
        
    }

    public override void OnClick_Slot()
    {
        ambList.PlayAMB(this);
    }
}
