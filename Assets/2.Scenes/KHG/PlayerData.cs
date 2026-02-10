using System;
using UnityEngine.TextCore.Text;

public class PlayerData
{
    public int CharID{ get; private set; }
    public string NameKey { get; private set; }

    public float Hunger { get; private set; }
    public float Stamina { get; private set; }
    public float MoveSpeed { get; private set; }
    public float FishingSpeed { get; private set; }
    public int DoongDoongStat { get; private set; }

    public string CurrentResourceID;
    public string CurrentPrefabPath;

    public event Action<float> OnHungerChanged;
    public event Action<float> OnStaminaChanged;
    public event Action<int> OnDoongDoongChanged;

    public void Initialize(CharacterDefinition characterDefinition)
    {
        CharID = characterDefinition.CharID;
        NameKey = characterDefinition.Name;

        SetHunger(characterDefinition.BaseHunger);
        SetStamina(characterDefinition.BaseStamina);
        SetDoongDoongStat(characterDefinition.BaseDoongDoongStat);
        MoveSpeed = characterDefinition.BaseMoveSpeed;
        FishingSpeed = characterDefinition.BaseFishingSpeed;
    }

    //이렇게 쓰면 된다
    //var characters = CharacterCsvLoader.LoadFromResources("Data/character");
    //var playerData = new PlayerData();
    //playerData.SetHunger(playerData.Hunger - 4); 예시


    public void SetHunger(float value)  
    {
        value = Math.Clamp(value, 0f, 100f);
        Hunger = value;
        OnHungerChanged?.Invoke(Hunger);
    }
    public void SetStamina(float value)
    {
        value = Math.Clamp(value, 0f, 100f);
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
        value = Math.Clamp(value, 0f, 100f);
        MoveSpeed = value;
    }
    public void SetFishingSpeed(float value)
    {
        value = Math.Clamp(value, 0f, 100f);
        FishingSpeed = value;
    }

}
