using System;
using System.Collections.Generic;
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
        public int IsRotated;
    }
    [SerializeField] private GridSystem _gridSystem;

    private Placeable3D _activePlaceable;
    private BuildingSnapshot _currentSnapshot;

    [Header("오브젝트 저장 담당")]
    private List<Placeable3D> _newBuildings = new(); // 새로 생긴 건물 담당
    private Dictionary<Placeable3D, BuildingSnapshot> _movedSnapshots = new();
    // 건물을 이동시켰을 때 원복시키기 위한 딕셔너리
    private List<Placeable3D> _deletedBuildings = new(); // 삭제 취소용

    #region 배치 결과 이벤트
    public event Action<GameObject> OnPlaceSuccess; // 배치 성공
    public event Action<GameObject> OnPlaceCancel; // 배치 실패 
    public event Action OnConfirm;   // 저장 
    public event Action OnRevert;    // 전체회수 
    public event Action OnClearAll;  // 초기화 
    #endregion
    #region 프로퍼티
    public Placeable3D ActivePlaceable => _activePlaceable;
    public GridSystem GridSystem => _gridSystem;
    #endregion


    public void PickUpBuilding(Placeable3D target)
    {
        // 편집모드 딱 들어왔을 때 저장한 값이 있다면 (기존 건물)
        // 그리고 이번 편집 모드에 새로 만든게 아니라면
        if (!_newBuildings.Contains(target) && !_movedSnapshots.ContainsKey(target))
        {
            _movedSnapshots.Add(target, new BuildingSnapshot
            {
                Target = target,
                Pos = target.PlacedIndex,
                Size = target.PlacedSize,
                Rotation = target.transform.eulerAngles.y,
                IsRotated = target.IsRotated
            });
        }

        _activePlaceable = target;

        // 현재 건물의 상태를 임시적으로 저장 (편집모드의 이동중 취소가 가능하게)
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
        if (target == null)
        {
            return;
        }

        _gridSystem.ClearGrid();
        // 그리드에서 삭제
        RemoveBuildingFromGrid(target);
        // 먼저 삭제하지 않고 나중에 되돌리기 될 수 있기 때문에 비활성화 처리
        target.gameObject.SetActive(false);
        // 삭제 대기열에 추가
        _deletedBuildings.Add(target);
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
            _newBuildings.Remove(_activePlaceable);
            Destroy(_activePlaceable.gameObject);
        }
        //배치할 물건 등록
        GameObject go = Instantiate(prefab, _gridSystem.transform);

        if (go.TryGetComponent(out _activePlaceable))
        {
            _currentSnapshot = new() { Pos = new Vector2Int(-1, -1) };

            _activePlaceable.Initialize(_gridSystem, this); //배치할 물건 초기화
        }

        else
        {
            Debug.LogError($"{prefab.name}에 스크립트가 없습니다!");
        }

        _newBuildings.Add(_activePlaceable);
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
    public void ConfirmAll()
    {
        // 삭제 확정 코드
        foreach (var b in _deletedBuildings)
        {
            if (b != null)
            {
                Destroy(b.gameObject);
            }
        }
        OnConfirm?.Invoke(); // 저장 알림 
        ClearSession();
    }

    private void ClearSession()
    {
        _newBuildings.Clear();
        _movedSnapshots.Clear();
        _deletedBuildings.Clear();
        _activePlaceable = null;
    }

    public void RevertAll()
    {
        // 새로 만든 건물들 전부 삭제
        foreach (var b in _newBuildings)
        {
            if (b != null)
            {
                RemoveBuildingFromGrid(b);
                Destroy(b.gameObject);
            }
        }


        // 이동 시켰던거 원래대로 되돌려 놓기
        foreach (var snap in _movedSnapshots.Values)
        {
            if (snap.Target != null)
            {

                RemoveBuildingFromGrid(snap.Target);

                _gridSystem.PlaceItem(snap.Pos.x, snap.Pos.y, snap.Size.x, snap.Size.y, snap.Target);

                Vector3 worldPos = _gridSystem.GetWorldPosition(snap.Pos.x, snap.Pos.y, snap.Size.x, snap.Size.y);
                snap.Target.RestoreState(worldPos, snap.Rotation, snap.IsRotated);
            }
        }
        // 삭제한 건물 부활 시키기
        foreach (var b in _deletedBuildings)
        {
            b.gameObject.SetActive(true);
            _gridSystem.PlaceItem(b.PlacedIndex.x, b.PlacedIndex.y, b.PlacedSize.x, b.PlacedSize.y, b);
        }

        ClearSession();
        OnRevert?.Invoke(); // 전체 회수 알림 
        _activePlaceable = null;
    }

    public void ClearAll()
    {
        if (_activePlaceable != null)
        {
            Destroy(_activePlaceable.gameObject);
        }
        //foreach (Transform child in _gridSystem.transform)
        //{
        //    Destroy(child.gameObject);
        //}

        _gridSystem.ClearAllItems();
        _gridSystem.ClearGrid();

        OnClearAll?.Invoke(); // 초기화 알림
        ClearSession();
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