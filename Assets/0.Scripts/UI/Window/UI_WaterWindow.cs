using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_WaterWindow : MonoBehaviour
{
    [Header("호수창")]
    [SerializeField] private RectTransform waterBackground;

    [Header("호수창 열기 버튼")]
    [SerializeField] private Button btnEnableWaterWindow;

    [Header("창 높이 설정 배율")]
    [SerializeField, Range(0f, 1f)] private float defaultRatio = 0.2f;
    [SerializeField, Range(0f, 1f)] private float maxRatio = 0.5f;

    public float Height
    {
        get { return waterBackground.sizeDelta.y; }
        set
        {
            int height = Mathf.RoundToInt(value);
            height = Mathf.Clamp(height, 0, maxHeight);

            waterBackground.sizeDelta = new Vector2(0, height);
            OnWaterWindowHeightChanged?.Invoke(height);
        }
    }

    public event Action<int> OnWaterWindowHeightChanged;

    private int maxHeight;          // 최대 호수/바다 창 높이
    private int defaultHeight;      // 시작 호수/바다 창 높이

    private void Start()
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
        OnWaterWindowHeightChanged -= SetActiveWindowButton;
    }

    private void OnValidate()
    {
        defaultRatio = Mathf.Clamp(defaultRatio, 0f, maxRatio);
    }
}
