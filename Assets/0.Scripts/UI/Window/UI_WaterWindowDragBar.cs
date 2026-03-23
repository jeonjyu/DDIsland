using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_WaterWindowDragBar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("호수/바다 창 컨트롤러")]
    [SerializeField] private UI_WaterWindow waterWindow;

    [Header("드래그 바")]
    [SerializeField] private Image dragBar;

    private bool isDragging;
    private float gapHeight;        // 마우스 클릭 지점과 호수창 높이의 차이값

    private RectTransform rect;
    private Vector2 startScale;
    private Vector2 expandScale;

    private Tween sizeTween;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        startScale = Vector2Int.RoundToInt(rect.sizeDelta);
        expandScale = startScale * 1.5f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true;
            gapHeight = waterWindow.Height - eventData.position.y;
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

            // 드래그가 끝났을 때 마우스 위치가 현재 스크립트를 가진 오브젝트가 아닐 경우 OnPointerExit이 실행되지 않으므로 여기서 처리
            GameObject obj = eventData.pointerCurrentRaycast.gameObject;

            if (obj != gameObject)
            {
                dragBar.SetAlpha(0f);
                DoScale(0.3f, false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDragging)
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
        sizeTween?.Kill();

        if (isExpand)
        {
            sizeTween = rect.DOSizeDelta(expandScale, duration).SetEase(Ease.OutBack);
        }
        else
        {
            sizeTween = rect.DOSizeDelta(startScale, duration).SetEase(Ease.OutBack);
        }
    }
}
