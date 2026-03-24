using System.Collections.Generic;
using UnityEngine;

public class FixedBuilding : MonoBehaviour
{
   [SerializeField] private FixGroup _locationID;
   
   [SerializeField] private int _currentItemID; 
    
    public int CurrentItemID => _currentItemID;
    public FixGroup LocationID => _locationID;

    // 시작 시 ID가 비어있으면 SO에서 찾아서 자동 할당 (인게임에 배치된 기본템용)
    void Awake()
    {
        if (_currentItemID <= 0 && _locationID != FixGroup.None)
        {
            // 같은 오브젝트의 InteriorDataSO에서 ID 가져오기
            var placeable = GetComponent<Placeable3D>();
            if (placeable != null)
            {
                int id = placeable.GetItemId();
                if (id > 0) _currentItemID = id;
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
