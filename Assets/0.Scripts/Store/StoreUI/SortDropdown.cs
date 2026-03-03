using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// to-do: 모든 정렬 기준에서 사용되는 기준과 드롭다운에서 선택될 때만 사용되는 기준이 따로 존재, 이부분 분리하여 다시 작업할 것

// 드롭다운 기준
// 보유/미보유, 가격, 카테고리(MVP 이후), id
public enum Comparer
{
    Gain,
    Price,
    ItemId
}


public class SortDropdown : StoreDropdownBase
{
    List<Comparer> comparers = new List<Comparer>();

    void Start()
    {
        // 드롭다운 옵션 리스트 전달
        comparers = (Enum.GetValues(typeof(Comparer)) as Comparer[]).ToList();
        optionList = StoreManager.Instance.GetEnumList<StoreSort>(Enum.GetValues(typeof(StoreSort)));
        SetOptions();
    }

    public override void SetOptions()
    {
        base.SetOptions();
        if (optionList.Count > 0)
        {
            dropdown.value = 1;
            //dropdown.captionText.text = optionList[1];
        }
    }

    // 정렬 우선순위 리스트화
    // 정렬 체이닝 돌리기
    public void SortSlots(Comparer comparer)
    {
        // 정렬 enum에 따라 해당 enum을 맨 앞으로 가져옴
        // 리스트가 있고 순서를 지키지만 특정한 하나를 1순위로 >
        // 해당 리스트 요소를 널으로 만들고 0 위치에 insert, 빈 거 제거/무시 후 리스트화 
        //Debug.Log("[SortDropdown] 정렬 시작");
        comparers = comparers.OrderBy(x => x).ToList();
        comparers.Remove(comparer);
        comparers.Insert(0, comparer);

        IEnumerable<StoreItem> items = ItemManager.Instance.displayItems;

        //Debug.Log("[SortDropdown] SortSlots | SelectedOption 1 : " + (StoreSort)SelectedOption);

        foreach (Comparer comp in comparers)
        {
            switch (comp)
            {
                case Comparer.Gain: // 기본적으로 오름차순
                    if ((StoreSort)SelectedOption == StoreSort.gain)
                        items = items.AppendOrderByDescending(x => x.IsGained);
                    else
                        items = items.AppendOrderBy(x => x.IsGained);
                    break;
                case Comparer.Price: // 기본적으로 오름차순
                    if ((StoreSort)SelectedOption == StoreSort.highToLow)
                        items = items.AppendOrderByDescending(x => x.PurchasePrice);
                    else 
                        items = items.AppendOrderBy(x => x.PurchasePrice);
                    break;
                //case Comparer.Name:
                //    items = items.AppendOrderBy(x => x.ItemName);
                //    break;
                case Comparer.ItemId:
                    items = items.AppendOrderBy(x => x.ItemId);
                    break;
            }
        }
        ItemManager.Instance.displayItems = items.ToList();
        //Debug.Log("[SortDropdown] SortSlots | SelectedOption 2 : " + (StoreSort)SelectedOption);

        //Debug.Log("정렬 기준 : " + string.Join(" > ", comparers.Select(x => x)));
        //Debug.Log("정렬 완료: " + string.Join(", ", ItemManager.Instance.displayItems.Select(x => x.ItemName + "(" + x.IsGained + "):" + x.PurchasePrice)));
    }

    public void ApplySortPriority()
    {
        //Debug.Log("[SortDropdown] ApplySortPriority | SelectedOption : " + (StoreSort)SelectedOption);
        // 선택된 옵션에 따라 StoreSort 적용 후 정렬
        switch ((StoreSort)(SelectedOption))
        {
            case StoreSort.gain:
            case StoreSort.unGain:
                SortSlots(Comparer.Gain);
                break;
            case StoreSort.highToLow:
            case StoreSort.lowToHigh:
                SortSlots(Comparer.Price);
                break;
                //case StoreSort.name:
                //    SortSlots(Comparer.Name);
                //    storeListViewModel.LoadSlotList();
                //    break;
        }
    }

    public override void OnDropdownValueChagned(int index)
    {
        base.OnDropdownValueChagned(index);
        ApplySortPriority();
        storeListViewModel.LoadSlotList();
    }
}
