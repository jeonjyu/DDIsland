using System;
using UnityEngine;
public struct FoodInstance
{
    public int FoodId;

}
public struct FoodStackSlot
{
    public int FoodId;
    public int Count;
    public long LastAcquiredOrder;
}

public class FoodStorageManager : Singleton<FoodStorageManager>
{
    FoodStackSlot?[] foodSlots;

    int _storageCapacity = 10;  //인벤크기
    int _storagelevel = 1;
    long _acquireCounter = 0;
    public const int MaxLevel = 5;
    public const int MaxCapacity = 50;

    public event Action<int> OnSlotChanged;
    public int Capacity => foodSlots?.Length ?? 0;
    public int StorageLevel => _storagelevel;
    public FoodStackSlot?[] FoodSlotData => foodSlots;
    private void Awake()
    {
        base.Awake();
        foodSlots = new FoodStackSlot?[_storageCapacity];
    }
    public bool TryAddToFoodStorage(FoodInstance food)
    {
        if (foodSlots == null || foodSlots.Length == 0)
            return false;

        for (int i = 0; i < foodSlots.Length; i++)
        {
            if (foodSlots[i].HasValue && foodSlots[i].Value.FoodId == food.FoodId)
            {
                var slot = foodSlots[i].Value;

                slot.Count += 1;
                slot.LastAcquiredOrder = ++_acquireCounter;

                foodSlots[i] = slot;
                OnSlotChanged?.Invoke(i);
                return true;
            }
        }

        for (int i = 0; i < foodSlots.Length; i++)
        {
            if (!foodSlots[i].HasValue)
            {
                foodSlots[i] = new FoodStackSlot
                {
                    FoodId = food.FoodId,
                    Count = 1,
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


        if (_storagelevel <= 1) newCap = 10;
        else if (_storagelevel == 2) newCap = 20;
        else if (_storagelevel == 3) newCap = 30;
        else if (_storagelevel == 4) newCap = 40;
        else newCap = 50;
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

    public int FoodGetUpgradeCost()  //비용 계산
    {

        if (_storagelevel <= 1) return 100;
        if (_storagelevel == 2) return 200;
        if (_storagelevel == 3) return 300;
        if (_storagelevel == 4) return 400;
        return 500;
    }
    public bool TryFoodRemoveAt(int slotIndex)
    {
        if (foodSlots == null) return false;
        if (slotIndex < 0 || slotIndex >= foodSlots.Length) return false;
        if (!foodSlots[slotIndex].HasValue) return false;

        var slot = foodSlots[slotIndex].Value;
        slot.Count--;

        if (slot.Count <= 0)
        {
            foodSlots[slotIndex] = null;
        }
        else
        {
            foodSlots[slotIndex] = slot;
        }

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

            var slot = foodSlots[i].Value;
            slot.Count--;

            if (slot.Count <= 0) foodSlots[i] = null;
            else foodSlots[i] = slot;

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

            if (foodSlots[i].Value.FoodId == foodId) return true;
        }
        return false;
    }
}
