using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 도감 데이터만 불러와서 처리하는 클래스
/// 카테고리별 SO 로드, 테이블 불러오기, 필터링 담당
/// </summary>
public class JournalDataLoader : MonoBehaviour
{
    // 도감 슬롯 하나에 들어갈 데이터들
    public class JournalItemData
    {
        public int JournalId;           // 도감 ID
        public int ItemId;              // 아이템 ID
        public bool IsUnlocked;         // 해금 여부
        public Sprite ItemSprite;       // 스프라이트 경로
        public string ItemName;         // 아이템 이름 (미해금이면 "???")
        public string Description;      // 설명 텍스트
        public JournalCategory Category;// 카테고리
        public Dictionary<string, string> SpecialInfo; // 특화 항목 (키:값)
    }


    // 카테고리별 전체 도감 데이터 가져오기
    public List<JournalItemData> GetJournalItems(JournalCategory category)
    {
        List<JournalItemData> items = new List<JournalItemData>();

        switch (category)
        {
            case JournalCategory.Fish:
                items = BuildFishItems();
                break;
            case JournalCategory.Costume:
                items = BuildCostumeItems();
                break;
            case JournalCategory.Interior:
                items = BuildInteriorItems();
                break;
            case JournalCategory.Food:
                items = BuildFoodItems();
                break;
            case JournalCategory.Album:
                // items = BuildAlbumItems();  // TODO: 데이터 들오면 주석해제 
                break;
        }

        // 정렬: 해금 우선, ID 오름차순
        items = items
            .OrderByDescending(item => item.IsUnlocked)
            .ThenBy(item => item.JournalId)
            .ToList();

        return items;
    }

    // 필터 적용
    public List<JournalItemData> ApplyFilter(List<JournalItemData> items, CollectionFilter filter)
    {
        switch (filter)
        {
            case CollectionFilter.Owned:
                return items.Where(item => item.IsUnlocked).ToList();
            case CollectionFilter.NotOwned:
                return items.Where(item => !item.IsUnlocked).ToList();
            case CollectionFilter.All:
            default:
                return items;
        }
    }

    // 카테고리별 빌드 메서드 
    private List<JournalItemData> BuildFishItems()
    {
        var items = new List<JournalItemData>();
        // DataManager에서 DatabaseSO 가져오기
        var journalDB = DataManager.Instance.JournalDatabase.JournalFishData;  
        var fishDB = DataManager.Instance.FishingDatabase.FishData;
        var unlockedIds = DataManager.Instance.Box.Collection._unlockedFishIds; // 런타임 해금 데이터
        foreach (var journal in journalDB.datas)
        {
            
            bool isUnlocked = unlockedIds.Contains(journal.FishID); // 고정된 SO 대신 Collection_Data에서 판단
            var item = new JournalItemData
            {
                JournalId = journal.JournalFishID,
                ItemId = journal.FishID,
                IsUnlocked = isUnlocked,
                ItemSprite = journal.FishImgPath_Sprite,
                Category = JournalCategory.Fish,
                SpecialInfo = new Dictionary<string, string>()
            };
            // Dictionary 접근으로 변경
            var fishSO = fishDB[journal.FishID];
            if (fishSO != null)
            {
                item.ItemName = isUnlocked ? fishSO.FishName_String : "???";
                item.Description = isUnlocked ? fishSO.FishDesc_String : "";
                item.SpecialInfo["등급"] = fishSO.gradeType.ToString();       
                item.SpecialInfo["서식지"] = fishSO.fishType.ToString();
                ArriveSeason allSeasons = ArriveSeason.Spring | ArriveSeason.Summer | ArriveSeason.Autumn | ArriveSeason.Winter;
                item.SpecialInfo["계절"] = (fishSO.arriveseasonType == allSeasons) ? "모든 계절" : fishSO.arriveseasonType.ToString();
                // TODO: 최고 기록은 CollectionData에서 가져와야 함
                item.SpecialInfo["최고 기록"] = "0cm";
            }
            else
            {
                item.ItemName = isUnlocked ? $"Fish{journal.FishID}" : "???"; 
                item.Description = "";
            }

            items.Add(item);
        }

        return items;
    }

    private List<JournalItemData> BuildCostumeItems()
    {
        var items = new List<JournalItemData>();
        // DataManager에서 가져오기
        var journalDB = DataManager.Instance.JournalDatabase.JournalCostumeData;
        var costumeDB = DataManager.Instance.DecorationDatabase.CostumeData;
        var unlockedIds = DataManager.Instance.Box.Collection._unlockedCostumeIds;

        foreach (var journal in journalDB.datas)
        {
            bool isUnlocked = unlockedIds.Contains(journal.CostumeID);

            var item = new JournalItemData
            {
                JournalId = journal.JournalCostumeID,
                ItemId = journal.CostumeID,
                IsUnlocked = isUnlocked,
                ItemSprite = journal.CostumeImgPath_Sprite,
                Category = JournalCategory.Costume,
                SpecialInfo = new Dictionary<string, string>()
            };


            // Dictionary 접근
            var costumeSO = costumeDB[journal.CostumeID];
            if (costumeSO != null)
            {
                item.ItemName = isUnlocked ? costumeSO.CostumeName_String : "???"; 
                item.Description = isUnlocked ? costumeSO.CostumeDesc_String : ""; 
                item.SpecialInfo["장착 파츠"] = costumeSO.costumeType.ToString();
            }
            else
            {
                item.ItemName = isUnlocked ? $"Costume{journal.CostumeID}" : "???";
                item.Description = "";
            }

            items.Add(item);
        }

        return items;
    }

    private List<JournalItemData> BuildInteriorItems()
    {
        var items = new List<JournalItemData>();
        // DataManager에서 가져오기
        var journalDB = DataManager.Instance.JournalDatabase.JournalInteriorData;
        var interiorDB = DataManager.Instance.DecorationDatabase.InteriorData;
        var unlockedIds = DataManager.Instance.Box.Collection._unlockedInteriorIds; 

        foreach (var journal in journalDB.datas) 
        {
            bool isUnlocked = unlockedIds.Contains(journal.InteriorID);
            var item = new JournalItemData
            {
                JournalId = journal.JournalInteriorID,
                ItemId = journal.InteriorID,
                IsUnlocked = isUnlocked, 
                ItemSprite = journal.InteriorImgPath_Sprite,
                Category = JournalCategory.Interior,
                SpecialInfo = new Dictionary<string, string>()
            };
            // Dictionary 접근
            var interiorSO = interiorDB[journal.InteriorID];
            if (interiorSO != null)
            {
                item.ItemName = isUnlocked ? interiorSO.InteriorName_String : "???"; 
                item.Description = isUnlocked ? interiorSO.InteriorDesc_String : ""; 
                item.SpecialInfo["배치 공간"] = interiorSO.locationType.ToString();
            }
            else
            {
                item.ItemName = isUnlocked ? $"Interior{journal.InteriorID}" : "???"; 
                item.Description = "";
            }

            items.Add(item);
        }

        return items;
    }

    private List<JournalItemData> BuildFoodItems()
    {
        var items = new List<JournalItemData>();

        // DataManager에서 가져오기
        var journalDB = DataManager.Instance.JournalDatabase.JournalFoodData;
        var foodDB = DataManager.Instance.FoodDatabase.FoodInfoData;
        var unlockedIds = DataManager.Instance.Box.Collection._unlockedFoodIds; 

        foreach (var journal in journalDB.datas) 
        {
            bool isUnlocked = unlockedIds.Contains(journal.FoodID); 

            var item = new JournalItemData
            {
                JournalId = journal.JournalFoodID,
                ItemId = journal.FoodID,
                IsUnlocked = isUnlocked,
                ItemSprite = journal.FoodImgPath_Sprite,
                Category = JournalCategory.Food,
                SpecialInfo = new Dictionary<string, string>()
            };
            // Dictionary 접근
            var foodSO = foodDB[journal.FoodID];
            if (foodSO != null)
            {
                item.ItemName = isUnlocked ? foodSO.FoodName_String : "???"; 
                item.Description = isUnlocked ? foodSO.FoodDesc_String : ""; 
                item.SpecialInfo["등급"] = foodSO.foodrateType.ToString();
                item.SpecialInfo["배고픔 지수"] = foodSO.HungerBuffRate.ToString();
                item.SpecialInfo["둥둥 지수"] = foodSO.DoongDoongBuffRate.ToString();
            }
            else
            {
                item.ItemName = isUnlocked ? $"Food{journal.FoodID}" : "???";
                item.Description = "";
            }

            items.Add(item);
        }

        return items;
    }

    // TODO: 음반 데이터 들어오면 아래 주석 해제
    // private List<JournalItemData> BuildAlbumItems()
    // {
    //     var items = new List<JournalItemData>();
    //     var journalDB = DataManager.Instance.JournalDatabase.JournalRecordData;
    //     var recordDB = DataManager.Instance.???;
    //     foreach (var journal in journalDB.datas)
    //     {
    //         var item = new JournalItemData
    //         {
    //             JournalId = journal.JournalRecordID,
    //             ItemId = journal.RecordID,
    //             IsUnlocked = journal.IsUnlocked,
    //             ItemSprite = journal.RecordImgPath_Sprite, 
    //             Category = JournalCategory.Album,
    //             SpecialInfo = new Dictionary<string, string>()
    //         };
    //         var recordSO = FindRecordData(journal.RecordID);
    //         if (recordSO != null)
    //         {
    //             item.ItemName = item.IsUnlocked ? recordSO.RecordName_String : "???";
    //             item.Description = item.IsUnlocked ? recordSO.RecordDesc_String : "";
    //             item.SpecialInfo["테마"] = recordSO.bgthemeType.ToString();
    //             item.SpecialInfo["아티스트"] = recordSO.RecordArtistString;
    //             item.SpecialInfo["재생 길이"] = "TODO";
    //         }
    //         items.Add(item);
    //     }
    //     return items;
    // }


}
