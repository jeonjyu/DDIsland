using UnityEngine;

// 박스 오브젝트에 붙이기
public class UpgradeClickHandler : MonoBehaviour
{
    public PanelHandler popupWindow;
    public UpgradeManager upgradeManager;

    void OnMouseDown() // 콜라이더 클릭 감지
    {
        popupWindow.Show();
        upgradeManager.OnPanelOpened();
    }
}