using System;
using UnityEngine;

public class FilterDropdown : StoreDropdownBase
{
    StoreCat storeType;

    void Start()
    {
        // 초반 드롭다운 옵션 리스트 전달
        optionList = StoreManager.Instance.GetEnumList<InteriorFilter>(Enum.GetValues(typeof(InteriorFilter)));
        
        SetOptions();
    }

    // 상점에 따른 필터 기준 변경
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

    // 필터 드롭다운 값이 변경되면 필터링
    public override void OnDropdownValueChagned(int index)
    {
        foreach(StoreItem item in ItemManager.Instance.displayItems)
        {
            //if(item )
        }
    }


}
