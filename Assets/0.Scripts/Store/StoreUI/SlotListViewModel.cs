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
    [SerializeField] List<DummyStoreItemSO> itemList = new List<DummyStoreItemSO>();

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

    public event PropertyChangedEventHandler PropertyChanged;


    void Start()
    {
        Debug.Log("[ItemListViewModel] Start");
        
        LoadSlotList();
    }

    // 바뀐 항목 갯수에 따라 아이템 슬롯 풀링해옴
    // 풀링해오면서 storeItemViewModels 리스트에 추가



    // 재정렬 
    // 필터 기준 변경
    public void UpdateCurrentCat(int catIdx)
    {
    // 현재 카테고리 변경
        CurrentCat = (StoreCat)catIdx + 1;
    // 아이템 리스트 업데이트
        LoadSlotList();
    }

    // itemList 갯수 맞춰서 갯수 정해두고
    // 로드
    public void LoadSlotList()
    {
        // test
        storeItemViewModels = itemContents.GetComponentsInChildren<ItemSlotViewModel>().ToList();
        // test-end

        // itemList에 선택된 상점 유형 아이템들로 설정
        // StoreManager 메서드 호출
        // 아니면 이벤트 구독해서 모두 설정


        // 슬롯 프리팹에 모델 넣어주기
        foreach (DummyStoreItemSO item in itemList)
        {
            storeItemViewModels[itemList.IndexOf(item)].SetModel(item);
            Debug.Log("[ItemListViewModel] UpdateSlotList | 슬롯 " + itemList.IndexOf(item));
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
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        filterDropdown.UpdateFilter((Filter)CurrentCat);
        sortDropdown.SetOptions();
    }

}
