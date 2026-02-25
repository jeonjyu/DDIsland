using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 3D 오브젝트의 배치 기능을 구현하는 클래스
/// </summary>
public class Placeable3D : Placeable
{

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _rotationStep = 90f; // 한 번 누를 때 회전할 각도 
    [SerializeField] private GameObject _editMenuUI; // 현재 선택된 건물의 편집 UI

    Camera _mainCamera;
    MeshRenderer _selectedRenderer;
    private float _currentYRotation = 0f; // 현재 유지 중인 회전값
    private bool _isRotated;
    private Color _originalColor;

    private Vector2Int _lastPlacedIndex;
    private Vector2Int _lastPlacedSize;

    [SerializeField] int _sizeX;
    [SerializeField] int _sizeY;

    #region 레이캐스트
    private Vector2Int _cachedIndex;
    //private bool _hasHit;
    #endregion

    Color _succees = new(0.5f, 1, 0.5f, 0.5f);
    Color _fail = new(1, 0.5f, 0.5f, 0.5f);

    #region 프로퍼티
    public Vector2Int PlacedIndex => _lastPlacedIndex;
    public Vector2Int PlacedSize => _lastPlacedSize;
    public bool IsRotated => _isRotated;
    public float CurrentYRotation => _currentYRotation;
    #endregion

    public void Initialize(GridSystem grid)
    {
        _targetGrid = grid;
        _mainCamera = Camera.main;
        _selectedRenderer = GetComponentInChildren<MeshRenderer>();
        _groundLayer = LayerMask.GetMask("Water"); //레이어 추가 설정하지 않아서 우선적으로 Water로 설정
                                                   //추후 레이아웃에 맞는 레이어로 변경 필요
        _originalColor = _selectedRenderer.material.color;

        ItemState = ItemState.Preview;
        enabled = true;
    }
    public void ToggleUI(bool isActive)
    {
        if (_editMenuUI != null)
        {
            _editMenuUI.SetActive(isActive);
        }
    }

    public void ObjectRotate()
    {
        if (ItemState == ItemState.Placed)
        {
            _targetGrid.RemoveItem(_lastPlacedIndex.x, _lastPlacedIndex.y, _lastPlacedSize.x, _lastPlacedSize.y);
        }

        _currentYRotation = (_currentYRotation + _rotationStep) % 360;
        _isRotated = !_isRotated;

        Vector2Int newSize = GetRotatedSize();
        if (ItemState == ItemState.Placed) _cachedIndex = _lastPlacedIndex;

        Vector3 snapPos = _targetGrid.GetWorldPosition(_cachedIndex.x, _cachedIndex.y, newSize.x, newSize.y);
        transform.SetPositionAndRotation(snapPos, Quaternion.Euler(0, _currentYRotation, 0));

        if (ItemState == ItemState.Placed)
        {
            _targetGrid.PlaceItem(_cachedIndex.x, _cachedIndex.y, newSize.x, newSize.y, this);
            _lastPlacedSize = newSize;
        }

        VisualFeedback();
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
    // 아이템을 그리드에 배치
    public override void Placement()
    {
        Vector2Int index = _cachedIndex;
        Vector2Int size = GetRotatedSize();

        //현재 데이터가 없어서 사이즈 고정. 추후에 SO데이터 받아서 변경 필요
        if (index.x != -1 && _targetGrid.IsCellEmpty(index.x, index.y, size.x, size.y, this))
        {
            if (ItemState == ItemState.Placed)
            {
                _targetGrid.RemoveItem(_lastPlacedIndex.x, _lastPlacedIndex.y, _lastPlacedSize.x, _lastPlacedSize.y);
            }
            // 그리드에 아이템 배치
            _targetGrid.PlaceItem(index.x, index.y, size.x, size.y, this);

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
        Vector2Int index = _cachedIndex;
        Vector2Int size = GetRotatedSize();

        if (index.x == -1)
        {
            return;
        }

        bool placeAble = _targetGrid.IsCellEmpty(index.x, index.y, size.x, size.y, this);

        if (ItemState == ItemState.Preview)
        {
            // 배치 중일 때는 물체 색상도 변경
            _selectedRenderer.material.color = placeAble ? _succees : _fail;
            _targetGrid.UpdateShaderHover(index, size, placeAble);
        }
        else
        {
            _selectedRenderer.material.color = _originalColor;
            _targetGrid.UpdateShaderHover(index, size, false);
        }

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


        _cachedIndex = ConvertedIndex();
        Vector2Int size = GetRotatedSize();

        VisualFeedback();
        if (_cachedIndex.x != -1)
        {
            Vector3 snapPos = _targetGrid.GetWorldPosition(_cachedIndex.x, _cachedIndex.y, size.x, size.y);

            transform.SetPositionAndRotation(snapPos, Quaternion.Euler(0, _currentYRotation, 0));

            Debug.Log($"현재 칸 좌표: {_cachedIndex}");
        }
    }
    public void RestoreState(Vector3 worldPos, float yRotation, bool isRotated)
    {

        transform.SetPositionAndRotation(worldPos, Quaternion.Euler(0, yRotation, 0));

        // 저장된 회전값과 회전 상태 복원
        _currentYRotation = yRotation;
        _isRotated = isRotated;

        // 저장 되어 있던 상태 정리
        ItemState = ItemState.Placed;
        _selectedRenderer.material.color = _originalColor;
        _targetGrid.ClearGrid();
        enabled = false;
    }
}
