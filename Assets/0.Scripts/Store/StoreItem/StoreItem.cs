using UnityEngine;
using System;

/// <summary>
/// 상점 아이템 관리를 위해 전반적으로 사용되는 아이템 클래스
/// 아이템 항목별로 해당 클래스 상속받아서 사용됨
/// </summary>
/// <typeparam name="T">아이템 아이디</typeparam>
public abstract class StoreItem<T> : IStoreItem where T : TableBase<int>
{
    protected T _data;

    //field
    [SerializeField] protected bool _isGained;
    [SerializeField] protected int _itemCount;

    //property

    public abstract int ID { get; }
    public virtual int ObjectId { get; }
    public abstract bool IsGained { get; set; }
    public virtual bool IsSaleable { get; }
    public abstract bool IsDefault { get; }
    public virtual int MaxCount { get; } 
    public virtual int ItemCount { get; set; } 
    public abstract int PurchasePrice { get; } 
    public virtual int SellPrice { get; } 
    public abstract string ItemName { get; } 
    public abstract string ItemDesc { get; } 
    public virtual string MainIngName { get; }
    public virtual string SubIngName { get; }
    public abstract Sprite ImgSprite { get; }
    public virtual Enum Filter { get; }
    public virtual Sprite MainIngSprite { get; }
    public virtual Sprite SubIngSprite { get; }

    public StoreItem(T data)
    {
        _data = data;
        if (IsDefault == true)
        {
            ItemCount = 1;
            IsGained = true;
        }
        //Debug.Log($"{ID} {ObjectId} {ItemName} {IsDefault} {IsGained}");
    }
}
