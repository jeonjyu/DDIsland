using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerItemDatabase : StoreItemDatabase
{

    // Item을 모두 탐색해서 플레이어 보유 아이템에 저장하는 기능
    public void SaveGainedItem(List<IStoreItem> items)
    {
        foreach(var item in items.Where(x => x.IsGained || x.IsDefault).ToList())
        {
            Debug.Log($"[PlayerItemDatabase] SaveGainedItem | {item.ItemName} 보유 여부 : {item.IsGained} | 기본 제공 여부 : {item.IsDefault} | 아이템 개수 : {item.ItemCount}");
            _itemDatas.Add(item);
        }

    }
}
