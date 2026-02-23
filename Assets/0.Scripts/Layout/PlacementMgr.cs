using UnityEngine;
using UnityEngine.InputSystem;

public enum PlacementState
{
    View,
    Edit
}


public class PlacementMgr : MonoBehaviour
{
    private PlacementMgr _instance;
    private BuildingManager _buildingManager;
    private Placeable3D _selectedTarget; // 현재 선택된 건물

    [SerializeField] private GameObject _editMenuUI; // 현재 선택된 건물의 편집 UI

    public PlacementMgr Instance => _instance;
    public PlacementState CurrentState { get; private set; } = PlacementState.View;

    private void Awake()
    {
        _instance = this;
    }
    public void ToggleEditMode()
    {
        CurrentState = (CurrentState == PlacementState.View) ? PlacementState.Edit : PlacementState.View;
        Debug.Log($"현재 모드: {CurrentState}");

        if (CurrentState == PlacementState.View)
        {
            //_buildingManager.CancelCurrentAction(); // 모드 나갈 때 들고 있던 거 취소
        }
    }

    private void OnClickInput(InputAction.CallbackContext ctx)
    {
        if (CurrentState != PlacementState.Edit) return;

        //if (_buildingManager.ActivePlaceable != null) return;

        TrySelectBuildingForMove();
    }

    private void TrySelectBuildingForMove()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Placeable3D target = hit.collider.GetComponentInParent<Placeable3D>();

            if (target != null && target.ItemState == ItemState.Placed)
            {
                _buildingManager.PickUpBuilding(target);
            }
        }
    }
    private void ShowEditMenu(Placeable3D target)
    {
        _editMenuUI.SetActive(true);
        // UI 위치를 선택한 건물 위치로 이동 (약간 위로 띄움)
        _editMenuUI.transform.position = target.transform.position + Vector3.up * 3f;
    }

    public void CloseEditMenu()
    {
        _selectedTarget = null;
        _editMenuUI.SetActive(false);
    }
    public void OnClickMove() // '이동' 버튼에 연결
    {
        if (_selectedTarget == null) return;
        _buildingManager.PickUpBuilding(_selectedTarget);
        CloseEditMenu();
    }
    public void OnClickRotate() // '회전' 버튼에 연결
    {
        if (_selectedTarget == null) return;
        // Placeable3D에 정의된 회전 로직 호출 (90도 회전 등)
        // _selectedTarget.RotateOnce(); 
    }
    public void OnClickDelete() // '삭제' 버튼에 연결
    {
        if (_selectedTarget == null) return;
        // 그리드 데이터 지우고 파괴
        // _targetGrid.RemoveItem(...);
        Destroy(_selectedTarget.gameObject);
        CloseEditMenu();
    }
}
