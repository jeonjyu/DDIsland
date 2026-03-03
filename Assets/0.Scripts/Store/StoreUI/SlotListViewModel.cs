using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class StoreListViewModel : MonoBehaviour, INotifyPropertyChanged
{
    [Tooltip("슬롯을 채워줄 아이템 그리드")]
    [SerializeField] GameObject itemContents;

    [Header("슬롯 프리팹")]
    [SerializeField] ItemSlotViewModel itemSlot;

    List<ItemSlotViewModel> storeItemViewModels = new List<ItemSlotViewModel>();
    [SerializeField] FilterDropdown filterDropdown;
    [SerializeField] SortDropdown sortDropdown;
    public StoreCat CurrentCat
    {
        get => StoreManager.Instance.currentCat;
        set
        {
            StoreManager.Instance.currentCat = value;
            OnPropertyChanged(null);
        }
    }

    public FilterDropdown Filter
    {
        get => filterDropdown;
        set
        {
            filterDropdown = value;
            OnPropertyChanged(nameof(filterDropdown));
        }
    }

    public SortDropdown Sort
    {
        get => sortDropdown;
        set
        {
            if (sortDropdown != value)
            {
                sortDropdown = value;
                OnPropertyChanged(nameof(sortDropdown));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;


    //void Start()
    //{
    //    UpdateCurrentCat(0);
    //}

    // 바뀐 항목 갯수에 따라 아이템 슬롯 풀링해옴
    // 풀링해오면서 storeItemViewModels 리스트에 추가

    public void OnEnable()
    {
        StoreManager.Instance.currentCat = StoreCat.interior;
        UpdateCurrentCat(0);

        //ItemManager.Instance.SetCurrentCategory();
    }

    public void UpdateCurrentCat(int catIdx)
    {
        Debug.Log("슬롯 리스트: " + string.Join(", ", ItemManager.Instance.displayItems.Select(x => x.ItemName + "(" + x.IsGained + "):" + x.PurchasePrice)));

        Debug.Log("[ItemListViewModel] UpdateCurrentCat");

        // 현재 카테고리 변경
        CurrentCat = (StoreCat)catIdx;

        // 카탈로그 변경
        ItemManager.Instance.SetCurrentCategory(CurrentCat);

        // 아이템 리스트 업데이트
        LoadSlotList();
    }

    // itemList 갯수 맞춰서 갯수 정해두고
    // 로드
    public void LoadSlotList()
    {
        Debug.Log("[ItemListViewModel] LoadSlotList");
        //Debug.Log("슬롯 리스트: " + string.Join(", ", ItemManager.Instance.displayItems.Select(x => x.ItemName + "(" + x.IsGained + "):" + x.PurchasePrice)));

        if (storeItemViewModels.Count > 0)
            ResetSlotList();

        // 오브젝트풀에서 가져온 뒤 자동으로 storeItemViewModels에 추가하기
        foreach(StoreItem item in ItemManager.Instance.displayItems)
        {
            ItemSlotViewModel itemViewmodel = ItemManager.Instance.itemSlotPool.Get(itemSlot);
            itemViewmodel.transform.SetParent(itemContents.transform);
            itemViewmodel.SetModel(item);
            storeItemViewModels.Add(itemViewmodel);
            //Debug.Log("[ItemListViewModel] UpdateSlotList | 슬롯 " + ItemManager.Instance.displayItems.IndexOf(item) + " " + item.ItemName);
        }
        Debug.Log("슬롯 리스트: " + string.Join(", ", ItemManager.Instance.displayItems.Select(x => x.ItemName + "(" + x.IsGained + "):" + x.PurchasePrice)));

        //Debug.Log("[ItemListViewModel] UpdateSlotList | 슬롯 로딩 완료");
    }

    // itemList 변경되면 그에 맞춰 초기화

    // storeItemPresenters에 있는 model들 null로 만들고
    // 오브젝트 풀로 반납 > 반납 메서드에서 비활성화
    public void ResetSlotList()
    {
        foreach (ItemSlotViewModel item in storeItemViewModels)
        {
            if (item == null)
            {
                Debug.Log("슬롯이 비었음");
                return;
            }
            else
            {
                item.Reset();
                item.transform.SetParent(ItemManager.Instance.itemSlotPool.transform);
                ItemManager.Instance.itemSlotPool.Release(item);
            }
        }

        storeItemViewModels.Clear();
        //Debug.Log("[ItemListViewModel] ResetSlotList | 슬롯 반납 완료");
    }

    // 슬롯 모델 바꾸려면 SetModel로 변경해줘야 함

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        //Debug.Log("[ItemListViewModel] OnPropertyChanged 실행");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //filterDropdown.UpdateFilter((Filter)CurrentCat);
        ////filterDropdown.FilterSlots(0); // 카테고리가 변할때만 
        //sortDropdown.SetOptions();
        //sortDropdown.ApplySortPriority();
        //LoadSlotList();
    }

}
