using System;
using UnityEngine;

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

    private DirEdgeMouseState edgeState;

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

    private void CheckMouseHover()
    {
        windowEdge.SetActive(edgeState != DirEdgeMouseState.None);
    }

}
