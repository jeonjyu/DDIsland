using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 코스튬 변경을 위한 싱글톤 매니저
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
#region field
    [Header("씬에 배치된 플레이어 오브젝트")]
    [SerializeField] GameObject _player;

    [Header("플레이어의 코스튬 장착 소켓")]
    [SerializeField] Transform _SocketCostumeHat;
    [SerializeField] Transform _SocketCostumeTie;
    

    int _currentHatID = 0;
    int _currentTieID = 0;
    int _currentPoleID = 0;
    int _currentBaitID = 0;
    int _currentBobberID = 0;

    // 플레이어가 장비중인 아이템 딕셔너리
    // 코스튬(모자, 한벌옷), 낚시 아이템 (미끼, 낚시대, 낚시찌)
    Dictionary<string, int> playerEquip = new Dictionary<string, int>();

#endregion

#region property
    public int CurrentHatID {get => _currentHatID; private set { _currentHatID = value; } }
    public int CurrentTieID {get => _currentTieID; private set { _currentTieID = value; } }
    public int CurrentPoleID { get => _currentPoleID; private set { _currentPoleID = value; } }
    public int CurrentBaitID { get => _currentBaitID; set => _currentBaitID = value; }
    public int CurrentBobberID { get => _currentBobberID; set => _currentBobberID = value; }

    public Dictionary<string, int> PlayerEquip = new Dictionary<string, int>();

    public Action<Enum, int> OnEquipChanged; // 장착중인 아이템 변경시 알림 
#endregion 

    protected override void Awake()
    {
        base.Awake();
        if(_SocketCostumeHat == null) _SocketCostumeHat = _player.transform.Find("Socket_CoustumeHat");
        if(_SocketCostumeTie == null) _SocketCostumeTie = _player.transform.Find("Socket_CoustumeTie");

        playerEquip.Clear();
        playerEquip.Add(CostumeType.Head.ToString(), _currentHatID);
        playerEquip.Add(CostumeType.Body.ToString(), _currentTieID);
        playerEquip.Add(FishingItemType.Pole.ToString(), _currentPoleID);
        playerEquip.Add(FishingItemType.Bait.ToString(), _currentBaitID);
        playerEquip.Add(FishingItemType.Bobber.ToString(), _currentBobberID);
    }

    // 현재 활성화상태인 코스튬 아이템의 아이디를 받아옴
    // todo : 플레이어 정보에 저장된 코스튬 아이디를 받아와 저장하도록 수정
    void Start()
    {
        playerEquip[CostumeType.Head.ToString()] = 0;
        playerEquip[CostumeType.Body.ToString()] = 0;
        playerEquip[FishingItemType.Pole.ToString()] = 0;
        playerEquip[FishingItemType.Bait.ToString()] = 0;
        playerEquip[FishingItemType.Bobber.ToString()] = 0;
    }

    // 코스튬 ID 변경
    public void SetEquip(IStoreItem item)
    {
        if(CompareID(item))
        {
            Debug.Log("장착 아이템 " + playerEquip[item.Filter.ToString()] + "에서 0으로 변경");
            playerEquip[item.Filter.ToString()] = 0;
        }
        else
        {
            string logTxt = "장착 아이템 " + playerEquip[item.Filter.ToString()];
        
            playerEquip[item.Filter.ToString()] = item.ObjectId;

            logTxt += "에서 " + item.ObjectId + "로 변경";
            Debug.Log(logTxt);
        }

        OnEquipChanged?.Invoke(item.Filter, playerEquip[item.Filter.ToString()]);
    }

    // 들어온 아이템과 현재 장착중인 아이템 비교
    public bool CompareID(IStoreItem item)
    {
        return playerEquip[item.Filter.ToString()] == item.ObjectId;
    }
}
