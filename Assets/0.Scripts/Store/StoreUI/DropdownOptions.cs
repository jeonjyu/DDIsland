using System;
using System.ComponentModel;


public enum StoreCat
{
    [Description("인테리어")] interior,
    [Description("코스튬")] costume,
    [Description("낚시")] fishing,
    [Description("레시피")] recipe
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
    CostumeFilter,
    FishingFilter
}

public enum InteriorFilter
{
    [Description("모든 아이템")] all,
    [Description("메인 하우스")] mainHouse,
    [Description("섬 바닥")] floor,
    [Description("섬 고정 장식물")] fix,
    [Description("섬 자유 배치물")] free,
    [Description("호수 바닥")] lakeFloor,
    [Description("호수 고정 장식물")] lakeFix,
    [Description("호수 자유 배치물")] lakeFree
}

public enum CostumeFilter 
{
    [Description("모든 아이템")] all,
    [Description("머리 장식")] head,
    [Description("한벌옷")] body
}

public enum FishingFilter
{
    [Description("모든 아이템")] all,
    [Description("낚싯대")] pole,
    [Description("낚시찌")] bait,
    [Description("미끼")] bobber
}