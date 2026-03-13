using System;
using UnityEngine;

// 타입
[Serializable]
public enum IslandStore_ItemType
{
    None = 0,
    MainHouse = 1,         //메인 하우스
    Floor = 2,             //바닥
    Fix = 3,               //고정
    Free = 4,              //자유
}


[CreateAssetMenu(fileName = "IslandStoreDataSO", menuName = "Scriptable Objects/Data/IslandStoreDataSO")]
public class IslandStoreDataSO : TableBase<int>
{
    // 섬인테리어상점id
    [field: SerializeField] public int IDInteriorStore { get; private set; }

    // 인테리어id
    [field: SerializeField] public int InteriorId { get; private set; }

    // 타입
    [field: SerializeField] public IslandStore_ItemType islandstore_itemType { get; private set; }

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
    public override int GetID() => IDInteriorStore;
}
