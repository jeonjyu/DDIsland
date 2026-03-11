using UnityEngine;

/// <summary>
/// 플레이어의 코스튬 변경을 위한 싱글톤 매니저
/// </summary>
public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] GameObject _player;
    [SerializeField] Transform _hatSocket;
    [SerializeField] Transform _tieSocket;

    int _currentHatID;
    public int CurrentHatID {get => _currentHatID; private set { _currentHatID = value; } }
    int _currentTieID;
    public int CurrentTieID {get => _currentTieID; private set { _currentTieID = value; } }

    int _toolID;
    public int ToolID { get => _toolID; private set { _toolID = value; } }

    // 현재 활성화상태인 코스튬 아이템의 아이디를 받아옴
    // todo : 플레이어 정보에 저장된 코스튬 아이디를 받아와 할당
    void Start()
    {
        //_currentHatID = ;
        //_currentTieID = ;
        //_toolID = ;
    }

    // 코스튬 ID 변경
    public void SetCostume(int ID, CostumeType costumeType)
    {
        switch (costumeType)
        {
            case CostumeType.Head:
                _currentHatID = ID;
                break;
            case CostumeType.Body:
                _currentTieID = ID;
                break;
            case CostumeType.Tool:
                _toolID = ID;
                break;
        }
    }

    // 들어온 아이템과 현재 장착중인 아이템 비교
    public bool CompareID(int ID, CostumeType costumeType)
    {
        switch (costumeType)
        {
            case CostumeType.Head:
                return _currentHatID == ID;
            case CostumeType.Body:
                return _currentTieID == ID;
            case CostumeType.Tool:
                return _toolID == ID;
            default: return false;
        }
    }

}
