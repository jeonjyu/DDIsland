using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UI_HoverOnly : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return !Mouse.current.leftButton.isPressed;
    }
}