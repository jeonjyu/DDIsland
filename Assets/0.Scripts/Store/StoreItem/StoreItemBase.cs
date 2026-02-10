using UnityEngine;

// 상점에서 구매 가능한 아이템 베이스 클래스
public class StoreItemBase : MonoBehaviour
{
//field
    private int _itemId;
    private bool _isGained;
    private bool _isSaleable;
    private int _maxCount;
    private int _itemCount;
    private int _purchasePrice;
    private int _sellPrice;
    private string _itemName;
    private string _itemDesc;

//property
    public int ItemId => _itemId;
    public bool IsGained => _isGained;
    public bool IsSaleable => _isSaleable ;
    public int MaxCount => _maxCount; 
    public int ItemCount { get => _itemCount; set => _itemCount = value; }
    public int PurchasePrice => _purchasePrice; 
    public int SellPrice  => _sellPrice;
    public string ItemName => _itemName; 
    public string ItemDesc => _itemDesc;
}
