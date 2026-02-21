using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Placeable3D : Placeable
{

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private InputAction _rotateAction;
    [SerializeField] private InputAction _initAction;
    [SerializeField] private float _rotationStep = 90f; // 한 번 누를 때 회전할 각도

    Camera _mainCamera;
    MeshRenderer _selectedRenderer;
    private float _currentYRotation = 0f; // 현재 유지 중인 회전값
    
    Color _succees = new (0, 1, 0, 0.1f);
    Color _fail = new (1, 0, 0, 0.1f);

    public void Initialize(GridSystem grid)
    {
        _targetGrid = grid;
        _mainCamera = Camera.main;
        _selectedRenderer = GetComponentInChildren<MeshRenderer>();
        _rotateAction = InputSystem.actions.FindAction("UI/Rotation");
        _initAction = InputSystem.actions.FindAction("UI/Init");
        _groundLayer = LayerMask.GetMask("Water"); //레이어 추가 설정하지 않아서 우선적으로 Water로 설정.
                                                   //추후 레이아웃에 맞는 레이어로 변경 필요
        enabled = true;
        _rotateAction.Enable(); // 입력 액션 활성화
        _rotateAction.performed += OnRotate;
        _initAction.performed += OnPlaceInput;

    }
    private void OnDisable()
    {
        _rotateAction.performed -= OnRotate;
        _initAction.performed -= OnPlaceInput;
        _rotateAction.Disable(); // 입력 액션 비활성화
    }
    private void OnRotate(InputAction.CallbackContext ctx)
    {
        Debug.Log("회전 입력");
        _currentYRotation = (_currentYRotation + _rotationStep) % 360;
    }
    private void OnPlaceInput(InputAction.CallbackContext ctx)
    {
        if (enabled)
        {
            Placement();
        }
    }
    public override Vector2Int ConvertedIndex()
    {
        float maxDistance = 1000f; // 레이캐스트 최대 거리
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, _groundLayer))
        {
            return _targetGrid.GetGridIndex(hit.point);
        }

        return new Vector2Int(-1, -1);
    }
    public override void Placement()
    {
        Vector2Int index = ConvertedIndex();

        //현재 데이터가 없어서 1,1 사이즈로 고정. 추후에 SO데이터 받아서 변경 필요
        if (index.x != -1 && _targetGrid.IsCellEmpty(index.x, index.y, 1, 1))
        {
            _targetGrid.PlaceItem(index.x, index.y, 1, 1);

            _selectedRenderer.material.color = Color.white;

            enabled = false;
        }
    }
    public override void VisualFeedback()
    {
        Vector2Int index = ConvertedIndex();
        if (index.x == -1)
        {
            _selectedRenderer.material.color = _fail;
            return;
        }

        bool flaceAble = _targetGrid.IsCellEmpty(index.x, index.y, 1, 1);

        _selectedRenderer.material.color = flaceAble ? _succees : _fail;
    }
    private void Update()
    {
        if (_mainCamera == null || _targetGrid == null) return;

        VisualFeedback();

        Vector2Int currentIndex = ConvertedIndex();

        if (currentIndex.x != -1)
        {
            Vector3 snapPos = _targetGrid.GetWorldPosition(currentIndex.x, currentIndex.y);

            transform.SetPositionAndRotation(snapPos, Quaternion.Euler(0, _currentYRotation, 0));

            Debug.Log($"현재 칸 좌표: {currentIndex}");
        }
    }
}
