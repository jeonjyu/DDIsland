using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유저의 모든 데이터를 관리하는 클래스
/// </summary>
[Serializable]
public class UserAllData
{
    //환경 및 시간
    public EnvironmentData Environment = new();

    //재화 및 능력치
    public CharacterData Character = new();

    //배치 및 꾸미기
    public DecorationData Decoration = new();

    //수집 및 도감
    public CollectionData Collection = new();

    //진행도
    public ProgressData Progress = new();
}

[Serializable]
public class EnvironmentData
{
    public string _lastDay;  // 마지막 저장 시간
    public Season _currentSeason;
    public DayilyCycle _currentCycle;
    public int[] _calculation;
}
[Serializable]
public class CharacterData
{
    public long _gold;           
    public float[] stats;       // 공격력, 속도 등 (인덱스로 관리)
    public int[] upgrades;      // 각 스탯의 강화 단계
}
[Serializable]
public class DecorationData
{
    public List<int> _ownedCostumes = new List<int>(); // 소유한 코스튬 ID
    public int _currentCostumeId;                      // 현재 착용 중인 ID
    public List<PlacedObject> _buildings = new List<PlacedObject>(); // 배치된 건물들
}
[Serializable]
public class PlacedObject
{
    public int _id;
    public float _posX, _posY; // 배치 좌표
}
[Serializable]
public class CollectionData
{
    public List<int> _unlockedAlbumIds = new List<int>(); // 해금된 음반 ID
    public List<int> _unlockedBookIds = new List<int>();  // 해금된 도감 ID
}
[Serializable]
public class ProgressData
{
    public List<int> activeQuestIds = new List<int>();   // 현재 진행 중인 퀘스트
    public List<int> shopPurchaseCounts = new List<int>(); // 상점 품목별 구매 횟수
}
