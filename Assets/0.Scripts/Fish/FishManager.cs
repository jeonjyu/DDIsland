using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct FishInstance
{
  public int fishId;
  public float length;
  public int price;
}
public class FishManager : Singleton<FishManager>
{
    Dictionary<int, FishDefinition> fishById;  //도감용
    List<FishDefinition> allFish;  
    List<FishDefinition> seasonFish;  //계절별 나올 물고기

     [SerializeField]private EnvironmentModel environment;
    private Season season;
    int seasonBit;

    private void Awake()
    {
        base.Awake();
    }
    private void Start()
    { 

       // season = environment.CurrentSeason;
       season = Season.Spring;  //테스트
        allFish = FishCsvLoader.LoadFromResources("Data/fish");
        seasonFish = new List<FishDefinition>();

        fishById = new Dictionary<int, FishDefinition>();

       // HandleSeasonChanged(environment.CurrentSeason);
        HandleSeasonChanged(Season.Spring);  //테스트
        foreach (var fish in allFish)
        {
            fishById[fish.ID] = fish;
        }
    }
    private void OnEnable()
    {
        if (environment != null)
            environment.OnSeasonChanged += HandleSeasonChanged;
    }
    private void OnDisable()
    {
        if (environment != null)
            environment.OnSeasonChanged -= HandleSeasonChanged;
    }
    private void Update()
    {

    }

    public int SeasonToBit(Season season)
    {
        return season switch
        {
            Season.Spring => 1,
            Season.Summer => 2,
            Season.Autumn => 4,
            Season.Winter => 8,
            _ => 0
        };
    }
    public void HandleSeasonChanged(Season newSeason)
    {
        season = newSeason;
        seasonBit = SeasonToBit(season);
        seasonFish.Clear();
        foreach (var fish in allFish)
            if ((fish.ArriveSeason & seasonBit) != 0)
                seasonFish.Add(fish);
    }
    //public void RandomFish()
    //{
    //    if (seasonFish.Count == 0) return;
    //    FishDefinition randomFish = seasonFish[UnityEngine.Random.Range(0, seasonFish.Count)];
    //    float length = UnityEngine.Random.Range(randomFish.MinLength, randomFish.MaxLength);
    //    int price = randomFish.Price;
    //    FishToStorage(randomFish, length, price);
    //    Debug.Log($"Name: {randomFish.NameKey}, price: {price}, Length: {length}");
    //}
    public void FishToStorage(FishDefinition randomFish, float length, int price)
    {
        FishInstance fish = new FishInstance();
        fish.fishId = randomFish.ID;
        fish.length = length;
        fish.price = price;
        bool success = StorageManager.Instance.TryAddToStorage(fish);
        if (!success)
        {
            //가득함
        }
    }

    public void PickRandomSeasonFish()
    {
        if (seasonFish.Count == 0) return;
        FishDefinition randomFish = seasonFish[UnityEngine.Random.Range(0, seasonFish.Count)];
        CreateInstance(randomFish);
    }
    public void CreateInstance(FishDefinition fish)
    {
        float length = UnityEngine.Random.Range(fish.MinLength, fish.MaxLength);
        int price = fish.Price;
        Debug.Log($"Name: {fish.NameKey}, price: {price}, Length: {length}");
        FishToStorage(fish, length, price);
    }
    public void SpecialConditions()
    {
       // FishDefinition specialFish = allFish.Find(f => f.ID == 10001);

    }
}
