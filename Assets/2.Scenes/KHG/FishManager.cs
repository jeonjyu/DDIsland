using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
/*
 * FishManager
 *
 * 역할
 * - CSV에서 물고기 정의(FishDefinition)를 로드한다.
 * - ID로 빠르게 찾기 위해 Dictionary<int, FishDefinition>을 만든다.
 * - 계절이 바뀌면 해당 시즌에 등장 가능한 물고기 리스트(_seasonFish)를 다시 구성한다.
 * - 낚시 결과로 FishInstance(실제 획득 개체)를 만들고 StorageManager에 저장한다.
 *
 * 용어
 * - FishDefinition : 종(species) 정보(이름,등급,스프라이트키,길이범위 등)
 * - FishInstance   : 획득 개체(instance) 정보(어느 종인지, 길이, 가격 등)
 */
public class FishManager : Singleton<FishManager>
{
    Dictionary<int, FishDefinition> _fishById; //도감용
    List<FishDefinition> _allFish;
    List<FishDefinition> _seasonFish; //계절별 나올 물고기

    [SerializeField] private EnvironmentModel _environment;

    private Season _season;
    int _seasonBit;

    private void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // season = environment.CurrentSeason;
        _season = Season.Spring; //테스트

        _allFish = FishCsvLoader.LoadFromResources("Data/fish");
        _seasonFish = new List<FishDefinition>();
        _fishById = new Dictionary<int, FishDefinition>();

        // HandleSeasonChanged(environment.CurrentSeason);
        HandleSeasonChanged(Season.Spring); //테스트

        foreach (var fish in _allFish)
        {
            _fishById[fish.ID] = fish;
        }
    }

    private void OnEnable()
    {
        if (_environment != null)
            _environment.OnSeasonChanged += HandleSeasonChanged;
    }

    private void OnDisable()
    {
        if (_environment != null)
            _environment.OnSeasonChanged -= HandleSeasonChanged;
    }

    private void Update()
    {
    }

    public int SeasonToBit(Season season)  // 계절 enum 플래그
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

    public void HandleSeasonChanged(Season newSeason)    // 계절 변경 시 시즌 물고기 리스트 재구성
    {
        _season = newSeason;
        _seasonBit = SeasonToBit(_season);

        _seasonFish.Clear();

        foreach (var fish in _allFish)
            if ((fish.ArriveSeason & _seasonBit) != 0)
                _seasonFish.Add(fish);
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

    public void FishToStorage(FishDefinition randomFish, float length, int price)    // StorageManager에 넣기 위한 FishInstance 생성
    {
        FishInstance fish = new FishInstance
        {
            FishId = randomFish.ID,
            Length = length,
            Price = price
        };

        bool success = StorageManager.Instance.TryAddToStorage(fish);
        if (!success)
        {
            // 가득함 처리
        }
    }

    public void PickRandomSeasonFish()    // 시즌 리스트에서 랜덤 선택 => 인스턴스 생성 => 창고 저장
    { 
        if (_seasonFish.Count == 0) return;

        FishDefinition randomFish =
            _seasonFish[UnityEngine.Random.Range(0, _seasonFish.Count)];

        CreateInstance(randomFish);
    }

    public void CreateInstance(FishDefinition fish) // 획득 개체 만들기(길이 랜덤, 가격 설정 등)
    {
        float length = UnityEngine.Random.Range(fish.MinLength, fish.MaxLength);
        int price = fish.Price;

        Debug.Log($"Name: {fish.FishName_String}, price: {price}, Length: {length}");

        FishToStorage(fish, length, price);
    }

    public FishDefinition GetDefinition(int id)     // 도감/정렬/표시용: ID로 정의 찾기
    {
        if (_fishById != null && _fishById.TryGetValue(id, out var def))
            return def;

        return null;
    }

    public void SpecialConditions()
    {
        // FishDefinition specialFish = allFish.Find(f => f.ID == 10001);
    }
}