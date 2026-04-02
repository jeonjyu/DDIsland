using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CurrentPlaySlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [field: SerializeField] public Slider PlaySlider { get; private set; }

    public event Action OnMouseDown;
    public event Action OnMouseUp;

    public void OnPointerDown(PointerEventData eventData) => OnMouseDown?.Invoke();

    public void OnPointerUp(PointerEventData eventData) => OnMouseUp?.Invoke();
}
