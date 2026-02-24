using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FilterDropdown : StoreDropdownBase
{
    Type currentFilter;
    List<Enum> filters;

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
                currentFilter = typeof(InteriorFilter);
                break;
            case Filter.CostumeFilter:
                optionList = StoreManager.Instance.GetEnumList<CostumeFilter>(Enum.GetValues(typeof(CostumeFilter))); 
                currentFilter = typeof(CostumeFilter);
                break;
            case Filter.FishingFilter:
                optionList = StoreManager.Instance.GetEnumList<FishingFilter>(Enum.GetValues(typeof(FishingFilter))); 
                currentFilter = typeof(FishingFilter);
                break;
        }
        Debug.Log($"[FilterDropdown] {currentFilter}");
        filters = Enum.GetValues(currentFilter).Cast<Enum>().ToList();
        SetOptions();
    }

    // 슬롯 필터링하여 해당하는 모델 외에는 displayItems에서 제거
    // 
    public void FilterSlots(int idx)
    {

        ItemManager.Instance.displayItems = ItemManager.Instance.currentCategory.Where(i => /*i.GetFilter().GetType() == currentFilter.GetType() &&*/ i.GetFilter().ToString() == filters[SelectedOption].ToString()).ToList();

        //ItemManager.Instance.displayItems.Clear();
        //List<StoreItem> itemlist = new List<StoreItem>();
        //Debug.Log(filters[idx]);
        //var filterType = ItemManager.Instance.currentCategory.FirstOrDefault().GetType();
        //foreach (var item in ItemManager.Instance.currentCategory)
        //{
        //Debug.Log(item.GetFilter().ToString().Equals(filters[idx].ToString()));

        //    if(item.GetFilter().ToString().Equals(filters[idx].ToString()))
        //    {
        //        itemlist.Add(item);
        //        Debug.Log("[FilterDropdwon] 필터에 해당하는 아이템 : " + item.ItemName);
        //    }
        //        //ItemManager.Instance.displayItems.Add(item);
        //}
        

        if(ItemManager.Instance.displayItems != null)
            Debug.Log("[FilterDropdown] 필터링 완료 : " + string.Join(", ", ItemManager.Instance.displayItems.Select(x => x.ItemName + "(" + x.ItemId + ")" + x.GetFilter().ToString())));
    }

    // 필터 드롭다운 값이 변경되면 필터링
    public override void OnDropdownValueChagned(int index)
    {
        FilterSlots(index);
        storeListViewModel.LoadSlotList();
    }


}
