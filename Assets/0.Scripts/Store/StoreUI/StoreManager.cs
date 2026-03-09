using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject BuyAndSellPanel => buyAndSellPanel;

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
        if (TradeModel != null)
        {

            // 추가할 때
            if (inCount > 0)
            {
                if (TradeItemCount == 0 && TradeModel.IsGained == false)
                {
                    TradeModel.IsGained = true;
                    Debug.Log($"[Storemanager] 새로운 아이템 추가 | IsGained : {TradeModel.IsGained} / TradeItemCount : {TradeItemCount + inCount}");
                    ItemManager.Instance.AddToPlayerItem(TradeModel, currentCat);
                }
            }
                // 감소할 때
            if (inCount < 0)
            {
                if (TradeItemCount + inCount == 0 && TradeModel.IsGained == true)
                {
                    TradeModel.IsGained = false;
                    Debug.Log($"[Storemanager] 아이템 삭제 | IsGained : {TradeModel.IsGained} / TradeItemCount : {TradeItemCount + inCount}"); ItemManager.Instance.RemoveFromPlayerItem(TradeModel, currentCat);
                }
            }

            TradeItemCount += inCount;
        }
        else
        {
            Debug.LogWarning("TradeModel이 없음");
        }
    }

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
