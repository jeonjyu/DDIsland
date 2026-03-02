using System;
using UnityEngine;

// 상호작용 종류
[Serializable]
public enum InteractionType
{
    None = 0,
    Bed = 1,                   //침대
    Campfire = 2,              //모닥불
    LpPlayer = 3,              //LP플레이어
    Storage = 4,               //물고기 및 요리 보관함
    PostBox = 5,               //우체통
}

// 배치 가능 공간
[Serializable]
public enum LocationType
{
    None = 0,
    Island = 1,    //섬
    Lake = 2,      //호수
}

// 배치 방식
[Serializable]
public enum Interior_ItemType
{
    None = 0,
    Floor = 1,        //바닥
    Fix = 2,          //고정장식물
    Free = 3,         //자유장식물
}


[CreateAssetMenu(fileName = "InteriorDataSO", menuName = "Scriptable Objects/Data/InteriorDataSO")]
public class InteriorDataSO : TableBase<int>
{
    // 인테리어 ID
    [field: SerializeField] public int InteriorID { get; private set; }

    // 인테리어명
    [SerializeField] private string interiorName;
    public string InteriorName_String => LocalizationManager.Instance.GetString(interiorName);

    // 설명 텍스트
    [SerializeField] private string interiorDesc;
    public string InteriorDesc_String => LocalizationManager.Instance.GetString(interiorDesc);

    // 가로 점유 칸 수
    [field: SerializeField] public int GridSizeX { get; private set; }

    // 세로 점유 칸 수 
    [field: SerializeField] public int GridSizeY { get; private set; }

    // 기본 제공 여부
    [field: SerializeField] public bool IsDefault { get; private set; }

    // 상호작용 종류
    [field: SerializeField] public InteractionType interactionType { get; private set; }

    // 배치 가능 공간
    [field: SerializeField] public LocationType locationType { get; private set; }

    // 배치 방식
    [field: SerializeField] public Interior_ItemType interior_itemType { get; private set; }

    // 리소스 경로
    [field: SerializeField] public GameObject InteriorPath_GameObject { get; private set; }

    // 부모 클래스의 ID 반환 추상 메서드
    public override int GetID() => InteriorID;
}
