using System;
using UnityEngine;

public class UI_WaterWindow : MonoBehaviour
{
    [Header("호수창")]
    [SerializeField] private RectTransform waterBackground;

    [Header("창 높이 설정 배율")]
    [SerializeField, Range(0f, 1f)] private float defaultRatio = 0.2f;
    [SerializeField, Range(0f, 1f)] private float maxRatio = 0.5f;

    public Vector2 WindowSize
    {
        get { return waterBackground.sizeDelta; }
        set
        {
            int height = Mathf.RoundToInt(value.y);
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

        WindowSize = new Vector2(0, defaultHeight);
    }

    private void OnValidate()
    {
        defaultRatio = Mathf.Clamp(defaultRatio, 0f, maxRatio);
    }
}
