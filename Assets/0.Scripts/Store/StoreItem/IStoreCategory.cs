using System;
using System.Collections.Generic;
using UnityEngine;

public interface IStoreCategory<T> where T : Enum
{
    void AddToCategory(StoreItemBaseSO<T> item);
    void RemoveFromCategory(StoreItemBaseSO<T> item);
}