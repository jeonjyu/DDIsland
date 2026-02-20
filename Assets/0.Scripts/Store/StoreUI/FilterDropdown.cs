using System;
using UnityEngine;

enum Filter
{
    InteriorFilter = 1,
    CostumeFilter,
    FishingFilter
}

public class FilterDropdown : StoreDropdownBase
{
    StoreCat storeType;

    void Start()
    {
        // 초반 드롭다운 옵션 리스트 전달
        optionList = StoreManager.Instance.GetEnumList<InteriorFilter>(Enum.GetValues(typeof(InteriorFilter)));
        
        SetOptions();
    }

}
