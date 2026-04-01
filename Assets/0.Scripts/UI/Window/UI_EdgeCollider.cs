using System;
using UnityEngine;
using UnityEngine.InputSystem;

// 네 방면의 마우스 호버 상태가 어떤지 체크하기 위한 플래그 속성의 열거형
[Flags]
public enum DirEdgeMouseState
{
    None = 0,
    Top = 1 << 0,
    Bottom = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3,
}

public class UI_EdgeCollider : MonoBehaviour
{
    [SerializeField] private GameObject windowEdge;     // 섬 창(Window) 테두리 오브젝트

    [Header("호버 감지 설정")]
    [SerializeField] private RectTransform islandWindowRect;
    [SerializeField] private float edgeThickness = 30f;  // 호버 감지 크기 

    private DirEdgeMouseState edgeState;

    private int lockCount;

    public void Lock()
    {
        lockCount++;
        windowEdge.SetActive(true);
    }

    public void Unlock()
    {
        lockCount = Mathf.Max(0, lockCount - 1);
        if (lockCount == 0) CheckMouseHover();
    }

    public void EnterMouse(DirEdgeMouseState dirState)
    {
        edgeState |= dirState;
        CheckMouseHover();
    }

    public void ExitMouse(DirEdgeMouseState dirState)
    {
        edgeState &= ~dirState;
        CheckMouseHover();
    }

    // 빌드에선 WindowController의 좌표 변환 사용
    private Vector2 GetMousePosition()
    {
#if UNITY_EDITOR
        return Mouse.current.position.ReadValue();
#else
        return WindowController.Instance.GetClientCursorPosition();
#endif
    }
    // 마우스 좌표 기반 호버 감지
    private void Update()
    {
        if (lockCount > 0) return;

        Vector2 mousePos = GetMousePosition();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            islandWindowRect, mousePos, null, out Vector2 localPos);

        Rect rect = islandWindowRect.rect;

        bool inRange = localPos.x >= rect.xMin - edgeThickness && localPos.x <= rect.xMax + edgeThickness
                    && localPos.y >= rect.yMin - edgeThickness && localPos.y <= rect.yMax + edgeThickness;

        if (!inRange)
        {
            edgeState = DirEdgeMouseState.None;
            CheckMouseHover();
            return;
        }

        DirEdgeMouseState newState = DirEdgeMouseState.None;

        if (localPos.y >= rect.yMax - edgeThickness) newState |= DirEdgeMouseState.Top;
        if (localPos.y <= rect.yMin + edgeThickness) newState |= DirEdgeMouseState.Bottom;
        if (localPos.x <= rect.xMin + edgeThickness) newState |= DirEdgeMouseState.Left;
        if (localPos.x >= rect.xMax - edgeThickness) newState |= DirEdgeMouseState.Right;

        edgeState = newState;
        CheckMouseHover();
    }

    private void CheckMouseHover()
    {
        if (lockCount > 0) return;
        windowEdge.SetActive(edgeState != DirEdgeMouseState.None);
    }

}
