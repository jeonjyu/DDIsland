using System.Collections.Generic;
using UnityEngine;

public class StoreItemDatabase : IStoreItemDatabase 
{
    List<IStoreItem> _itemDatas = new List<IStoreItem>();
    public List<IStoreItem> Items { get => _itemDatas; set => _itemDatas = value; }

    public IStoreItem this[int id] => Items[id];

    public StoreItemDatabase() { }

    // 받아온 SO를 판별해서 인게임 상점에서 사용할 각 상점 데이터베이스로 만들어줌
    public StoreItemDatabase(ScriptableObject database)
    {
        //Debug.Log(database.GetType().Name);
        switch (database)
        {
            case TableDatabase<int, InteriorStoreDataSO> interiorDatabase:
                foreach (var item in interiorDatabase.datas)
                {
                    InteriorStoreDataSO data = item;
                    StoreItem<InteriorStoreDataSO> storeItem = new InteriorStoreItem(data);
                    Items.Add(storeItem);
                }
                break;
            case TableDatabase<int, FishingStoreDataSO> fishingDatabase:
                foreach (var item in fishingDatabase.datas)
                {
                    FishingStoreDataSO data = item;
                    StoreItem<FishingStoreDataSO> storeItem = new FishingStoreItem(data);
                    Items.Add(storeItem);
                }
                break;
            case TableDatabase<int, CostumeStoreDataSO> costumeDatabase:
                foreach (var item in costumeDatabase.datas)
                {
                    CostumeStoreDataSO data = item;
                    StoreItem<CostumeStoreDataSO> storeItem = new CostumeStoreItem(data);
                    Items.Add(new CostumeStoreItem(item));
                }
                break;
        }
    }
    public void AddToDatabase(IStoreItem data)
    {   
        if(!Items.Contains(data))
            Items.Add(data); 
    }

    public void RemoveFromDatabase(IStoreItem data)
    {
        if(Items.Contains(data))
            Items.Remove(data);
    }
}
