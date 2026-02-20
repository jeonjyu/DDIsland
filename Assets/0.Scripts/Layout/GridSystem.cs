using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private int _width = 12; //실제로 가지는 셀의 수
    [SerializeField] private int _height = 12;

    [SerializeField] private Transform _cell; //셀의 크기를 결정하는 Transform, 이 크기에 맞춰 셀의 실제 크기를 계산

    private int[,] _grid; //셀의 상태를 나타내는 2차원 배열, 0은 빈 셀, 1은 채워진 셀

    private float _cellSize; //셀의 실제 크기

    private void Awake()
    {
        _grid = new int[_width, _height];

        _cellSize = _cell.localScale.x; //셀의 크기를 Transform의 스케일에서 가져옴
    }
    public bool IsCellEmpty(int startX, int startY, int itemWidth, int itemHeight)
    {
        if(startX<0 || startY < 0 || startX + itemWidth > _width || startY + itemHeight > _height)
        {
            return false;
        }
        for (int i = 0; i<itemWidth; i++ )
        {
            for(int j = 0; j < itemHeight; j++)
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
        if(startX<0 || startY < 0 || startX + itemWidth > _width || startY + itemHeight > _height)
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
    }
    // 간단하게 말해서 셀 좌표 번역기임
    public Vector2Int GetGridIndex(Vector3 worldPosition)
    {
        // worldPosition이 셀의 중심으로부터 떨어진 거리를 계산
        float diffX = worldPosition.x - _cell.position.x;
        float diffZ = worldPosition.z - _cell.position.z;

        // 위에서 계산한 거리를 셀의 크기로 나누어 몇 번째 셀에 해당되는지 계산
        int x = Mathf.FloorToInt(diffX / _cellSize);
        int y = Mathf.FloorToInt(diffZ / _cellSize);

        // 소수점 버림
        return new Vector2Int(x, y);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        // 셀의 좌표를 실제 월드 좌표로 전환
        return new Vector3(
            _cell.position.x + (x * _cellSize) + (_cellSize * 0.5f),
            _cell.position.y,
            _cell.position.z + (y * _cellSize) + (_cellSize * 0.5f)
        );
    }
}
