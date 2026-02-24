using UnityEngine;


/// <summary>
/// 오브젝트 배치를 관리하는 클래스
/// </summary>
public class BuildingManager : MonoBehaviour
{
    // 건물의 현재 상태를 저장하는 구조체
    private struct BuildingSnapshot
    {
        public Placeable3D Target;
        public Vector2Int Pos;
        public Vector2Int Size;
        public float Rotation;
        public bool IsRotated;
    }

    [SerializeField] private GridSystem _gridSystem;

    private Placeable3D _activePlaceable;
    private BuildingSnapshot _currentSnapshot;

    #region 프로퍼티
    public Placeable3D ActivePlaceable => _activePlaceable;
    public GridSystem GridSystem => _gridSystem;
    #endregion
    public void PickUpBuilding(Placeable3D target)
    {
        _activePlaceable = target;

        // 현재 건물의 상태를 스냅샷으로 저장
        _currentSnapshot = new()
        {
            Target = target,
            Pos = target.PlacedIndex,
            Size = target.PlacedSize,
            Rotation = target.transform.eulerAngles.y,
            IsRotated = target.IsRotated
        };

        RemoveBuildingFromGrid(target); // 그리드에서 해당 건물 제거

        target.ItemState = ItemState.Preview;
        target.enabled = true; // 다시 Update와 VisualFeedback이 작동하게 함
    }
    public void DeleteBuilding(Placeable3D target)
    {
        if (target == null) return;
        RemoveBuildingFromGrid(target);
        Destroy(target.gameObject);
    }
    private void RemoveBuildingFromGrid(Placeable3D target)
    {
        _gridSystem.RemoveItem(target.PlacedIndex.x, target.PlacedIndex.y,
                               target.PlacedSize.x, target.PlacedSize.y);
    }

    public void StartPlacement(GameObject prefab)
    {

        //현재 배치 중인 물건이 있으면 제거
        if (_activePlaceable != null && _activePlaceable.ItemState == ItemState.Preview)
        {
            Destroy(_activePlaceable.gameObject);
        }
        //배치할 물건 등록
        GameObject go = Instantiate(prefab);

        if (go.TryGetComponent(out _activePlaceable))
        {
            _currentSnapshot = new() { Pos = new Vector2Int(-1, -1) };

            _activePlaceable.Initialize(_gridSystem); //배치할 물건 초기화
        }
        else
        {
            Debug.LogError($"{prefab.name}에 스크립트가 없습니다!");
        }
    }
    public void CancelCurrentAction()
    {
        if (_activePlaceable == null) return;

        //만약 기존 위치가 -1(배치 안됨)이라면 그냥 제거
        if (_currentSnapshot.Pos.x == -1)
        {
            Destroy(_activePlaceable.gameObject);
        }
        else
        {
            //이전 위치를 저장했을테니 그 위치로 복구
            _gridSystem.PlaceItem(_currentSnapshot.Pos.x, _currentSnapshot.Pos.y,
                                  _currentSnapshot.Size.x, _currentSnapshot.Size.y);

            Vector3 originWorldPos = _gridSystem.GetWorldPosition(
                _currentSnapshot.Pos.x, _currentSnapshot.Pos.y,
                _currentSnapshot.Size.x, _currentSnapshot.Size.y);

            _activePlaceable.RestoreState(originWorldPos, _currentSnapshot.Rotation, _currentSnapshot.IsRotated);
        }

        _activePlaceable = null;
    }
    private void Update()
    {
        //배치 가능한 물건이 배치되었으면
        if (_activePlaceable != null && _activePlaceable.ItemState == ItemState.Placed)
        {
            // 매니저의 관리 대상에서 해제
            _activePlaceable = null;
        }

    }
}
