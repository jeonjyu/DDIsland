using System;
using UnityEngine;
/// <summary>
/// 그리드 시스템을 관리하는 클래스
/// </summary>
public class GridSystem : MonoBehaviour
{
    [SerializeField] private int _width = 12; //실제로 가지는 셀의 수 
    [SerializeField] private int _height = 12;

    [Header("Visual Settings")]
    [SerializeField] private Transform _cell; //셀의 크기를 결정하는 Transform, 이 크기에 맞춰 셀의 실제 크기를 계산
    [SerializeField] private MeshRenderer _gridRenderer; //렌더러 컴포넌트
    [SerializeField] private GameObject _gridObject; //그리드 시각화 오브젝트

    private Texture2D _gridDataTexture;

    private Placeable[,] _grid; //셀의 상태를 나타내는 2차원 배열, 0은 빈 셀, 1은 채워진 셀

    private float _cellSize; //셀의 실제 크기
    private bool _rotation = false;

    Bounds bounds;

    #region 프로퍼티
    public int Width => _width;   
    public int Height => _height;
    public float CellSize => _cellSize;
    public bool Rotaion => _rotation;
    #endregion

    private void Awake()
    {
        _grid = new Placeable[_width, _height];

        //Plane에서 기준점을 잡기위한 용도
        bounds = _gridRenderer.GetComponent<MeshFilter>().sharedMesh.bounds;
    }
    private void Start()
    {
        // 그리드의 데이터를 저장하는 텍스처, 셰이더에 셀의 상태를 전달하기 위함
        _gridDataTexture = new(_width, _height)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        ApplyGridToShader();
    }
    public void SetGridActive(bool isActive)
    {
        if (_gridObject != null)
        {
            Debug.Log($"그리드 시각화 오브젝트의 활성 상태를 {(isActive ? "활성화" : "비활성화")}합니다.");
            _gridObject.SetActive(isActive);
        }
    }
    // 그리드 정보를 셰이더에 전달하는 메서드
    public void ApplyGridToShader()
    {
        if (_gridRenderer != null)
        {
            _gridRenderer.material.SetVector("_Gridsize", new Vector2(_width, _height));
        }
    }
    // 해당 오브젝트가 그리드에 배치 될 수 있는지 확인하는 메서드
    public bool IsCellEmpty(int startX, int startY, int itemWidth, int itemHeight, Placeable self)
    {

        int minX = Math.Min(startX, startX + itemWidth);
        int maxX = Math.Max(startX, startX + itemWidth);
        int minY = Math.Min(startY, startY + itemHeight);
        int maxY = Math.Max(startY, startY + itemHeight);

        if (minX < 0 || minY < 0 || maxX > _width || maxY > _height)
        {
            Debug.LogError("그리드 범위를 벗어났습니다");
            return false;
        }
       
        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                if (_grid[i, j] != null && _grid[i, j] != self)
                {
                    return false;
                }
            }
        }
        return true;
    }
    // 그리드에 오브젝트를 배치하는 메서드
    public void PlaceItem(int startX, int startY, int itemWidth, int itemHeight, Placeable item)
    {

        // 각 시작점의 최소, 최대 거리
        int minX = Math.Min(startX, startX + itemWidth);
        int maxX = Math.Max(startX, startX + itemWidth);
        int minY = Math.Min(startY, startY + itemHeight);
        int maxY = Math.Max(startY, startY + itemHeight);
        
        if (minX < 0 || minY < 0 || maxX > _width || maxY > _height)
        {
            Debug.LogError("그리드 범위를 벗어나 배치할 수 없습니다.");
            return;
        }

        // 둘다 양수일 때 하나만 음수일 때(세로 음수, 가로 음수) 둘다 음수일 때
        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                _grid[i, j] = item;
            }
        }

        UpdateGridTexture();
    }
    public void RemoveItem(int startX, int startY, int itemWidth, int itemHeight)
    {
        // 각 시작점의 최소, 최대 거리
        int minX = Math.Min(startX, startX + itemWidth);
        int maxX = Math.Max(startX, startX + itemWidth);
        int minY = Math.Min(startY, startY + itemHeight);
        int maxY = Math.Max(startY, startY + itemHeight);

        if (minX < 0 || minY < 0 || maxX > _width || maxY > _height)
        {
            Debug.LogError("그리드 범위를 벗어났습니다");
            return;
        }

        // 둘다 양수일 때 하나만 음수일 때(세로 음수, 가로 음수) 둘다 음수일 때
        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                _grid[i, j] = null;
            }
        }
        UpdateGridTexture();
    }
    // 월드 좌표를 셀 좌표로 번역
    public Vector2Int GetGridIndex(Vector3 worldPosition)
    {

        if (bounds.size == Vector3.zero && _gridRenderer != null)
        {
            if (_gridRenderer.TryGetComponent<MeshFilter>(out var filter))
            {
                bounds = filter.sharedMesh.bounds;
            }
        }
        //바닥에 있는 월드 좌표를 로컬 좌표로 가져옴
        Vector3 localPos = _gridRenderer.transform.InverseTransformPoint(worldPosition);

        float xPos = (localPos.x - bounds.min.x) / bounds.size.x;
        float zPos = (localPos.z - bounds.min.z) / bounds.size.z;

        int x = Mathf.FloorToInt(xPos * _width);
        int y = Mathf.FloorToInt(zPos * _height);

        return new Vector2Int(Mathf.Clamp(x, 0, _width - 1), Mathf.Clamp(y, 0, _height - 1));
    }

    // 셀 좌표를 실제 월드 좌표로 번역
    public Vector3 GetWorldPosition(int x, int y, int sizeX, int sizeY)
    {
        float xPos = (x + sizeX * 0.5f) / _width;
        float ZPos = (y + sizeY * 0.5f) / _height;

        float localX = bounds.min.x + (xPos * bounds.size.x);
        float localZ = bounds.min.z + (ZPos * bounds.size.z);

        Vector3 localPos = new (localX, 0, localZ);

        return _gridRenderer.transform.TransformPoint(localPos);
    }

    // 셰이더의 색을 바꿔주기 위해 
    public void UpdateShaderHover(Vector2Int index, Vector2Int size, bool canPlace)
    {
        if (_gridRenderer == null) return;

        _gridRenderer.material.SetFloat("_IsBuilding", 1f);

        _gridRenderer.material.SetVector("_Hoverinfo", new Vector4(index.x, index.y, size.x, size.y));
        _gridRenderer.material.SetFloat("_Canplace", canPlace ? 1f : 0f);
    }

    // 그리드의 상태를 텍스쳐로 업데이트하여 셰이더에게 전달 해줌
    public void UpdateGridTexture()
    {
        if (_gridDataTexture ==null ||_grid == null) return;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Color color = _grid[x, y] != null ? Color.red : Color.clear; //하나하나 전부 색을 바꿔주는 방식, 추후 개선 할 수 있으면 개선 필요
                _gridDataTexture.SetPixel(_width - 1 - x, (_height - 1) - y, color); // 1인 경우 빨간색, 0인 경우 투명색으로 설정
                //추가로 Plane에 맞춰 x,y값을 반대로 적용시켜 하이라이트가 대칭점에 나오는 문제를 수정
            }
        }
        // 텍스쳐 업데이트
        _gridDataTexture.Apply();
        _gridRenderer.material.SetTexture("_GridDataTex", _gridDataTexture); // 업데이트된 텍스쳐를 셰이더에 전달
    }

    public void ClearAllItems()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_grid[x, y] != null)
                {
                    _grid[x, y] = null;
                }
            }
        }
      
        UpdateGridTexture();
    }

    public void ClearGrid()
    {
        if (_gridRenderer == null) return;
        _gridRenderer.material.SetFloat("_IsBuilding", 0f); //만약 건물을 배치하는 중이 아니라면 셰이더에서 색 제거
        _gridRenderer.material.SetVector("_Hoverinfo", new Vector4(-100, -100, 0, 0)); // 좌표 초기화
    }
}
