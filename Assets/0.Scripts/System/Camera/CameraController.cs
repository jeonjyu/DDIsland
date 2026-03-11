using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("카메라")]
    [SerializeField] private Camera mainCam;

    [Header("Projection-Size 수치 설정")]
    [SerializeField] private float minSize = 25f;
    [SerializeField] private float maxSize = 50f;

    private Vector3 startVec;           // 스크린 좌표 (0, 0) 기준 카메라 위치 → 오브젝트는 (Screen.Width, Screen.Height) 위치에 있음
    private Vector3 endVec;             // 스크린 좌표 (Screen.Width, Screen.Height) 기준 카메라 위치 → 오브젝트는 (0, 0) 위치에 있음
    private Vector3 gapDistance;        // 스크린 좌표 (0, 0) ~ (Screen.Width, Screen.Height)의 월드 좌표 거리

    private void Start()
    {
        GameManager.Instance.IslandWindow.OnScaleChanged += SetCamSize;
        GameManager.Instance.IslandWindow.OnPosChanged += SetCamPos;

        startVec = mainCam.ScreenToWorldPoint(Vector3.zero);
        endVec = mainCam.ScreenToWorldPoint(GameManager.Instance.IslandWindow.IslandWindowRect.sizeDelta);
        gapDistance = endVec - startVec;
    }

    private void SetCamSize(float ratio)
    {
        mainCam.orthographicSize = minSize + ((maxSize - minSize) * (1f - ratio));
    }

    /// <summary>
    /// 섬 창(Window)에 대응하여 카메라 위치 이동
    /// </summary>
    /// <param name="widthRatio"> 가로 비율 </param>
    /// <param name="heightRatio"> 세로 비율 </param>
    private void SetCamPos(float widthRatio, float heightRatio)
    {
        transform.position = startVec + (transform.right * gapDistance.x * ( 1f - widthRatio)) + (transform.up * gapDistance.y * (1f - heightRatio));

    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null && GameManager.Instance.IslandWindow != null)
        {
            GameManager.Instance.IslandWindow.OnScaleChanged -= SetCamSize;
            GameManager.Instance.IslandWindow.OnPosChanged -= SetCamPos;
        }
    }
}
