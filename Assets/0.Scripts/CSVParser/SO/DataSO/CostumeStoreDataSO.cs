using System;
using UnityEngine;

// 타입
[Serializable]
public enum CostumeType
{
    None = 0,
    Head = 1,      //머리
    Body = 2,      //의류
}


[CreateAssetMenu(fileName = "CostumeStoreDataSO", menuName = "Scriptable Objects/Data/CostumeStoreDataSO")]
public class CostumeStoreDataSO : TableBase<int>
{
    // 코스튬상점id
    [field: SerializeField] public int ID { get; private set; }

    // 코스튬id
    [field: SerializeField] public int CostumeId { get; private set; }

    // 타입
    [field: SerializeField] public CostumeType costumeType { get; private set; }

    // 최대 보유 가능 갯수
    [field: SerializeField] public int MaxCount { get; private set; }

    // 판매 가능 여부
    [field: SerializeField] public bool IsSaleable { get; private set; }

    // 구매 가격
    [field: SerializeField] public int PurchasePrice { get; private set; }

    // 판매 가격
    [field: SerializeField] public int SellPrice { get; private set; }

    // UI 미니 이미지 리소스
    [field: SerializeField] public Sprite CostumeImgPath_Sprite { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => ID;
}
