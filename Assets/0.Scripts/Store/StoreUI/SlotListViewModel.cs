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

// test
    //[SerializeField] List<StoreItem> itemList = new List<StoreItem>();

    List<ItemSlotViewModel> storeItemViewModels = new List<ItemSlotViewModel>();
    [SerializeField] FilterDropdown filterDropdown;
    [SerializeField] SortDropdown sortDropdown;
    public StoreCat CurrentCat
    {
        get => StoreManager.Instance.currentCat;
        set
        {
            if(value != StoreManager.Instance.currentCat)
            {
                StoreManager.Instance.currentCat = value;
                OnPropertyChanged(null);
            }
        }
    }

    public FilterDropdown Filter
    {
        get => filterDropdown;
        set
        {
            filterDropdown = value;
            OnPropertyChanged(null);
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
                OnPropertyChanged(null);
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;


    void Start()
    {
        Debug.Log("[ItemListViewModel] Start");

        UpdateCurrentCat(1);
    }

    // 바뀐 항목 갯수에 따라 아이템 슬롯 풀링해옴
    // 풀링해오면서 storeItemViewModels 리스트에 추가

    public void OnEnable()
    {
        StoreManager.Instance.currentCat = StoreCat.costume;
        CurrentCat = StoreCat.interior;
        ItemManager.Instance.SetCurrentCategory();
    }


    public void UpdateCurrentCat(int catIdx)
    {
    // 현재 카테고리 변경
        CurrentCat = (StoreCat)catIdx;

        // 카탈로그 변경
        ItemManager.Instance.SetCurrentCategory(CurrentCat);

        // test 
        //ItemManager.Instance.displayItems.Clear();
        //foreach (IStoreItem item in ItemManager.Instance._currentCategory.Values.ToList())
        //{
        //    ItemManager.Instance.displayItems.Add(item as StoreItem);
        //    Debug.Log($"{(item as StoreItem).ItemName}");
        //}
        // test - end 

        // 필터 기준 변경
        // 재정렬



        // 아이템 리스트 업데이트
        Debug.Log("[StoreListViewModel] UpdateCurrentCat LoatSlotList 시작");
        LoadSlotList();
    }

    // itemList 갯수 맞춰서 갯수 정해두고
    // 로드
    public void LoadSlotList()
    {
        // test
        storeItemViewModels = itemContents.GetComponentsInChildren<ItemSlotViewModel>().ToList();
        // 오브젝트풀에서 가져온 뒤 자동으로 storeItemViewModels에 추가하기

        ResetSlotList();
        // test-end

        // itemList에 선택된 상점 유형 아이템들로 설정
        // StoreManager 메서드 호출
        // 아니면 이벤트 구독해서 모두 설정

        // 아이템 매니저의 아이템 리스트 
        // 

        // 슬롯 프리팹에 모델 넣어주기
        foreach (StoreItem item in ItemManager.Instance.displayItems)
        {
            storeItemViewModels[ItemManager.Instance.displayItems.IndexOf(item)].SetModel(item);
            //Debug.Log("[ItemListViewModel] UpdateSlotList | 슬롯 " + ItemManager.Instance.displayItems.IndexOf(item));
        }

        Debug.Log("[ItemListViewModel] UpdateSlotList | 슬롯 로딩 완료");
    }

    // itemList 변경되면 그에 맞춰 초기화

    // storeItemPresenters에 있는 model들 null로 만들고
    // 오브젝트 풀로 반납 > 반납 메서드에서 비활성화
    public void ResetSlotList()
    {
        foreach (var item in storeItemViewModels)
        {
            item.Reset();
        }

        Debug.Log("[ItemListViewModel] ResetSlotList | 슬롯 반납 완료");
    }

    // 슬롯 모델 바꾸려면 SetModel로 변경해줘야 함

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        Debug.Log("[ItemListViewModel] OnPropertyChanged 실행");

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        filterDropdown.UpdateFilter((Filter)CurrentCat);
        sortDropdown.SetOptions();
        LoadSlotList();
    }

}
