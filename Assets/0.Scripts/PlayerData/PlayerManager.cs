using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 코스튬 변경을 위한 싱글톤 매니저
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    [Header("씬에 배치된 플레이어 오브젝트")]
    [SerializeField] GameObject _player;

    [Header("플레이어의 코스튬 장착 소켓")]
    [SerializeField] Transform _SocketCostumeHat;
    [SerializeField] Transform _SocketCostumeTie;

    int _currentHatID = 0;
    public int CurrentHatID {get => _currentHatID; private set { _currentHatID = value; } }
    int _currentTieID = 0;
    public int CurrentTieID {get => _currentTieID; private set { _currentTieID = value; } }

    int _toolID = 0;
    public int ToolID { get => _toolID; private set { _toolID = value; } }

    Dictionary<CostumeType, int> playerEquip = new Dictionary<CostumeType, int>();

    public Dictionary<CostumeType, int> PlayerEquip = new Dictionary<CostumeType, int>();

    protected override void Awake()
    {
        base.Awake();
        if(_SocketCostumeHat == null) _SocketCostumeHat = _player.transform.Find("Socket_CoustumeHat");
        if(_SocketCostumeTie == null) _SocketCostumeTie = _player.transform.Find("Socket_CoustumeTie");

        playerEquip.Clear();
        playerEquip.Add(CostumeType.Head, _currentHatID);
        playerEquip.Add(CostumeType.Body, _currentTieID);
        playerEquip.Add(CostumeType.Tool, _toolID);
    }

    // 현재 활성화상태인 코스튬 아이템의 아이디를 받아옴
    // todo : 플레이어 정보에 저장된 코스튬 아이디를 받아와 저장하도록 수정
    void Start()
    {
        playerEquip[CostumeType.Head] = 0;
        playerEquip[CostumeType.Body] = 0;
        //playerEquip[CostumeType.Tool] = 0;
    }

    // 코스튬 ID 변경
    public void SetCostume(int ID, CostumeType costumeType)
    {
        string logTxt = "현재 : " + playerEquip[costumeType];
        
        playerEquip[costumeType] = ID;

        logTxt += "에서 " + ID + "로 변경";
        Debug.Log(logTxt);
    }

    // 들어온 아이템과 현재 장착중인 아이템 비교
    public bool CompareID(int ID, CostumeType costumeType)
    {
        return playerEquip[costumeType] == ID;
    }


}
