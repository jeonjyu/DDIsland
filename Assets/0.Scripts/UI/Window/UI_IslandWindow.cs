using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_IslandWindow : MonoBehaviour
{
    /// <summary>
    /// 크기 조절 드래그의 위치 타입
    /// </summary>
    public enum PointDirection
    {
        None,
        LeftUp,             
        RightUp,            
        LeftDown,           
        RightDown           
    }

    [Header("섬 창")]
    [SerializeField] private RectTransform islandWindowRect;

    [Header("호수/바다 창")]
    [SerializeField] private UI_WaterWindow waterWindow;

    [Header("최소 / 최대 비율")]
    [SerializeField] private float defaultRatio = 0.5f;
    [SerializeField] private float maxRatio = 1f;
    [SerializeField] private float minRatio = 0.375f;

    private float maxFitRatio;      // 모니터 범위 밖, 호수창에 겹치지 않는 최대 비율

    private Vector2Int restorePos;     // 크기 조절 드래그를 종료할 때 피봇을 중앙에 놓으면서 복구할 위치값
    public float WidthOffset => islandWindowRect.sizeDelta.x * islandWindowRect.localScale.x * 0.5f;
    public float HeightOffset => islandWindowRect.sizeDelta.y * islandWindowRect.localScale.y * 0.5f;

    public event Action<float, float> OnPosChanged;
    public event Action<float> OnScaleChanged;

    #region 프로퍼티
    public RectTransform IslandWindowRect => islandWindowRect;

    public Vector2 WindowPosition
    {
        get { return islandWindowRect.position; }
        set
        {
            float posX = Mathf.Clamp(value.x, WidthOffset, islandWindowRect.sizeDelta.x - WidthOffset);
            float posY = Mathf.Clamp(value.y, waterWindow.Height + HeightOffset, islandWindowRect.sizeDelta.y - HeightOffset);

            islandWindowRect.position = new Vector2(posX, posY);

            OnPosChanged?.Invoke(
                posX / islandWindowRect.sizeDelta.x,
                posY / islandWindowRect.sizeDelta.y);
        }
    }

    public Vector3 WindowScale
    {
        get { return islandWindowRect.localScale; }
        set
        {
            islandWindowRect.localScale = new Vector3(
                GetScaleRatio(value.x),
                GetScaleRatio(value.y),
                GetScaleRatio(value.z));

            OnScaleChanged?.Invoke((float)Math.Round((islandWindowRect.localScale.x - minRatio) / (maxRatio - minRatio), 2));
        }
    }
    #endregion

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.IslandWindow = this;
        }

        maxFitRatio = maxRatio;

#if UNITY_EDITOR
        WindowScale = new Vector3(defaultRatio, defaultRatio, defaultRatio);
#endif
    }


    private IEnumerator Start()
    {
        yield return null;

#if !UNITY_EDITOR
        islandWindowRect.sizeDelta = WindowController.Instance.WindowSize;
        WindowScale = new Vector3(defaultRatio, defaultRatio, defaultRatio);
#endif
    }

    private void Update()
    {
        if(Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log(islandWindowRect.position.x + WidthOffset / islandWindowRect.sizeDelta.x);
            Debug.Log((islandWindowRect.sizeDelta.y - islandWindowRect.position.y + HeightOffset) / islandWindowRect.sizeDelta.y);
        }
    }

    /// <summary>
    /// 크기를 조절할 드래그 방향에 따른 Pivot / Position 수정
    /// </summary>
    /// <param name="dir"></param>
    public void SetPivot(PointDirection dir)
    {
        Vector2 pivot = Vector2.zero;
        Vector2 offset = islandWindowRect.position;

        switch (dir)
        {
            case PointDirection.None:
                pivot = new Vector2(0.5f, 0.5f);
                offset += new Vector2(WidthOffset * restorePos.x, HeightOffset * restorePos.y);
                maxFitRatio = maxRatio;
                break;
            case PointDirection.LeftUp:
                pivot = new Vector2(1f, 0f);
                offset += new Vector2(WidthOffset, -HeightOffset);
                restorePos = new Vector2Int(-1, 1);
                maxFitRatio = Mathf.Min(
                    (islandWindowRect.position.x + WidthOffset) / islandWindowRect.sizeDelta.x,
                    (islandWindowRect.sizeDelta.y - islandWindowRect.position.y + HeightOffset) / islandWindowRect.sizeDelta.y);
                break;
            case PointDirection.RightUp:
                pivot = new Vector2(0f, 0f);
                offset += new Vector2(-WidthOffset, -HeightOffset);
                restorePos = new Vector2Int(1, 1);
                maxFitRatio = Mathf.Min(
                    (islandWindowRect.sizeDelta.x - islandWindowRect.position.x + WidthOffset) / islandWindowRect.sizeDelta.x,
                    (islandWindowRect.sizeDelta.y - islandWindowRect.position.y + HeightOffset) / islandWindowRect.sizeDelta.y);
                break;
            case PointDirection.LeftDown:
                pivot = new Vector2(1f, 1f);
                offset += new Vector2(WidthOffset, HeightOffset);
                restorePos = new Vector2Int(-1, -1);
                maxFitRatio = Mathf.Min(
                    (islandWindowRect.position.x + WidthOffset) / islandWindowRect.sizeDelta.x,
                    (islandWindowRect.position.y - waterWindow.Height + HeightOffset) / islandWindowRect.sizeDelta.y);
                break;
            case PointDirection.RightDown:
                pivot = new Vector2(0f, 1f);
                offset += new Vector2(-WidthOffset, HeightOffset);
                restorePos = new Vector2Int(1, -1);
                maxFitRatio = Mathf.Min(
                    (islandWindowRect.sizeDelta.x - islandWindowRect.position.x + WidthOffset) / islandWindowRect.sizeDelta.x,
                    (islandWindowRect.position.y - waterWindow.Height + HeightOffset) / islandWindowRect.sizeDelta.y);
                break;
        }

        islandWindowRect.pivot = pivot;
        islandWindowRect.position = offset;
    }

    /// <summary>
    /// 섬 창(Window)의 Scale 값을 소수점 두 자리까지 정리 후 최소, 최대 비율 범위 내로 반환하는 메서드
    /// </summary>
    /// <param name="scale"></param>
    /// <returns></returns>
    private float GetScaleRatio(float scale)
    {
        scale = (float)Math.Round(scale, 4);
        return Mathf.Clamp(scale, minRatio, maxFitRatio);
    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null && GameManager.Instance.IslandWindow == this)
            GameManager.Instance.IslandWindow = null;
    }
}
