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
    [Description("보유 아이템 순")] gain,
    [Description("미보유 아이템순")] unGain,
    [Description("높은 가격 순")] highToLow,
    [Description("낮은 가격 순")] lowToHigh,
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
    [Description("IslandInteriorShop_Filter_All_Text")] all,
    [Description("IslandInteriorShop_Filter_MainHouse_Text")] mainHouse,
    [Description("IslandInteriorShop_Filter_Floor_Text")] floor,
    [Description("IslandInteriorShop_Filter_Fix_Text")] fix,
    [Description("IslandInteriorShop_Filter_Free_Text")] free,

}

//public enum LakeFilter
//{
//    [Description("모든 아이템")] all,
//    [Description("바닥재")] floor,
//    [Description("장식물")] ornament
//}

public enum CostumeFilter 
{
    [Description("CostumeShop_Filter_All_Text")] all,
    [Description("CostumeShop_Filter_Head_Text")] head,
    [Description("CostumeShop_Filter_Body_Text")] body
}

public enum FishingFilter
{
    [Description("FishingItemShop_Filter_All_Text")] all,
    [Description("FishingItemShop_Filter_Pole_Text")] pole,
    [Description("FishingItemShop_Filter_Bait_Text")] bait,
    [Description("FishingItemShop_Filter_Bobber_Text")] bobber
}