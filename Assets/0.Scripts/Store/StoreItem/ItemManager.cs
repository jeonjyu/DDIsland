using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    // 아이템 카탈로그 딕셔너리 모음
    public Dictionary<StoreCat, StoreItemDatabase> storeDatas = new Dictionary<StoreCat, StoreItemDatabase>();

    // 플레이어가 소유한 아이템 딕셔너리 모음
    public Dictionary<StoreCat, PlayerItemDatabase> playerItemDatas = new Dictionary<StoreCat, PlayerItemDatabase>();

    // 현재 카탈로그 
    public List<IStoreItem> currentDatabase = new List<IStoreItem>();

    // 슬롯으로 보여줄 아이템
    public List<IStoreItem> displayDatas = new List<IStoreItem>();

    [Header("슬롯 아이템 풀")]
    [SerializeField] public ItemSlotPool itemSlotPool;
    public ItemSlotViewModelBase itemSlot;


    [Header("파서로 받아온 상점 데이터 베이스")]
    [SerializeField] IslandStoreDatabaseSO InteriorDatabase;
    [SerializeField] LakeStoreDatabaseSO LakeDatabase;
    [SerializeField] CostumeStoreDatabaseSO CostumeDatabase;
    [SerializeField] FishingStoreDatabaseSO FishingDatabase;
    [SerializeField] FoodDatabaseSO FoodDatabase;

    
   
    // 아이템 변동 이벤트 
    public event System.Action<IStoreItem, StoreCat> OnPlayerItemAdded;
    public event System.Action<IStoreItem, StoreCat> OnPlayerItemRemoved;

    private void Start()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            if (DataManager.Instance.Hub.IsLoaded)
            {
                SyncInventoryDataSave();
            }
            else
            {
                DataManager.Instance.Hub.OnDataLoaded += SyncInventoryDataLoad;
            }
        }

        CreateDatabase();
    }
    private void OnEnable()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
            DataManager.Instance.Hub.OnRequestSave += SyncInventoryDataSave;
    }
    private void OnDisable()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
            DataManager.Instance.Hub.OnRequestSave -= SyncInventoryDataSave;
    }

    public void CreateDatabase()
    {
        InteriorDatabase = DataManager.Instance.StoreDatabase.IslandStoreData;
        LakeDatabase = DataManager.Instance.StoreDatabase.LakeStoreData;
        CostumeDatabase = DataManager.Instance.StoreDatabase.CostumeStoreData;
        FishingDatabase = DataManager.Instance.StoreDatabase.FishingStoreData;
        FoodDatabase = DataManager.Instance.FoodDatabase.FoodInfoData;

        storeDatas.Add(StoreCat.interior, new StoreItemDatabase(InteriorDatabase));
        storeDatas.Add(StoreCat.lake, new StoreItemDatabase(LakeDatabase));
        storeDatas.Add(StoreCat.costume, new StoreItemDatabase(CostumeDatabase));
        storeDatas.Add(StoreCat.fishing, new StoreItemDatabase(FishingDatabase));
        storeDatas.Add(StoreCat.recipe, new StoreItemDatabase(FoodDatabase));

        Debug.Log("[ItemManger] CreateDatabase | 데이터베이스 생성");

        foreach (var category in storeDatas)
        {
            CreatePlayerItemDatabase(category.Key, storeDatas[category.Key]);
        }

        Debug.Log("[ItemManger] CreateDatabase | 사용자 소유 데이터베이스 생성");
    }

    public void CreatePlayerItemDatabase(StoreCat cat, StoreItemDatabase storeItem)
    {
        PlayerItemDatabase playerItems = new PlayerItemDatabase();
        playerItems.SaveGainedItem(storeItem.Items);
        playerItemDatas.Add(cat, playerItems);

        //Debug.Log("[ItemManager] CreatePlayerItemDatabase | " + cat + "의 playerItemDatas 개수 : " + playerItemDatas[cat].Items.Count);
    }

    // 플레이어 소유 아이템 딕셔너리에 추가
    public void AddToPlayerItem(IStoreItem item, StoreCat storeCat)
    {
        // 카테고리에 해당하는 딕셔너리 검색
        if (!playerItemDatas.ContainsKey(storeCat)) // 처음에 만들어두는데 왜 없는지 모르겠고 새로 만들 경우 처음에 저장해뒀던 기본 제공 아이템들이 사라진다
        {
            Debug.LogWarning($"[ItemManager] AddToPlayerItem | {item.ItemName}({item.ID})에 해당하는 플레이어 아이템 딕셔너리{storeCat}가 없습니다");
            playerItemDatas.Add(storeCat, new PlayerItemDatabase());
        }
        if (storeCat == StoreCat.costume) QuestManager.Instance.AddSimpleProgress(QuestConditionKey.BuyCostumeCount, 1); 
        if (storeCat == StoreCat.interior) QuestManager.Instance.AddSimpleProgress(QuestConditionKey.BuylandStoreCount, 1);
        if (storeCat == StoreCat.fishing) QuestManager.Instance.AddSimpleProgress(QuestConditionKey.BuyFishingItemCount, 1); 
        if (storeCat == StoreCat.recipe) QuestManager.Instance.AddSimpleProgress(QuestConditionKey.BuyFoodCount, 1);


        //Debug.LogWarning($"[ItemManager] AddToPlayerItem | {item.ItemName}({item.ID}) 추가");
        playerItemDatas[storeCat].AddToDatabase(item);

        // 플레이어가 보유한 아이템 딕셔너리에 추가되었는지 확인
        //Debug.Log($"[ItemManager] AddToPlayerItem | 사용자가 보유한 {storeCat}아이템의 개수 : {playerItemDatas[storeCat].Items.Count}");

        OnPlayerItemAdded?.Invoke(item, storeCat);
    }

    // 플레이어 소유 아이템 딕셔너리에서 제거
    public void RemoveFromPlayerItem(IStoreItem item, StoreCat storeCat)
    {
        // 카테고리에 해당하는 딕셔너리 검색
        if (!playerItemDatas.ContainsKey(storeCat))
        {
            Debug.LogWarning($"[ItemManager] RemoveFromPlayerItem | {item.ItemName}({item.ID})에 해당하는 플레이어 아이템 딕셔너리가 없음");
            return;
        }
        else
        {
            // 아이템이 딕셔너리에 존재하는지 검색
            playerItemDatas[storeCat].RemoveFromDatabase(item);
            OnPlayerItemRemoved?.Invoke(item, storeCat);

            // 플레이어가 보유한 아이템 딕셔너리에서 제거되었는지 확인
            //Debug.Log(playerItemDatas[storeCat].Items.Count); 

            OnPlayerItemRemoved?.Invoke(item, storeCat);
        }
    }

    // 상점 선택에 따라 현재 상점에 해당하는 딕셔너리로 변경
    public void SetCurrentCategory(StoreCat storeCat = StoreCat.interior)
    {
        // StorePanel 활성화 한 채 실행하면 에러 발생
        if(!storeDatas.ContainsKey(StoreCat.interior)) Debug.LogError("아이템 딕셔너리 생성 전에 StorePanel 활성화 한 채 실행하면 에러 발생, 비활성화한 뒤 재시작해주세요");

        currentDatabase = storeDatas[storeCat].Items;
        displayDatas = currentDatabase;
        displayDatas = displayDatas.OrderBy(x => x.IsGained).ThenBy(x => x.PurchasePrice).ThenBy(x => x.ID).ToList();

        //Debug.Log("정렬 완료: " + string.Join(", ", ItemManager.Instance.displayDatas.Select(x => x.ItemName + "(" + x.ID + "):" + x.PurchasePrice)));
    }

    private void SyncInventoryDataSave()
    {
        var box = DataManager.Instance.Hub._allUserData;

        if (playerItemDatas.TryGetValue(StoreCat.interior, out var interiorDb))
        {
            box.Store._inventory = interiorDb.Items
                .Where(x => x.IsGained && x.ItemCount > 0)
                .Select(x => new LakeInvenSlot
                {
                    itemId = x.ID,
                    quantity = x.ItemCount
                })
                .ToList();
        }

        if (playerItemDatas.TryGetValue(StoreCat.costume, out var costumeDb))
        {
            box.Store._ownedCostumes = costumeDb.Items.Select(x => x.ID).Distinct().ToList();
        }

        if (playerItemDatas.TryGetValue(StoreCat.fishing, out var fishingDb))
        {
            box.Store._ownedFishings = fishingDb.Items.Select(x => x.ID).Distinct().ToList();
        }

        if (playerItemDatas.TryGetValue(StoreCat.lake, out var lakeDb))
        {
            box.Store._ownedLakeItems = lakeDb.Items.Select(x => x.ID).Distinct().ToList();
        }

        if (playerItemDatas.TryGetValue(StoreCat.recipe, out var recipeDb))
        {
            box.Collection._unlockedFoodIds = recipeDb.Items.Select(x => x.ID).Distinct().ToList();
        }

        Debug.Log("<color=yellow> 인벤토리 데이터가 저장되었습니다 </color>");
    }
    private void SyncInventoryDataLoad()
    {
        var box = DataManager.Instance.Hub._allUserData;

        if (box.Store._inventory == null &&
            box.Store._ownedCostumes == null &&
            box.Store._ownedFishings == null &&
            box.Store._ownedLakeItems == null &&
            box.Store._ownedRecipes == null)
        {
            return;
        }

        foreach (var category in playerItemDatas.Values)
        {
            category.Items.Clear();
        }

        var interiorDict = storeDatas[StoreCat.interior].Items.ToDictionary(x => x.ID);
        var costumeDict = storeDatas[StoreCat.costume].Items.ToDictionary(x => x.ID);
        var fishingDict = storeDatas[StoreCat.fishing].Items.ToDictionary(x => x.ID);
        var lakeDict = storeDatas[StoreCat.lake].Items.ToDictionary(x => x.ID);
        var recipeDict = storeDatas[StoreCat.recipe].Items.ToDictionary(x => x.ID);

        if (box.Store._inventory != null)
        {
            foreach (var slot in box.Store._inventory)
            {
                if (interiorDict.TryGetValue(slot.itemId, out IStoreItem original))
                {
                    original.ItemCount = slot.quantity;
                    original.IsGained = true;
                    AddToPlayerItem(original, StoreCat.interior);
                }
            }
        }

        if (box.Store._ownedCostumes != null)
        {
            foreach (int id in box.Store._ownedCostumes)
            {
                if (costumeDict.TryGetValue(id, out IStoreItem costume))
                {
                    costume.IsGained = true;
                    costume.ItemCount = 1;
                    AddToPlayerItem(costume, StoreCat.costume);
                }
            }
        }

        if (box.Store._ownedFishings != null)
        {
            foreach (int id in box.Store._ownedFishings)
            {
                if (fishingDict.TryGetValue(id, out IStoreItem fishingItem))
                {
                    fishingItem.IsGained = true;
                    fishingItem.ItemCount = 1;
                    AddToPlayerItem(fishingItem, StoreCat.fishing);
                }
            }
        }

        if (box.Store._ownedLakeItems != null && lakeDict != null)
        {
            foreach (int id in box.Store._ownedLakeItems)
            {
                if (lakeDict.TryGetValue(id, out IStoreItem lakeItem))
                {
                    lakeItem.IsGained = true;
                    lakeItem.ItemCount = 1;
                    AddToPlayerItem(lakeItem, StoreCat.lake);
                }
            }
        }

        if (box.Store._ownedRecipes != null && recipeDict != null)
        {
            foreach (int id in box.Store._ownedRecipes)
            {
                if (recipeDict.TryGetValue(id, out IStoreItem recipeItem))
                {
                    recipeItem.IsGained = true;
                    recipeItem.ItemCount = 1;
                    AddToPlayerItem(recipeItem, StoreCat.recipe);
                }
            }
        }

    }
}
