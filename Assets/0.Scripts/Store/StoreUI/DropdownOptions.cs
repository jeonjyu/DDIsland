using System;
using System.ComponentModel;


public enum StoreCat
{
    [Description("섬 인테리어")] interior,
    [Description("호수 인테리어")] lake,
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
    LakeFilter,
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

}

public enum LakeFilter
{
    [Description("모든 아이템")] all,
    [Description("바닥재")] floor,
    [Description("장식물")] ornament
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
    [Description("미끼")] bait,
    [Description("낚시찌")] bobber
}