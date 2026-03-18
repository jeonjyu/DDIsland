using System.Collections.Generic;

// 임시 업그레이드 String 테이블 (나중에 CSV 파서로 교체)
// 나중에 한/영 토글 지원용 
public static class UpgradeStringData
{
    // 타입이 뭐냐에 따라 테이블 키 변환
    public static string GetKeyStatType(StatType type)
    {
        switch (type)
        {
            case StatType.BaseHunger: return "HungerStatDes";
            case StatType.BaseStamina: return "StaminaStatDes";
            case StatType.BaseMoveSpeed: return "MoveSpeedStatDes";
            case StatType.BaseFishingSpeed: return "FishingSpeedStatDes";
            case StatType.StaminaHeal: return "RestSpeedStatDes";
            default: return "";
        }
    }

    // String 테이블 구조: ID [0]=한국어, [1]=영어
    private static Dictionary<string, string[]> stringTable = new Dictionary<string, string[]>
    {
        { "UpgradeStatDes",      new string[] { "캐릭터 성장",   "Upgrade" } },
        { "HungerStatDes",       new string[] { "포만감",        "Hunger" } },
        { "StaminaStatDes",      new string[] { "스태미너",      "Stamina" } },
        { "MoveSpeedStatDes",    new string[] { "이동속도",      "MoveSpeed" } },
        { "FishingSpeedStatDes", new string[] { "낚시 숙련도",   "FishingSpeed" } },
        { "RestSpeedStatDes",    new string[] { "휴식속도",      "RestSpeed" } },
    };

    // String 테이블에서 키로 문자열 가져오기
    // languageIndex: 0=한국어, 1=영어
    public static string Get(string key, int languageIndex = 0)
    {
        if (stringTable.ContainsKey(key))
            return stringTable[key][languageIndex];
        return key;
    }

    // 편의 함수: StatType 바로 한글 이름
    // 예: StatType.BaseHunger "포만감"
    public static string GetDisplayName(StatType type)
    {
        return Get(GetKeyStatType(type));
    }

    // 편의 함수: 타이틀 텍스트
    // "캐릭터 성장" 반환
    public static string GetTitle()
    {
        return Get("UpgradeStatDes");
    }
}
