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
    public TMP_Dropdown sortDrop;

    private void Awake()
    {
        sortDrop = GetComponent<TMP_Dropdown>();
    }

    void Start()
    {
        // 드롭다운 옵션 리스트 전달
        comparers = (Enum.GetValues(typeof(Comparer)) as Comparer[]).ToList();
        //optionList = DescriptionExtracter.GetFilterEnumList<StoreSort>(Enum.GetValues(typeof(StoreSort)));
        SetOptions();
    }

    public override void SetOptions()
    {
        //base.SetOptions();
        //foreach (var option in optionList)
        //{
        //    Debug.Log(option.ToString());
        //    dropdown.options.Add(new TMP_Dropdown.OptionData(option.ToString()));
        //}

        if (optionList.Count > 0)
        {
            dropdown.value = 0;
            //dropdown.captionText.text = optionList[1];
        }
    }

    // 정렬 우선순위 리스트화
    // 정렬 체이닝 돌리기
    public void SortSlots(Comparer comparer)
    {
        comparers = comparers.OrderBy(x => x).ToList();
        comparers.Remove(comparer);
        comparers.Insert(0, comparer);
        // todo : 이부분 리스트 재할당 등 확인해보기

        // 여기서부터 정렬 기준에 맞춰 아이템 정렬
        IEnumerable<IStoreItem> items = ItemManager.Instance.displayDatas;


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
                    items = items.AppendOrderBy(x => x.ID);
                    break;
            }
        }
        ItemManager.Instance.displayDatas = items.ToList();

        //Debug.Log("정렬 기준 : " + string.Join(" > ", comparers.Select(x => x)));
        //Debug.Log("정렬 완료: " + string.Join(", ", ItemManager.Instance.displayItems.Select(x => x.ItemName + "(" + x.IsGained + "):" + x.PurchasePrice)));
    }

    public void ApplySortPriority()
    {
        //Debug.Log("정렬");
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
            default:
                SortSlots(Comparer.Gain);
                break;
        }
    }

    public override void OnDropdownValueChagned(int index)
    {
        base.OnDropdownValueChagned(index);
        ApplySortPriority();
        storeListViewModel.LoadSlotList();
    }
}
