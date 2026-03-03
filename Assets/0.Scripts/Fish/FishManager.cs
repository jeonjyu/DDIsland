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
    public int GoldAmount;
    public int SeasonChange;
}

public enum SpecialType
{
    None,
    GoldenMandarin = 10018,
    Oarfish = 10019,
    WhiteShark = 10020,
    SpermWhale = 10021,
    Coelacanth = 10022,
    Piraruku = 10025
}
public class FishManager : Singleton<FishManager>
{
    Dictionary<int, FishDataSO> _fishById;
    List<FishDataSO> _allFish;

    private EnvironmentModel _environment;

    private ArriveSeason _currentSeason = ArriveSeason.Spring;

     int _seasonChangeCount;  //실러캔스
    bool _canCoelacanth =  false;
    float _pirarukuTimer; 
    bool _canCanPiraruque = false;  

    private void Awake()
    {
        base.Awake();
        _environment = new EnvironmentModel();
    }

    private void Start()
    {
        _allFish = new List<FishDataSO>(DataManager.Instance.FishingDatabase.FishData.datas);
        _fishById = new Dictionary<int, FishDataSO>();
        _seasonChangeCount = 0;
        foreach (var fish in _allFish)
        {
            _fishById[fish.ID] = fish;
        }

        if (_environment != null)
        {
            _currentSeason = ConvertSeason(_environment.CurrentSeason);
        }
    }
    private void Update()
    {
        if (_canCanPiraruque)
        {
            _pirarukuTimer += Time.deltaTime;
            if (_pirarukuTimer >= 60f)
            {
                _canCanPiraruque = false;
            }
        }
    }
    private void OnEnable()
    {
        if (_environment != null)
        {
            _environment.OnSeasonChanged += HandleSeasonChanged;
            _environment.OnDailyChanged += OpenPiraruku;
        }
    }

    private void OnDisable()
    {
        if (_environment != null)
        {
            _environment.OnSeasonChanged -= HandleSeasonChanged;
            _environment.OnDailyChanged -= OpenPiraruku;
        }
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
        _seasonChangeCount++;
        _canCoelacanth = (_seasonChangeCount >= 24);
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
        ctx.IsMorning = _environment.CurrentDay == DayilyCycle.Day;
        ctx.IsAfternoon = _environment.CurrentDay == DayilyCycle.Sunset;
        ctx.IsNight = _environment.CurrentDay == DayilyCycle.Night;
        //날씨, 골드도 필요함 일단테스트
        ctx.GoldAmount = GameManager.Instance.PlayerGold;
        ctx.IsCherryblossom = true; 
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

        var pickedDrop = PickWeighted(candidates, ctx);  //확률 뽑기
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

            int fishId = drop.RewardItem;
            if (!PassSpecialRuleByFishId(fishId, ctx)) continue;
            result.Add(drop);
        }


        return result;
    }
    private bool PassSpecialRuleByFishId(int fishId, FishingContext ctx)  //특별한물고기들조건
    {
        if (fishId != ((int)SpecialType.GoldenMandarin) &&
               fishId != (int)SpecialType.Oarfish &&
               fishId != (int)SpecialType.WhiteShark &&
               fishId != (int)SpecialType.SpermWhale &&
               fishId != (int)SpecialType.Coelacanth &&
               fishId != (int)SpecialType.Piraruku)
        {
            return true;
        }
        switch (fishId)
        {
            case (int)SpecialType.GoldenMandarin:
                if (ctx.GoldAmount < 100) return false;
                if (!ctx.IsNight) return false;
                bool anyWeather = ctx.IsCherryblossom || ctx.IsRain || ctx.IsMaple || ctx.IsSnow;
                if (!anyWeather) return false;
                return true;
            case (int)SpecialType.Oarfish:
                return (ctx.Season == ArriveSeason.Spring);

            case (int)SpecialType.WhiteShark:
                if (ctx.Season != ArriveSeason.Summer) return false;
                if (!ctx.IsRain) return false;
                return true;

            case (int)SpecialType.SpermWhale:
                return ctx.IsNight;

            case (int)SpecialType.Coelacanth:
                if (!_canCoelacanth) return false;
                if (!ctx.IsNight) return false;
                return true;

            case (int)SpecialType.Piraruku:
                return _canCanPiraruque;

            default:
                return true;
        }
    }
    private FishingDropDataSO PickWeighted(List<FishingDropDataSO> candidates, FishingContext ctx)  //가중치랜덤
    {
        if (candidates == null || candidates.Count == 0) return null;
        float total = 0;
        for (int i = 0; i < candidates.Count; i++)
        {
            total += GetWeight(candidates[i], ctx);
        }

        if (total <= 0)
        {
            return candidates[Random.Range(0, candidates.Count)];
        }

        float r = Random.Range(0, total);
        float acc = 0;

        for (int i = 0; i < candidates.Count; i++)
        {
            acc += GetWeight(candidates[i], ctx);
            if (r < acc) return candidates[i];
        }

        return candidates[candidates.Count - 1];
    }
    public float GetWeight(FishingDropDataSO drop, FishingContext ctx)
    {
        float weight = Mathf.Max(0f, drop.Probability);

        if (drop.RewardItem == (int)SpecialType.Oarfish && ctx.IsCherryblossom)
        {
            weight *= 1.5f;
        }
        return weight;
    }
    public void OpenPiraruku(DayilyCycle dayilyCycle)  
    {
        _pirarukuTimer = 0f;
        _canCanPiraruque = true;
    }
}