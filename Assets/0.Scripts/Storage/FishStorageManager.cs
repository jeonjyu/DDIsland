using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public struct FishInstance
{
    public int FishId;
    public float Length;
    public int Price;
}

public struct FishStackSlot
{
    public int FishId;
    public long LastAcquiredOrder;
    public int Price;
}

public class FishStorageManager : Singleton<FishStorageManager>
{
    FishStackSlot?[] FishSlots;

    int _storageCapacity = 7;  //인벤크기
    int _storagelevel = 1;
    Dictionary<int,int> _storageDatalevelCost;
    Dictionary<int, int> _storageDatalevelCap;
    long _acquireCounter = 0;
    public const int MaxLevel = 5;
    public const int MaxCapacity = 27;

    public event Action<int> OnSlotChanged; // 특정 슬롯이 바뀌었음을 알림(UI는 보통 RefreshAll)
    public int Capacity => FishSlots?.Length ?? 0;
    public int StorageLevel => _storagelevel;
    public FishStackSlot?[] FishSlotData => FishSlots;

    private void Awake()
    {
        base.Awake();
        FishSlots = new FishStackSlot?[_storageCapacity];
        _storageDatalevelCost = new Dictionary<int, int>();
        _storageDatalevelCap = new Dictionary<int, int>();
        GetUpgradeData();
    }
    private void Start()
    {
        StartCoroutine(Frame());
    }

    IEnumerator Frame()
    {
        yield return null;
        yield return null;
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            DataManager.Instance.Hub.OnDataLoaded += SyncFishStorageLoadData;
        }
    }

    private void OnEnable()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
            DataManager.Instance.Hub.OnRequestSave += SyncFishStorageSaveData;
    }
    private void OnDisable()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
            DataManager.Instance.Hub.OnRequestSave -= SyncFishStorageSaveData;
    }

    public bool TryAddToStorage(FishInstance fish)
    {
        if (FishSlots == null || FishSlots.Length == 0)
            return false;

        for (int i = 0; i < FishSlots.Length; i++)
        {
            if (!FishSlots[i].HasValue)
            {
                FishSlots[i] = new FishStackSlot
                {
                    FishId = fish.FishId,
                    Price = fish.Price,
                    LastAcquiredOrder = ++_acquireCounter,
                };

                OnSlotChanged?.Invoke(i);
                return true;
            }
        }
        // 꽉 참
        return false;
    }

    public bool TryRemoveAt(int slotIndex)
    {
        if (FishSlots == null) return false;
        if (slotIndex < 0 || slotIndex >= FishSlots.Length) return false;
        if (!FishSlots[slotIndex].HasValue) return false;

        var slot = FishSlots[slotIndex].Value;

        FishSlots[slotIndex] = null;

        OnSlotChanged?.Invoke(slotIndex);
        return true;
    }
    public bool TryRemoveFishById(int fishId)  //물고기ID로 없애는함수
    {
        if (FishSlots == null) return false;

        for (int i = 0; i < FishSlots.Length; i++)
        {
            if (!FishSlots[i].HasValue) continue;
            if (FishSlots[i].Value.FishId != fishId) continue;

            var slot = FishSlots[i].Value;
            FishSlots[i] = null;

            OnSlotChanged?.Invoke(i);
            return true;
        }
        return false;
    }
    public FishStackSlot? GetSlot(int index)
    {
        if (FishSlots == null || index < 0 || index >= FishSlots.Length)
            return null;

        return FishSlots[index];
    }
    public bool FishEmptyCheck()
    {
        for (int i = 0; i < FishSlots.Length; i++)
        {
            if (FishSlots[i].HasValue)
                return false; // 하나라도 있으면 안 비었음
        }
        return true;
    }
    public bool ShouldSellFish()  //보관함 5칸이하로 비어있는지 체크
    {
        int emptyCount = 0;
        for (int i = 0; i < FishSlots.Length; i++)
        {
            if (!FishSlots[i].HasValue) emptyCount++;
        }

        return emptyCount < 5;
    }

    public void SellAllFish()  //물고기 판매
    {
        int totalGold = 0;
        for (int i = 0; i < FishSlots.Length; i++)
        {
            if (FishSlots[i].HasValue)
            {
                totalGold += FishSlots[i].Value.Price;
                FishSlots[i] = null;

                OnSlotChanged?.Invoke(i);
            }
        }
        if (totalGold > 0)
        {
            GameManager.Instance.SetGold(totalGold);
            RewardEffect.Instance.PlaySellGoldEffect();
        }
    }

    public void UpgradeStorageindex()
    {
        if (Capacity >= MaxCapacity || StorageLevel >= MaxLevel)    //이미 최대면 업그레이드 막기
        {
            return;
        }
        _storagelevel++;   //레벨 올리고, 그 레벨에 맞는 Capacity 적용
        ApplyCapacityByLevel();
    }
    private void ApplyCapacityByLevel()  //레벨에 맞게 창고용량 적용
    {
        int newCap = _storageCapacity; //현재 용량 기준으로 새 용량 계산

        if (_storagelevel <= 1) newCap = _storageDatalevelCap[_storagelevel];
        else if (_storagelevel == 2) newCap = _storageDatalevelCap[_storagelevel];
        else if (_storagelevel == 3) newCap = _storageDatalevelCap[_storagelevel];
        else if (_storagelevel == 4) newCap = _storageDatalevelCap[_storagelevel];
        else newCap = 27;
        if (_storageCapacity == newCap) return;

        if (FishSlots == null) FishSlots = new FishStackSlot?[newCap];   //실제 슬롯 배열 크기 변경(기존 데이터 유지)
        else if (FishSlots.Length != newCap) Array.Resize(ref FishSlots, newCap);

        _storageCapacity = newCap;     //현재 용량 갱신
    }

    public bool PayStorageUpgrade()  //돈낼수 있는가 확인
    {
        int cost = GetUpgradeCost();
        int money = GameManager.Instance.PlayerGold;
        //int money = 100000000;

        if (money < cost)
            return false;

        GameManager.Instance.SetGold(-cost);
        return true;
    }
    public void DebugFillForSellTest(int FishListNum)  //테스트용 함수  테스트로 좋은 블롭이는 29번  
    {
        if (FishSlots == null || FishSlots.Length == 0) return;

        // 실제 물고기 데이터 하나 가져오기
        var fishDb = DataManager.Instance.FishingDatabase.FishData;

        // 첫 번째 물고기 하나 사용
        var fishList = new List<FishDataSO>(fishDb.datas);
        var fishDef = fishList[FishListNum];

        int fishId = fishDef.ID;
        int price = fishDef.Price;

        int targetFillCount = FishSlots.Length - 4; // 빈칸 4칸 남기기 (판매조건)

        for (int i = 0; i < targetFillCount; i++)
        {
            FishSlots[i] = new FishStackSlot
            {
                FishId = fishId,
                Price = price,
                LastAcquiredOrder = ++_acquireCounter
            };

            OnSlotChanged?.Invoke(i);
        }
    }
    public void GetUpgradeData()
    {
        var boxData = DataManager.Instance.BoxDatabase.BoxInfoData;
        foreach (var box in boxData.datas)
        {
            if (box.BoxID >= 80001 && box.BoxID <= 80005) //창고 업그레이드 박스 ID
            {
                _storageDatalevelCost.Add(box.BoxLevel, box.ExpansionCost);
                _storageDatalevelCap.Add(box.BoxLevel, box.SlotCount);
            }
        }
    }
    public int GetUpgradeCost()  //비용 계산
    {
        if (_storagelevel <= 1) return _storageDatalevelCost[_storagelevel];
        if (_storagelevel == 2) return _storageDatalevelCost[_storagelevel];
        if (_storagelevel == 3) return _storageDatalevelCost[_storagelevel];
        if (_storagelevel == 4) return _storageDatalevelCost[_storagelevel];
        return _storageDatalevelCost[_storagelevel];
    }

    private void SyncFishStorageSaveData()
    {
        var userData = DataManager.Instance.Hub._allUserData;
        if (userData == null || userData.fishstoage == null) return;

        userData.fishstoage.StorageLevel = _storagelevel;
        userData.fishstoage.AcquireCounter = _acquireCounter;

        userData.fishstoage.SlotList.Clear();

        for (int i = 0; i < FishSlots.Length; i++)
        {
            if (FishSlots[i].HasValue)
            {
                var slot = FishSlots[i].Value;
                userData.fishstoage.SlotList.Add(new SavedFishSlotData
                {
                    Index = i,
                    FishId = slot.FishId,
                    LastAcquiredOrder = slot.LastAcquiredOrder,
                    MaxPrice = slot.Price,
                });
            }
        }
    }

    private void SyncFishStorageLoadData()
    {
        DataManager.Instance.Hub.OnDataLoaded -= SyncFishStorageLoadData;

        var userData = DataManager.Instance.Hub._allUserData.fishstoage;

        Debug.Log(userData.AcquireCounter);
        Debug.Log(DataManager.Instance.Hub._allUserData.Currency._gold);
        
        if (userData == null || userData == null) return;

        _storagelevel = userData.StorageLevel;
        _acquireCounter = userData.AcquireCounter;


        ApplyCapacityByLevel();

        int dataCount = userData.SlotList != null ? userData.SlotList.Count : -2;

        // 기존 슬롯 초기화
        for (int i = 0; i < FishSlots.Length; i++) FishSlots[i] = null;
        if (userData.SlotList != null)
        {
            foreach (var savedSlot in userData.SlotList)
            {
                if (savedSlot.Index >= 0 && savedSlot.Index < FishSlots.Length)
                {
                    FishSlots[savedSlot.Index] = new FishStackSlot
                    {
                        FishId = savedSlot.FishId,
                        LastAcquiredOrder = savedSlot.LastAcquiredOrder,
                        Price = savedSlot.MaxPrice,
                    };
                    Debug.Log($"<color=green> 인덱스: {savedSlot.Index} 에 물고기 ID: {savedSlot.FishId} 넣음</color>");
                }
            }
        }
        for (int i = 0; i < FishSlots.Length; i++) OnSlotChanged?.Invoke(i);
    }
}