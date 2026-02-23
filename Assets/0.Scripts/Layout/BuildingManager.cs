using UnityEngine;

/// <summary>
/// 오브젝트 배치를 관리하는 클래스
/// </summary>
public class BuildingManager : MonoBehaviour
{
    [SerializeField] private GridSystem _gridSystem;

    private Placeable3D _activePlaceable;
    public void PickUpBuilding(Placeable3D target)
    {
        _activePlaceable = target;

        // 중요: 그리드 데이터에서 일단 지워야 이동 중에 '자리가 차 있다'고 안 뜹니다.
        //_gridSystem.RemoveItem(target.PlacedIndex.x, target.PlacedIndex.y,
                              //target.PlacedSize.x, target.PlacedSize.y);

        target.enabled = true; // 다시 Update와 VisualFeedback이 작동하게 함
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
            _activePlaceable.Initialize(_gridSystem); //배치할 물건 초기화
        }
        else
        {
            Debug.LogError($"{prefab.name}에 스크립트가 없습니다!");
        }
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
