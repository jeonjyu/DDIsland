using System;
using UnityEngine;

// 스탯 적용 방식
[Serializable]
public enum ApplyType
{
    None = 0,
    Add = 1,      //가산
    Set = 2,      //변경
}

// 스탯 타입
[Serializable]
public enum StatType
{
    None = 0,
    BaseHunger = 1,              //배고픔 수치
    BaseStamina = 2,             //스태미너 수치
    BaseMoveSpeed = 3,           //이동 속도
    BaseFishingSpeed = 4,        //낚시 속도
    StaminaHeal = 5,             //스태미너 회복
}


[CreateAssetMenu(fileName = "CharacterUpgradeDataSO", menuName = "Scriptable Objects/Data/CharacterUpgradeDataSO")]
public class CharacterUpgradeDataSO : TableBase<int>
{
    // 식별자
    [field: SerializeField] public int ID { get; private set; }

    // 스탯 그룹 ID
    [field: SerializeField] public int GroupID { get; private set; }

    // 스탯 적용 방식
    [field: SerializeField] public ApplyType applyType { get; private set; }

    // 스탯 타입
    [field: SerializeField] public StatType statType { get; private set; }

    // 스탯 이름
    [SerializeField] private string statName;
    public string StatName_String => LocalizationManager.Instance.GetString(statName);

    // 레벨
    [field: SerializeField] public int Level { get; private set; }

    // 비용
    [field: SerializeField] public int Cost { get; private set; }

    // 변경값
    [field: SerializeField] public float Value { get; private set; }

    // 확률
    [field: SerializeField] public float Probability { get; private set; }

    // 최대치 여부
    [field: SerializeField] public bool IsMax { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => ID;
}
