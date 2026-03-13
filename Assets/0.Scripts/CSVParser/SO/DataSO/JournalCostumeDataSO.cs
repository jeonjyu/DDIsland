using System;
using UnityEngine;


[CreateAssetMenu(fileName = "JournalCostumeDataSO", menuName = "Scriptable Objects/Data/JournalCostumeDataSO")]
public class JournalCostumeDataSO : TableBase<int>
{
    // 코스튬 도감 ID
    [field: SerializeField] public int JournalCostumeID { get; private set; }

    // 코스튬 ID
    [field: SerializeField] public int CostumeID { get; private set; }

    // 해금 여부
    [field: SerializeField] public bool IsUnlocked { get; private set; }

    // 스프라이트 경로
    [field: SerializeField] public Sprite CostumeImgPath_Sprite { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => JournalCostumeID;
}
