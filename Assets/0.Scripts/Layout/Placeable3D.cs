using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 3D 오브젝트의 배치 기능을 구현하는 클래스
/// </summary>
public class Placeable3D : Placeable
{

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private InputAction _rotateAction;
    [SerializeField] private InputAction _initAction;
    [SerializeField] private float _rotationStep = 90f; // 한 번 누를 때 회전할 각도

    Camera _mainCamera;
    MeshRenderer _selectedRenderer;
    private float _currentYRotation = 0f; // 현재 유지 중인 회전값
    private bool _isRotated;
    private Color _originalColor;

    private Vector2Int _lastPlacedIndex;
    private Vector2Int _lastPlacedSize;

    [SerializeField] int _sizeX;
    [SerializeField] int _sizeY;
    
    Color _succees = new (0.5f, 1, 0.5f, 0.5f);
    Color _fail = new (1, 0.5f, 0.5f, 0.5f);

    public void Initialize(GridSystem grid)
    {
        _targetGrid = grid;
        _mainCamera = Camera.main;
        _selectedRenderer = GetComponentInChildren<MeshRenderer>();
        _rotateAction = InputSystem.actions.FindAction("UI/Rotation");
        _initAction = InputSystem.actions.FindAction("UI/Init");
        _groundLayer = LayerMask.GetMask("Water"); //레이어 추가 설정하지 않아서 우선적으로 Water로 설정.
                                                   //추후 레이아웃에 맞는 레이어로 변경 필요
        _originalColor = _selectedRenderer.material.color;

        ItemState = ItemState.Preview;
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
        if (ItemState == ItemState.Placed) return;

        _currentYRotation = (_currentYRotation + _rotationStep) % 360;
        _isRotated = !_isRotated;

        VisualFeedback();
    }
    private void OnPlaceInput(InputAction.CallbackContext ctx)
    {
        if (ItemState == ItemState.Preview)
        {
            Placement();
        }
    }
    // 마우스 위치를 그리드 좌표로 변환
    public override Vector2Int ConvertedIndex()
    {
        float maxDistance = 1000f; // 레이캐스트 최대 거리
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, _groundLayer))
        {
            // 레이캐스트가 그라운드에 닿았을 때, 해당 위치를 그리드 좌표로 변환
            Vector2Int mouseIndex = _targetGrid.GetGridIndex(hit.point);

            Vector2Int size = GetRotatedSize(); // 사이즈 계산

            // 아이템의 중심이 마우스 위치에 오도록 시작 좌표 계산
            int startX = mouseIndex.x - (size.x - 1) / 2;
            int startY = mouseIndex.y - (size.y - 1) / 2;

            // 그리드 범위를 벗어나지 않도록 좌표 조정
            return new Vector2Int(
                Mathf.Clamp(startX, 0, _targetGrid.Width - size.x),
                Mathf.Clamp(startY, 0, _targetGrid.Height - size.y)
            );
        }

        return new Vector2Int(-1, -1);
    }
    public override void Placement()
    {
        Vector2Int index = ConvertedIndex();
        Vector2Int size = GetRotatedSize();

        //현재 데이터가 없어서 사이즈 고정. 추후에 SO데이터 받아서 변경 필요
        if (index.x != -1 && _targetGrid.IsCellEmpty(index.x, index.y, size.x, size.y))
        {
            // 그리드에 아이템 배치
            _targetGrid.PlaceItem(index.x, index.y, size.x, size.y);

            _lastPlacedIndex = index;
            _lastPlacedSize = size;

            // 아이템의 색을 원래대로 돌려놓기
            _selectedRenderer.material.color = _originalColor;

            _targetGrid.ClearGrid(); // 셰이더 하이라이트 초기화

            ItemState = ItemState.Placed;
        }
    }
    public override void VisualFeedback()
    {
        Vector2Int index = ConvertedIndex();

        Vector2Int size = GetRotatedSize();

        if (index.x == -1)
        {
            _selectedRenderer.material.color = _fail;
            // 그리드 셰이더에도 실패 상태 전달 (좌표를 맵 밖으로 보내서 하이라이트 제거)
            _targetGrid.UpdateShaderHover(new Vector2Int(-10, -10), size, false);
            return;
        }

        bool placeAble = _targetGrid.IsCellEmpty(index.x, index.y, size.x, size.y);

        _selectedRenderer.material.color = placeAble ? _succees : _fail;

        _targetGrid.UpdateShaderHover(index, size, placeAble);
    }
    //회전된 상태에서의 사이즈 계산
    private Vector2Int GetRotatedSize()
    {
        return _isRotated ? new Vector2Int(_sizeY, _sizeX) : new Vector2Int(_sizeX, _sizeY);
    }
    private void Update()
    {
        if (ItemState == ItemState.Placed) return;

        if (_mainCamera == null || _targetGrid == null) return;

        VisualFeedback();

        Vector2Int currentIndex = ConvertedIndex();
        Vector2Int size = GetRotatedSize();

        if (currentIndex.x != -1)
        {
            Vector3 snapPos = _targetGrid.GetWorldPosition(currentIndex.x, currentIndex.y, size.x, size.y);

            transform.SetPositionAndRotation(snapPos, Quaternion.Euler(0, _currentYRotation, 0));

            Debug.Log($"현재 칸 좌표: {currentIndex}");
        }
    }
}
