using UnityEngine;
using UnityEngine.EventSystems;

public class UI_DirEdgeCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UI_EdgeCollider edgeCollider;

    [SerializeField] private DirEdgeMouseState edgeState;

    public void OnPointerEnter(PointerEventData eventData)
    {
        edgeCollider.EnterMouse(edgeState);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        edgeCollider.ExitMouse(edgeState);
    }
}
