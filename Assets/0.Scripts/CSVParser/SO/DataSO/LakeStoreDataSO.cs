using System;
using UnityEngine;

// 타입
[Serializable]
public enum LakeStore_ItemType
{
    None = 0,
    Floor = 1,          //바닥재
    ornament = 2,       //장식물
}


[CreateAssetMenu(fileName = "LakeStoreDataSO", menuName = "Scriptable Objects/Data/LakeStoreDataSO")]
public class LakeStoreDataSO : TableBase<int>
{
    // 호수인테리어상점id
    [field: SerializeField] public int IDLakeStore { get; private set; }

    // 인테리어id
    [field: SerializeField] public int InteriorId { get; private set; }

    // 테마 이름
    [field: SerializeField] public string LakeThemeName { get; private set; }

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
    public override int GetID() => IDLakeStore;
}
