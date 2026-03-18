using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CurrentPlaySlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action OnMouseDown;
    public event Action OnMouseUp;

    public void OnPointerDown(PointerEventData eventData) => OnMouseDown?.Invoke();

    public void OnPointerUp(PointerEventData eventData) => OnMouseUp?.Invoke();
}
