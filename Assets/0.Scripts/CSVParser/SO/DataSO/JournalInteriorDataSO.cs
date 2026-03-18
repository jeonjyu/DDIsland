using System;
using UnityEngine;


[CreateAssetMenu(fileName = "JournalInteriorDataSO", menuName = "Scriptable Objects/Data/JournalInteriorDataSO")]
public class JournalInteriorDataSO : TableBase<int>
{
    // 인테리어 도감 ID
    [field: SerializeField] public int JournalInteriorID { get; private set; }

    // 인테리어 ID
    [field: SerializeField] public int InteriorID { get; private set; }

    // 해금 여부
    [field: SerializeField] public bool IsUnlocked { get; private set; }

    // 스프라이트 경로
    [field: SerializeField] public Sprite InteriorImgPath_Sprite { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => JournalInteriorID;
}
