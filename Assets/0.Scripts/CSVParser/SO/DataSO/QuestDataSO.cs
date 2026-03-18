using System;
using UnityEngine;

// 퀘스트 종류
[Serializable]
public enum QuestType
{
    None = 0,
     Store = 1,       //상점
     Fishing = 2,     //낚시
     Guide = 3,       //도감
     Growth = 4,      //성장
}

// 상점 종류
[Serializable]
public enum StoreType
{
    None = 0,
     IslandStore = 1,             //섬 인테리어
     LakeStore = 2,               //호수 인테리어
     CostumeStore = 3,            //코스튬
     FishingItemStore = 4,        //낚시아이템
     Food = 5,                    //레시피
}

// 도감 종류
[Serializable]
public enum JournalType
{
    None = 0,
     JournalFish = 1,              //물고기 도감
     JournalCostume = 2,           //코스튬 도감
     JournalFood = 3,              //음식 도감
     JounalInterior = 4,           //인테리어 도감
}


[CreateAssetMenu(fileName = "QuestDataSO", menuName = "Scriptable Objects/Data/QuestDataSO")]
public class QuestDataSO : TableBase<int>
{
    // id
    [field: SerializeField] public int ID { get; private set; }

    // 퀘스트 이름
    [SerializeField] private string questName;
    public string QuestName_String => LocalizationManager.Instance.GetString(questName);

    // 퀘스트 그룹
    [field: SerializeField] public int QuestGroup { get; private set; }

    // 선행 조건
    [field: SerializeField] public int Prerequisite { get; private set; }

    // 퀘스트 종류
    [field: SerializeField] public QuestType questType { get; private set; }

    // 상점 종류
    [field: SerializeField] public StoreType storeType { get; private set; }

    // 도감 종류
    [field: SerializeField] public JournalType journalType { get; private set; }

    // 스탯 타입
    [field: SerializeField] public StatType statType { get; private set; }

    // 요구 아이템
    [field: SerializeField] public int RequireItem { get; private set; }

    // 요구 수치
    [field: SerializeField] public int Requirement { get; private set; }

    // 시작 시간
    [field: SerializeField] public string StartTime { get; private set; }

    // 종료 시간
    [field: SerializeField] public string FinishTime { get; private set; }

    // 보상 아이템ID1
    [field: SerializeField] public int RewardItemId1 { get; private set; }

    // 보상 수량1
    [field: SerializeField] public int RewardCount1 { get; private set; }

    // 보상 아이템ID2
    [field: SerializeField] public int RewardItemId2 { get; private set; }

    // 보상 수량2
    [field: SerializeField] public int RewardCount2 { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => ID;
}
