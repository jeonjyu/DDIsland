using UnityEngine;
using System.Collections.Generic;
using System;

// 호수전용데이터 
// [편집 모드] 타일 상태 
public enum LakeTileState
{
    Empty,      // 흰색,   배치 가능한 빈 타일
    Fixed,      // 빨간색, 배치 불가한 고정 오브젝트 
    Occupied,   // 회색,   배치된 오브젝트, 이동/회수 가능
    Preview,    // 초록색, 배치 예상 위치 
    Invalid     // 빨간색, 배치 불가 
}
public enum LakeObjectSize
{
    Small_1x1,  // (1,1)
    Medium_2x1, // (2,1)
    Large_4x2   // (4,2) 
}
// 타일 한칸 데이터 
[Serializable]
public class LakeTileData
{
    public Vector2Int gridPos;    // 0부터 최대 30x2 그리드 (0,0) ~ (29,1)
    public LakeTileState state;   // 현재 타일 상태
    public string placedObjectId; // 배치된 오브젝트 ID
}


// 배치된 오브젝트 데이터 
[Serializable]
public class LakePlacedObjectData 
{
    public string objectId;           // 배치 ID (같은 템 여러 개 구분용)
    public int itemId;                // 아이템 종류 ID (바위, 수초)
    // public InteriorType interiorType; //  LakeFix, LakeFree //  BuildingItem.cs의 enum 사용 // 현재 참조할 수 없음
    public Vector2Int gridPos;        // 배치 좌표 
    public Vector2Int size;           // 타일 크기 (1,1) (2,1) (4,2) 
}

// 하단에 인벤 슬롯 
[Serializable]
public class LakeInvenSlot
{
    public int itemId;
    public int quantity;  // 수량표시(슬롯 좌상단) 
}

// 저장용 
[Serializable]
public class LakeEditSaveData
{
    public List<LakePlacedObjectData> placedObjects;    // 배치된 오브젝트 목록
    public List<LakeInvenSlot> inventory;       // 보관함 상태
    public LakeFloorData floor;      // 현재 바닥재
}
[Serializable]
public class LakeFloorData // 바닥 전체 일괄적용할 바닥재 (자갈, 모래, 얼음 같은) 
{
    public int placedFloorId; // 현재 적용 중인 바닥재
}