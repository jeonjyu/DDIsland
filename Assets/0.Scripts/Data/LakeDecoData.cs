using UnityEngine;
using System.Collections.Generic;
using System;

#region 데이터테이블 BuildingItem.cs 상점에서 
// 1=MainHouse, 2=Floor, 3=Fix, 4=Free, 5=LakeFloor, 6=LakeFix, 7=LakeFree
//public enum InteriorType // BuildingItem.cs에 이미 있음 
//{
//    None = 0,
//    MainHouse = 1,
//    Floor = 2,
//    Fix = 3,
//    Free = 4,
//    LakeFloor = 5,  // 바닥재 (모래, 자갈, 얼음 등 등)
//    LakeFix = 6,   /// 고정 장식물 (유저가 수정 불가)
//    LakeFree = 7    // 자유 배치물 (유저가 배치/이동/회수)
//}

//// Interior 데이터테이블 // 상점에서 하는 거 같음
//[Serializable]
//public class InteriorData
//{
//    public int ID;   // 인테리어 id  
//    public InteriorType InteriorType;
//    public int NumPurchase;    // 구매가능횟수          
//    public bool IsSaleable; // 판매가능여부              
//    public int PurchasePrice;  // 구매가격        
//    public int SellPrice;     // 판매 가격     
//    public string InteriorImg;   //이미지 리소스    
//    public string InteriorName; // 이름
//    public string InteriorDesc; // 설명 
//}
#endregion

///  호수 전용 데이터 
//public enum LakeObjectType
//{
//    none = 0,
//    LakeFloor = 5, // 바닥재 (모래, 자갈, 얼음 등)
//    LakeFix = 6,   // 고정 장식물 (유저가 수정 불가)
//    LakeFree = 7   // 자유 배치물 (유저가 배치/이동/회수)
//}

// 호수전용데이터 
// [편집 모드] 타일 상태 
public enum LakeTileState
{
    Empty,      // 흰색,   배치 가능한 빈 타일
    Fixed,      // 빨간색, 수정 불가한 고정 오브젝트 
    Occupied,   // 회색,   유저가 배치한 옵젝 있음, 이동/회수 가능
    Preview,    // 초록색, 드래그 예상 위치 
    Invalid     // 빨간색, 배치 불가 
}

// 타일 한칸 데이터 
[Serializable]
public class LakeTileData
{
    public Vector2Int gridPos;    // 0부터 최대 30x2 그리드 (0,0) ~ (29,1)
    public LakeTileState state;   // 현재 타일 상태
    public string placedObjectId; // 배치된 오브젝트 ID
}
public enum LakeObjectSize
{
    Small_1x1,  // (1,1)
    Medium_2x1, // (2,1)
    Large_4x2   // (4,2) 
}

// 배치된 오브젝트 데이터 
[Serializable]
internal class LakePlacedObjectData 
{
    public string objectId;           // 배치 ID (같은 템 여러 개 구분용)
    public int itemId;                // 아이템 종류 ID (바위, 수초)
    public InteriorType interiorType; //  LakeFix, LakeFree //  BuildingItem.cs의 enum 사용 
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
class LakeEditSaveData
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