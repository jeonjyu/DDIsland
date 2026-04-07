using System;
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
    public int FishPool;
    public float FishLengthBonus;
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
    [SerializeField] PlayerController _playerController;
    [SerializeField] AquariumMgr _aquariumMgr;
    Dictionary<int, FishDataSO> _fishById;
    List<FishDataSO> _allFish;
    // 추가한 부분입니다
    Dictionary<(ArriveSeason, FishType), List<FishDataSO>> _fishData;
    public event System.Action<int, float> OnFishGet; // 도감 연동용 이벤트 (id와 길이) 
    private EnvironmentModel _environment;

    private ArriveSeason _currentSeason;
    private bool _isRain;
    private bool _isCherryblossom;
    private bool _isMaple;
    private bool _isSnow;

    public ArriveSeason CurrentSeason => _currentSeason;

    int _seasonChangeCount;  //실러캔스
    bool _canCoelacanth = false;
    float _pirarukuTimer;
    bool _canCanPiraruque = false;

    private void Awake()
    {
        base.Awake();
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

        // 추가한 부분입니다
        _fishData = new();
        ArriveSeason[] seasons = { ArriveSeason.Spring, ArriveSeason.Summer, ArriveSeason.Autumn, ArriveSeason.Winter };
        FishType[] types = { FishType.Sea, FishType.Lake };

        foreach (ArriveSeason s in seasons)
        {
            foreach (FishType t in types)
            {
                _fishData[(s, t)] = new List<FishDataSO>();
            }
        }

        var db = DataManager.Instance.FishingDatabase.FishData;

        foreach (var fish in _allFish)
        {
            if (fish == null) continue;

            foreach (var s in seasons)
            {
                if ((fish.arriveseasonType & s) != 0)
                {
                    if (_fishData.ContainsKey((s, fish.fishType)))
                    {
                        _fishData[(s, fish.fishType)].Add(fish);
                    }
                }
            }
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
    private void OnDisable()
    {
        if (_environment != null)
        {
            _environment.OnSeasonChanged -= HandleSeasonChanged;
            _environment.OnDailyChanged -= OpenPiraruku;
            _environment.OnWeatherChanged -= HandleWeatherChanged;
        }
    }
    public void SetEnvironment(EnvironmentModel time)
    {
        if (_environment != null)
        {
            _environment.OnSeasonChanged -= HandleSeasonChanged;
            _environment.OnDailyChanged -= OpenPiraruku;
            _environment.OnWeatherChanged -= HandleWeatherChanged;
        }

        _environment = time;

        if (_environment != null)
        {
            _environment.OnSeasonChanged += HandleSeasonChanged;
            _environment.OnDailyChanged += OpenPiraruku;
            _environment.OnWeatherChanged += HandleWeatherChanged;

            _currentSeason = ConvertSeason(_environment.CurrentSeason);
            HandleWeatherChanged(_environment.CurrentSeason, _environment.IsWeatherActive);
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

    private void HandleWeatherChanged(Season season, bool isPlaying)
    {
        _isRain = false;
        _isCherryblossom = false;
        _isMaple = false;
        _isSnow = false;

        if (!isPlaying) return;
        switch (season)
        {
            case Season.Spring:
                _isCherryblossom = isPlaying;
                break;
            case Season.Summer:
                _isRain = isPlaying;
                break;
            case Season.Autumn:
                _isMaple = isPlaying;
                break;
            case Season.Winter:
                _isSnow = isPlaying;
                break;
        }
    }

    //ctx.Season에 현재 계절 넣어줌
    private FishingContext BuildContextFromCurrentState()
    {
        FishingContext ctx = new FishingContext();
        ctx.Season = _currentSeason;
        ctx.IsMorning = _environment.CurrentDay == DayilyCycle.Day;
        ctx.IsAfternoon = _environment.CurrentDay == DayilyCycle.Sunset;
        ctx.IsNight = _environment.CurrentDay == DayilyCycle.Night;

        ctx.IsLake = _aquariumMgr.CurrentType == FishType.Lake;
        ctx.IsSea = _aquariumMgr.CurrentType == FishType.Sea;

        //날씨, 골드도 필요함 일단테스트
        //ctx.GoldAmount = GameManager.Instance.PlayerGold;
        ctx.GoldAmount = 50000;
        ctx.FishPool = _playerController.BaitFishPool;
        ctx.FishLengthBonus = _playerController.BobberLengthBonus;
        ctx.IsCherryblossom = _isCherryblossom;
        ctx.IsRain = _isRain;
        ctx.IsMaple = _isMaple;
        ctx.IsSnow = _isSnow;
        return ctx;
    }

    // 군집 알고리즘의 데이터 메서드입니다
    public List<FishDataSO> GetFishData(FishType type, bool isFlocking)
    {
        if (!_fishData.TryGetValue((_currentSeason, type), out var baseList))
        {
            return new ();
        }

        var result = new List<FishDataSO>();
        var ctx = BuildContextFromCurrentState();

        foreach (var fish in baseList)
        {
            if (fish.CrowdingAlgorithm != isFlocking) continue;
            
            if (!PassSpecialRuleByFishId(fish.ID, ctx)) continue;

            result.Add(fish);
        }

        return result;
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
        GiveFishingReward(pickedDrop.RewardItem, ctx);
    }

    public void CreateInstance(FishDataSO fish, FishingContext ctx)  
    {
        float min = fish.MinLength + ctx.FishLengthBonus;
        float max = fish.MaxLength + ctx.FishLengthBonus;

        float length = UnityEngine.Random.Range(min, max);
        int price = fish.Price;

        Debug.Log($"Name: {fish.FishName_String}, price: {price}, Length: {length}");

        FishToStorage(fish, length, price);
        EmojiController.Instance.ShowFishEmoji(fish);
        OnFishGet?.Invoke(fish.ID, length); // 낚았다 
    }

    public void FishToStorage(FishDataSO randomFish, float length, int price)
    {
        FishInstance fish = new FishInstance
        {
            FishId = randomFish.ID,
            Length = length,
            Price = price
        };

        bool success = FishStorageManager.Instance.TryAddToStorage(fish);
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

            if (ctx.FishPool != 0 && drop.RewardGroup != ctx.FishPool) continue;

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
            return candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }
        float r = UnityEngine.Random.Range(0f, total);
        float acc = 0f;

        for (int i = 0; i < candidates.Count; i++)
        {
            acc += GetWeight(candidates[i], ctx);
            if (r < acc)
                return candidates[i];
        }
        return null;
    }
    public float GetWeight(FishingDropDataSO drop, FishingContext ctx)
    {
        float weight = Mathf.Max(0f, drop.Probability);

        if (drop.RewardItem == (int)SpecialType.Oarfish && ctx.IsCherryblossom)
        {
            weight += drop.UpProbability;
        }
        return weight;
    }
    public void GiveFishingReward(int rewardItemId, FishingContext ctx)
    {
        if (rewardItemId == 201)  //LP보상
        {
            GiveCurrencyReward(rewardItemId);
            return;
        }
        var fishSO = GetFishSO(rewardItemId);  //물고기
        if (fishSO == null) 
        {
            Debug.LogWarning($"[FishManager] FishDataSO 못찾음 rewardItemId={rewardItemId}");
            return;
        }
        QuestManager.Instance.AddSimpleProgress(QuestConditionKey.FishingCount, 1);

        if (rewardItemId == 10001 || rewardItemId == 10005)
        {
            QuestManager.Instance.AddDetailsProgress(QuestConditionKey.FishCatchById,rewardItemId.ToString(),1);
        }

        CreateInstance(fishSO,ctx);
        
        if ((int)fishSO.gradeType == 4)
        {
            DataManager.Instance.Hub.SaveAllData();
        }
    }
    private void GiveCurrencyReward(int rewardItemId)
    {
        switch (rewardItemId)
        {
            case 201:
                DataManager.Instance.RecordDatabase.LpPieceCount++;
                Debug.Log("[FishManager] LP 조각 1개 획득");
                break;

            default:
                Debug.LogWarning($"[FishManager] LP말고 다른 재화 보상나옴 ID={rewardItemId}");
                break;
        }
    }
    public void OpenPiraruku(DayilyCycle dayilyCycle)
    {
        _pirarukuTimer = 0f;
        _canCanPiraruque = true;
    }
}