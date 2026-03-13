using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StoreManager : Singleton<StoreManager>, INotifyPropertyChanged
{ 
    List<Enum> category = new List<Enum>();

    [SerializeField] GameObject storeListPanel;
    [SerializeField] GameObject buyAndSellPanel;
    [SerializeField] public FilterDropdown filterDropdown;
    [SerializeField] public SortDropdown sortDropdown;

    List<GameObject> stores = new List<GameObject>();

    public StoreCat currentCat;

    [SerializeField] private IStoreItem tradeModel;
    ItemSlotViewModel tradeItemSlot;
    [SerializeField] StoreListViewModel storeListVM;

    public GameObject BuyAndSellPanel => buyAndSellPanel;
    public ItemSlotViewModel TradeItemSlot
    {
        get => tradeItemSlot;
        set => tradeItemSlot = value;
    }

    public StoreListViewModel StoreListVM => storeListVM;

    /// <summary>
    /// 현재 아이템 모델
    /// </summary>
    public IStoreItem TradeModel
    {
        get => tradeModel;
        set
        {
            if (tradeModel != value)
            {
                tradeModel = value;
                OnTradeModelChanged(null);
            }
        }
    }

    /// <summary>
    /// 거래 아이템의 현재 개수
    /// </summary>
    public int TradeItemCount
    {
        get => tradeModel.ItemCount;
        set
        {
            if (tradeModel.ItemCount != value)
            {
                tradeModel.ItemCount = value;
                //Debug.Log("[StoreManager] | 아이템 개수 변경 " + TradeItemCount);

                OnTradeModelChanged(nameof(TradeItemCount));
            }
        }
    }

    public bool IsTradeItemGained
    {
        get => tradeModel.IsGained;
        set
        {
            if(tradeModel.IsGained != value)
            {
                tradeModel.IsGained = value;
                Debug.Log(this.name + " 아이템 보유 여부 변경 " + tradeModel.IsGained);
                OnTradeModelChanged(nameof(IsTradeItemGained));
            }
        }
    }

    void Start() 
    {
        // 초기 카테고리 설정
        currentCat = (StoreCat)0;
    }

    // 각 enum에 설정된 Description으로 리스트 반환
    public List<string> GetEnumList<T>(Array optionEnum) where T : Enum
    {
        List<string> lst = new List<string>(optionEnum.Length);

        foreach (T option in optionEnum)
            lst.Add(GetEnumDesc(option));

        return lst;
    }

    // 각 enum에 지정된 description 반환
    public string GetEnumDesc<T>(T value) where T : Enum
    {
        FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
        DescriptionAttribute description = fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;

        return description.Description;
    }

    /// <summary>
    /// 거래 아이템 보유 개수 변경 후 소유 여부, 사용자 소유 아이템 딕셔너리에 추가
    /// </summary>
    /// <param name="inCount">변동할 아이템 수량</param>
    public void ItemCountChanged(int inCount)
    {
        Debug.Log("아이템 개수 변경");
        if (TradeModel != null)
        {

            // 추가할 때
            if (inCount > 0)
            {
                if (TradeItemCount == 0 && TradeModel.IsGained == false)
                {
                    IsTradeItemGained = true;
                    //Debug.Log("아이템 보유 여부 true" + IsTradeItemGained); 
                    ItemManager.Instance.AddToPlayerItem(TradeModel, currentCat);
                    Debug.Log($"[Storemanager] 새로운 아이템 추가 | IsGained : {TradeModel.IsGained} / TradeItemCount : {TradeItemCount + inCount}");
                    //TradeItemSlot.IsGained = IsTradeItemGained;
                }
            }
                // 감소할 때
            if (inCount < 0)
            {
                if (TradeItemCount + inCount == 0 && TradeModel.IsGained == true)
                {
                    IsTradeItemGained = false;
                    //Debug.Log("아이템 보유 여부 false" + IsTradeItemGained);
                    ItemManager.Instance.RemoveFromPlayerItem(TradeModel, currentCat);
                    Debug.Log($"[Storemanager] 아이템 삭제 | IsGained : {TradeModel.IsGained} / TradeItemCount : {TradeItemCount + inCount}");
                }
            }

            TradeItemCount += inCount;
            //IsTradeItemGained = true;
            TradeItemSlot.IsGained = IsTradeItemGained; // TradeModel.IsGained;
            TradeItemSlot.ItemCount = TradeItemCount;
        }
        else
        {
            Debug.LogWarning("TradeModel이 없음");
        }
    }

    // 거래 팝업 활성화에 따라 드롭다운 선택 가능 여부 변경
    public void ChangeDropdownAvailability(bool isAvailable)
    {
        filterDropdown.filterDrop.interactable = isAvailable;
        sortDropdown.sortDrop.interactable = isAvailable;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    // TradeUnitViewModelBase에서 구독
    protected virtual void OnTradeModelChanged([CallerMemberName] string propertyName = null)
    {
        //Debug.Log("[StoreManager] | " + propertyName);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
