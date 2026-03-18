using System;
using System.Collections.Generic;
using UnityEngine;
public enum QuestRewardType
{
    None = 0,
    Gold = 1,
    LPPiece = 2
}
public enum QuestStateType
{
    Locked,          // 이전 단계 미완료로 아직 안 보임
    InProgress,      // 진행 중
    Claimable,       // 보상 수령 가능
    Completed,       // 보상 수령 완료
    Expired          // 기간 종료
}
public enum QuestConditionKey  //퀘스트 진행도? 필요할떄마다 추가하면될듯
{
    None = 0,

    // Fishing
    FishingCount,
    FishCatchById,              // 특정 어종 잡기

    // Store
    BuyCostumeCount,
    BuylandStoreCount,
    BuyLakeStoreCount,
    BuyFishingItemCount,
    BuyFoodCount,

    // Guide
    FishGuideRegisteredCount,
    CostumeGuideRegisteredCount,
    FoodGuideRegisteredCount,
    InteriorGuideRegisteredCount,

    // Growth
    StaminaLevel,
    HungerLevel,
    MoveSpeedLevel,
    FishingSpeedLevel,
    StaminaHealLevel,
}
public struct QuestRewardData  //실제 보상 
{
    public QuestRewardType RewardType;
    public int Count;
}
public class QuestManager : Singleton<QuestManager>
{
    private List<QuestDataSO> _quests;
    private List<CurrencyDataSO> _reaward;
    private Dictionary<int, QuestDataSO> _questById;
    private Dictionary<int, CurrencyDataSO> _reawardById;
    private Dictionary<QuestConditionKey, int> _simpleProgress;
    private Dictionary<string, int> _detailsProgress;
    private HashSet<int> _completedQuests;

    public event Action OnQuestAddValue;

    public HashSet<int> CompletedQuests => _completedQuests;
    public List<CurrencyDataSO> Reaward => _reaward;
    private void Awake()
    {
        base.Awake();
        _quests = new List<QuestDataSO>(DataManager.Instance.QuestDatabase.QuestInfoData.datas);
        _reaward = new List<CurrencyDataSO>(DataManager.Instance.CurrencyDatabase.CurrencyInfoData.datas);
        _questById = new Dictionary<int, QuestDataSO>();
        _simpleProgress = new Dictionary<QuestConditionKey, int>();
        _detailsProgress = new Dictionary<string, int>();
        _completedQuests = new HashSet<int>();
        _reawardById = new Dictionary<int, CurrencyDataSO>();
        SetQuestData(_quests);
        SetRewardData(_reaward);
    }

    private void Start()
    {
        QuestManager.Instance.AddDetailsProgress(QuestConditionKey.FishCatchById, "10001", 100);  //테스트
        QuestManager.Instance.SetSimpleProgress(QuestConditionKey.FishGuideRegisteredCount, 10);
        QuestManager.Instance.SetSimpleProgress(QuestConditionKey.CostumeGuideRegisteredCount, 10);
        QuestManager.Instance.SetSimpleProgress(QuestConditionKey.FoodGuideRegisteredCount, 10);
        QuestManager.Instance.SetSimpleProgress(QuestConditionKey.InteriorGuideRegisteredCount, 10);
    }

    //데이터 보관용
    public void SetQuestData(List<QuestDataSO> data)
    {
        _quests = data;

        foreach (var quest in data)
        {
            _questById[quest.ID] = quest;

        }
    }
    public void SetRewardData(List<CurrencyDataSO> data)
    {
        _reaward = data;
        _reawardById.Clear();

        foreach (var reward in data)
        {
            if (reward == null) continue;
            _reawardById[reward.ID] = reward;
        }
    }
    public QuestDataSO GetByQuestId(int id)
    {
        if (_questById.TryGetValue(id, out QuestDataSO data))
        {
            return data;
        }
        return null;
    }
    public CurrencyDataSO GetewardId(int id)
    {
        if (_reawardById.TryGetValue(id, out CurrencyDataSO data))
        {
            return data;
        }
        return null;
    }

    public List<QuestDataSO> GetByQuesCategory(QuestType category)
    {
        List<QuestDataSO> result = new List<QuestDataSO>();

        if (_quests == null) return result;

        for (int i = 0; i < _quests.Count; i++)
        {
            QuestDataSO quest = _quests[i];

            if (quest == null) continue;

            var slotType = GetQuestState(_quests[i].ID);

            if (slotType == QuestStateType.Completed) continue;
            if (slotType == QuestStateType.Locked) continue;  //일단은
            if (slotType == QuestStateType.Expired) continue;

            if (_quests[i].questType == category)
            {
                result.Add(_quests[i]);
            }
        }
        return result;
    }

    public QuestConditionKey GetConditionKey(QuestDataSO quest)  //퀘스트의 타입에 따른 퀘스트 진행도? 맞추기
    {
        switch (quest.questType)
        {
            case QuestType.Fishing:
                // 특정 물고기 퀘스트면 details 사용
                if (quest.RequireItem > 0)
                    return QuestConditionKey.FishCatchById;

                return QuestConditionKey.FishingCount;

            case QuestType.Store:
                if (quest.storeType == StoreType.CostumeStore) return QuestConditionKey.BuyCostumeCount;
                if (quest.storeType == StoreType.FishingItemStore) return QuestConditionKey.BuyFishingItemCount;
                if (quest.storeType == StoreType.IslandStore) return QuestConditionKey.BuylandStoreCount;
                if (quest.storeType == StoreType.LakeStore) return QuestConditionKey.BuyLakeStoreCount;
                if (quest.storeType == StoreType.Food) return QuestConditionKey.BuyFoodCount;
                break;

            case QuestType.Guide:
                // QuestDataSO enum은 안 건드리니까 int로 비교
                if ((int)quest.journalType == 1) return QuestConditionKey.FishGuideRegisteredCount;
                if ((int)quest.journalType == 2) return QuestConditionKey.CostumeGuideRegisteredCount;
                if ((int)quest.journalType == 3) return QuestConditionKey.FoodGuideRegisteredCount;
                if ((int)quest.journalType == 4) return QuestConditionKey.InteriorGuideRegisteredCount;
                break;

            case QuestType.Growth:
                if (quest.statType == StatType.BaseHunger) return QuestConditionKey.HungerLevel;
                if (quest.statType == StatType.BaseStamina) return QuestConditionKey.StaminaLevel;
                if (quest.statType == StatType.BaseMoveSpeed) return QuestConditionKey.MoveSpeedLevel;
                if (quest.statType == StatType.BaseFishingSpeed) return QuestConditionKey.FishingSpeedLevel;
                if (quest.statType == StatType.StaminaHeal) return QuestConditionKey.StaminaHealLevel;
                break;
        }

        return QuestConditionKey.None;
    }
    private bool IsDetailsQuest(QuestDataSO quest)
    {
        QuestConditionKey key = GetConditionKey(quest);
        return key == QuestConditionKey.FishCatchById;
    }

    private string GetConditionParam(QuestDataSO quest)
    {
        // 특정 물고기 퀘스트는 RequireItem = 물고기 ID
        if (GetConditionKey(quest) == QuestConditionKey.FishCatchById)
            return quest.RequireItem.ToString();

        return string.Empty;
    }

    private int GetCurrentProgress(QuestDataSO quest)
    {
        QuestConditionKey key = GetConditionKey(quest);
        if (key == QuestConditionKey.None) return 0;

        if (IsDetailsQuest(quest))
            return GetDetailsProgress(key, GetConditionParam(quest));

        return GetSimpleProgress(key);
    }
    public int GetQuestCurrentProgress(int questId)
    {
        QuestDataSO quest = GetByQuestId(questId);
        if (quest == null) return 0;

        return GetCurrentProgress(quest);
    }
    public List<QuestDataSO> GetByQuesGroup(int groupId)
    {
        List<QuestDataSO> result = new List<QuestDataSO>();
        for (int i = 0; i < _quests.Count; i++)
        {
            if (_quests[i].QuestGroup == groupId)
            {
                result.Add(_quests[i]);
            }
        }
        return result;
    }

    public QuestStateType GetQuestState(int questId)  //퀘스트의 상태 확인
    {
        QuestDataSO quest = GetByQuestId(questId);
        if (quest == null) return QuestStateType.Locked;

        if (!string.IsNullOrWhiteSpace(quest.StartTime))
        {
            if (DateTime.TryParse(quest.StartTime, out DateTime start))
            {
                if (DateTime.Now < start)
                    return QuestStateType.Locked;
            }
        }

        if (!string.IsNullOrWhiteSpace(quest.FinishTime))
        {
            if (DateTime.TryParse(quest.FinishTime, out DateTime end))
            {
                if (DateTime.Now > end)
                    return QuestStateType.Expired;
            }
        }

        if (quest.Prerequisite != 0)
        {
            if (!_completedQuests.Contains(quest.Prerequisite)) return QuestStateType.Locked;
        }

        if (_completedQuests.Contains(questId)) return QuestStateType.Completed;

        QuestConditionKey key = GetConditionKey(quest);
        if (key == QuestConditionKey.None) return QuestStateType.Locked;

        int currentProgress = GetCurrentProgress(quest);
        return currentProgress >= quest.Requirement ? QuestStateType.Claimable : QuestStateType.InProgress;
    }
    //실제 데이터 수치 Add는 누석하는방식   Set은 현재값으로 덮어쓰는방식  
    //판매,하면 진행도 감소 미구현
    public void AddSimpleProgress(QuestConditionKey key, int amount)
    {
        if (!_simpleProgress.ContainsKey(key)) _simpleProgress[key] = 0;

        _simpleProgress[key] += amount;
        OnQuestAddValue?.Invoke();
    }
    public void AddDetailsProgress(QuestConditionKey key, string param, int amount)
    {
        string id = $"{key}_{param}";

        if (!_detailsProgress.ContainsKey(id))
            _detailsProgress[id] = 0;

        _detailsProgress[id] += amount;
        OnQuestAddValue?.Invoke();
    }
    public void SetSimpleProgress(QuestConditionKey key, int value)  
    {
        if (!_simpleProgress.ContainsKey(key)) _simpleProgress[key] = 0;
        _simpleProgress[key] = value;
        OnQuestAddValue?.Invoke();
    }
    public void SetDetailsProgress(QuestConditionKey key, string param, int value)
    {
        string id = $"{key}_{param}";

        if (!_detailsProgress.ContainsKey(id))
            _detailsProgress[id] = 0;

        _detailsProgress[id] = value;
        OnQuestAddValue?.Invoke();
    }

    public int GetSimpleProgress(QuestConditionKey key)   //현재값 확인
    {
        if (_simpleProgress.TryGetValue(key, out int value)) return value;
        else return 0;
    }
    public int GetDetailsProgress(QuestConditionKey key, string param)
    {
        string id = $"{key}_{param}";

        if (_detailsProgress.TryGetValue(id, out int value))
            return value;

        return 0;
    }

    public bool GiveQuestReward(int questId)  //보상주기
    {
        QuestDataSO quest = GetByQuestId(questId);
        if (quest == null) return false;

        QuestStateType questState = GetQuestState(questId);
        if (questState != QuestStateType.Claimable) return false;


        // 보상 1 지급
        GiveSingleReward(quest.RewardItemId1, quest.RewardCount1);

        // 보상 2 지급
        GiveSingleReward(quest.RewardItemId2, quest.RewardCount2);

        // 완료 처리
        _completedQuests.Add(questId);

        return true;
    }

    private void GiveSingleReward(int rewardItemId, int rewardCount)  //보상의 타입에따라
    {
        if (rewardItemId <= 0 || rewardCount <= 0) return;

        if (rewardItemId == 1) // 예시: 골드 ID
        {
            GameManager.Instance.SetGold(rewardCount);
        }
        else if (rewardItemId == 2) // 예시: LP조각 ID
        {
            DataManager.Instance.RecordDatabase.LpPieceCount += rewardCount;
        }
        else
        {
            // 일반 아이템 지급
        }
    }
}
