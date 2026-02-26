using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class StorageManager : Singleton<StorageManager>
{
    FishInstance?[] fishSlots;  //현재는 물고기만 요리는 상정아직 안함
    [SerializeField] int storageCapacity = 20;  //창고 업글하면 이거 늘리면돰 현재는 물고기만 다룸


    public event Action<int> OnSlotChanged;


    public int Capacity => fishSlots?.Length ?? 0;

    private void Awake()
    {
        base.Awake();
        fishSlots = new FishInstance?[storageCapacity];
    }

    private void Start()
    {

    }
    public bool TryAddToStorage(FishInstance fish)  //첫 빈칸에 넣기
    {
        if (fishSlots == null || fishSlots.Length == 0) return false;

        for (int i = 0; i < fishSlots.Length; i++)
        {
            if (!fishSlots[i].HasValue)
            {
                fishSlots[i] = fish;
                OnSlotChanged?.Invoke(i);
                return true;
            }
        }
        return false;
    }
    public bool TryRemoveAt(int slotIndex) //해당 칸만 비우기
    {
        if (fishSlots == null) return false;
        if (slotIndex < 0 || slotIndex >= fishSlots.Length) return false;
        if (!fishSlots[slotIndex].HasValue) return false;

        //대강 여기쯤에 돈더하기/판매 처리기능 
        fishSlots[slotIndex] = null;
        OnSlotChanged?.Invoke(slotIndex);
        return true;
    }

    public FishInstance? GetSlot(int index)  //조회용
    {
        if (fishSlots == null || index < 0 || index >= fishSlots.Length)
            return null;
        return fishSlots[index];
    }


    //나중에 업그레이드 기능 넣기 
}
