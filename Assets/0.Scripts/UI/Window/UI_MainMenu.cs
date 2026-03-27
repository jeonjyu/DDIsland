using DG.Tweening;
using System;
using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    [Header("Show / Hide 상태 포지션 값")]
    [SerializeField] private Vector2 showPos;
    [SerializeField] private Vector2 hidePos;

    [Header("위치 이동 시간")]
    [SerializeField] private float moveDuration = 0.7f;

    private bool isShow;
    public bool IsShow
    {
        get { return isShow; }
        set
        {
            isShow = value;

            if (value)
                MoveTweening(hidePos, showPos);
            else
                MoveTweening(showPos, hidePos);

                OnMenuStateChanged?.Invoke(value);
        }
    }

    private RectTransform rectTransform;

    public event Action<bool> OnMenuStateChanged;   // 메뉴 상태가 변할 때 실행할 이벤트

    public void OnClick_Arrow() => IsShow = !IsShow;

    public void SetShowPos(Vector2 pos) => showPos = pos;
    public void SetHidePos(Vector2 pos) => hidePos = pos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void MoveTweening(Vector2 fromPos, Vector2 toPos)
    {
        if (rectTransform == null) return;

        rectTransform.anchoredPosition = fromPos;
        rectTransform.DOAnchorPos(toPos, moveDuration).SetEase(Ease.OutQuart);
    }
}
