using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_WaterWindowDragBar : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("호수/바다 창 컨트롤러")]
    [SerializeField] private UI_WaterWindow waterWindow;

    [Header("드래그 바")]
    [SerializeField] private Image dragBar;

    private bool isDragging;
    private float gapHeight;        // 마우스 클릭 지점과 호수창 높이의 차이값

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true;
            gapHeight = waterWindow.Height - eventData.position.y;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            waterWindow.Height = eventData.position.y + gapHeight;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false;
            gapHeight = 0f;

            GameObject obj = eventData.pointerCurrentRaycast.gameObject;

            if(obj != gameObject)
            {
                dragBar.SetAlpha(0f);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        dragBar.SetAlpha(1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isDragging)
            dragBar.SetAlpha(0f);
    }
}
