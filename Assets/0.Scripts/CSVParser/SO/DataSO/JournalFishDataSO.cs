using System;
using UnityEngine;


[CreateAssetMenu(fileName = "JournalFishDataSO", menuName = "Scriptable Objects/Data/JournalFishDataSO")]
public class JournalFishDataSO : TableBase<int>
{
    // 어종 도감 ID
    [field: SerializeField] public int JournalFishID { get; private set; }

    // 어종 ID
    [field: SerializeField] public int FishID { get; private set; }

    // 해금 여부
    [field: SerializeField] public bool IsUnlocked { get; private set; }

    // 도감 메인 슬롯 이미지 경로
    [field: SerializeField] public Sprite FishSlotImgPath_Sprite { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => JournalFishID;
}
