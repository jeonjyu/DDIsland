using UnityEngine;
using UnityEngine.InputSystem;

public class Placeable3D : Placeable
{
    Camera _mainCamera;

    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private float _rotationStep = 90f; // 한 번 누를 때 회전할 각도
    private float _currentYRotation = 0f; // 현재 유지 중인 회전값

    void Awake()
    {
        _mainCamera = Camera.main;
    }
    public override Vector2Int ConvertedIndex()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
        {
            return _targetGrid.GetGridIndex(hit.point);
        }

        return new Vector2Int(-1, -1);
    }
    private void Update()
    {
        // 회전 입력 처리(아직 뭘 누를지 몰라서 주석으로 처리)
        //if ()
        //{
        //    _currentYRotation += _rotationStep;
        //}

        Vector2Int currentIndex = ConvertedIndex();

        if (currentIndex.x != -1)
        {
            Vector3 snapPos = _targetGrid.GetWorldPosition(currentIndex.x, currentIndex.y);

            transform.SetPositionAndRotation(snapPos, Quaternion.Euler(0, _currentYRotation, 0));
        }
    }
}
