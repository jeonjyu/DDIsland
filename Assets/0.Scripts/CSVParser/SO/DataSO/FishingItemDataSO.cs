using System;
using UnityEngine;

// 타입
[Serializable]
public enum FishingItemType
{
    None = 0,
    Pole = 1,         //낚싯대
    Bait = 2,         //미끼
    Bobber = 3,       //낚시찌
}


[CreateAssetMenu(fileName = "FishingItemDataSO", menuName = "Scriptable Objects/Data/FishingItemDataSO")]
public class FishingItemDataSO : TableBase<int>
{
    // id
    [field: SerializeField] public int ID { get; private set; }

    // 이름
    [SerializeField] private string itemName;
    public string ItemName_String => LocalizationManager.Instance.GetString(itemName);

    // 설명
    [SerializeField] private string itemDesc;
    public string ItemDesc_String => LocalizationManager.Instance.GetString(itemDesc);

    // 타입
    [field: SerializeField] public FishingItemType fishingitemType { get; private set; }

    // 낚시 속도 상승량
    [field: SerializeField] public float FishingSpeed { get; private set; }

    // 물고기 풀
    [field: SerializeField] public int FishPool { get; private set; }

    // 물고기 크기 상승량
    [field: SerializeField] public float FishLength { get; private set; }

    // 기본 제공 여부
    [field: SerializeField] public bool IsDefault { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => ID;
}
