using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 3D 오브젝트의 배치 기능을 구현하는 클래스
/// </summary>
public class Placeable3D : Placeable
{
    // todo: 아무것도 안하기
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _rotationStep = 90f; // 한 번 누를 때 회전할 각도 
    [SerializeField] private GameObject _editMenuUI; // 현재 선택된 건물의 편집 UI

    [SerializeField] private BuildingManager _build;
    [SerializeField] private InteriorDataSO _data;

    [SerializeField] Camera _mainCamera;
    [SerializeField] MeshRenderer _selectedRenderer;
    [SerializeField] private float _currentYRotation = 0f; // 현재 유지 중인 회전값
    [SerializeField] private int _isRotated;
    [SerializeField] private bool _isPlaced;
    [SerializeField] private Color _originalColor;

    // 인스펙터에선 가려놓고 직렬화로 저장시켜놓음
    [SerializeField, HideInInspector] private Vector2Int _lastPlacedIndex;
    [SerializeField, HideInInspector] private Vector2Int _lastPlacedSize;

    [SerializeField] int _sizeX;
    [SerializeField] int _sizeY;

    [SerializeField] private AudioClip _placedAudio;

    private bool _isInvalidRotation = false;
    private int _lastRotated;

    #region 레이캐스트
    [SerializeField, ReadOnly] private Vector2Int _cachedIndex;
    //private bool _hasHit;
    #endregion

    Color _succees = new(0.5f, 1, 0.5f, 0.5f);
    Color _fail = new(1, 0.5f, 0.5f, 0.5f);

    #region 프로퍼티
    public Vector2Int PlacedIndex => _lastPlacedIndex;
    public Vector2Int PlacedSize => _lastPlacedSize;
    public int IsRotated => _isRotated;
    public float CurrentYRotation => _currentYRotation;
    public bool IsPlaced => _isPlaced;
    public bool IsEditable => _data != null && _data.interior_itemType != Interior_ItemType.Fix;
    #endregion

    private void Start()
    {
        if (ItemState == ItemState.Placed)
        {
            Invoke(nameof(RegisterToGrid), 0.01f);
        }
    }
    private void RegisterToGrid()
    {
        if (_targetGrid != null)
        {
            // 현재 내 위치와 크기 정보를 바탕으로 그리드 점유
            _targetGrid.PlaceItem(PlacedIndex.x, PlacedIndex.y, PlacedSize.x, PlacedSize.y, this);
            Debug.Log($"[Auto-Register] {gameObject.name}이 ({PlacedIndex.x}, {PlacedIndex.y}) 위치에 자동 등록되었습니다.");
        }
    }

    public void Initialize(GridSystem grid, BuildingManager build, InteriorDataSO data)
    {
        _build = build;
        _targetGrid = grid;
        _data = data;
        _mainCamera = Camera.main;
        _selectedRenderer = GetComponentInChildren<MeshRenderer>();
        _groundLayer = LayerMask.GetMask("Ground");

        if (Application.isPlaying)
        {
            _originalColor = _selectedRenderer.material.color;

            ItemState = ItemState.Preview;
        }

        _sizeX = _data.GridSizeX;
        _sizeY = _data.GridSizeY;



        enabled = true;
    }
    public void ToggleUI(bool isActive)
    {
        if (_editMenuUI != null)
        {
            _editMenuUI.SetActive(isActive);
            if (isActive)
            {
                VisualFeedback();
            }
            else
            {
                if (_isInvalidRotation)
                {
                    _isRotated = _lastRotated;
                    _currentYRotation = _isRotated * _rotationStep;
                    _cachedIndex = _lastPlacedIndex;

                    Vector3 snapPos = _targetGrid.GetWorldPosition(_lastPlacedIndex.x, _lastPlacedIndex.y, _lastPlacedSize.x, _lastPlacedSize.y);
                    transform.SetPositionAndRotation(snapPos, Quaternion.Euler(0, _currentYRotation, 0));

                    _isInvalidRotation = false;
                    _targetGrid.PlaceItem(_lastPlacedIndex.x, _lastPlacedIndex.y, _lastPlacedSize.x, _lastPlacedSize.y, this);

                    VisualFeedback();
                }
                _targetGrid.ClearGrid();
            }
        }
    }

    public void ObjectRotate()
    {
        if (IsEditable == false) return;

        Vector2Int currentPivotCell = _cachedIndex + GetRotatedPivot();

        int next = (_isRotated + 1) % 4;
        int originalRotated = _isRotated;

        _isRotated = next;
        Vector2Int newSize = GetRotatedSize();
        Vector2Int newIndex = currentPivotCell - GetRotatedPivot();

        if (ItemState == ItemState.Placed)
        {
            if (!_isInvalidRotation)
            {
                _targetGrid.RemoveItem(_lastPlacedIndex.x, _lastPlacedIndex.y, _lastPlacedSize.x, _lastPlacedSize.y);
            }

            bool canRotate = _targetGrid.IsCellEmpty(newIndex.x, newIndex.y, newSize.x, newSize.y, this);

            _currentYRotation = _isRotated * _rotationStep;
            _cachedIndex = newIndex;
            Vector3 snapPos = _targetGrid.GetWorldPosition(_cachedIndex.x, _cachedIndex.y, newSize.x, newSize.y);
            transform.SetPositionAndRotation(snapPos, Quaternion.Euler(0, _currentYRotation, 0));

            if (!canRotate)
            {
                _isInvalidRotation = true;
            }
            else
            {
                _isInvalidRotation = false;
                _lastRotated = _isRotated;
                _lastPlacedIndex = _cachedIndex;
                _lastPlacedSize = newSize;
                _targetGrid.PlaceItem(_cachedIndex.x, _cachedIndex.y, newSize.x, newSize.y, this);
            }
        }
        else if (ItemState == ItemState.Preview)
        {
            _currentYRotation = _isRotated * _rotationStep;
            _cachedIndex = newIndex;

            Vector3 snapPos = _targetGrid.GetWorldPosition(_cachedIndex.x, _cachedIndex.y, newSize.x, newSize.y);
            transform.SetPositionAndRotation(snapPos, Quaternion.Euler(0, _currentYRotation, 0));

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
            Vector2Int rotatedPivot = GetRotatedPivot();
            Vector2Int size = GetRotatedSize();

            // 아이템의 중심이 마우스 위치에 오도록 시작 좌표 계산
            int startX = mouseIndex.x - rotatedPivot.x;
            int startY = mouseIndex.y - rotatedPivot.y;

            // 그리드 범위를 벗어나지 않도록 좌표 조정
            return new Vector2Int(
                Mathf.Clamp(startX, 0, _targetGrid.Width - size.x),
                Mathf.Clamp(startY, 0, _targetGrid.Height - size.y)
            );
        }

        return new Vector2Int(-1, -1);
    }
    private Vector2Int GetRotatedPivot()
    {
        int px = (_sizeX - 1) / 2;
        int py = (_sizeY - 1) / 2;

        switch (_isRotated)
        {
            case 0: return new Vector2Int(px, py);                                 // 0도
            case 1: return new Vector2Int(py, (_sizeX - 1) - px);                // 90도
            case 2: return new Vector2Int((_sizeX - 1) - px, (_sizeY - 1) - py); // 180도
            case 3: return new Vector2Int((_sizeY - 1) - py, px);                // 270도
            default: return _pivot;
        }
    }

    // 아이템을 그리드에 배치
    public override void Placement(AudioClip audio)
    {
        if (IsEditable == false && ItemState == ItemState.Placed) return;
        Vector2Int index = _cachedIndex;
        Vector2Int size = GetRotatedSize();

        //현재 데이터가 없어서 사이즈 고정. 추후에 SO데이터 받아서 변경 필요
        if (index.x != -1 && _targetGrid.IsCellEmpty(index.x, index.y, size.x, size.y, this))
        {
            //만약 아이템이 설치 되어 있다면 이동을 위해서 설치되어 있는 걸 제거
            if (ItemState == ItemState.Placed)
            {
                _targetGrid.RemoveItem(_lastPlacedIndex.x, _lastPlacedIndex.y, _lastPlacedSize.x, _lastPlacedSize.y);
            }

            _isPlaced = true;
            ItemState = ItemState.Placed;

            // 그리드에 아이템 배치
            _targetGrid.PlaceItem(index.x, index.y, size.x, size.y, this);

            _lastPlacedIndex = index;
            _lastPlacedSize = size;

            // 아이템의 색을 원래대로 돌려놓기
            _selectedRenderer.material.color = _originalColor;

            _targetGrid.ClearGrid(); // 셰이더 하이라이트 초기화

            SoundManager.Instance.PlaySFX(audio);

            _build.CompletePlacement(this);
        }
    }
    public override void VisualFeedback()
    {
        Vector2Int index = _cachedIndex;
        Vector2Int size = GetRotatedSize();

        if (index.x == -1)
        {
            _targetGrid.ClearGrid();
            return;
        }

        bool placeAble = _targetGrid.IsCellEmpty(index.x, index.y, size.x, size.y, this);

        if (ItemState == ItemState.Preview)
        {
            // 배치 중일 때는 물체 색상도 변경
            _selectedRenderer.material.color = placeAble ? _succees : _fail;
        }
        else
        {
            _selectedRenderer.material.color = _isInvalidRotation ? _fail : _originalColor;
        }
        _targetGrid.UpdateShaderHover(index, size, placeAble);

    }

    private Vector2Int GetRotatedSize()
    {
        return (_isRotated % 2 == 1) ? new Vector2Int(_sizeY, _sizeX) : new Vector2Int(_sizeX, _sizeY);
    }

    private void Update()
    {
        if (ItemState == ItemState.Placed) return;

        if (_mainCamera == null || _targetGrid == null) return;

        // 마우스 좌표 따오는거
        _cachedIndex = ConvertedIndex();
        // 회전 사이즈 계산하는거
        Vector2Int size = GetRotatedSize();

        // 만약 마우스에 좌표가 있다면
        if (_cachedIndex.x != -1)
        {
            Vector3 snapPos = _targetGrid.GetWorldPosition(_cachedIndex.x, _cachedIndex.y, size.x, size.y);

            //위에서 좌표를 계산해서 거기다가 따라감
            transform.SetPositionAndRotation(snapPos, Quaternion.Euler(0, _currentYRotation, 0));

            //계속해서 밑에 그리드는 색칠해주고
            VisualFeedback();
        }
        else
        {
            _targetGrid.ClearGrid();
        }
    }
    public void RestoreState(Vector3 worldPos, float yRotation, int isRotated)
    {
        transform.SetPositionAndRotation(worldPos, Quaternion.Euler(0, yRotation, 0));

        // 저장된 회전값과 회전 상태 복원
        _currentYRotation = yRotation;
        _isRotated = isRotated;
        _lastPlacedSize = GetRotatedSize();

        // 저장 되어 있던 상태 정리
        ItemState = ItemState.Placed;
        _selectedRenderer.material.color = _originalColor;
        _targetGrid.ClearGrid();
        //enabled = false;
    }

    public int GetItemId()
    {
        return _data != null ? _data.InteriorID : -1;
    }

    /// <summary>
    /// 베이크 전용 함수입니다
    /// </summary>
    #region 베이크전용함수
    public void SetBakeData(Vector2Int index, Vector2Int size)
    {
        _cachedIndex = index;
        _lastPlacedIndex = index;
        _lastPlacedSize = size;
        _sizeX = size.x;
        _sizeY = size.y;
        ItemState = ItemState.Placed;
    }
    #endregion
}
