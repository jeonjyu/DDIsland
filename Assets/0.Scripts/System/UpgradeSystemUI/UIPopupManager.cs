using UnityEngine;

/// <summary>
/// UI 팝업창들의 상태를 감시, 하나라도 열려있으면 IslandWindow(테두리)를 비활성화
/// 확장 시 popupPanels 배열에 팝업창 GameObject만 추가
/// 현재 업그레이드 창은 이 배열을 받아가서 안열리게 리턴시키는 구조 
/// </summary>
public class UIPopupManager : MonoBehaviour
{
    public static UIPopupManager Instance { get; private set; }

    [Header("감시할 팝업창들")]
    [Tooltip("하나라도 활성화되면 IslandWindow가 비활성화됨")]
    [SerializeField] private GameObject[] popupPanels;

    [Header("팝업 열릴 때 비활성화할 대상")]
    [SerializeField] private GameObject islandWindow;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (islandWindow == null) return;

        bool anyOpen = IsAnyPopupOpen();

        // 상태가 바뀔 때만 SetActive 호출 (매 프레임 호출 방지)
        if (islandWindow.activeSelf == anyOpen)
        {
            islandWindow.SetActive(!anyOpen);
        }
    }


    // 등록된 팝업창 중 하나라도 활성화되어 있는지 반환
    public bool IsAnyPopupOpen()
    {
        if (popupPanels == null) return false;

        foreach (var panel in popupPanels)
        {
            if (panel != null && panel.activeInHierarchy) return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}