using System;
using UnityEngine;

public class PlayerData
{
    public int CharID{ get; private set; }
    public string NameKey { get; private set; }

    public float Hunger { get; private set; }
    public float Stamina { get; private set; }
    public float MoveSpeed { get; private set; }
    public float FishingSpeed { get; private set; }
    public float RestSpeed { get; private set; } 
    public int DoongDoongStat { get; private set; }

    public string CurrentResourceID;
    public string CurrentPrefabPath;

    public int VisualGroupID { get; private set; }
    public int UpgradGroupID { get; private set; }

    public event Action<float> OnHungerChanged;
    public event Action<float> OnStaminaChanged;
    public event Action<int> OnDoongDoongChanged;

    // 추가된 변수들  
    public float MaxHunger = 100f; 
    public float MaxStamina = 100f;
    public float MaxMoveSpeed = 100f;
    public float MaxFishingSpeed = 100f;
    public float MaxRestSpeed = 2f;

    public int HungerLevel = 0;
    public int StaminaLevel = 0;
    public int MoveSpeedLevel = 0;
    public int FishingSpeedLevel = 0;
    public int RestSpeedLevel = 0;
   
    public void Initialize(CharacterDataSO SO)
    {
        SetHunger(SO.BaseHunger);
        SetStamina(SO.BaseStamina);
        SetMoveSpeed(SO.BaseMoveSpeed);
        SetFishingSpeed(SO.BaseFishingSpeed);
        SetRestSpeed(SO.BaseRestSpeed);
        SetDoongDoongStat(SO.BaseDoongDoongStat);

        MaxHunger = SO.BaseHunger;
        MaxStamina = SO.BaseStamina;

        CharID = SO.ID;
        NameKey = SO.CharacterName_String;
        VisualGroupID = SO.VisualGroupID;
        UpgradGroupID = SO.UpgradGroupID;

        HungerLevel = 0;
        StaminaLevel = 0;
        MoveSpeedLevel = 0;
        FishingSpeedLevel = 0;
        RestSpeedLevel = 0;
    }

    //이렇게 쓰면 된다
    //var characters = CharacterCsvLoader.LoadFromResources("Data/character");
    //var playerData = new PlayerData();
    //playerData.SetHunger(playerData.Hunger - 4); 예시


    public void SetHunger(float value)  
    {
        value = Math.Clamp(value, 0f, MaxHunger);
        Hunger = value;
        OnHungerChanged?.Invoke(Hunger);
    }
    public void SetStamina(float value)
    {
        value = Math.Clamp(value, 0f, MaxStamina);
        Stamina = value;
        OnStaminaChanged?.Invoke(Stamina);
    }
    public void SetDoongDoongStat(int value)
    {
        value = Math.Clamp(value, 0, 1000);
        DoongDoongStat = value;
        OnDoongDoongChanged?.Invoke(DoongDoongStat);
    }

    //혹시 모를까봐 미리 만들어둠
    public void SetMoveSpeed(float value)
    {
        value = Math.Clamp(value, 0f, MaxMoveSpeed);
        MoveSpeed = value;
    }
    public void SetFishingSpeed(float value)
    {
        value = Math.Clamp(value, 0f, MaxFishingSpeed);
        FishingSpeed = value;
    }
    public void SetRestSpeed(float value)
    {
        value = Math.Clamp(value, 0f, MaxRestSpeed);
        RestSpeed = value;
    }


    //여기서 이벤트 발행이 되지 않아 나중에 구독 시켜줘야함!
    public void SyncCharacterDataSave()
    {
        var data = DataManager.Instance.Box.Character;

        data._hunger.Value = Hunger;
        data._hunger.Level = HungerLevel;

        data._stamina.Value = Stamina;
        data._stamina.Level = StaminaLevel;

        data._moveSpeed.Value = MoveSpeed;
        data._moveSpeed.Level = MoveSpeedLevel;

        data._fishingSpeed.Value = FishingSpeed;
        data._fishingSpeed.Level = FishingSpeedLevel;

        data._restSpeed.Value = RestSpeed;
        data._restSpeed.Level = RestSpeedLevel;

        data._doongdoongStat = DoongDoongStat;

        Debug.Log("<color=cyan>캐릭터 스탯 저장</color>");
    }

    public void SyncCharacterDataLoad()
    {
        var data = DataManager.Instance.Box.Character;
        if (data == null) return;

        HungerLevel = data._hunger.Level;
        StaminaLevel = data._stamina.Level;
        MoveSpeedLevel = data._moveSpeed.Level;
        FishingSpeedLevel = data._fishingSpeed.Level;
        RestSpeedLevel = data._restSpeed.Level;

        SetHunger(data._hunger.Value);
        SetStamina(data._stamina.Value);
        SetMoveSpeed(data._moveSpeed.Value);
        SetFishingSpeed(data._fishingSpeed.Value);
        SetRestSpeed(data._restSpeed.Value);
        SetDoongDoongStat(data._doongdoongStat);
    }
}