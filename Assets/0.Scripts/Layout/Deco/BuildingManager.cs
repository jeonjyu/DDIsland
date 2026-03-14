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
    private Dictionary<Placeable3D, BuildingSnapshot> _movedSnapshots = new();
    // 건물을 이동시켰을 때 원복시키기 위한 딕셔너리
    private List<Placeable3D> _deletedBuildings = new(); // 삭제 취소용
    //건물 담당
    private List<Placeable3D> _activeBuildings = new();
    private List<Placeable3D> _allBuildings = new();

    #region 배치 결과 이벤트
    public event Action<GameObject> OnPlaceSuccess; // 배치 성공
    public event Action<GameObject> OnPlaceCancel; // 배치 실패 
    public event Action OnConfirm;   // 저장 
    public event Action OnRevert;    // 전체회수 
    public event Action<List<int>> OnClearAll;  // 초기화 
    #endregion
    #region 프로퍼티
    public Placeable3D ActivePlaceable => _activePlaceable;
    public GridSystem GridSystem => _gridSystem;
    #endregion

    #region 단일 건물 편집
    public void PickUpBuilding(Placeable3D target)
    {
        // 편집모드 딱 들어왔을 때 저장한 값이 있다면 (기존 건물)
        // 그리고 이번 편집 모드에 새로 만든게 아니라면
        if (!_activeBuildings.Contains(target) && !_movedSnapshots.ContainsKey(target))
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

        // 이동 시작 시 배치되어 있는 곳에서는 제거
        if (_activeBuildings.Contains(target))
        {
            _activeBuildings.Remove(target);
        }

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

    public void CompletePlacement(Placeable3D target)
    {
        if (target == null) return;

        if (!_activeBuildings.Contains(target))
        {
            _activeBuildings.Add(target);
        }

        if (_activePlaceable == target)
        {
            OnPlaceSuccess?.Invoke(target.gameObject);
            _activePlaceable = null;
        }
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


        if (_allBuildings.Contains(target) && !_movedSnapshots.ContainsKey(target))
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

        // 먼저 삭제하지 않고 나중에 되돌리기 될 수 있기 때문에 비활성화 처리
        target.gameObject.SetActive(false);

        // 삭제 대기열에 추가
        _deletedBuildings.Add(target);

        // 설치 되어 있는걸 지웠으니 일단 관리 목록에서 제거
        if (_activeBuildings.Contains(target))
        {
            _activeBuildings.Remove(target);
        }
        if (_allBuildings.Contains(target))
        {
            _allBuildings.Remove(target);
        }
    }
    private void RemoveBuildingFromGrid(Placeable3D target)
    {
        _gridSystem.RemoveItem(target.PlacedIndex.x, target.PlacedIndex.y,
                               target.PlacedSize.x, target.PlacedSize.y);
    }

    public void StartPlacement(int itemId)
    {
        var obj = DataManager.Instance.DecorationDatabase.InteriorData[itemId];
        
        //현재 배치 중인 물건이 있으면 들고 있는 물체를 제거
        if (_activePlaceable != null && _activePlaceable.ItemState == ItemState.Preview)
        {
            _activeBuildings.Remove(_activePlaceable);
            Destroy(_activePlaceable.gameObject);
        }
        //배치할 물건 등록
        GameObject go = Instantiate(obj.InteriorPath_GameObject, _gridSystem.transform);


        if (!go.TryGetComponent(out _activePlaceable))
        {
            _activePlaceable = go.AddComponent<Placeable3D>();
        }

            _currentSnapshot = new() { Pos = new Vector2Int(-1, -1) };

            // 인테리어 데이터 SO 추가해야함
            _activePlaceable.Initialize(_gridSystem, this,obj); //배치할 물건 초기화
        if(!go.TryGetComponent(out _activePlaceable))
        {
            Debug.LogError($"{go.name}에 스크립트가 없습니다!");
            Destroy(go);
        }

        //건물 관리용
        _activeBuildings.Add(_activePlaceable);
    }
    public void CancelCurrentAction()
    {
        if (_activePlaceable == null) return;

        //만약 기존 위치가 -1(배치 안됨)이라면 그냥 제거
        if (_currentSnapshot.Pos.x == -1)
        {
            _activeBuildings.Remove(_activePlaceable);
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

            // 이동을 취소 했을 경우 다시 배치된 건물 관리 목록에 추가
            if (!_activeBuildings.Contains(_activePlaceable))
            {
                _activeBuildings.Add(_activePlaceable);
            }

            _activePlaceable.RestoreState(originWorldPos, _currentSnapshot.Rotation, _currentSnapshot.IsRotated);

        }

            _gridSystem.ClearGrid();
        OnPlaceCancel?.Invoke(null); // 배치 취소 알림 
        _activePlaceable = null;
    }
    #endregion

    #region 전체 건물 편집
    public void ConfirmAll()
    {
        // 저장이 완료된 건물을 옮기기
        foreach (var b in _activeBuildings)
        {
            if (!_allBuildings.Contains(b)) _allBuildings.Add(b);
        }
        // 삭제 확정 코드
        foreach (var b in _deletedBuildings)
        {
            if (b != null)
            {
                Destroy(b.gameObject);
            }
        }
        _activeBuildings.Clear();

        DataManager.Instance.Hub.SaveAllData();

        OnConfirm?.Invoke(); // 저장 알림 
        ClearSession();
    }

    private void ClearSession()
    {
        _movedSnapshots.Clear();
        _deletedBuildings.Clear();
        _activePlaceable = null;
    }

    public void RevertAll()
    {
        // 새로 만든 건물들 전부 삭제
        for (int i = _activeBuildings.Count - 1; i >= 0; i--)
        {
            var b = _activeBuildings[i];
            if (b != null)
            {
                if (!_movedSnapshots.ContainsKey(b))
                {
                    RemoveBuildingFromGrid(b);
                    _activeBuildings.RemoveAt(i);
                    Destroy(b.gameObject);
                }
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

                if (!_allBuildings.Contains(snap.Target)) _allBuildings.Add(snap.Target);
            }
        
        }

        // 삭제한 건물 부활 시키기
        foreach (var b in _deletedBuildings)
        {
            if(b!=null)
            {
                b.gameObject.SetActive(true);
                _gridSystem.PlaceItem(b.PlacedIndex.x, b.PlacedIndex.y, b.PlacedSize.x, b.PlacedSize.y, b);
                if (_movedSnapshots.ContainsKey(b))
                {
                    if (!_allBuildings.Contains(b)) _allBuildings.Add(b);
                }
                else
                {
                    if (!_activeBuildings.Contains(b)) _activeBuildings.Add(b);
                }
            }

        }
        if (_activePlaceable != null)
        {
            if (!_movedSnapshots.ContainsKey(_activePlaceable))
            {
                Destroy(_activePlaceable.gameObject);
            }
        }

        ClearSession();

        OnRevert?.Invoke(); // 전체 회수 알림 
        _activePlaceable = null;
    }

    public void ClearAll()
    {
        List<int> destroyedIds = new(); // 삭제된 아이템 ID를 추적         
        HashSet<Placeable3D> processed = new(); // 이미 처리한 건물 중복 방지          

        if (_activePlaceable != null)
        {
            processed.Add(_activePlaceable);              
            int id = _activePlaceable.GetItemId();        
            if (id >= 0) destroyedIds.Add(id);            

            Destroy(_activePlaceable.gameObject);
            _activePlaceable = null;
        }

        for (int i = _activeBuildings.Count - 1; i >= 0; i--)
        {
            var b = _activeBuildings[i];
            if (b != null && !processed.Contains(b))
            {
                processed.Add(b);                         
                int id = b.GetItemId();                   
                if (id >= 0) destroyedIds.Add(id);        

                RemoveBuildingFromGrid(b);
                Destroy(b.gameObject);
            }
        }

        for (int i = _allBuildings.Count - 1; i >= 0; i--)
        {

            var b = _allBuildings[i];
            if (b != null && !processed.Contains(b))
            {
                if (!b.IsEditable) continue;

                processed.Add(b);                          
                int id = b.GetItemId();                    
                if (id >= 0) destroyedIds.Add(id);         

                RemoveBuildingFromGrid(b);
                // Destroy(b.gameObject);
                b.gameObject.SetActive(false); // 파괴 대신 비활성화
                _deletedBuildings.Add(b);      // 복원할 수 있게 보관
            }
        }
        _activeBuildings.Clear();
        _allBuildings.Clear();

        foreach (var b in _deletedBuildings)
        {
            if (b != null && !processed.Contains(b))
            {
                processed.Add(b);                          
                int id = b.GetItemId();                    
                if (id >= 0) destroyedIds.Add(id);         

                Destroy(b.gameObject);
            }
        }

        _gridSystem.ClearGrid();

        //SyncDataClear();

        //ClearSession(); // _deletedBuildings 까지 비우지는 않고 직접 정리
        _movedSnapshots.Clear();          
        _activePlaceable = null;

        OnClearAll?.Invoke(destroyedIds);  // 초기화 알림
    }
    #endregion

    #region 파이어베이스 데이터 저장
    private void OnEnable()
    {
        DataManager.Instance.Hub.OnRequestSave += SyncBuildingDataSave;
    }
    private void OnDisable()
    {
        DataManager.Instance.Hub.OnRequestSave -= SyncBuildingDataSave;
    }
    private void SyncBuildingDataSave()
    {
        List<PlacedObject> syncList = new ();

        foreach (var b in _allBuildings)
        {
            if (b == null) continue;

            syncList.Add(new PlacedObject
            {
                _id = b.GetItemId(),
                _posX = b.PlacedIndex.x,
                _posY = b.PlacedIndex.y,
                _rotation = b.IsRotated

            });
        }

        DataManager.Instance.Hub._allUserData.Decoration._buildings = syncList;

        Debug.Log("<color=green>모든 배치 정보가 서버와 동기화되었습니다!</color>");
    }
    public void SyncDataLoad()
    {
        var savedData = DataManager.Instance.Hub._allUserData.Decoration._buildings;
        if (savedData == null || savedData.Count == 0) return;

        var database = DataManager.Instance.DecorationDatabase;

        foreach (var data in savedData)
        {
            InteriorDataSO interiorData;
            try
            {
                interiorData = database.InteriorData[data._id];
            }
            catch
            {
                Debug.LogError($"ID {data._id}를 찾을 수 없어 로드에 실패했습니다.");
                continue;
            }
            if (interiorData == null) continue;
            GameObject go = Instantiate(interiorData.InteriorPath_GameObject, _gridSystem.transform);
            if (!go.TryGetComponent(out Placeable3D placeable))
            {
                placeable = go.AddComponent<Placeable3D>();
            }

            placeable.Initialize(_gridSystem, this, interiorData);

            Vector3 worldPos = _gridSystem.GetWorldPosition(data._posX, data._posY, interiorData.GridSizeX, interiorData.GridSizeY);
            placeable.RestoreState(worldPos, data._rotation * 90f, data._rotation);

            Vector2Int size = (data._rotation % 2 == 1) ?
                new Vector2Int(interiorData.GridSizeY, interiorData.GridSizeX) :
                new Vector2Int(interiorData.GridSizeX, interiorData.GridSizeY);

            if (_gridSystem.IsCellEmpty(data._posX, data._posY, size.x, size.y, placeable))
            {
                _gridSystem.PlaceItem(data._posX, data._posY, size.x, size.y, placeable);

                if (!_allBuildings.Contains(placeable)) _allBuildings.Add(placeable);

                placeable.SetBakeData(new Vector2Int(data._posX, data._posY), size);
            }
            else
            {
                Debug.LogWarning($"{interiorData.InteriorName_String}: 해당 위치에 이미 물체가 있습니다.");
                Destroy(go);
            }
        }
    }
    #endregion
}