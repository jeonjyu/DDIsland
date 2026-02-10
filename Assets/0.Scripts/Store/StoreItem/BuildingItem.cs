enum InteriorType
{
    MainHouse, Floor, Fix, Free, LakeFloor, LakeFix, LakeFree
}

public class BuildingItem : StoreItemBase
{
    InteriorType _buildingType;

    internal InteriorType BuildingType => _buildingType;
}
