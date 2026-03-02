using System;
using UnityEngine;

// 타입
[Serializable]
public enum InteriorStore_ItemType
{
    None = 0,
    MainHouse = 1,         //메인 하우스
    Floor = 2,             //바닥
    Fix = 3,               //고정
    Free = 4,              //자유
    LakeFloor = 5,         //호수 바닥
    LakeFix = 6,           //호수 고정
    LakeFree = 7,          //호수 자유
}


[CreateAssetMenu(fileName = "InteriorStoreDataSO", menuName = "Scriptable Objects/Data/InteriorStoreDataSO")]
public class InteriorStoreDataSO : TableBase<int>
{
    // 인테리어상점id
    [field: SerializeField] public int ID { get; private set; }

    // 인테리어id
    [field: SerializeField] public int InteriorId { get; private set; }

    // 타입
    [field: SerializeField] public InteriorStore_ItemType interiorstore_itemType { get; private set; }

    // 최대 보유 가능 갯수
    [field: SerializeField] public int MaxCount { get; private set; }

    // 판매 가능 여부
    [field: SerializeField] public bool IsSaleable { get; private set; }

    // 구매 가격
    [field: SerializeField] public int PurchasePrice { get; private set; }

    // 판매 가격
    [field: SerializeField] public int SellPrice { get; private set; }

    // UI 미니 이미지 리소스
    [field: SerializeField] public Sprite InteriorImgPath_Sprite { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => ID;
}
