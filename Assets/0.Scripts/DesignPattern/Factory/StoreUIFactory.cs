using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StoreUIFactory : MonoBehaviour
{
    [SerializeField] StoreProductDatabase stores;
    [SerializeField] GameObject storeUI;
    // 아이템 목록 레이아웃 프리팹 리스트
    List<StoreProduct> Products => stores.storeProducts;
    Dictionary<StoreCat, GameObject> popups = new Dictionary<StoreCat, GameObject>();
    List<TradeViewModelBase> popupList = new List<TradeViewModelBase>();

    void Awake()
    {
        StoreManager.Instance.storeUIFactory = this; 
        popupList = GetComponentsInChildren<TradeViewModelBase>(true).ToList();
        popups.Clear();
     
        foreach (StoreProduct product in Products)
        {
            TradeViewModelBase pop = popupList.Find(match: x => x.gameObject.ToString() == product.TradePopup.ToString());
            popups.Add(product.cat, pop.gameObject);
        }
    }

    // 상점 변경시 아이템 목록 레이아웃, 슬롯 변경해주는 메서드
    public void GetLayout(GridLayoutGroup targetGridLayout, StoreCat storeCat)
    {
        LayoutValue gridLayout = Products.Find((x) => x.cat == storeCat).gridLayout;

        targetGridLayout.constraintCount = gridLayout.constraintCount;
        targetGridLayout.padding = gridLayout.padding;
        targetGridLayout.cellSize = gridLayout.cellSize;
        targetGridLayout.childAlignment = gridLayout.alignorder;

        LayoutRebuilder.ForceRebuildLayoutImmediate(targetGridLayout.GetComponent<RectTransform>());
    }

    public ItemSlotViewModelBase GetItemSlot(StoreCat storeCat)
    {
        //GameObject itemSlot = Products.Find((x) => x.cat == storeCat).itemSlot;
        return Products.Find((x) => x.cat == storeCat).itemSlot;
    }

    public GameObject GetPopupPanel(StoreCat storeCat) 
    {
        return popups[storeCat];
    }
}
