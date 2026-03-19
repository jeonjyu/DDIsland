using System.Collections.Generic;

/// <summary>
/// 도감 enum 로컬라이징 (임시 하드코딩)
/// TODO: 스트링테이블 추가되면 LocalizationManager.GetString()으로 교체
/// </summary>
public static class JournalLocalize
{
    public static bool IsKr => PlayerPrefsDataManager.Language == 0; // 0=한국어, 1=영어

    public static string Enum(System.Enum value)
    {
        return value switch
        {
            // 등급
            Grade.Normal => IsKr ? "일반" : "Normal",
            Grade.Rare => IsKr ? "고급" : "Rare",
            Grade.Epic => IsKr ? "희귀" : "Epic",
            Grade.Legendary => IsKr ? "전설" : "Legendary",
            // 서식지
            FishType.Lake => IsKr ? "민물" : "Freshwater",
            FishType.Sea => IsKr ? "바다" : "Sea",
            // 장착 파츠
            CostumeType.Head => IsKr ? "머리 장식" : "Headwear",
            CostumeType.Body => IsKr ? "한벌옷" : "Outfit",
            CostumeType.Tool => IsKr ? "도구 스킨" : "Tool Skin",
            // 배치 공간
            LocationType.Island => IsKr ? "섬" : "Island",
            LocationType.Lake => IsKr ? "호수" : "Lake",
            // 음식 등급
            FoodRateType.Normal => IsKr ? "일반" : "Normal",
            FoodRateType.Rare => IsKr ? "고급" : "Rare",
            FoodRateType.Epic => IsKr ? "희귀" : "Epic",
            FoodRateType.Legendary => IsKr ? "전설" : "Legendary",
            // 배경음 테마
            BgTheme.General => IsKr ? "기본" : "General",
            BgTheme.Spring => IsKr ? "봄" : "Spring",
            BgTheme.Summer => IsKr ? "여름" : "Summer",
            BgTheme.Autumn => IsKr ? "가을" : "Autumn",
            BgTheme.Winter => IsKr ? "겨울" : "Winter",
            BgTheme.Collaboration => IsKr ? "콜라보" : "Collaboration",
            _ => value.ToString()
        };
    }

    public static string Season(ArriveSeason season)
    {
        ArriveSeason all = ArriveSeason.Spring | ArriveSeason.Summer | ArriveSeason.Autumn | ArriveSeason.Winter;
        if ((season & all) == all) return IsKr ? "모든 계절" : "All Seasons";

        List<string> parts = new List<string>();
        if ((season & ArriveSeason.Spring) != 0) parts.Add(IsKr ? "봄" : "Spring");
        if ((season & ArriveSeason.Summer) != 0) parts.Add(IsKr ? "여름" : "Summer");
        if ((season & ArriveSeason.Autumn) != 0) parts.Add(IsKr ? "가을" : "Autumn");
        if ((season & ArriveSeason.Winter) != 0) parts.Add(IsKr ? "겨울" : "Winter");
        return string.Join(", ", parts);
    }
    // 카테고리 버튼
    public static string Category(JournalCategory category)
    {
        return category switch
        {
            JournalCategory.Fish => IsKr ? "어종" : "Fish",
            JournalCategory.Costume => IsKr ? "코스튬" : "Costume",
            JournalCategory.Interior => IsKr ? "인테리어" : "Interior",
            JournalCategory.Album => IsKr ? "음반" : "Album",
            JournalCategory.Food => IsKr ? "음식" : "Food",
            _ => category.ToString()
        };
    }

    // 템정보 팝업창 
    public static string InfoKey(string key)
    {
        return key switch
        {
            "등급" => IsKr ? "등급" : "Grade",
            "서식지" => IsKr ? "서식지" : "Habitat",
            "계절" => IsKr ? "계절" : "Season",
            "최고 기록" => IsKr ? "최고 기록" : "Best Record",
            "장착 파츠" => IsKr ? "장착 파츠" : "Parts",
            "배치 공간" => IsKr ? "배치 공간" : "Location",
            "배고픔 지수" => IsKr ? "배고픔 지수" : "Hunger",
            "둥둥 지수" => IsKr ? "둥둥 지수" : "DoongDoong",
            "테마" => IsKr ? "테마" : "Theme",
            "아티스트" => IsKr ? "아티스트" : "Artist",
            "재생 길이" => IsKr ? "재생 길이" : "Duration",
            _ => key
        };
    }

    // 물고기 기록 저장된게 없으면
    public static string NoRecord => IsKr ? "기록 없음" : "No Record";

    // 대분류 탭
    public static string Tab(MainTab tab)
    {
        return tab switch
        {
            MainTab.Quest => IsKr ? "퀘스트" : "Quest",
            MainTab.Journal => IsKr ? "도감" : "Journal",
            _ => tab.ToString()
        };
    }
    // 드롭다운 정렬 필터
    public static string Filter(CollectionFilter filter)
    {
        return filter switch
        {
            CollectionFilter.All => IsKr ? "전체" : "All",
            CollectionFilter.Owned => IsKr ? "등록" : "Owned",
            CollectionFilter.NotOwned => IsKr ? "미등록" : "Not Owned",
            _ => filter.ToString()
        };
    }

}