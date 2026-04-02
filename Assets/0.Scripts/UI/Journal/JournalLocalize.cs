using System.Collections.Generic;

public static class JournalLocalize
{
    //   public static bool IsKr => PlayerPrefsDataManager.Language == 0; // 0=한국어, 1=영어

    // StringUI 테이블에서 가져오는 헬퍼
    private static string Get(string key)
    {
        try
        {
            return DataManager.Instance.StringUIDatabase.StringUIInfoData[key].ID_String;
        }
        catch
        {
            return key; // 키 없으면 키 자체를 폴백
        }
    }

    public static string Enum(System.Enum value)
    {
        return value switch
        {
            // 어종 등급
            Grade.Normal => Get("Journal_Grade_Normal_Text"),
            Grade.Rare => Get("Journal_Grade_Rare_Text"),
            Grade.Epic => Get("Journal_Grade_Epic_Text"),
            Grade.Legendary => Get("Journal_Grade_Legendary_Text"),
            // 서식지
            FishType.Lake => Get("Journal_Type_Lake_Text"),
            FishType.Sea => Get("Journal_Type_Sea_Text"),
            // 장착 파츠
            CostumeType.Head => Get("Journal_CostumeType_Head_Text"),
            CostumeType.Body => Get("Journal_CostumeType_Body_Text"),
            // 배치 공간
            LocationType.Island => Get("Journal_LocationType_Island_Text"),
            LocationType.Lake => Get("Journal_LocationType_Lake_Text"),
            // 음식 등급
            FoodRateType.Normal => Get("Journal_FoodGrade_Normal_Text"),
            FoodRateType.Rare => Get("Journal_FoodGrade_Rare_Text"),
            FoodRateType.Epic => Get("Journal_FoodGrade_Epic_Text"),
            FoodRateType.Legendary => Get("Journal_FoodGrade_Legendary_Text"),
            // 배경음 테마
            BgTheme.General => Get("Journal_RecordBG_General_Text"),
            BgTheme.Spring => Get("Journal_RecordBG_Spring_Text"),
            BgTheme.Summer => Get("Journal_RecordBG_Summer_Text"),
            BgTheme.Autumn => Get("Journal_RecordBG_Autumn_Text"),
            BgTheme.Winter => Get("Journal_RecordBG_Winter_Text"),
            BgTheme.Collaboration => Get("Journal_RecordBG_Collaboration_Text"),
            _ => value.ToString()
        };
    }

    public static string Season(ArriveSeason season)
    {
        ArriveSeason all = ArriveSeason.Spring | ArriveSeason.Summer | ArriveSeason.Autumn | ArriveSeason.Winter;
        if ((season & all) == all) return Get("Journal_Season_All_Text");

        List<string> parts = new List<string>();
        if ((season & ArriveSeason.Spring) != 0) parts.Add(Get("Journal_Season_Spring_Text"));
        if ((season & ArriveSeason.Summer) != 0) parts.Add(Get("Journal_Season_Summer_Text")); // TODO: summer 오타 있는데??
        if ((season & ArriveSeason.Autumn) != 0) parts.Add(Get("Journal_Season_Autumn_Text"));
        if ((season & ArriveSeason.Winter) != 0) parts.Add(Get("Journal_Season_Winter_Text"));
        return string.Join(", ", parts);
    }
    // 카테고리 버튼
    public static string Category(JournalCategory category)
    {
        return category switch
        {
            JournalCategory.Fish => Get("Journal_Category_Fish_Text"),
            JournalCategory.Costume => Get("Journal_Category_Costume_Text"),
            JournalCategory.Interior => Get("Journal_Category_Interior_Text"),
            JournalCategory.Album => Get("Journal_Category_Record_Text"),
            JournalCategory.Food => Get("Journal_Category_Food_Text"),
            _ => category.ToString()
        };
    }

    // 템정보 팝업창 
    public static string InfoKey(string key)
    {
        return key switch
        {
            "등급" => Get("Journal_Grade_Text"),
            "서식지" => Get("Journal_Type_Text"),
            "계절" => Get("Journal_Season_Text"),
            "최고 기록" => Get("Journal_Size_Best_Text"),
            "장착 파츠" => Get("Journal_CostumeType_Text"),
            "배치 공간" => Get("Journal_LocationType_Text"),
            "배고픔 지수" => Get("Journal_FoodEffect_HungerBuff_Text"),
            "둥둥 지수" => Get("Journal_FoodEffect_DoongDoongBuff_Text"),
            "테마" => Get("Journal_RecordBG_Text"),
            //"아티스트" => Get("Journal_RecordArtist_Text"), // TODO: StringUI 테이블 추가되면 교체 
            "아티스트" => LocalizationManager.Instance.GetString("JournalRecordArtist"),
            _ => key
        };
    }


    //// 대분류 탭
    //public static string Tab(MainTab tab)
    //{
    //    return tab switch
    //    {
    //        MainTab.Quest => IsKr ? "퀘스트" : "Quest",
    //        MainTab.Journal => IsKr ? "도감" : "Journal",
    //        _ => tab.ToString()
    //    };
    //}

    // 드롭다운 정렬 필터
    public static string Filter(CollectionFilter filter)
    {
        return filter switch
        {
            CollectionFilter.All => Get("Journal_Dropdown_All_Text"),
            CollectionFilter.Owned => Get("Journal_Dropdown_Unlocked_Text"),
            CollectionFilter.NotOwned => Get("Journal_Dropdown_Locked_Text"),
            _ => filter.ToString()
        };
    }

    // 물고기 기록 저장된게 없으면 폴백
    public static string NoRecord => "No Record";

}