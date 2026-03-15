using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StoreListViewModel : MonoBehaviour, INotifyPropertyChanged
{
    [Tooltip("슬롯을 채워줄 아이템 그리드")]
    public GameObject itemContents;

    [Header("슬롯 프리팹")]
    public ItemSlotViewModelBase itemSlot;

    List<ItemSlotViewModelBase> storeItemViewModels = new List<ItemSlotViewModelBase>();
    //[SerializeField] FilterDropdown filterDropdown;
    //[SerializeField] SortDropdown sortDropdown;
    StoreListView view;

    public GridLayoutGroup listGirdLayout;

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
        get => StoreManager.Instance.filterDropdown;
        set
        {
            StoreManager.Instance.filterDropdown = value;
            OnPropertyChanged(nameof(StoreManager.Instance.filterDropdown));
        }
    }

    public SortDropdown Sort
    {
        get => StoreManager.Instance.sortDropdown;
        set
        {
            if (StoreManager.Instance.sortDropdown != value)
            {
                StoreManager.Instance.sortDropdown = value;
                OnPropertyChanged(nameof(StoreManager.Instance.sortDropdown));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void Awake()
    {
        view = GetComponent<StoreListView>();
    }

    public void OnEnable()
    {
        StoreManager.Instance.currentCat = StoreCat.interior;
        UpdateCurrentCat(0);
    }

    public void UpdateCurrentCat(int catIdx)
    {
        //Debug.Log("슬롯 리스트: " + string.Join(", ", ItemManager.Instance.displayItems.Select(x => x.ItemName + "(" + x.IsGained + "):" + x.PurchasePrice)));

        //Debug.Log("[ItemListViewModel] UpdateCurrentCat");

        //Debug.Log((int)CurrentCat);

        if(view.Stores.Count > 0)
            view.SetSelectedCatBtnColor(view.Stores[(int)CurrentCat], false);

        // 현재 카테고리 변경
        CurrentCat = (StoreCat)catIdx;

        if (StoreManager.Instance != null)
            StoreManager.Instance.ChangeLayout();
        else
            Debug.Log("StoreManager가 없음");

        // 카탈로그 변경
        ItemManager.Instance.SetCurrentCategory(CurrentCat);

        if (view.Stores.Count > 0)
            view.SetSelectedCatBtnColor(view.Stores[(int)CurrentCat], true);

        LoadSlotList();
        // 아이템 리스트 업데이트
    }

    // itemList 갯수 맞춰서 갯수 정해두고
    // 로드
    public void LoadSlotList()
    {
        //Debug.Log("[ItemListViewModel] LoadSlotList");
        //Debug.Log("슬롯 리스트: " + string.Join(", ", ItemManager.Instance.displayItems.Select(x => x.ItemName + "(" + x.IsGained + "):" + x.PurchasePrice)));

        if (storeItemViewModels.Count > 0)
            ResetSlotList();

        // 오브젝트풀에서 가져온 뒤 자동으로 storeItemViewModels에 추가하기
        foreach(IStoreItem item in ItemManager.Instance.displayDatas)
        {
            ItemSlotViewModelBase itemViewmodel = ItemManager.Instance.itemSlotPool.Get(ItemManager.Instance.itemSlot);
            itemViewmodel.transform.SetParent(itemContents.transform);
            itemViewmodel.SetModel(item);
            storeItemViewModels.Add(itemViewmodel);
            //Debug.Log("[ItemListViewModel] UpdateSlotList | 슬롯 " + ItemManager.Instance.displayItems.IndexOf(item) + " " + item.ItemName);
        }
        //Debug.Log("슬롯 리스트: " + string.Join(", ", ItemManager.Instance.displayItems.Select(x => x.ItemName + "(" + x.IsGained + "):" + x.PurchasePrice)));

        //Debug.Log("[ItemListViewModel] UpdateSlotList | 슬롯 로딩 완료");
    }

    // itemList 변경되면 그에 맞춰 초기화

    // storeItemPresenters에 있는 model들 null로 만들고
    // 오브젝트 풀로 반납 > 반납 메서드에서 비활성화
    public void ResetSlotList()
    {
        foreach (ItemSlotViewModelBase item in storeItemViewModels)
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



    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        //Debug.Log("[ItemListViewModel] OnPropertyChanged 실행");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
