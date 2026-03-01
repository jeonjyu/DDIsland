using System;
using UnityEngine;

// 음식 등급
[Serializable]
public enum FoodRateType
{
    None = 0,
    Normal = 1,         //일반
    Rare = 2,           //고급
    Epic = 3,           //희귀
    Legendary = 4,      //전설
}

// 섭취시 효과
[Flags, Serializable]
public enum FoodEffectType : byte
{
    None = 0,
    HungerBuff = 1,                //배고픔 지수 증가
    DoongDoongBuff = 2,            //둥둥 지수 증가
}


[CreateAssetMenu(fileName = "FoodDataSO", menuName = "Scriptable Objects/Data/FoodDataSO")]
public class FoodDataSO : TableBase<int>
{
    // 음식 id
    [field: SerializeField] public int ID { get; private set; }

    // 음식 이름
    [SerializeField] private string foodName;
    public string FoodName_String => LocalizationManager.Instance.GetString(foodName);

    // 음식 설명
    [SerializeField] private string foodDesc;
    public string FoodDesc_String => LocalizationManager.Instance.GetString(foodDesc);

    // 레시피 구매 가격
    [field: SerializeField] public int PurchasePrice { get; private set; }

    // 음식 등급
    [field: SerializeField] public FoodRateType foodrateType { get; private set; }

    // 주재료
    [field: SerializeField] public int MainIngredient { get; private set; }

    // 부재료
    [field: SerializeField] public int SubIngredient { get; private set; }

    // 섭취시 효과
    [field: SerializeField] public FoodEffectType foodeffectType { get; private set; }

    // 배고픔 지수 증가 수치
    [field: SerializeField] public float HungerBuffRate { get; private set; }

    // 둥둥 지수 증가 수치
    [field: SerializeField] public int DoongDoongBuffRate { get; private set; }

    // 음식 리소스
    [field: SerializeField] public Sprite FoodImgPath_Sprite { get; private set; }

    // 음식 3d리소스
    [field: SerializeField] public GameObject FoodIconPath_GameObject { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => ID;
}
