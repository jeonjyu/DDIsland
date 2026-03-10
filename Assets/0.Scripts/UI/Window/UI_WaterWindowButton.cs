using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_WaterWindowButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image btnImage;
    [SerializeField] private Image arrowImage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        btnImage.SetAlpha(1f);
        arrowImage.SetAlpha(1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        btnImage.SetAlpha(0f);
        arrowImage.SetAlpha(0f);
    }
}
