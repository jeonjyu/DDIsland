using UnityEngine;
using System;

public abstract class StoreItem<T> : IStoreItem where T : TableBase<int>
{
    protected T _data;

    //field
    [SerializeField] protected bool _isGained;
    [SerializeField] protected int _itemCount;

    //property

    public abstract int ID { get; }
    public abstract int ObjectId { get; }
    public abstract bool IsGained { get; set; }
    public abstract bool IsSaleable { get; }
    public abstract int MaxCount { get; } 
    public abstract int ItemCount { get; set; } 
    public abstract int PurchasePrice { get; } 
    public abstract int SellPrice { get; } 
    public abstract string ItemName { get; } 
    public abstract string ItemDesc { get; } 
    public abstract Sprite ImgSprite { get; }
    public abstract Enum Filter { get; }

    public StoreItem(T data)
    {
        _data = data;
        Debug.Log(_data.GetID());
    }
}
