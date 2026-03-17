using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_RecordSlot : MonoBehaviour
{
    [Header("음반 이미지")]
    [SerializeField] private Image recordImage;

    [Header("음반 제목 텍스트")]
    [SerializeField] private TMP_Text titleText;

    [field: SerializeField] public RecordDataSO Record { get; private set; }

    // 슬롯 초기화
    public virtual void InitData<T>(RecordDataSO record, UI_RecordList<T> recordList) where T : UI_RecordSlot
    {
        Record = record;

        recordImage.sprite = record.RecordImgPath_Sprite;
        titleText.text = record.RecordName_String;
    }

    public abstract void CheckUserData();       // 유저 저장 데이터 체크

    public abstract void OnClick_Slot();        // 음반 슬롯 클릭 메서드
}