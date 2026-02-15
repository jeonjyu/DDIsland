using System.Collections.Generic;
using UnityEngine;

// 상점 항목별 아이템 관리하는 클래스
public class StoreCategory<T> : IStoreItem where T : StoreItemBase
{
    // itemid, 아이템SO
    Dictionary<int, T> _category = new Dictionary<int, T>();

    public Dictionary<int, T> Category => _category;
    public int CategoryCount => _category.Count;

    public void AddToCategory(T item)
    {
        _category.Add(item.ItemDataSO.ItemId, item);
    }

    public ScriptableObject GetStoreItem()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveFromCategory(T item) 
    {
        _category.Remove(item.ItemDataSO.ItemId);
    }
}
