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

    [SerializeField] int _storageCapacity = 20;  //인벤크기
    long _acquireCounter = 0;

    public event Action<int> OnSlotChanged; // 특정 슬롯이 바뀌었음을 알림(UI는 보통 RefreshAll)

    public int Capacity => FishSlots?.Length ?? 0;

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
}