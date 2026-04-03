
using System;
using System.Collections.Generic;
using UnityEngine;
public struct FoodInstance
{
    public int FoodId;

}
public struct FoodStackSlot
{
    public int FoodId;
    public long LastAcquiredOrder;
}

public class FoodStorageManager : Singleton<FoodStorageManager>
{
    FoodStackSlot?[] foodSlots;

    int _storageCapacity = 5;  //인벤크기
    int _storagelevel = 1;
    long _acquireCounter = 0;
    Dictionary<int, int> _storageDatalevelCost;
    Dictionary<int, int> _storageDatalevelCap;
    public const int MaxLevel = 5;
    public const int MaxCapacity = 25;

    public event Action<int> OnSlotChanged;
    public int Capacity => foodSlots?.Length ?? 0;
    public int StorageLevel => _storagelevel;
    public FoodStackSlot?[] FoodSlotData => foodSlots;
    private void Awake()
    {
        base.Awake();
        foodSlots = new FoodStackSlot?[_storageCapacity];
        _storageDatalevelCost = new Dictionary<int, int>();
        _storageDatalevelCap = new Dictionary<int, int>();
    }

    private void OnEnable()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
            DataManager.Instance.Hub.OnRequestSave += SyncFoodStorageSaveData;
    }

    private void OnDisable()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
            DataManager.Instance.Hub.OnRequestSave -= SyncFoodStorageSaveData;
    }
    private void Start()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            if (DataManager.Instance.Hub.IsLoaded)
            {
                SyncFoodStorageLoadData();
            }
            else
            {
                DataManager.Instance.Hub.OnDataLoaded += SyncFoodStorageLoadData;
            }
        }
        GetFoodUpgradeData();
    }

    public bool TryAddToFoodStorage(FoodInstance food)
    {
        if (foodSlots == null || foodSlots.Length == 0)
            return false;

        for (int i = 0; i < foodSlots.Length; i++)
        {
            if (!foodSlots[i].HasValue)
            {
                foodSlots[i] = new FoodStackSlot
                {
                    FoodId = food.FoodId,
                    LastAcquiredOrder = ++_acquireCounter,
                };

                OnSlotChanged?.Invoke(i);
                return true;
            }
        }
        return false;
    }
    public FoodStackSlot? GetFoodSlot(int index)
    {
        if (foodSlots == null || index < 0 || index >= foodSlots.Length)
            return null;
        
        return foodSlots[index];
    }

    public int TakeOutRandomFood()  //랜덤 음식
    {
        List<int> candidates = new List<int>();

        for (int i = 0; i < foodSlots.Length; i++)
        {
            if (foodSlots[i].HasValue) candidates.Add(i);
        }
        if (candidates.Count == 0) return -1;

        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }
    public bool FoodEmptyCheck()
    {
        for (int i = 0; i < foodSlots.Length; i++)
        {
            if (foodSlots[i].HasValue)
                return false; // 하나라도 있으면 안 비었음
        }
        return true;
    }
    public void UpgradeFoodStorageindex()
    {
        if (Capacity >= MaxCapacity || StorageLevel >= MaxLevel)    //이미 최대면 업그레이드 막기
        {
            return;
        }
        _storagelevel++;   //레벨 올리고, 그 레벨에 맞는 Capacity 적용
        ApplyFoodCapacityByLevel();
    }
    private void ApplyFoodCapacityByLevel()  //레벨에 맞게 창고용량 적용
    {
        int newCap = _storageCapacity; //현재 용량 기준으로 새 용량 계산

        if (_storagelevel <= 1) newCap = _storageDatalevelCap[_storagelevel];
        else if (_storagelevel == 2) newCap = _storageDatalevelCap[_storagelevel];
        else if (_storagelevel == 3) newCap = _storageDatalevelCap[_storagelevel];
        else if (_storagelevel == 4) newCap = _storageDatalevelCap[_storagelevel];
        else newCap = _storageDatalevelCap[_storagelevel];
        if (_storageCapacity == newCap) return;

        if (foodSlots == null) foodSlots = new FoodStackSlot?[newCap];   //실제 슬롯 배열 크기 변경(기존 데이터 유지)
        else if (foodSlots.Length != newCap) Array.Resize(ref foodSlots, newCap);

        _storageCapacity = newCap;     //현재 용량 갱신
    }

    public bool PayFoodStorageUpgrade()  //돈낼수 있는가 확인
    {
        int cost = FoodGetUpgradeCost();
        int money = GameManager.Instance.PlayerGold;

        if (money < cost)
            return false;

        GameManager.Instance.SetGold(-cost);
        return true;
    }
    public void GetFoodUpgradeData()
    {
        var boxData = DataManager.Instance.BoxDatabase.BoxInfoData;
        foreach (var box in boxData.datas)
        {
            if (box.BoxID >= 80006 && box.BoxID <= 80010) //창고 업그레이드 박스 ID
            {
                _storageDatalevelCost.Add(box.BoxLevel, box.ExpansionCost);
                _storageDatalevelCap.Add(box.BoxLevel, box.SlotCount);
            }
        }
    }
    public int FoodGetUpgradeCost()  //비용 계산
    {

        if (_storagelevel <= 1) return _storageDatalevelCost[_storagelevel];
        if (_storagelevel == 2) return _storageDatalevelCost[_storagelevel];
        if (_storagelevel == 3) return _storageDatalevelCost[_storagelevel];
        if (_storagelevel == 4) return _storageDatalevelCost[_storagelevel];
        return _storageDatalevelCost[_storagelevel];
    }
    public bool TryFoodRemoveAt(int slotIndex)
    {
        if (foodSlots == null) return false;
        if (slotIndex < 0 || slotIndex >= foodSlots.Length) return false;
        if (!foodSlots[slotIndex].HasValue) return false;

        foodSlots[slotIndex] = null;

        OnSlotChanged?.Invoke(slotIndex);
        return true;
    }
    public bool TryRemoveFoodById(int foodhId)  //음식ID로 없애는함수
    {
        if (foodSlots == null) return false;

        for (int i = 0; i < foodSlots.Length; i++)
        {
            if (!foodSlots[i].HasValue) continue;
            if (foodSlots[i].Value.FoodId != foodhId) continue;

            foodSlots[i] = null;

            OnSlotChanged?.Invoke(i);
            return true;
        }
        return false;
    }  
    public bool HasSpaceForFood(int foodId)  //보관함 공간 확인
    {
        if (foodSlots == null || foodSlots.Length == 0) return false;
        for (int i = 0; i < foodSlots.Length; i++)
        {
            if (!foodSlots[i].HasValue) return true;
        }
        return false;
    }

    private void SyncFoodStorageSaveData()
    {
        var data = DataManager.Instance.Box.Foodstorage;
        data.StorageLevel = _storagelevel;
        data.AcquireCounter = _acquireCounter;
        data.SavedSlots.Clear();

        for (int i = 0; i < foodSlots.Length; i++)
        {
            if (foodSlots[i].HasValue)
            {
                data.SavedSlots.Add(new SavedFoodSlotData
                {
                    Index = i,
                    FoodId = foodSlots[i].Value.FoodId,
                    LastAcquiredOrder = foodSlots[i].Value.LastAcquiredOrder
                });
            }
        }
    }

    private void SyncFoodStorageLoadData()
    {
        DataManager.Instance.Hub.OnDataLoaded -= SyncFoodStorageLoadData;

        var data = DataManager.Instance.Box.Foodstorage;
        _storagelevel = data.StorageLevel > 0 ? data.StorageLevel : 1;
        _acquireCounter = data.AcquireCounter;

        ApplyFoodCapacityByLevel(); 

        for (int i = 0; i < foodSlots.Length; i++) foodSlots[i] = null;
        foreach (var saved in data.SavedSlots)
        {
            if (saved.Index >= 0 && saved.Index < foodSlots.Length)
            {
                foodSlots[saved.Index] = new FoodStackSlot
                {
                    FoodId = saved.FoodId,
                    LastAcquiredOrder = saved.LastAcquiredOrder
                };
                OnSlotChanged?.Invoke(saved.Index);
            }
        }
    }
}
