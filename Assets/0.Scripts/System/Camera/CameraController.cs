using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("카메라")]
    [SerializeField] private Camera mainCam;

    [Header("Projection-Size 수치 설정")]
    [SerializeField] private float minSize = 25f;
    [SerializeField] private float maxSize = 50f;

    private Vector3 originPos;      // 시작 카메라 포지션

    private void Awake()
    {
        originPos = transform.position;
    }

    private void Start()
    {
        GameManager.Instance.IslandWindow.OnScaleChanged += SetCamSize;
        GameManager.Instance.IslandWindow.OnPosChanged += SetCamPos;
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
        float height = mainCam.orthographicSize * 2f;
        float width = mainCam.aspect * height;

        // 1f에 받아온 비율을 빼주는 이유는 카메라의 위치를 변경시키기 때문에 실제 오브젝트는 반대방향으로 이동하는 것처럼 보이기 때문
        float xOffset = (1f - widthRatio - 0.5f) * width;
        float yOffset = (1f - heightRatio - 0.5f) * height;

        transform.position = originPos + (transform.right * xOffset) + (transform.up * yOffset);
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
