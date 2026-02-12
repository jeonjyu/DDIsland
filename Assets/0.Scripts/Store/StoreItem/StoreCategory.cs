using System.Collections.Generic;
using UnityEngine;

// 상점 항목별 아이템 관리하는 클래스
public class StoreCategory<T> : IStoreItem where T : StoreItemBase
{
    Dictionary<int, T> _category = new Dictionary<int, T>();

    public Dictionary<int, T> Category => _category;
    public int CategoryCount => _category.Count;

    public void AddToCategory(T item)
    {
        _category.Add(item.ItemId, item);
    }

    public void RemoveFromCategory(T item) 
    {
        _category.Remove(item.ItemId);
    }
}
