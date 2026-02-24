using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// 드롭다운 기준
// 보유/미보유, 가격, 카테고리(MVP 이후), id
public enum Comparer
{
    Gain,
    Price,
    ItemId
}

// 정렬 enum에 따라 해당 enum을 맨 앞으로 가져옴
// 리스트가 있고 순서를 지키지만 특정한 하나를 1순위로 > 해당 리스트 요소를 널으로 만들고 0 위치에 insert, 빈 거 제거/무시 후 리스트화 

public class SortDropdown : StoreDropdownBase
{
    List<Comparer> comparers = new List<Comparer>();

    //private void Awake()
    //{
    //    Debug.Log("[SortDropdown] Awake");
    //    dropdown = GetComponent<TMP_Dropdown>();
    //}

    void Start()
    {
        // 드롭다운 옵션 리스트 전달
        optionList = StoreManager.Instance.GetEnumList<StoreSort>(Enum.GetValues(typeof(StoreSort)));
        comparers = (Enum.GetValues(typeof(Comparer)) as Comparer[]).ToList();
        SetOptions();
    }

    // 정렬 우선순위 리스트화
    // 정렬 체이닝 돌리기
    public void SortSlots(Comparer comparer)
    {
        Debug.Log("[SortDropdown] 정렬 시작");
        comparers = comparers.OrderBy(x => x).ToList();
        comparers.Remove(comparer);
        comparers.Insert(0, comparer);

        IEnumerable<StoreItem> items = ItemManager.Instance.displayItems;

        foreach (Comparer comp in comparers)
        {
            switch (comp)
            {
                case Comparer.Gain: // 기본적으로 오름차순
                    if((StoreSort)SelectedOption == StoreSort.gain)
                        items = items.AppendOrderBy(x => x.IsGained);
                    else
                        items = items.AppendOrderByDescending(x => x.IsGained);
                    break;
                case Comparer.Price: // 기본적으로 오름차순
                    if ((StoreSort)SelectedOption == StoreSort.highToLow)
                        items = items.AppendOrderBy(x => x.PurchasePrice);
                    else 
                        items = items.AppendOrderByDescending(x => x.PurchasePrice);
                        break;
                case Comparer.ItemId:
                    items = items.AppendOrderBy(x => x.ItemId);
                    break;
            }
            Debug.Log("[SortDropdown] 정렬 기준 " + comp);
        }

        ItemManager.Instance.displayItems = items.ToList();

        Debug.Log("정렬 완료: " + string.Join(", ", ItemManager.Instance.displayItems.Select(x => x.ItemName + "(" + x.ItemId + "):" + x.PurchasePrice)));
    }


    public override void OnDropdownValueChagned(int index)
    {
        base.OnDropdownValueChagned(index);

        // 선택된 옵션에 따라 StoreSort 적용 후 정렬
        switch((StoreSort)(SelectedOption + 1))
        {
            case StoreSort.gain:  case StoreSort.unGain:
                SortSlots(Comparer.Gain);
                storeListViewModel.LoadSlotList();
                break;
            case StoreSort.highToLow: case StoreSort.lowToHigh:
                SortSlots(Comparer.Price);
                storeListViewModel.LoadSlotList();
                break;
        }
    }
}
