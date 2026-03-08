using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_WaterWindowDragBar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image dragBar;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log(eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        dragBar.SetAlpha(1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        dragBar.SetAlpha(0f);
    }
}
