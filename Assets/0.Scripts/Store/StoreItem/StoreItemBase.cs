using System;
using UnityEngine;

// 상점에서 구매 가능한 아이템 베이스 클래스
[Serializable]
public class StoreItemBase : MonoBehaviour
{
//field
    public int _itemId;
    public bool _isGained;
    public bool _isSaleable;
    public int _maxCount;
    public int _itemCount;
    public int _purchasePrice;
    public int _sellPrice;
    public string _itemName;
    public string _itemDesc;
}
