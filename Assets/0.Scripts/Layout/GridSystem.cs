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

    #region 프로퍼티
    public int Width => _width;   
    public int Height => _height;
    #endregion

    private void Awake()
    {
        _grid = new Placeable[_width, _height];

        _cellSize = (_cell.localScale.x * 10f) / _width; //셀의 크기를 Transform의 스케일에서 가져옴
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
        if (startX < 0 || startY < 0 || startX + itemWidth > _width || startY + itemHeight > _height)
        {
            return false;
        }
        for (int i = 0; i < itemWidth; i++)
        {
            for (int j = 0; j < itemHeight; j++)
            {
                int checkX = startX + i;
                int checkY = startY + j;

                if (_grid[checkX, checkY] != null && _grid[checkX, checkY] != self)
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
        if (startX < 0 || startY < 0 || startX + itemWidth > _width || startY + itemHeight > _height)
        {
            return;
        }
        for (int i = 0; i < itemWidth; i++)
        {
            for (int j = 0; j < itemHeight; j++)
            {
                _grid[startX + i, startY + j] = item; // 해당 셀이 채워졌음을 나타냄
            }
        }
        UpdateGridTexture();
    }
    public void RemoveItem(int startX, int startY, int itemWidth, int itemHeight)
    {
        if (startX < 0 || startY < 0 || startX + itemWidth > _width || startY + itemHeight > _height)
        {
            return;
        }
        for (int i = 0; i < itemWidth; i++)
        {
            for (int j = 0; j < itemHeight; j++)
            {
                _grid[startX + i, startY + j] = null; // 해당 셀을 비움
            }
        }
        UpdateGridTexture();
    }
    // 월드 좌표를 셀 좌표로 번역
    public Vector2Int GetGridIndex(Vector3 worldPosition)
    {
        float halfWidth = (_width * _cellSize) * 0.5f;
        float halfHeight = (_height * _cellSize) * 0.5f;
        Vector3 origin = _cell.position - new Vector3(halfWidth, 0, halfHeight);

        // worldPosition이 셀의 중심으로부터 떨어진 거리를 계산
        float diffX = worldPosition.x - origin.x;
        float diffZ = worldPosition.z - origin.z;

        // 위에서 계산한 거리를 셀의 크기로 나누어 몇 번째 셀에 해당되는지 계산
        int x = Mathf.FloorToInt(diffX / _cellSize);
        int y = Mathf.FloorToInt(diffZ / _cellSize);

        // 소수점 버림
        return new Vector2Int(Mathf.Clamp(x, 0, _width - 1), Mathf.Clamp(y, 0, _height - 1));
    }

    // 셀 좌표를 실제 월드 좌표로 번역
    public Vector3 GetWorldPosition(int x, int y, int sizeX, int sizeY)
    {
        float halfWidth = (_width * _cellSize) * 0.5f;
        float halfHeight = (_height * _cellSize) * 0.5f;
        Vector3 origin = _cell.position - new Vector3(halfWidth, 0, halfHeight);
        // 셀의 좌표를 실제 월드 좌표로 전환

        float centerX = x + (sizeX * 0.5f);
        float centerY = y + (sizeY * 0.5f);

        return new Vector3
        (
            origin.x + (centerX * _cellSize),
            _cell.position.y,
            origin.z + (centerY * _cellSize)
        );
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
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Color color = _grid[x, y] != null ? Color.red : new Color(0, 0, 0, 0); //하나하나 전부 색을 바꿔주는 방식, 추후 개선 할 수 있으면 개선 필요
                _gridDataTexture.SetPixel(x, y, color); // 1인 경우 빨간색, 0인 경우 투명색으로 설정
            }
        }
        // 텍스쳐 업데이트
        _gridDataTexture.Apply();
        _gridRenderer.material.SetTexture("_GridDataTex", _gridDataTexture); // 업데이트된 텍스쳐를 셰이더에 전달
    }

    public void ClearGrid()
    {
        if (_gridRenderer == null) return;
        _gridRenderer.material.SetFloat("_IsBuilding", 0f); //만약 건물을 배치하는 중이 아니라면 셰이더에서 색 제거
        _gridRenderer.material.SetVector("_Hoverinfo", new Vector4(-10, -10, 0, 0)); // 좌표 초기화
    }
}
