using System;
using UnityEngine;


[CreateAssetMenu(fileName = "CharacterVisualDataSO", menuName = "Scriptable Objects/Data/CharacterVisualDataSO")]
public class CharacterVisualDataSO : TableBase<int>
{
    // ID
    [field: SerializeField] public int ID { get; private set; }

    // 캐릭터 그룹
    [field: SerializeField] public int GroupID { get; private set; }

    // 범위 최소치(이상)
    [field: SerializeField] public int MinIndex { get; private set; }

    // 범위 최소치(미만)
    [field: SerializeField] public int MaxIndex { get; private set; }

    // 리소스 경로
    [field: SerializeField] public GameObject ResourcePath_GameObject { get; private set; }

    // 컨셉 리소스 경로
    [field: SerializeField] public Sprite CharacterVisualImgPath_Sprite { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => ID;
}
