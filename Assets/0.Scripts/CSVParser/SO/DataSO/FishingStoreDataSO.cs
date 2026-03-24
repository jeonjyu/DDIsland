using System;
using UnityEngine;

// 타입
[Serializable]
public enum FishingCategory
{
    None = 0,
    Pole = 1,         //낚싯대
    Bait = 2,         //미끼
    Bobber = 3,       //낚시찌
}


[CreateAssetMenu(fileName = "FishingStoreDataSO", menuName = "Scriptable Objects/Data/FishingStoreDataSO")]
public class FishingStoreDataSO : TableBase<int>
{
    // 낚시 상점id
    [field: SerializeField] public int ID { get; private set; }

    // 낚시 아이템 id
    [field: SerializeField] public int FishingItemId { get; private set; }

    // 외형 변화
    [field: SerializeField] public bool IsChange { get; private set; }

    // 타입
    [field: SerializeField] public FishingCategory fishingcategoryType { get; private set; }

    // 최대 보유 개수
    [field: SerializeField] public int MaxCount { get; private set; }

    // 판매 가능 여부
    [field: SerializeField] public bool IsSaleable { get; private set; }

    // 구매 가격
    [field: SerializeField] public int PurchasePrice { get; private set; }

    // 판매 가격
    [field: SerializeField] public int SellPrice { get; private set; }

    // UI 미니 이미지 리소스
    [field: SerializeField] public Sprite ItemImgPath_Sprite { get; private set; }

    // 오브젝트 리소스 경로
    [field: SerializeField] public GameObject ObjectPath_GameObject { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => ID;
}
