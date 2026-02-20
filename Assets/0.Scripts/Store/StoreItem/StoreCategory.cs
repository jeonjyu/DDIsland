using System.Collections.Generic;
using UnityEngine;

// 상점 항목별 아이템 관리하는 클래스
public class StoreCategory<T> : IStoreCategory<T> where T : System.Enum
{
    [SerializeField] Dictionary<int, IStoreItem> _category = new Dictionary<int, IStoreItem>();

    public void AddToCategory(StoreItemBaseSO<T> item)
    {
        _category.Add(item.ItemId, item);
    }

    public void RemoveFromCategory(StoreItemBaseSO<T> item)
    {
        _category.Remove(item.ItemId);
    }
}
