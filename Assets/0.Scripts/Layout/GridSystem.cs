using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private int _width = 12; //실제로 가지는 셀의 수
    [SerializeField] private int _height = 12;

    [Header("Visual Settings")]
    [SerializeField] private Transform _cell; //셀의 크기를 결정하는 Transform, 이 크기에 맞춰 셀의 실제 크기를 계산
    [SerializeField] private MeshRenderer _gridRenderer; //렌더러 컴포넌트

    private Texture2D _gridDataTexture;

    private int[,] _grid; //셀의 상태를 나타내는 2차원 배열, 0은 빈 셀, 1은 채워진 셀

    private float _cellSize; //셀의 실제 크기

    public int Width => _width;   
    public int Height => _height;

    private void Awake()
    {
        _grid = new int[_width, _height];

        _cellSize = (_cell.localScale.x * 10f) / _width; //셀의 크기를 Transform의 스케일에서 가져옴
    }
    private void Start()
    {
        _gridDataTexture = new(_width, _height)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        ApplyGridToShader();
    }
    public void ApplyGridToShader()
    {
        if (_gridRenderer != null)
        {
            _gridRenderer.material.SetVector("_Gridsize", new Vector2(_width, _height));
        }
    }
    public bool IsCellEmpty(int startX, int startY, int itemWidth, int itemHeight)
    {
        if (startX < 0 || startY < 0 || startX + itemWidth > _width || startY + itemHeight > _height)
        {
            return false;
        }
        for (int i = 0; i < itemWidth; i++)
        {
            for (int j = 0; j < itemHeight; j++)
            {
                if (_grid[startX + i, startY + j] != 0)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void PlaceItem(int startX, int startY, int itemWidth, int itemHeight)
    {
        if (startX < 0 || startY < 0 || startX + itemWidth > _width || startY + itemHeight > _height)
        {
            return;
        }
        for (int i = 0; i < itemWidth; i++)
        {
            for (int j = 0; j < itemHeight; j++)
            {
                _grid[startX + i, startY + j] = 1;
            }
        }
        UpdateGridTexture();
    }
    // 간단하게 말해서 셀 좌표 번역기임
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

    public void UpdateShaderHover(Vector2Int index, Vector2Int size, bool canPlace)
    {
        if (_gridRenderer == null) return;

        _gridRenderer.material.SetFloat("_IsBuilding", 1f);

        _gridRenderer.material.SetVector("_Hoverinfo", new Vector4(index.x, index.y, size.x, size.y));
        _gridRenderer.material.SetFloat("_Canplace", canPlace ? 1f : 0f);
    }

    public void UpdateGridTexture()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Color color = _grid[x, y] == 1 ? Color.red : new Color(0, 0, 0, 0);
                _gridDataTexture.SetPixel(x, y, color);
            }
        }
        _gridDataTexture.Apply();
        _gridRenderer.material.SetTexture("_GridDataTex", _gridDataTexture);
    }

    public void ClearGrid()
    {
        if (_gridRenderer == null) return;
        _gridRenderer.material.SetFloat("_IsBuilding", 0f); // 하이라이트 끄기
        _gridRenderer.material.SetVector("_Hoverinfo", new Vector4(-10, -10, 0, 0)); // 좌표 초기화
    }
}
