using System;
using UnityEngine;


[CreateAssetMenu(fileName = "CharacterDataSO", menuName = "Scriptable Objects/Data/CharacterDataSO")]
public class CharacterDataSO : TableBase<int>
{
    // 캐릭터 ID
    [field: SerializeField] public int ID { get; private set; }

    // 캐릭터 이름
    [SerializeField] private string characterName;
    public string CharacterName_String => LocalizationManager.Instance.GetString(name);

    // 기본 배고픔 수치
    [field: SerializeField] public float BaseHunger { get; private set; }

    // 기본 스태미너수치
    [field: SerializeField] public float BaseStamina { get; private set; }

    // 기본 이동속도
    [field: SerializeField] public float BaseMoveSpeed { get; private set; }

    // 기본 낚시 속도
    [field: SerializeField] public float BaseFishingSpeed { get; private set; }

    // 스태미너 회복 속도
    [field: SerializeField] public float BaseRestSpeed { get; private set; }

    // 외형 이미지 결정 스탯
    [field: SerializeField] public int BaseDoongDoongStat { get; private set; }

    // 외형 그룹 ID
    [field: SerializeField] public int VisualGroupID { get; private set; }

    // 업그레이드 ID
    [field: SerializeField] public int UpgradGroupID { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => ID;
}
