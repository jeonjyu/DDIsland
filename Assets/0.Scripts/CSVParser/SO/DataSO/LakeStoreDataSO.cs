using System;
using UnityEngine;


[CreateAssetMenu(fileName = "LakeStoreDataSO", menuName = "Scriptable Objects/Data/LakeStoreDataSO")]
public class LakeStoreDataSO : TableBase<int>
{
    // 호수인테리어상점id
    [field: SerializeField] public int IDLakeStore { get; private set; }

    // 테마 번호
    [field: SerializeField] public int LakeThemeNumber { get; private set; }

    // 인테리어id1
    [field: SerializeField] public int InteriorId1 { get; private set; }

    // 인테리어id2
    [field: SerializeField] public int InteriorId2 { get; private set; }

    // 테마 이름
    [field: SerializeField] public string LakeThemeName { get; private set; }

    // 테마 설명
    [field: SerializeField] public string LakeThemeDesc { get; private set; }

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
