using UnityEngine;
using System;

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
        public int IsRotated;
    }

    [SerializeField] private GridSystem _gridSystem;

    private Placeable3D _activePlaceable;
    private BuildingSnapshot _currentSnapshot;

    #region 배치 결과 이벤트
    public event Action<GameObject> OnPlaceSuccess;
    public event Action<GameObject> OnPlaceCancel;
    #endregion
    #region 프로퍼티
    public Placeable3D ActivePlaceable => _activePlaceable;
    public GridSystem GridSystem => _gridSystem;
    #endregion
    public void PickUpBuilding(Placeable3D target)
    {
        _activePlaceable = target;

        // 현재 건물의 상태를 저장
        _currentSnapshot = new()
        {
            Target = target,
            Pos = target.PlacedIndex,
            Size = target.PlacedSize,
            Rotation = target.transform.eulerAngles.y,
            IsRotated = target.IsRotated
        };

        RemoveBuildingFromGrid(target); // 그리드에서 해당 건물 제거

        target.ToggleUI(false);

        target.ItemState = ItemState.Preview;
    }
    // 오브젝트(건물) 제거
    public void DeleteBuilding(Placeable3D target)
    {
        if (target == null) return;
        _gridSystem.ClearGrid();
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

        //현재 배치 중인 물건이 있으면 들고 있는 물체를 제거
        if (_activePlaceable != null && _activePlaceable.ItemState == ItemState.Preview)
        {
            Destroy(_activePlaceable.gameObject);
        }
        //배치할 물건 등록
        GameObject go = Instantiate(prefab);

        if (go.TryGetComponent(out _activePlaceable))
        {
            _currentSnapshot = new() { Pos = new Vector2Int(-1, -1) };

            _activePlaceable.Initialize(_gridSystem,this); //배치할 물건 초기화
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
            //배치된 물체를 이동할 경우
            //이전 위치를 저장했을테니 그 위치로 복구
            _gridSystem.PlaceItem(_currentSnapshot.Pos.x, _currentSnapshot.Pos.y,
                                  _currentSnapshot.Size.x, _currentSnapshot.Size.y, _activePlaceable);

            Vector3 originWorldPos = _gridSystem.GetWorldPosition(
                _currentSnapshot.Pos.x, _currentSnapshot.Pos.y,
                _currentSnapshot.Size.x, _currentSnapshot.Size.y);

            _activePlaceable.RestoreState(originWorldPos, _currentSnapshot.Rotation, _currentSnapshot.IsRotated);
        }

        OnPlaceCancel?.Invoke(null); // 배치 취소 알림 
        _activePlaceable = null;
    }
    public void ClearAll()
    {
        if (_activePlaceable != null && _activePlaceable.ItemState == ItemState.Preview)
        {
            Destroy(_activePlaceable.gameObject);
            _activePlaceable = null;
        }

        _gridSystem.ClearAllItems();

        _gridSystem.ClearGrid();
    }
    private void Update()
    {
        //배치 가능한 물건이 배치되었으면
        if (_activePlaceable != null && _activePlaceable.ItemState == ItemState.Placed)
        {
            OnPlaceSuccess?.Invoke(_activePlaceable.gameObject); // 배치 성공 알림
            // 매니저의 관리 대상에서 해제
            _activePlaceable = null;
        }

    }
}
