using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
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
    private InputAction _cancel;
    private InputAction _scrollAction;

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera _viewCamera;
    [SerializeField] private CinemachineCamera _editCamera;

    [SerializeField] private BuildingManager _buildingManager;
    public event Action<Placeable3D> OnBuildingPick;
    public event Action OnBuildingDrop;

    [Header("Zoom Settings")]
    [SerializeField] private float _zoomSpeed = 10f; // 줌 속도
    [SerializeField] private float _minFOV = 20f;   // 가장 가까운 곳
    [SerializeField] private float _maxFOV = 70f;   // 가장 먼 곳

    #region 프로퍼티
    static public PlacementMgr Instance => _instance;
    public PlacementState CurrentState { get; private set; } = PlacementState.View;
    #endregion

    private void Awake()
    {
        _instance = this;
        _clickAction = InputSystem.actions.FindAction("UI/Select");
        _rotation = InputSystem.actions.FindAction("UI/Rotation");
        _cancel = InputSystem.actions.FindAction("UI/Cancel");
        _scrollAction = InputSystem.actions.FindAction("UI/ScrollWheel");
    }
    private void OnEnable()
    {
        _clickAction.Enable();
        _clickAction.performed += OnClickInput;

        _cancel.Enable();
        _cancel.performed += OnCancelInput;

        _scrollAction?.Enable();

        _rotation.Enable();
        _rotation.performed += OnRotate;
    }

    private void OnDisable()
    {
        _clickAction.performed -= OnClickInput;
        _clickAction?.Disable();

        _cancel.performed -= OnCancelInput;
        _cancel?.Disable();

        _scrollAction?.Disable();

        _rotation.performed -= OnRotate;
        _rotation.Disable();
    }

    private void Update()
    {
        HandleZoom();
    }

    private void HandleZoom()
    {
        if (CurrentState != PlacementState.Edit) return;

        float scrollValue = _scrollAction.ReadValue<Vector2>().y;
        if (Mathf.Abs(scrollValue) < 0.01f) return;

        if (_editCamera != null)
        {
            float currentFOV = _editCamera.Lens.FieldOfView;

            float nextFOV = currentFOV - (scrollValue * _zoomSpeed * Time.deltaTime);

            _editCamera.Lens.FieldOfView = Mathf.Clamp(nextFOV, _minFOV, _maxFOV);
        }
    }

    public void ToggleEditMode()
    {
        CurrentState = (CurrentState == PlacementState.View) ? PlacementState.Edit : PlacementState.View;

        bool isEditMode = (CurrentState == PlacementState.Edit);

        _buildingManager.GridSystem.SetGridActive(isEditMode);

        Debug.Log($"현재 모드: {CurrentState}");

        if (isEditMode)
        {
            _editCamera.Priority = 100;
            _viewCamera.Priority = 10;
        }
        else
        {
            _editCamera.Priority = 10;
            _viewCamera.Priority = 100;

            _buildingManager.CancelCurrentAction();
            CloseEditMenu();
        }
    }
    // ToDo: 배치중 회전 방식을 추가해야하는데 아직 추가 안함
    private void OnRotate(InputAction.CallbackContext ctx)
    {
        OnClickRotate();
    }
    private void OnCancelInput(InputAction.CallbackContext ctx)
    {
        // 편집 모드 또는 3D오브젝트가 있을 때
        if (CurrentState == PlacementState.Edit && _buildingManager.ActivePlaceable != null)
        {
            _buildingManager.CancelCurrentAction();

            CloseEditMenu();
        }
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
                if(target.IsEditable)
                {
                    ShowEditMenu(target); // 편집 메뉴 표시
                }
                else
                {   
                    //TODO : 프리팹 교체 UI 추가
                }
            }
            else
            {
                // 빈 땅을 클릭하면 메뉴 닫기
                CloseEditMenu();
            }
        }
        else
        {
            //여긴 빈 땅 뿐만이 아니라 아무데나 입력해도 창 닫히게 해놓음
            CloseEditMenu();
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

        List<RaycastResult> results = new();

        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
    // 선택된 건물에 대해 편집 메뉴를 표시하는 메서드
    private void ShowEditMenu(Placeable3D target)
    {
        if (_selectedTarget != null && _selectedTarget != target)
        {
            //_selectedTarget.ToggleUI(false);
            OnBuildingDrop?.Invoke();
        }

        _selectedTarget = target;
        //_selectedTarget.ToggleUI(true);
        OnBuildingPick?.Invoke(target);
    }
    // 편집 메뉴를 닫는 메서드
    public void CloseEditMenu()
    {
        if (_selectedTarget != null)
        {
            //_selectedTarget.ToggleUI(false);
            OnBuildingDrop?.Invoke();
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
    public void OnClickAllDelete()
    {
        CloseEditMenu();
        _buildingManager.ClearAll();

        Debug.Log("전체 회수 버튼 클릭: 모든 건물을 삭제했습니다.");
    }
    public void OnClickConfirmSession()
    {
        _buildingManager.ConfirmAll();

        CloseEditMenu();

        Debug.Log("모든 변경 사항이 확정되어 저장되었습니다.");
    }
    public void OnClickCancelSession()
    {
        _buildingManager.RevertAll();

        _buildingManager.CancelCurrentAction();

        CloseEditMenu();

        Debug.Log("모든 변경 사항이 취소되고 편집 전으로 되돌아갔습니다.");
    }

    public void OnClickConstructionButton(int itemId)
    {
        // 편집 모드일 때만 건설 시작 가능
        if (CurrentState != PlacementState.Edit)
        {
            Debug.LogWarning("편집 모드에서만 건물을 배치할 수 있습니다.");
            return;
        }

        
        _buildingManager.StartPlacement(itemId);
    }


}
