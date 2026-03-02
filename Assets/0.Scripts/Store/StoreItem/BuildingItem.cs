using System;
using UnityEngine;

[Serializable]
public class BuildingItem : StoreItemBase
{    
    [SerializeField] InteriorStore_ItemType _buildingType;

    internal InteriorStore_ItemType BuildingType => _buildingType;

    // SO로 공통 정보 받아옴
}
