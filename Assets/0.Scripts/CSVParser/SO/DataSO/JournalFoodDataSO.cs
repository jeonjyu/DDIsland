using System;
using UnityEngine;


[CreateAssetMenu(fileName = "JournalFoodDataSO", menuName = "Scriptable Objects/Data/JournalFoodDataSO")]
public class JournalFoodDataSO : TableBase<int>
{
    // 음식 도감 id
    [field: SerializeField] public int JournalFoodID { get; private set; }

    // 음식 id
    [field: SerializeField] public int FoodID { get; private set; }

    // 해금 여부
    [field: SerializeField] public bool IsUnlocked { get; private set; }

    // 스프라이트 경로
    [field: SerializeField] public Sprite FoodImgPath_Sprite { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => JournalFoodID;
}
