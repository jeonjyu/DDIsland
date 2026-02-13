using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestSlotRenderer : MonoBehaviour
{
    [SerializeField] GameObject itemContents;
    [SerializeField] List<DummyStoreItemSO> itemList = new List<DummyStoreItemSO> ();
    List<StoreItemPresenter> storeItemPresenters = new List<StoreItemPresenter>();

    void Start()
    {
        Debug.Log("[TestSlotRenderer] Start");

        storeItemPresenters = itemContents.GetComponentsInChildren<StoreItemPresenter>().ToList();

        int pIdx = 0;
        foreach (DummyStoreItemSO item in itemList)
        {
            storeItemPresenters[pIdx].SetModel(item);
            if(pIdx < storeItemPresenters.Count)
                pIdx++;
            Debug.Log("[TestSlotRenderer] Start | 슬롯 " + pIdx);
        }

        Debug.Log("[TestSlotRenderer] Start | 슬롯 로딩 완료");
    }
}
