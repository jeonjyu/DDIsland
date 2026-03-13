using System;
using UnityEngine;
using System.Collections.Generic;

public struct FishInstance
{
    public int FishId;
    public float Length;
    public int Price;
}

public struct FishStackSlot
{
    public int FishId;
    public int Count;
    public long LastAcquiredOrder;
    public int MaxPrice;
    public int TotalPrice;
}

public class FishStorageManager : Singleton<FishStorageManager>
{
    FishStackSlot?[] FishSlots;

    int _storageCapacity = 10;  //인벤크기
    int _storagelevel = 1;
    long _acquireCounter = 0;
    public const int MaxLevel = 5;
    public const int MaxCapacity = 50;

    public event Action<int> OnSlotChanged; // 특정 슬롯이 바뀌었음을 알림(UI는 보통 RefreshAll)
    public int Capacity => FishSlots?.Length ?? 0;
    public int StorageLevel => _storagelevel;
    public FishStackSlot?[] FishSlotData => FishSlots;

    private void Awake()
    {
        base.Awake();
        FishSlots = new FishStackSlot?[_storageCapacity];
    }

    public bool TryAddToStorage(FishInstance fish)
    {
        if (FishSlots == null || FishSlots.Length == 0)
            return false;

        // 1 같은 FishId 있으면 스택 증가
        for (int i = 0; i < FishSlots.Length; i++) 
        {
            if (FishSlots[i].HasValue && FishSlots[i].Value.
                FishId == fish.FishId)
            {
                var slot = FishSlots[i].Value;

                slot.Count += 1;
                //slot.lastLength = fish.length;
                slot.MaxPrice = Mathf.Max(slot.MaxPrice, fish.Price);
                slot.TotalPrice += fish.Price;
                slot.LastAcquiredOrder = ++_acquireCounter;

                FishSlots[i] = slot;
                OnSlotChanged?.Invoke(i);
                return true;
            }
        }
        // 2 빈 슬롯에 새로 추가
        for (int i = 0; i < FishSlots.Length; i++)
        {
            if (!FishSlots[i].HasValue)
            {
                FishSlots[i] = new FishStackSlot
                {
                    FishId = fish.FishId,
                    Count = 1,
                    MaxPrice = fish.Price,
                    TotalPrice = fish.Price,
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

        int removedPrice = Mathf.RoundToInt((float)slot.TotalPrice / slot.Count);
        slot.Count--;
        slot.TotalPrice -= removedPrice;

        if (slot.TotalPrice < 0) slot.TotalPrice = 0;

        if (slot.Count <= 0)
        {
            FishSlots[slotIndex] = null;
        }
        else
        {
            FishSlots[slotIndex] = slot;
        }

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

            int removedPrice = Mathf.RoundToInt((float)slot.TotalPrice / slot.Count);
            slot.Count--;
            slot.TotalPrice -= removedPrice;

            if (slot.TotalPrice < 0) slot.TotalPrice = 0;

            if (slot.Count <= 0) FishSlots[i] = null;
            else FishSlots[i] = slot;

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
    public bool FishFullCheck()  //보관함 5칸이하로 비어있는지 체크
    {
       int emptyCount = 0;
        for (int i = 0; i < FishSlots.Length; i++)
        {
            if (!FishSlots[i].HasValue) emptyCount++;
        }

        return emptyCount <= 5;
    }

    public void SellAllFish()  //물고기 판매
    {
        int totalGold = 0;
        for (int i = 0; i < FishSlots.Length; i++)
        {
            if (FishSlots[i].HasValue)
            {
                totalGold += FishSlots[i].Value.TotalPrice;
                FishSlots[i] = null;
                OnSlotChanged?.Invoke(i);
            }
        }
        if (totalGold > 0) GameManager.Instance.SetGold(totalGold);
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

        if (_storagelevel <= 1) newCap = 10;
        else if (_storagelevel == 2) newCap = 20;
        else if (_storagelevel == 3) newCap = 30;
        else if (_storagelevel == 4) newCap = 40;
        else newCap = 50;
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
                Count = 1,
                MaxPrice = price,
                TotalPrice = price,
                LastAcquiredOrder = ++_acquireCounter
            };

            OnSlotChanged?.Invoke(i);
        }
    }
    public int GetUpgradeCost()  //비용 계산
    {

        if (_storagelevel <= 1) return 100;
        if (_storagelevel == 2) return 200;
        if (_storagelevel == 3) return 300;
        if (_storagelevel == 4) return 400;
        return 500; 
    }
}