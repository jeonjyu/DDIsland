using System;
using UnityEngine;

[Serializable]
public class BuildingItem : StoreItemBase
{    
    [SerializeField] InteriorType _buildingType;

    internal InteriorType BuildingType => _buildingType;

    // SO로 공통 정보 받아옴
}
