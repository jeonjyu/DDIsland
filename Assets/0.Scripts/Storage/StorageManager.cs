using System;
using UnityEngine;

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
}

public class StorageManager : Singleton<StorageManager>
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
            if (FishSlots[i].HasValue && FishSlots[i].Value.FishId == fish.FishId)
            {
                var slot = FishSlots[i].Value;

                slot.Count += 1;
                //slot.lastLength = fish.length;
                slot.MaxPrice = Mathf.Max(slot.MaxPrice, fish.Price);
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
        slot.Count--;

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

    public FishStackSlot? GetSlot(int index)
    {
        if (FishSlots == null || index < 0 || index >= FishSlots.Length)
            return null;

        return FishSlots[index];
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

        if (money < cost)
            return false;

        GameManager.Instance.SetGold(-cost);
        return true;
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