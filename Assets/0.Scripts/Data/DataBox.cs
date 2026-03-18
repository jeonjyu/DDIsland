using System;
using System.Collections.Generic;

/// <summary>
/// 유저의 모든 데이터를 관리하는 클래스
/// </summary>
[Serializable]
public class UserAllData
{
    //환경 및 시간
    public Environment_Data Environment = new ();

    //재화
    public Character_Data Character = new ();

    // 상점 관련
    public Store_Data Store = new();

    //배치 상태
    public Decoration_Data Decoration = new ();

    //수집 및 도감
    public Collection_Data Collection = new ();

    //진행도
    public Progress_Data Progress = new ();

    //물고기 보관함
    public Fishstoage_Data fishstoage = new();

    //음식 보관함
    public Foodstorage_Data Foodstorage = new();

    //재화
    public Currency_Data Currency = new();
}

[Serializable]
public class Environment_Data
{
    public int[] _calculation;
}
[Serializable]
public class Character_Data
{
    public StatData _hunger = new();
    public StatData _stamina = new();
    public StatData _moveSpeed = new();
    public StatData _fishingSpeed = new();
    public StatData _restSpeed = new();

    public int _doongdoongStat;
}
[Serializable]
public class Decoration_Data
{
    public List<PlacedObjectData> _buildings = new (); // 섬에 배치된 건물들

    public List<FixedObjectData> _fixedBuildings = new();
}

[Serializable]
public class Collection_Data
{
    public List<int> _unlockedFishIds = new();    // 해금된 어종 ID
    public List<int> _unlockedCostumeIds = new(); // 해금된 코스튬 ID
    public List<int> _unlockedInteriorIds = new();// 해금된 인테리어 ID
    public List<int> _unlockedFoodIds = new();    // 해금된 음식 ID
    public List<int> _unlockedAlbumIds = new ();  // 해금된 음반 ID 
    public List<FishRecordData> _fishRecords = new(); // 어종 최고 기록
}

// 얘는 아직 어떻게 활용해야할지 모르겠음. 필요 없으면 그냥 갖다 버릴 것
[Serializable]
public class Progress_Data
{
    public List<int> activeQuestIds = new ();   // 현재 진행 중인 퀘스트
    public List<int> shopPurchaseCounts = new (); // 상점 품목별 구매 횟수
}
[Serializable]
public class Currency_Data
{
    public int _gold;
    
}
[Serializable]
public class Store_Data
{
    public List<int> _ownedCostumes = new(); // 소유한 코스튬 ID
    public List<int> _ownedFishings = new(); // 소유한 낚시 장비 ID
    public List<LakeInvenSlot> _inventory = new(); // 구매해서 현재 보유한 건물들
    public int _currentCostumeId;  // 현재 착용 중인 ID
}
[Serializable]
public class Fishstoage_Data
{
    public int StorageLevel = 1;      
    public long AcquireCounter = 0;   // 획득 순서 카운터

    public List<SavedFishSlotData> SlotList = new();
}
[Serializable]
public class Foodstorage_Data
{
    public int StorageLevel;       
    public long AcquireCounter;  
    
    public List<SavedFoodSlotData> SavedSlots = new();
}
[Serializable]
public class Record_Data
{

}

#region 변수 저장 클래스
[Serializable]
public class FixedObjectData
{
    public FixGroup _locationID; // 자리값
    public int _id;         // 프리팹 ID
}
[Serializable]
public class PlacedObjectData
{
    public int _id;
    public int _posX, _posY; // 배치 좌표
    public int _rotation;
}
[Serializable]
public class SavedFishSlotData
{
    public int Index; // 보관함 위치 인덱스
    public int FishId;
    public int Count;
    public int MaxPrice;
    public int TotalPrice;
    public long LastAcquiredOrder;
}
[Serializable]
public class SavedFoodSlotData
{
    public int Index; // 보관함 위치 인덱스
    public int FoodId;
    public int Count;  
    public long LastAcquiredOrder;
}
[Serializable]
public class StatData
{
    public float Value; // 현재 수치
    public int Level;   // 강화 단계
}
[Serializable]
public class FishRecordData
{
    public int FishId;
    public float MaxLength; // 최고 기록 (cm)
}
#endregion