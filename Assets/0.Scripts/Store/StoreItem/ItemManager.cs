using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemManager : Singleton<ItemManager>
{
    // 아이템 카탈로그 딕셔너리 모음
    public Dictionary<StoreCat, Dictionary<int, IStoreItem>> _storeCategories = new Dictionary<StoreCat, Dictionary<int, IStoreItem>>();

    // 플레이어가 소유한 아이템 딕셔너리 모음
    public Dictionary<StoreCat, Dictionary<int, IStoreItem>> _playerOwnedItems = new Dictionary<StoreCat, Dictionary<int, IStoreItem>>();

    // 현재 카탈로그 
    public List<StoreItem> currentCategory = new List<StoreItem>();

    // 슬롯으로 보여줄 아이템
    public List<StoreItem> displayItems = new List<StoreItem>();

    // 정렬 우선순위 리스트
    //public List<Comparer> sortPriority = new List<Comparer>();

    [Header("슬롯 아이템 풀")]
    [SerializeField] public ItemSlotPool itemSlotPool;

    [Header("테스트용 아이템 리스트")]
    [SerializeField] List<StoreItem> interiorItem = new List<StoreItem>();
    [SerializeField] List<StoreItem> costumeItem = new List<StoreItem>();


    // Start시 카탈로그 딕셔너리, 플레이어 소유 아이템 딕셔너리 넣어주기
    // 카탈로그 딕셔너리에 넣어둔 아이템 항목들로 아이템 딕셔너리 만들기
    protected override void Awake()
    {
        base.Awake();
        //Debug.Log("[ItemManager] Awake");
        //_storeCategories.Add(StoreCat.interior, StoreManager.Instance.interiorItem);
        CreateDictionary();
    }

    public void CreateDictionary()
    {
        Dictionary<int, IStoreItem> keyValues = new Dictionary<int, IStoreItem>();
        foreach (StoreItem item in interiorItem)
            keyValues.Add(item.ItemId, item);
        _storeCategories.Add(StoreCat.interior, keyValues);
        

        Dictionary<int, IStoreItem> keyValues1 = new Dictionary<int, IStoreItem>();
        foreach (StoreItem item in costumeItem)
            keyValues1.Add(item.ItemId, item);
        _storeCategories.Add(StoreCat.costume, keyValues1);

        //Debug.Log("[ItemManger] 딕셔너리 생성");
    }


    // 플레이어 소유 아이템 딕셔너리에 추가
    public void AddToPlayerItem(StoreItem item, StoreCat storeCat)
    {
        // 카테고리에 해당하는 딕셔너리 검색
        if (!_playerOwnedItems.ContainsKey(storeCat))
        {
            _playerOwnedItems.Add(storeCat, new Dictionary<int, IStoreItem>());
            Debug.LogWarning($"[ItemManager] AddToPlayerItem | {item.ItemName}({item.ItemId})에 해당하는 플레이어 아이템 딕셔너리가 없습니다");
        }
        // 아이템이 딕셔너리에 존재하는지 검색
        if (!_playerOwnedItems[storeCat].ContainsKey(item.ItemId))
        {
            _playerOwnedItems[storeCat].Add(item.ItemId, item);
            Debug.Log($"[ItemManager] 플레이어 아이템 딕셔너리에 {item.ItemName}({item.ItemId}) 추가");

        }
    }

    public void RemoveFromPlayerItem(StoreItem item, StoreCat storeCat)
    {
        // 카테고리에 해당하는 딕셔너리 검색
        if (!_playerOwnedItems.ContainsKey(storeCat))
        {
            Debug.LogWarning($"[ItemManager] RemoveFromPlayerItem | {item.ItemName}({item.ItemId})에 해당하는 플레이어 아이템 딕셔너리가 없음");
            return;
        }
        else 
        {
            // 아이템이 딕셔너리에 존재하는지 검색
            if (!_playerOwnedItems[storeCat].ContainsKey(item.ItemId))
            {
                _playerOwnedItems[storeCat].Remove(item.ItemId);
                Debug.Log($"[ItemManager] 플레이어 아이템 딕셔너리에서 {item.ItemName}({item.ItemId}) 제거");
            }
        }
    }

    // 상점 선택에 따라 현재 상점에 해당하는 딕셔너리로 변경
    public void SetCurrentCategory(StoreCat storeCat = StoreCat.interior)
    {
        //    currentCategory = _storeCategories[storeCat];
        //    displayItems = currentCategory.Values.Cast<StoreItem>().ToList();


        currentCategory = _storeCategories[storeCat].Values.Cast<StoreItem>().ToList();
        displayItems = new List<StoreItem>(currentCategory);
        //Debug.Log("[ItemManger] 현재 카테고리 딕셔너리 : " + storeCat.ToString());

        displayItems = displayItems.OrderBy((StoreItem x) => x.IsGained).ThenBy(x => x.PurchasePrice).ThenBy(x => x.ItemId).ToList();

        //Debug.Log("정렬 완료: " + string.Join(", ", ItemManager.Instance.displayItems.Select(x => x.ItemName + "(" + x.ItemId + "):" + x.PurchasePrice)));
    }
}
