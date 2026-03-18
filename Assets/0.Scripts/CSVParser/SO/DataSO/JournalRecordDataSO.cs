using System;
using UnityEngine;


[CreateAssetMenu(fileName = "JournalRecordDataSO", menuName = "Scriptable Objects/Data/JournalRecordDataSO")]
public class JournalRecordDataSO : TableBase<int>
{
    // 음반 도감 ID
    [field: SerializeField] public int JournalRecordID { get; private set; }

    // 음반 ID
    [field: SerializeField] public int JournalID { get; private set; }

    // 해금 여부
    [field: SerializeField] public bool IsUnlocked { get; private set; }

    // 스프라이트 경로
    [field: SerializeField] public Sprite RecordImgPath_Sprite { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => JournalRecordID;
}
