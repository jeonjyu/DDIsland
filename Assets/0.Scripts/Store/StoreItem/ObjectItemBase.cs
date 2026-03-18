using System;
using UnityEngine;

// 씬에 배치될 아이템 베이스 클래스
[Serializable]
public class ObjectItemBase : MonoBehaviour
{
    [SerializeField] private int _itemId;
    [SerializeField] private string _itemName;
    
    private string _itemDesc;

    public int ItemId => _itemId;
    public string ItemName => _itemName;
    public string ItemDesc => _itemDesc;
}
