using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StoreManager : Singleton<StoreManager>, INotifyPropertyChanged
{
    #region field
    List<Enum> category = new List<Enum>();

    [SerializeField] GameObject storeListPanel;
    [SerializeField] TradeViewModelBase buyAndSellPanel;
    [SerializeField] public FilterDropdown filterDropdown;
    [SerializeField] public SortDropdown sortDropdown;

    List<GameObject> stores = new List<GameObject>();

    public StoreCat currentCat = 0;

    [SerializeField] private IStoreItem tradeModel;
    ItemSlotViewModelBase tradeItemSlot;
    [SerializeField] StoreListViewModel storeListVM;
    public StoreUIFactory storeUIFactory;

    public event PropertyChangedEventHandler PropertyChanged;

    public TradePopupBase tradePopup;

    #endregion

    #region properties
    public TradeViewModelBase BuyAndSellPanel => buyAndSellPanel;
    public ItemSlotViewModelBase TradeItemSlot
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
                Debug.Log("[StoreManager] 아이템 개수 변경 " + TradeItemCount);
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
                Debug.Log("[StoreManager] 아이템 보유 여부 변경 " + IsTradeItemGained);
                OnTradeModelChanged(nameof(IsTradeItemGained));
            }
        }
    }
    #endregion

    protected override void Awake() 
    {
        base.Awake();
    }

    /// <summary>
    /// 거래 아이템 보유 개수 변경 후 소유 여부, 사용자 소유 아이템 딕셔너리에 추가
    /// </summary>
    /// <param name="inCount">변동할 아이템 수량</param>
    public void ItemCountChanged(int inCount)
    {
        if (TradeModel != null)
        {

            // 추가할 때
            if (inCount > 0)
            {
                if (TradeItemCount == 0 && TradeModel.IsGained == false)
                {
                    IsTradeItemGained = true;
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
                    ItemManager.Instance.RemoveFromPlayerItem(TradeModel, currentCat);
                    Debug.Log($"[Storemanager] 아이템 삭제 | IsGained : {TradeModel.IsGained} / TradeItemCount : {TradeItemCount + inCount}");
                }
            }

            Debug.Log("[StoreManager] 아이템 정보 변경됨 ");
            TradeItemCount += inCount;
            TradeItemSlot.IsGained = IsTradeItemGained;
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
        // 첫실행시 필터 드롭다운이 없어 return시킴
        if (filterDropdown.filterDrop == null)
            return;
        
        filterDropdown.filterDrop.interactable = isAvailable;
        sortDropdown.sortDrop.interactable = isAvailable;
    }

    public void ChangeLayout()
    {
        storeUIFactory.GetLayout(storeListVM.listGirdLayout, currentCat);
        buyAndSellPanel = storeUIFactory.GetPopupPanel(currentCat);
        ItemManager.Instance.itemSlot = storeUIFactory.GetItemSlot(currentCat);
        ChangeDropdownByCategory();
    }

    // TradeUnitViewModelBase에서 구독
    protected virtual void OnTradeModelChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void ChangeDropdownByCategory()
    {
        //Debug.Log("카테고리에 따라 드롭다운 사용 가능 여부 변경됨 : " + currentCat);
        // 카테고리에 따라 드롭다운 사용 가능 여부 변경
        switch (currentCat)
        {
            case StoreCat.interior:
            case StoreCat.costume:
            case StoreCat.fishing:
                ChangeDropdownAvailability(true);
                break;
            case StoreCat.recipe:
                sortDropdown.sortDrop.interactable = true;
                break;
            case StoreCat.lake:
                ChangeDropdownAvailability(false);
                break;
        }
    }
}
