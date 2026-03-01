using System.Collections.Generic;
using UnityEngine;
public struct FishingContext 
{
    public ArriveSeason Season;
    public bool IsMorning;
    public bool IsAfternoon;
    public bool IsNight;
    public bool IsLake;
    public bool IsSea;
    public bool IsRain;
    public bool IsCherryblossom;
    public bool IsMaple;
    public bool IsSnow;
}
public class FishManager : Singleton<FishManager>
{
    Dictionary<int, FishDataSO> _fishById;
    List<FishDataSO> _allFish;

    [SerializeField] private EnvironmentModel _environment;

    private ArriveSeason _currentSeason = ArriveSeason.Spring;

    private void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _allFish = new List<FishDataSO>(DataManager.Instance.FishingDatabase.FishData.datas);

        _fishById = new Dictionary<int, FishDataSO>();

        foreach (var fish in _allFish)
        {
            _fishById[fish.ID] = fish;
        }

        if (_environment != null)
        {
            _currentSeason = ConvertSeason(_environment.CurrentSeason);
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
    private ArriveSeason ConvertSeason(Season season)
    {
        return season switch
        {
            Season.Spring => ArriveSeason.Spring,
            Season.Summer => ArriveSeason.Summer,
            Season.Autumn => ArriveSeason.Autumn,
            Season.Winter => ArriveSeason.Winter,
            _ => ArriveSeason.Spring
        };
    }
    //환경에서 계절 바뀔 때 호출될 함수
    private void HandleSeasonChanged(Season newSeason)
    {
        _currentSeason = ConvertSeason(newSeason);
        Debug.Log($"[FishManager] Season Converted => {_currentSeason}");
    }

    public void PickRandomSeasonFish()
    {
        FishingContext ctx = BuildContextFromCurrentState();
        PickRandomSeasonFish(ctx);
    }

    //ctx.Season에 현재 계절 넣어줌
    private FishingContext BuildContextFromCurrentState()
    {
        FishingContext ctx = new FishingContext();
        ctx.Season = _currentSeason;

        // 일단 테스트용 기본값
        ctx.IsMorning = true;
        ctx.IsLake = true;

        return ctx;
    }

    public void PickRandomSeasonFish(FishingContext ctx)  //낚시 결과 뽑기
    {
        var candidates = BuildCandidates(ctx);  //후보 선정
        if (candidates == null || candidates.Count == 0)
        {
            Debug.LogWarning("[FishManager] 후보가 없음");
            return;
        }

        var pickedDrop = PickWeighted(candidates);  //확률 뽑기
        if (pickedDrop == null)
        {
            Debug.LogWarning("[FishManager] PickWeighted 실패");
            return;
        }

        int fishId = pickedDrop.RewardItem;  //뽑힌 드랍의 RewardItem을 fishId로 사용

        var fishSO = GetFishSO(fishId);  //뽑힌 드랍의 RewardItem을 fishId로 사용
        if (fishSO == null)
        {
            Debug.LogWarning($"[FishManager] FishDataSO 못찾음 fishId={fishId}");
            return;
        }

        CreateInstance(fishSO);  //길이,가격 계산 후 저장
    }

    public void CreateInstance(FishDataSO fish)
    {
        float length = Random.Range(fish.MinLength, fish.MaxLength);
        int price = fish.Price;

        Debug.Log($"Name: {fish.FishName_String}, price: {price}, Length: {length}");

        FishToStorage(fish, length, price);
    }

    public void FishToStorage(FishDataSO randomFish, float length, int price)
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

    private FishDataSO GetFishSO(int fishId)
    {
        if (_fishById != null && _fishById.TryGetValue(fishId, out var fish))
            return fish;
        return null;
    }

    private List<FishingDropDataSO> BuildCandidates(FishingContext ctx)
    {
        var db = DataManager.Instance.FishingDatabase.FishingDropData;
        var result = new List<FishingDropDataSO>();

        foreach (var drop in db.datas)
        {
            if (drop == null) continue;

            //계절 조건
            if (ctx.Season == ArriveSeason.Spring && !drop.IsSpring) continue;
            if (ctx.Season == ArriveSeason.Summer && !drop.IsSummer) continue;
            if (ctx.Season == ArriveSeason.Autumn && !drop.IsAutumn) continue;
            if (ctx.Season == ArriveSeason.Winter && !drop.IsWinter) continue;

            //시간 조건
            if (ctx.IsMorning && !drop.IsMorning) continue;
            if (ctx.IsAfternoon && !drop.IsAfternoon) continue;
            if (ctx.IsNight && !drop.IsNight) continue;

            //장소 조건
            if (ctx.IsLake && !drop.IsLake) continue;
            if (ctx.IsSea && !drop.IsSea) continue;

            result.Add(drop);
        }

        return result;
    }

    private FishingDropDataSO PickWeighted(List<FishingDropDataSO> candidates)  //가중치랜덤
    {
        if (candidates == null || candidates.Count == 0) return null;

        float total = 0;
        for (int i = 0; i < candidates.Count; i++)
        {
            total += Mathf.Max(0, candidates[i].Probability);
        }

        if (total <= 0)
        {
            return candidates[Random.Range(0, candidates.Count)];
        }

        float r = Random.Range(0, total);
        float acc = 0;

        for (int i = 0; i < candidates.Count; i++)
        {
            acc += Mathf.Max(0, candidates[i].Probability);
            if (r < acc) return candidates[i];
        }

        return candidates[candidates.Count - 1];
    }
}