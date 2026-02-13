using System;
using UnityEngine;

enum InteriorType
{
    MainHouse, Floor, Fix, Free, LakeFloor, LakeFix, LakeFree
}

[Serializable]
public class BuildingItem : StoreItemBase
{    
    [SerializeField] InteriorType _buildingType;

    internal InteriorType BuildingType => _buildingType;

    // SO로 공통 정보 받아옴
}
