using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixBuildingUI : MonoBehaviour
{
    [SerializeField] private BuildingManager _buildingManager;

    [Header("UI 연결 부분")]
    [SerializeField] private GameObject _popUpPanel; 
    [SerializeField] private Transform _contentTransform; 
    [SerializeField] private FixBuildingButton _slotPrefab;

    private List<FixBuildingButton> _createdSlots = new ();

    private FixedBuilding _currentTarget;
   
    private void OnEnable()
    {
        if (PlacementMgr.Instance != null)
        {
            PlacementMgr.Instance.OnFixedBuildingPick += OpenUI;
        }
    }

    private void OnDisable()
    {
        if (PlacementMgr.Instance != null)
        {
            PlacementMgr.Instance.OnFixedBuildingPick -= OpenUI;
        }
    }

    
    public void OpenUI(FixedBuilding targetBuilding)
    {
        _currentTarget = targetBuilding;
        _popUpPanel.SetActive(true); 

        RefreshSlots(); 
    }

    
    private void RefreshSlots()
    {

        foreach (var slot in _createdSlots)
        {
            slot.gameObject.SetActive(false);
        }

        var myInventory = DataManager.Instance.Hub._allUserData.Store._inventory;
        var database = DataManager.Instance.DecorationDatabase;
        FixGroup targetGroup = _currentTarget.LocationID;

        int activeSlotIndex = 0;

        foreach (var slotData in myInventory)
        {
            int myItemId = slotData.itemId;
            var itemData = database.InteriorData[myItemId];

            if (itemData != null &&
                itemData.interior_itemType == Interior_ItemType.Fix &&
                itemData.fixgroupType == targetGroup)
            {
                FixBuildingButton slot;

                if (activeSlotIndex < _createdSlots.Count)
                {
                    slot = _createdSlots[activeSlotIndex];
                }
                else
                {
                    slot = Instantiate(_slotPrefab, _contentTransform);
                    _createdSlots.Add(slot);
                }

                slot.Setup(myItemId, OnClickChangeItem);

                activeSlotIndex++;
            }
        }
    }

    
    public void OnClickChangeItem(int selectedItemId)
    {
        if (_currentTarget == null) return;

        _buildingManager.SwapFixBuilding(_currentTarget, selectedItemId);

        CloseUI();
        PlacementMgr.Instance.CloseEditMenu();
    }

    public void CloseUI()
    {
        _popUpPanel.SetActive(false);
        _currentTarget = null;
    }
}
