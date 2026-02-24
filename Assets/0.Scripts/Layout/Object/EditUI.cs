using UnityEngine;

public class EditUI : MonoBehaviour
{
    [SerializeField] private GameObject _ui;
    [SerializeField] private float _size;
    private Transform _mainCameraTransform;
    void Start()
    {
        if (Camera.main != null)
        {
            _mainCameraTransform = Camera.main.transform;
        }
    }
    public void CallMove() => PlacementMgr.Instance.OnClickMove();
    public void CallRotate() => PlacementMgr.Instance.OnClickRotate();
    public void CallDelete() => PlacementMgr.Instance.OnClickDelete();

    private void LateUpdate()
    {
        if (_mainCameraTransform == null)
        {
            return;
        }
        _ui.transform.rotation = _mainCameraTransform.rotation;


        float sizeFactor = _size;
        _ui.transform.localScale = Vector3.one * sizeFactor;
    }
}
