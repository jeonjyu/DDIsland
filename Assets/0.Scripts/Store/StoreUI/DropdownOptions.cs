using System;
using System.ComponentModel;


public enum StoreCat
{
    [Description("Shop_Category_IslandInterior_Text")] interior,
    [Description("Shop_Category_LakeInterior_Text")] lake,
    [Description("Shop_Category_Costume_Text")] costume,
    [Description("Shop_Category_FishingItem_Text")] fishing,
    [Description("Shop_Category_Recipe_Text")] recipe
}

// 드롭다운 옵션으로 설정해줄 enum들
public enum StoreSort
{
    [Description("ShopSortHave")] unGain,
    [Description("ShopSortNotHave")] gain,
    [Description("ShopSortHighPrice")] highToLow,
    [Description("ShopSortLowPrice")] lowToHigh,
    //[Description("최근 구매 순")] recentPurchase,
    //[Description("이름 순")] name
}

public enum Filter
{
    InteriorFilter,
    LakeFilter,
    CostumeFilter,
    FishingFilter
}

public enum InteriorFilter
{
    [Description("IslandInteriorShopFilterAllText")] all,
    [Description("IslandInteriorShopFilterMainHouse")] mainHouse,
    [Description("IslandInteriorShopFilterFloor")] floor,
    [Description("IslandInteriorShopFilterFix")] fix,
    [Description("IslandInteriorShopFilterFree")] free,

}

//public enum LakeFilter
//{
//    [Description("모든 아이템")] all,
//    [Description("바닥재")] floor,
//    [Description("장식물")] ornament
//}

public enum CostumeFilter 
{
    [Description("CostumeShopFilterAllText")] all,
    [Description("CostumeShopFilterHead")] head,
    [Description("CostumeShopFilterBody")] body
}

public enum FishingFilter
{
    [Description("FishingItemShopFilterAllText")] all,
    [Description("FishingItemShopFilterPole")] pole,
    [Description("FishingItemShopFilterBait")] bait,
    [Description("FishingItemShopFilterBobber")] bobber
}