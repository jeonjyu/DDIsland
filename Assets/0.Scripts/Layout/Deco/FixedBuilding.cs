using System.Collections.Generic;
using UnityEngine;

public class FixedBuilding : MonoBehaviour
{
   [SerializeField] private FixGroup _locationID;

    private int _currentItemID;

    public int CurrentItemID => _currentItemID;
    public FixGroup LocationID => _locationID;    

    // 초기화용 메서드
    public void Setup(FixGroup locationId, int itemId)
    {
        _locationID = locationId;
        _currentItemID = itemId;
    }
}
