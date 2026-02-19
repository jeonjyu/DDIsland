using System;
using UnityEngine;

public enum Filter
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

    public void UpdateFilter(Filter filter)
    {
        switch (filter)
        {
            case Filter.InteriorFilter:
                optionList = StoreManager.Instance.GetEnumList<InteriorFilter>(Enum.GetValues(typeof(InteriorFilter))); 
                break;
            case Filter.CostumeFilter:
                optionList = StoreManager.Instance.GetEnumList<CostumeFilter>(Enum.GetValues(typeof(CostumeFilter))); 
                break;
            case Filter.FishingFilter:
                optionList = StoreManager.Instance.GetEnumList<FishingFilter>(Enum.GetValues(typeof(FishingFilter))); 
                break;
        }

        SetOptions();
    }
}
