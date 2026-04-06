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
    private InputAction _cameraWalk;

    [SerializeField] private AudioClip _placedAudio;


    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera _viewCamera;
    [SerializeField] private CinemachineCamera _editCamera;

    [SerializeField] private BuildingManager _buildingManager;
    public event Action<Placeable3D> OnBuildingPick;
    public event Action OnBuildingDrop;
    public event Action<FixedBuilding> OnFixedBuildingPick;
    public event Action<Placeable3D> OnGridFixBuildingPick;

    [Header("Zoom Settings")]
    [SerializeField] private float _zoomSpeed = 10f; // 줌 속도
    [SerializeField] private float _minFOV = 20f;   // 가장 가까운 곳
    [SerializeField] private float _maxFOV = 70f;   // 가장 먼 곳

    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _moveSpeed = 20f;     // 이동 속도
    [SerializeField] private Vector2 _mapLimitMin;       // 섬 최소 범위
    [SerializeField] private Vector2 _mapLimitMax;       // 섬 최대 범위
    private Vector3 _targetCameraPos;

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
        _cameraWalk = InputSystem.actions.FindAction("UI/CameraWalk");
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
    #region 섬 카메라 관련
    private void Update()
    {
        if (CurrentState != PlacementState.Edit) return;

        HandleZoom();
        HandleCameraMovement();
    }

    private void HandleZoom()
    {

        float scrollValue = _scrollAction.ReadValue<Vector2>().y;
        if (Mathf.Abs(scrollValue) < 0.01f) return;

        if (_editCamera != null)
        {
            float currentSize = _editCamera.Lens.OrthographicSize;

            float nextSize = currentSize - (scrollValue * _zoomSpeed * Time.deltaTime);

            _editCamera.Lens.OrthographicSize = Mathf.Clamp(nextSize, _minFOV, _maxFOV);
        }
    }

    public void ToggleEditMode()
    {
        CurrentState = (CurrentState == PlacementState.View) ? PlacementState.Edit : PlacementState.View;

        bool isEditMode = (CurrentState == PlacementState.Edit);

        _buildingManager.GridSystem.SetGridActive(isEditMode);

        if (isEditMode)
        {
            _targetCameraPos = _cameraTarget.position;

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

    private void HandleCameraMovement()
    {
        if (_editCamera == null || _cameraWalk == null) return;

        Vector2 input = _cameraWalk.ReadValue<Vector2>();
        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 forward = _editCamera.transform.forward; forward.y = 0; forward.Normalize();
            Vector3 right = _editCamera.transform.right; right.y = 0; right.Normalize();

            Vector3 moveDir = (forward * input.y + right * input.x).normalized;
            _targetCameraPos += moveDir * _moveSpeed * Time.deltaTime;

            // 이동 제한 (Clamp)
            _targetCameraPos.x = Mathf.Clamp(_targetCameraPos.x, _mapLimitMin.x, _mapLimitMax.x);
            _targetCameraPos.z = Mathf.Clamp(_targetCameraPos.z, _mapLimitMin.y, _mapLimitMax.y);
        }

        _cameraTarget.position = _targetCameraPos;
    }
    #endregion
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
            _buildingManager.ActivePlaceable.Placement(_placedAudio); // 건물의 Placement 실행
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

            FixedBuilding fixTarget = hit.collider.GetComponentInParent<FixedBuilding>();

            if (target != null && target.ItemState == ItemState.Placed)
            {
                if (target.IsEditable)
                {
                    ShowEditMenu(target); // 편집 메뉴 표시
                }
                else
                {
                    CloseEditMenu();
                    OnGridFixBuildingPick?.Invoke(target);
                }
            }
            else if (fixTarget != null)
            {
                CloseEditMenu(); 
                OnFixedBuildingPick?.Invoke(fixTarget);
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
       
        if (_buildingManager.ActivePlaceable != null)
        {
            _buildingManager.ActivePlaceable.ObjectRotate();
            return;
        }

        if (_selectedTarget != null)
        {
            _selectedTarget.ObjectRotate();
        }
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

    }
    public void OnClickConfirmSession()
    {
        _buildingManager.ConfirmAll();

        CloseEditMenu();
    }
    public void OnClickCancelSession()
    {
        _buildingManager.RevertAll();

        _buildingManager.CancelCurrentAction();

        CloseEditMenu();
    }

    public void OnClickConstructionButton(int itemId)
    {
        // 편집 모드일 때만 건설 시작 가능
        if (CurrentState != PlacementState.Edit)
        {
            return;
        }

        
        _buildingManager.StartPlacement(itemId);
    }


}
