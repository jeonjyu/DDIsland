using System;
using UnityEngine;


[CreateAssetMenu(fileName = "StringUIDataSO", menuName = "Scriptable Objects/Data/StringUIDataSO")]
public class StringUIDataSO : TableBase<string>
{
    // UI스트링ID
    [field: SerializeField] public string ID { get; private set; }

    // 스트링테이블ID
    [SerializeField] private string iD;
    public string ID_String => LocalizationManager.Instance.GetString(iD);

    // 부모 클래스의 ID 반환 추상 메서드
    public override string GetID() => ID;
}
