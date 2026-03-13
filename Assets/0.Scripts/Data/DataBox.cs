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
    public Environment_Data Environment = new ();

    //재화 및 능력치
    public Charcter_Data Character = new ();

    // 상점 관련
    public Store_Data Store = new();

    //배치 및 꾸미기
    public Decoration_Data Decoration = new ();

    //수집 및 도감
    public Collection_Data Collection = new ();

    //진행도
    public Progress_Data Progress = new ();
}

[Serializable]
public class Environment_Data
{
    public int[] _calculation;
}
[Serializable]
public class Charcter_Data
{
    public long _gold;           
    public float[] stats;       // 공격력, 속도 등 (인덱스로 관리)
    public int[] upgrades;      // 각 스탯의 강화 단계
}
[Serializable]
public class Decoration_Data
{
    public List<int> _ownedCostumes = new (); // 소유한 코스튬 ID
    public int _currentCostumeId;  // 현재 착용 중인 ID
    public List<PlacedObject> _buildings = new (); // 배치된 건물들
}
[Serializable]
public class PlacedObject
{
    public int _id;
    public int _posX, _posY; // 배치 좌표
    public int _rotation;
}
[Serializable]
public class Collection_Data
{
    public List<int> _unlockedAlbumIds = new (); // 해금된 음반 ID
    public List<int> _unlockedBookIds = new ();  // 해금된 도감 ID
}
[Serializable]
public class Progress_Data
{
    public List<int> activeQuestIds = new ();   // 현재 진행 중인 퀘스트
    public List<int> shopPurchaseCounts = new (); // 상점 품목별 구매 횟수
}

public class Store_Data
{
    public int gold = new();
}
