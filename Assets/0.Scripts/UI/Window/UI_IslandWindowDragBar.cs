using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_IslandWindowDragBar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("UI_IslandWindow.cs")]
    [SerializeField] private UI_IslandWindow ui_IslandWindow;

    [Header("드래그 바 이미지")]
    [SerializeField] private Image dragBar;

    [SerializeField] private UI_EdgeCollider edgeCollider;

    private Vector2 gapPos;
    private bool isDragging;

    private RectTransform rect;
    private Vector3 startScale;
    private Vector3 expandScale;

    private Sequence seq;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        startScale = Vector3Int.RoundToInt(rect.sizeDelta);
        expandScale = startScale * 1.5f;

        seq = DOTween.Sequence();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            edgeCollider?.Lock();

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

            edgeCollider?.Unlock();
            // 드래그가 끝났을 때 마우스 위치가 현재 스크립트를 가진 오브젝트가 아닐 경우 OnPointerExit이 실행되지 않으므로 여기서 처리
            GameObject obj = eventData.pointerCurrentRaycast.gameObject;

            if (obj != gameObject)
            {
                dragBar.SetAlpha(0.6f);
                DoScale(0.3f, false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isDragging)
        {
            dragBar.SetAlpha(1f);
            DoScale(0.3f, true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging)
        {
            dragBar.SetAlpha(0.6f);
            DoScale(0.3f, false);
        }
    }

    /// <summary>
    /// 이미지 크기 변경 트위닝 애니메이션 메서드
    /// </summary>
    /// <param name="duration"> 트위닝 애니메이션 시간 </param>
    /// <param name="isExpand"> 확장인지 축소인지 여부 </param>
    private void DoScale(float duration, bool isExpand)
    {
        rect.DOKill(false);

        if(isExpand)
        {
            Debug.Log("확장");
            seq.Append(rect.DOSizeDelta(expandScale * 1.05f, duration * 0.7f).SetEase(Ease.OutQuad))
            .Append(rect.DOSizeDelta(expandScale, duration * 0.3f).SetEase(Ease.OutBack));
        }
        else
        {
            Debug.Log("축소");
            seq.Append(rect.DOSizeDelta(startScale * 0.95f, duration * 0.3f).SetEase(Ease.OutQuad))
            .Append(rect.DOSizeDelta(startScale, duration * 0.3f).SetEase(Ease.OutBack));
        }

        seq.SetUpdate(true);
    }
}
