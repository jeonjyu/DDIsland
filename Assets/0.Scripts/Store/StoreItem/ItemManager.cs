using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    // 아이템 카탈로그 딕셔너리 모음
    public Dictionary<StoreCat, StoreItemDatabase> storeDatas = new Dictionary<StoreCat, StoreItemDatabase>();

    // 플레이어가 소유한 아이템 딕셔너리 모음
    public Dictionary<StoreCat, StoreItemDatabase> playerItemDatas = new Dictionary<StoreCat, StoreItemDatabase>();

    // 현재 카탈로그 
    public List<IStoreItem> currentDatabase = new List<IStoreItem>();

    // 슬롯으로 보여줄 아이템
    public List<IStoreItem> displayDatas = new List<IStoreItem>();

    [Header("슬롯 아이템 풀")]
    [SerializeField] public ItemSlotPool itemSlotPool;


    [Header("파서로 받아온 상점 데이터 베이스")]
    [SerializeField] InteriorStoreDatabaseSO InteriorDatabase;
    [SerializeField] CostumeStoreDatabaseSO CostumeDatabase;
    [SerializeField] FishingStoreDatabaseSO FishingDatabase;

    // Start시 카탈로그 딕셔너리, 플레이어 소유 아이템 딕셔너리 넣어주기
    // 카탈로그 딕셔너리에 넣어둔 아이템 항목들로 아이템 딕셔너리 만들기
    protected override void Awake()
    {
        base.Awake();
        CreateDatabase();
    }
   
    public void CreateDatabase()
    {
        InteriorDatabase = DataManager.Instance.StoreDatabase.InteriorStoreData;
        CostumeDatabase = DataManager.Instance.StoreDatabase.CostumeStoreData;
        FishingDatabase = DataManager.Instance.StoreDatabase.FishingStoreData;

        storeDatas.Add(StoreCat.interior, new StoreItemDatabase(InteriorDatabase));
        storeDatas.Add(StoreCat.costume, new StoreItemDatabase(CostumeDatabase));
        storeDatas.Add(StoreCat.fishing, new StoreItemDatabase(FishingDatabase));

        Debug.Log("[ItemManger] 데이터베이스 생성");
    }

    // 플레이어 소유 아이템 딕셔너리에 추가
    public void AddToPlayerItem(IStoreItem item, StoreCat storeCat)
    {
        // 카테고리에 해당하는 딕셔너리 검색
        if (!playerItemDatas.ContainsKey(storeCat))
        {
            playerItemDatas.Add(storeCat, new StoreItemDatabase());
            //Debug.LogWarning($"[ItemManager] AddToPlayerItem | {item.ItemName}({item.ID})에 해당하는 플레이어 아이템 딕셔너리가 없습니다");
        }
        // 아이템이 딕셔너리에 존재하는지 검색
        //if (playerItemDatas[storeCat][item.ID] is null)// ContainsKey(item.ID))
        //{
            playerItemDatas[storeCat].AddToDatabase(item);
            //Debug.Log($"[ItemManager] 플레이어 아이템 딕셔너리에 {item.ItemName}({item.ID}) 추가");
        //}
    }

    public void RemoveFromPlayerItem(IStoreItem item, StoreCat storeCat)
    {
        // 카테고리에 해당하는 딕셔너리 검색
        if (!playerItemDatas.ContainsKey(storeCat))
        {
            //Debug.LogWarning($"[ItemManager] RemoveFromPlayerItem | {item.ItemName}({item.ID})에 해당하는 플레이어 아이템 딕셔너리가 없음");
            return;
        }
        else 
        {
            // 아이템이 딕셔너리에 존재하는지 검색
            //if (playerItemDatas[storeCat][item.ID] is not null)
            //{
                playerItemDatas[storeCat].RemoveFromDatabase(item);
            //    Debug.Log($"[ItemManager] 플레이어 아이템 딕셔너리에서 {item.ItemName}({item.ID}) 제거");
            //}
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
        //Debug.Log("[ItemManger] 현재 카테고리 딕셔너리 : " + storeCat.ToString());

        //Debug.Log("정렬 완료: " + string.Join(", ", ItemManager.Instance.displayDatas.Select(x => x.ItemName + "(" + x.ID + "):" + x.PurchasePrice)));
    }
}
