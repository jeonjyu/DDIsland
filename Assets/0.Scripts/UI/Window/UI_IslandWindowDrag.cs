using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_IslandWindowDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image dragImage;

    [SerializeField] private UI_IslandWindow ui_IslandWindow;
    [SerializeField] private UI_IslandWindow.PointDirection direction;

    private float startDistance;
    private Vector3 startScale;
    private Vector2 gapPos;

    private bool isDragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true;

            if (ui_IslandWindow == null || direction == UI_IslandWindow.PointDirection.None) return;

            ui_IslandWindow.SetPivot(direction);

            RectTransform targetRect = ui_IslandWindow.IslandWindowRect;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                targetRect, targetRect.position, eventData.pressEventCamera, out _);
            gapPos = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, targetRect.position);

            startDistance = Vector2.Distance(gapPos, eventData.position);
            startScale = targetRect.localScale;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            float currentDistance = Vector2.Distance(gapPos, eventData.position);

            float ratio = currentDistance / startDistance;

            float newScale = startScale.x * ratio;

            ui_IslandWindow.WindowScale = new Vector3(newScale, newScale, newScale);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false;

            ui_IslandWindow.SetPivot(UI_IslandWindow.PointDirection.None);

            // 드래그가 끝났을 때 마우스 위치가 현재 스크립트를 가진 오브젝트가 아닐 경우 OnPointerExit이 실행되지 않으므로 여기서 처리
            GameObject obj = eventData.pointerCurrentRaycast.gameObject;

            if (obj != gameObject)
            {
                dragImage.SetAlpha(0f);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        dragImage.SetAlpha(1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isDragging)
            dragImage.SetAlpha(0f);
    }
}
