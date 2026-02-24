using UnityEngine;
using System;

public abstract class StoreItem : ScriptableObject, IStoreItem
{
    //field
    [SerializeField] private int _itemId;
    [SerializeField] private bool _isGained;
    [SerializeField] private bool _isSaleable;
    [SerializeField] private int _maxCount;
    [SerializeField] private int _itemCount;
    [SerializeField] private int _purchasePrice;
    [SerializeField] private int _sellPrice;
    [SerializeField] private string _itemImage;
    [SerializeField] private string _itemName;
    [SerializeField] private string _itemDesc;

    //property
    public int ItemId => _itemId;
    public bool IsGained { get => _isGained; set => _isGained = value; }
    public bool IsSaleable => _isSaleable;
    public int MaxCount => _maxCount;
    public int ItemCount { get => _itemCount; set => _itemCount = value; }
    public int PurchasePrice => _purchasePrice;
    public int SellPrice => _sellPrice;
    public string ItemImage => _itemImage;
    public string ItemName => _itemName;
    public string ItemDesc => _itemDesc;

    public ScriptableObject GetStoreItem()
    {
        return this; 
    }

    public abstract Enum GetFilter();
}
