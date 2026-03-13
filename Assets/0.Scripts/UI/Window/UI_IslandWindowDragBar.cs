using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_IslandWindowDragBar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI_IslandWindow.cs")]
    [SerializeField] private UI_IslandWindow ui_IslandWindow;

    [Header("드래그 바 이미지")]
    [SerializeField] private Image dragBar;

    private Vector2 gapPos;
    private bool isDragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true;

            gapPos = new Vector2(
                ui_IslandWindow.IslandWindowRect.position.x - eventData.position.x,
                ui_IslandWindow.IslandWindowRect.position.y - eventData.position.y);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ui_IslandWindow.WindowPosition = eventData.position + gapPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false;


            // 드래그가 끝났을 때 마우스 위치가 현재 스크립트를 가진 오브젝트가 아닐 경우 OnPointerExit이 실행되지 않으므로 여기서 처리
            GameObject obj = eventData.pointerCurrentRaycast.gameObject;

            if (obj != gameObject)
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
        if (!isDragging)
            dragBar.SetAlpha(0f);
    }
}
