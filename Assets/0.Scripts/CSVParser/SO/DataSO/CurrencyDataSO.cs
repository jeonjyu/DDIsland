using System;
using UnityEngine;


[CreateAssetMenu(fileName = "CurrencyDataSO", menuName = "Scriptable Objects/Data/CurrencyDataSO")]
public class CurrencyDataSO : TableBase<int>
{
    // id
    [field: SerializeField] public int ID { get; private set; }

    // 재화 이름
    [SerializeField] private string currencyName;
    public string CurrencyName_String => LocalizationManager.Instance.GetString(currencyName);

    // 재화 이미지
    [field: SerializeField] public Sprite CurrencyImgPath_Sprite { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => ID;
}
