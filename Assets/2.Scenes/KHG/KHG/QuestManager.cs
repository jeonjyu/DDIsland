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

    //Fishing
    FishingCount,

    //Store
    BuyCostumeCount,

    //Guide
    FishGuideRegisteredCount,
    CostumeGuideRegisteredCount,

    //Growth
    StaminaLevel,
    HungerLevel,
}
public struct QuestRewardData  //실제 보상 
{
    public QuestRewardType RewardType;
    public int Count;
}
public class QuestManager : Singleton<QuestManager>
{
    private List<QuestDataSO> _quests;
    private Dictionary<int, QuestDataSO> _questById;
    private Dictionary<QuestConditionKey, int> _simpleProgress;
    private Dictionary<string, int> _detailsProgress;
    private HashSet<int> _completedQuests;

    private void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        _quests = new List<QuestDataSO>(DataManager.Instance.QuestDatabase.QuestInfoData.datas);
        _questById = new Dictionary<int, QuestDataSO>();
        _simpleProgress = new Dictionary<QuestConditionKey, int>();
        _detailsProgress = new Dictionary<string, int>();
        _completedQuests = new HashSet<int>();
        SetQuestData(_quests);
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
    public QuestDataSO GetByQuestId(int id)
    {
        if (_questById.TryGetValue(id, out QuestDataSO data))
        {
            return data;
        }
        return null;
    }
    public List<QuestDataSO> GetByQuesCategory(QuestType category)
    {
        List<QuestDataSO> result = new List<QuestDataSO>();
        for (int i = 0; i < _quests.Count; i++)
        {
            if (_quests[i].questType == category)
            {
                result.Add(_quests[i]);
            }
        }
        return result;
    }

    private QuestConditionKey GetConditionKey(QuestDataSO quest)  //퀘스트의 타입에 따른 퀘스트 진행도? 맞추기
    {
        switch (quest.questType)
        {
            case QuestType.Fishing:
                return QuestConditionKey.FishingCount;

            case QuestType.Store:
                if (quest.storeType == StoreType.CostumeStore)
                    return QuestConditionKey.BuyCostumeCount;
                break;

            case QuestType.Guide:
                return QuestConditionKey.FishGuideRegisteredCount;

            case QuestType.Growth:
                if (quest.statType == StatType.BaseStamina)
                    return QuestConditionKey.StaminaLevel;

                if (quest.statType == StatType.BaseHunger)
                    return QuestConditionKey.HungerLevel;
                break;
        }

        return QuestConditionKey.None;
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

        if (quest.Prerequisite != 0)
        {
            if (!_completedQuests.Contains(quest.Prerequisite)) return QuestStateType.Locked;
        }

        if (_completedQuests.Contains(questId)) return QuestStateType.Completed;

        QuestConditionKey key = GetConditionKey(quest);
        if (key == QuestConditionKey.None) return QuestStateType.Locked;

        int currentProgress = GetSimpleProgress(key);
        return currentProgress >= quest.Requirement ? QuestStateType.Claimable : QuestStateType.InProgress;
    }
    //실제 데이터 수치 Add는 누석하는방식   Set은 현재값으로 덮어쓰는방식
    public void AddSimpleProgress(QuestConditionKey key, int amount)
    {
        if (!_simpleProgress.ContainsKey(key)) _simpleProgress[key] = 0;

        _simpleProgress[key] += amount;
    }
    public void AddDetailsProgress(QuestConditionKey key, string param, int amount)
    {

    }
    public void SetSimpleProgress(QuestConditionKey key, int value)
    {
        if (!_simpleProgress.ContainsKey(key)) _simpleProgress[key] = 0;
        _simpleProgress[key] = value;
    }
    public void SetDetailsProgress(QuestConditionKey key, string param, int value)
    {

    }

    public int GetSimpleProgress(QuestConditionKey key)   //현재값 확인
    {
        if (_simpleProgress.TryGetValue(key, out int value)) return value;
        else return 0;
    }
    public int GetDetailsProgress(QuestConditionKey key, string param)
    {
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

        }
        else if (rewardItemId == 2) // 예시: LP조각 ID
        {

        }
        else
        {
            // 일반 아이템 지급
        }
    }
}
