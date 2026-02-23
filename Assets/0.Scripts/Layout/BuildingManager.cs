using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private GridSystem _gridSystem;

    private Placeable3D _activePlaceable;

    public void StartPlacement(GameObject prefab)
    {
        //현재 배치 중인 물건이 있으면 제거
        if (_activePlaceable != null)
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
        if (_activePlaceable != null && !_activePlaceable.enabled)
        {
            // 매니저의 관리 대상에서 해제
            _activePlaceable = null;
        }
        
    }
}
