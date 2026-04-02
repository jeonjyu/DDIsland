using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecoInventoryManager : Singleton<DecoInventoryManager>
{
    [SerializeField] List<LakeInvenSlot> invenList = new List<LakeInvenSlot>();

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
          //if (item.IsGained && item.ItemCount > 0)
            if (item.IsGained)
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
    // 파베 로드용 
    public void SetInven(List<LakeInvenSlot> loaded)
    {
        invenList = loaded ?? new List<LakeInvenSlot>();
    }


    // 배치 시 수량 감소
    public void UseItem(int itemId)
    {
        var playerItem = FindPlayerItem(itemId);
        if (playerItem == null) return;

        playerItem.ItemCount--;
        //if (playerItem.ItemCount <= 0)
        //    playerItem.IsGained = false;

        var slot = FindSlot(itemId);
        if (slot != null)
            slot.quantity--;
    }

    // 회수 시 수량 복구
    public void RestoreItem(int itemId)
    {
        var playerItem = FindPlayerItem(itemId);
        if (playerItem == null)
        {
            var originalItem = ItemManager.Instance.storeDatas[StoreCat.interior].Items.FirstOrDefault(x => x.ObjectId == itemId); // ObjectId로 찾기
            if (originalItem != null)
            {
                originalItem.ItemCount = 1;
                originalItem.IsGained = true;
                ItemManager.Instance.AddToPlayerItem(originalItem, StoreCat.interior);

                invenList.Add(new LakeInvenSlot { itemId = itemId, quantity = 1 });
            }
            return;
        }


        // 고정물(Fix)은 최대 1개까지만 보유
        try
        {
            var interiorData = DataManager.Instance.DecorationDatabase.InteriorData[itemId];
            if (interiorData != null && interiorData.interior_itemType == Interior_ItemType.Fix)
            {
                if (playerItem.ItemCount >= 1) return; // 이미 1개면 증가 안 함
            }
        }
        catch { }

        playerItem.ItemCount++;
        if (!playerItem.IsGained)
            playerItem.IsGained = true;

        var slot = FindSlot(itemId);
        if (slot != null)
            slot.quantity++;
    }
    // 전체회수/스냅샷 복원용
    public void SetItemCount(int itemId, int count)
    {
        var playerItem = FindPlayerItem(itemId);
        if (playerItem == null) return;

        playerItem.ItemCount = count;
        playerItem.IsGained = count > 0; // 수량이 0이면 미소유 처리 or 
                                         // TODO: 나중에 파베/도감/상점 등에서 터진다면 아이템 데이터는 남게 아래 주석으로 교체
                                         // if (count > 0) playerItem.IsGained = true; 


        var slot = FindSlot(itemId);
        if (slot != null)
            slot.quantity = count;
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
    // 리스트에서 슬롯 찾기
    LakeInvenSlot FindSlot(int itemId)
    {
        for (int i = 0; i < invenList.Count; i++)
        {
            if (invenList[i].itemId == itemId)
                return invenList[i];
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