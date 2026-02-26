using System;
using UnityEngine;

// "사는 공간
[Serializable]
public enum FishType
{
    None = 0,
    Lake = 1,     //민물
    Sea = 2,      //바다
}

// "등급
[Serializable]
public enum Grade
{
    None = 0,
    Normal = 1,         //일반
    Rare = 2,           //고급
    Epic = 3,           //희귀
    Legendary = 4,      //전설
}

// "등장 계절
[Flags, Serializable]
public enum ArriveSeason : byte
{
    None = 0,
    Spring = 1,      //봄
    Summer = 2,      //여름
    Autumn = 4,      //가을
    Winter = 8,      //겨울
}


[CreateAssetMenu(fileName = "FishDataSO", menuName = "Scriptable Objects/Data/FishDataSO")]
public class FishDataSO : TableBase<int>
{
    // id
    [field: SerializeField] public int ID { get; private set; }

    // 이름
    public string FishName_String => LocalizationManager.Instance.GetString("FishName_String");

    // 설명
    public string FishDesc_String => LocalizationManager.Instance.GetString("FishDesc_String");

    // "사는 공간
    [field: SerializeField] public FishType fishType { get; private set; }

    // "등급
    [field: SerializeField] public Grade gradeType { get; private set; }

    // "등장 계절
    [field: SerializeField] public ArriveSeason arriveseasonType { get; private set; }

    // 특수 조건
    [field: SerializeField] public bool IsSpecial { get; private set; }

    // 군집 알고리즘 사용 여부
    [field: SerializeField] public bool CrowdingAlgorithm { get; private set; }

    // 최소 길이(cm)
    [field: SerializeField] public float MinLength { get; private set; }

    // 최대 길이(cm)
    [field: SerializeField] public float MaxLength { get; private set; }

    // 길이 단위(cm)
    [field: SerializeField] public float Measure { get; private set; }

    // 판매 시 금액(원)
    [field: SerializeField] public int Price { get; private set; }

    // 스프라이트 
    [field: SerializeField] public Sprite FishImgPath_Sprite { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => ID;
}
