using System.Collections.Generic;
using UnityEngine;

public class FixedBuilding : MonoBehaviour
{
   [SerializeField] private FixGroup _locationID;
   
   [SerializeField] private int _currentItemID; 
    
    public int CurrentItemID => _currentItemID;
    public FixGroup LocationID => _locationID;

    // 시작 시 ID가 비어있으면 DB에서 찾아서 자동 할당 (인게임에 배치된 기본템용)
    void Start()
    {
        if (_currentItemID <= 0 && _locationID != FixGroup.None)
        {
            var database = DataManager.Instance.DecorationDatabase.InteriorData;
            foreach (var data in database.datas)
            {
                if (data.fixgroupType == _locationID && data.IsDefault)
                {
                    _currentItemID = data.InteriorID;
                    break;
                }
            }
        }
    }
    // 초기화용 메서드
    public void Setup(FixGroup locationId, int itemId)
    {
        _locationID = locationId;
        _currentItemID = itemId;
    }
}
