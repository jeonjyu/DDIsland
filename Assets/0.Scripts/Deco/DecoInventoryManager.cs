using System.Collections.Generic;
using UnityEngine;

public class DecoInventoryManager : Singleton<DecoInventoryManager>
{
    List<LakeInvenSlot> invenList = new List<LakeInvenSlot>();

    protected override void Awake()
    {
        base.Awake(); 
    }
    //void Start()
    //{
    //    ItemManager.Instance.OnPlayerItemAdded += OnItemAdd;
    //    ItemManager.Instance.OnPlayerItemRemoved += OnItemRemove;
    //}

    public List<LakeInvenSlot> GetInven()
    {
        invenList.Clear();

        if (!ItemManager.Instance.playerItemDatas.ContainsKey(StoreCat.interior))
            return invenList;

        var playerItems = ItemManager.Instance.playerItemDatas[StoreCat.interior].Items;

        foreach (var item in playerItems)
        {
            if (item.IsGained && item.ItemCount > 0)
            {
                invenList.Add(new LakeInvenSlot
                {
                    itemId = item.ObjectId,
                    quantity = item.ItemCount
                });
            }
        }

        return invenList;
    }

    public void SetInven(List<LakeInvenSlot> loaded)
    {
        invenList = loaded ?? new List<LakeInvenSlot>();
    }


    // 배치 시 수량 감소
    public void UseItem(int itemId)
    {
        var item = FindPlayerItem(itemId);
        if (item == null) return;

        item.ItemCount--;
        if (item.ItemCount <= 0)
            item.IsGained = false;
    }

    // 회수 시 수량 복구
    public void RestoreItem(int itemId)
    {
        var item = FindPlayerItem(itemId);
        if (item == null) return;

        item.ItemCount++;
        if (!item.IsGained)
            item.IsGained = true;
    }
    // 데이터에서 아이템 찾기
    IStoreItem FindPlayerItem(int itemId)
    {
        if (!ItemManager.Instance.playerItemDatas.ContainsKey(StoreCat.interior))
            return null;

        foreach (var item in ItemManager.Instance.playerItemDatas[StoreCat.interior].Items)
        {
            if (item.ObjectId == itemId)
                return item;
        }
        return null;
    }

    //public void OnItemAdd(IStoreItem item, StoreCat cat)
    //{
    //    if (cat != StoreCat.interior) return;

    //    for (int i = 0; i < invenList.Count; i++)
    //    {
    //        if (invenList[i].itemId == item.ObjectId)
    //        {
    //            invenList[i].quantity = item.ItemCount;
    //            return;
    //        }
    //    }

    //    invenList.Add(new LakeInvenSlot
    //    {
    //        itemId = item.ObjectId,
    //        quantity = item.ItemCount
    //    });
    //}

    //public void OnItemRemove(IStoreItem item, StoreCat cat)
    //{
    //    if (cat != StoreCat.interior) return;

    //    for (int i = 0; i < invenList.Count; i++)
    //    {
    //        if (invenList[i].itemId == item.ObjectId)
    //        {
    //            if (item.ItemCount <= 0)
    //                invenList.RemoveAt(i);
    //            else
    //                invenList[i].quantity = item.ItemCount;
    //            return;
    //        }
    //    }
    //}
}