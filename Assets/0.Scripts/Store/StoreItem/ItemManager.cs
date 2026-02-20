using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    // 아이템 카탈로그 딕셔너리 모음
    public Dictionary<StoreCat, Dictionary<int, IStoreItem>> _storeCategories = new Dictionary<StoreCat, Dictionary<int, IStoreItem>>();

    // 플레이어가 소유한 아이템 딕셔너리 모음
    public List<StoreCategory<StoreCat>> _playerOwnedItems = new List<StoreCategory<StoreCat>>();

    // 현재 카탈로그 
    public Dictionary<int, IStoreItem> _currentCategory = new Dictionary<int, IStoreItem>();

    // 슬롯으로 보여줄 아이템
    public List<StoreItem> displayItems = new List<StoreItem>();

    [Header("테스트용 아이템 리스트")]
    [SerializeField] List<StoreItem> interiorItem = new List<StoreItem>();
    [SerializeField] List<StoreItem> costumeItem = new List<StoreItem>();

    // Start시 카탈로그 딕셔너리, 플레이어 소유 아이템 딕셔너리 넣어주기
    // 카탈로그 딕셔너리에 넣어둔 아이템 항목들로 아이템 딕셔너리 만들기
    void Start()
    {
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

        Debug.Log("[ItemManger] 딕셔너리 생성");
    }


    // 플레이어 소유 아이템 소유 딕셔너리에 추가
    //public void AddToPlayerItem(T item) where T : StoreItemBaseSO<>
    //{
    //    // item의 StoreCat에 따라 다른 _playerOwnedItems에 add
    //    item.store
    //}


    // 선택된 정렬 기준 받아서 우선순위 리스트에 따라 for문으로 체이닝 돌려주기
    // 돌린 아이템 리스트를 displayItems에 넣어주기


    // 상점 선택에 따라 현재 상점에 해당하는 딕셔너리로 변경
    public void SetCurrentCategory(StoreCat storeCat)
    {
        _currentCategory = _storeCategories[storeCat];
        Debug.Log("[ItemManger] 현재 카테고리 딕셔너리 : " + storeCat.ToString());
    }


    // 필터링 기능
    // 필터에 해당하지 않는 아이템만 리스트에 남기고 모델에서 지움
}
