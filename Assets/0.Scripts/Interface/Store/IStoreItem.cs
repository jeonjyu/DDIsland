using System;
using UnityEngine;

public interface IStoreItem
{
    int ID { get; }
    int ObjectId { get; }
    string ItemName { get; }
    string ItemDesc { get; }
    int MaxCount { get; }
    int ItemCount { get; set; }
    bool IsGained { get; set; }
    bool IsSaleable { get; }
    int PurchasePrice { get; }
    int SellPrice { get; }
    Sprite ImgSprite { get; }
    Enum Filter { get; }
}
