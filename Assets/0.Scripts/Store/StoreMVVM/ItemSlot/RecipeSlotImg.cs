using UnityEngine;
using UnityEngine.EventSystems;

public abstract class RecipeSlotImg : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected string ingredientName;
    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        SlotMouseOver.Instance.OnMouseIn(rect, ingredientName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SlotMouseOver.Instance.OnMouseOut();
    }
}
