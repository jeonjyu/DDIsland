using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_WaterWindow : MonoBehaviour
{
    [Header("호수 창")]
    [SerializeField] private RectTransform waterBackground;

    [Header("섬 창")]
    [SerializeField] private UI_IslandWindow islandWindow;

    [Header("호수 창 열기 버튼")]
    [SerializeField] private Button btnEnableWaterWindow;

    [Header("창 높이 설정 배율")]
    [SerializeField, Range(0f, 1f)] private float defaultRatio = 0.2f;
    [SerializeField, Range(0f, 1f)] private float maxRatio = 0.5f;

    public float Height
    {
        get { return waterBackground.sizeDelta.y; }
        set
        {
            int height = Mathf.RoundToInt(Mathf.Min(value, islandWindow.WindowPosition.y - islandWindow.HeightOffset));
            height = Mathf.Clamp(height, 0, maxHeight);

            waterBackground.sizeDelta = new Vector2(0, height);
            OnWaterWindowHeightChanged?.Invoke(height);
        }
    }

    public event Action<int> OnWaterWindowHeightChanged;

    private int maxHeight;          // 최대 호수/바다 창 높이
    private int defaultHeight;      // 시작 호수/바다 창 높이

    private void Awake()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.WaterWindow = this;
    }

    /// <summary>
    /// UI_IslandWindow 스크립트와 실행 순서가 정해져 있으므로 Init() 메서드를 통해 관리
    /// </summary>
    public void Init()
    {
#if !UNITY_EDITOR
        defaultHeight = Mathf.RoundToInt(WindowController.Instance.WindowSize.y * defaultRatio);
        maxHeight = Mathf.RoundToInt(WindowController.Instance.WindowSize.y * maxRatio);
#else
        defaultHeight = 206;
        maxHeight = 516;
#endif

        Height = defaultHeight;
    }

    public void OnClick_ShowWaterwindow() => Height = defaultHeight;

    private void SetActiveWindowButton(int height)
    {
        btnEnableWaterWindow.gameObject.SetActive(height <= 0);
    }

    private void OnEnable()
    {
        OnWaterWindowHeightChanged += SetActiveWindowButton;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null && GameManager.Instance.WaterWindow == this)
            GameManager.Instance.WaterWindow = null;

        OnWaterWindowHeightChanged -= SetActiveWindowButton;
    }

    private void OnValidate()
    {
        defaultRatio = Mathf.Clamp(defaultRatio, 0f, maxRatio);
    }
}
