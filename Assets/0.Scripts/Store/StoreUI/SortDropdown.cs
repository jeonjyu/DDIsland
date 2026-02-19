using System;
using UnityEngine;
using System.ComponentModel;


public enum StoreSort
{
    [Description("보유")]
    gain = 1,
    [Description("미보유")]
    unGain,
    [Description("가격 높은 순")]
    highToLow,
    [Description("최근 구매 순")]
    recentPurchase,
    [Description("이름 순")]
    name
}

public class SortDropdown : StoreDropdownBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 드롭다운 옵션 리스트 전달
        optionList = StoreManager.Instance.GetEnumList<StoreSort>(Enum.GetValues(typeof(StoreSort)));

        SetOptions();
    }
}
