using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UpgradeClickHandler : MonoBehaviour
{
    [Header("참조")]
    public PanelHandler popupWindow;
    //public UpgradeManager upgradeManager;
    public UpgradeManagerV2 upgradeManager;
    public GameObject playerCharacter;

    [Header("설정")]
    public float rayDistance = 100f;

    /// <summary>
    /// 실제 렌더링 중인 카메라를 찾는다.
    /// Camera.main이 없거나 엉뚱한 카메라일수 있으므로
    /// depth가 가장 높은 활성화된 카메라를 사용한다.
    /// </summary>
    Camera GetActiveCamera()
    {
        Camera best = null;
        float bestDepth = float.MinValue;

        foreach (var cam in Camera.allCameras)
        {
            if (cam.enabled && cam.depth > bestDepth)
            {
                bestDepth = cam.depth;
                best = cam;
            }
        }
        return best;
    }

    void Update()
    {
        if (Mouse.current == null || Mouse.current.leftButton.wasPressedThisFrame == false) return;

        // ui 클릭 방지
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Camera cam = GetActiveCamera();
        if (cam == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance) == false) return;

        // pc인가 
        if (IsPlayerCharacter(hit.collider.gameObject) == false) return;

        popupWindow.Show();
        upgradeManager.OnPanelOpened();
    }

    bool IsPlayerCharacter(GameObject hitObject)
    {
        if (playerCharacter == null) return false;

        Transform current = hitObject.transform;
        while (current != null)
        {
            if (current.gameObject == playerCharacter)
                return true;
            current = current.parent;
        }
        return false;
    }
}