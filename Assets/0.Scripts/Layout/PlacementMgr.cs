using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum PlacementState
{
    View,
    Edit
}

public class PlacementMgr : MonoBehaviour
{
    static private PlacementMgr _instance;
    private Placeable3D _selectedTarget; // 현재 선택된 건물 
    private InputAction _clickAction; // 클릭 입력 액션
    private InputAction _rotation;

    [SerializeField] private BuildingManager _buildingManager;

    #region 프로퍼티
    static public PlacementMgr Instance => _instance;
    public PlacementState CurrentState { get; private set; } = PlacementState.View;
    #endregion

    private void Awake()
    {
        _instance = this;
        _clickAction = InputSystem.actions.FindAction("UI/Select");
        _rotation = InputSystem.actions.FindAction("UI/Roation");
    }
    private void OnEnable()
    {
        _clickAction.Enable();
        _clickAction.performed += OnClickInput;
        //_rotation.performed += OnRoation;
    }

    private void OnDisable()
    {
        _clickAction.performed -= OnClickInput; 
        _clickAction.Disable();
    }
    public void ToggleEditMode()
    {
        CurrentState = (CurrentState == PlacementState.View) ? PlacementState.Edit : PlacementState.View;

        bool isEditMode = (CurrentState == PlacementState.Edit);

        _buildingManager.GridSystem.SetGridActive(isEditMode);

        Debug.Log($"현재 모드: {CurrentState}");

        if (CurrentState == PlacementState.View)
        {
            _buildingManager.CancelCurrentAction(); // 모드 나갈 때 들고 있던 거 취소
            CloseEditMenu();
        }
    }
    public void OnClickConstructionButton(GameObject prefab)
    {
        // 편집 모드일 때만 건설 시작 가능
        if (CurrentState != PlacementState.Edit)
        {
            Debug.LogWarning("편집 모드에서만 건물을 배치할 수 있습니다.");
            return;
        }

        _buildingManager.StartPlacement(prefab);
    }

    private void OnClickInput(InputAction.CallbackContext ctx)
    {
        //현재 상태가 편집모드가 아니라면 넘기기
        if (CurrentState != PlacementState.Edit || IsPointerOverUI()) return;

        if (_buildingManager.ActivePlaceable != null)
        {
            _buildingManager.ActivePlaceable.Placement(); // 건물의 Placement 실행
            return; 
        }

        TrySelectBuildingForMove();
    }

    private void TrySelectBuildingForMove()
    {
        // UI 요소 위에서 클릭이 발생하면 건물 선택을 무시, 즉 else가 실행되지 않음
        if (IsPointerOverUI()) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Placeable3D target = hit.collider.GetComponentInParent<Placeable3D>();

            if (target != null && target.ItemState == ItemState.Placed)
            {
                ShowEditMenu(target); // 편집 메뉴 표시
            }
            else
            {
                // 빈 땅을 클릭하면 메뉴 닫기
                CloseEditMenu();
            }
        }
    }
    // UI 요소 위에서 클릭이 발생했는지 확인하는 메서드
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        List<RaycastResult> results = new ();

        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
    // 선택된 건물에 대해 편집 메뉴를 표시하는 메서드
    private void ShowEditMenu(Placeable3D target)
    {
        if (_selectedTarget != null && _selectedTarget != target)
        {
            _selectedTarget.ToggleUI(false);
        }

        _selectedTarget = target;
        _selectedTarget.ToggleUI(true); 
    }
    // 편집 메뉴를 닫는 메서드
    public void CloseEditMenu()
    {
        if (_selectedTarget != null)
        {
            _selectedTarget.ToggleUI(false);
        }
        _selectedTarget = null;
    }
    public void OnClickMove() // 이동 버튼에 연결
    {
        if (_selectedTarget == null) return;

        _buildingManager.PickUpBuilding(_selectedTarget);

        CloseEditMenu();
    }
    public void OnClickRotate() // 회전 버튼에 연결
    {
        if (_selectedTarget == null) return;

        _selectedTarget.ObjectRotate();
    }
    public void OnClickDelete() // 삭제 버튼에 연결
    {
        if (_selectedTarget == null) return;

        _buildingManager.DeleteBuilding(_selectedTarget);


        CloseEditMenu();
    }
}
