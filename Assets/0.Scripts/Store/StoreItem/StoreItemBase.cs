using System;
using UnityEngine;

상점에서 구매 가능한 아이템 베이스 클래스
[Serializable]
public class StoreItemBase : MonoBehaviour
{
    [SerializeField] private StoreItem _itemDataSO;

    public StoreItem ItemDataSO => _itemDataSO;
}
