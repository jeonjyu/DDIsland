using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FishingHookEffect : MonoBehaviour
{
    [SerializeField] private Image _fishingHook;
    [SerializeField] private RectTransform _hookSpawnRect;
    [SerializeField] private Transform _fishingTr;
    [SerializeField] private RectTransform _effectRoot;

    public float duration = 0.5f;

    private bool _isPlaying;
    private Tween _moveTween;
    private void Start()
    {
        _fishingHook.gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if (!_isPlaying) return;

        Vector2 localPoint;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(_fishingTr.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle( _effectRoot, screenPoint, null, out localPoint);

        _hookSpawnRect.anchoredPosition = new Vector2(
            localPoint.x,
            _hookSpawnRect.anchoredPosition.y
        );
    }

    public void PlayerFishingHookEffect()
    {
        _moveTween?.Kill();
        _isPlaying = false;
        Vector2 localPoint;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(_fishingTr.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_effectRoot, screenPoint, null, out localPoint );

        _fishingHook.gameObject.SetActive(true);
        _isPlaying = true;

        float startY = _effectRoot.rect.yMax;
        float targetY = _effectRoot.rect.center.y;

        _hookSpawnRect.anchoredPosition = new Vector2(localPoint.x, startY);

        _moveTween = _hookSpawnRect.DOAnchorPosY(targetY, duration).SetEase(Ease.OutQuad);
    }

    public void PullUpHook()
    {
        _moveTween?.Kill();

        float topY = _effectRoot.rect.yMax;

        _moveTween = _hookSpawnRect.DOAnchorPosY(topY, duration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                _isPlaying = false;
                _fishingHook.gameObject.SetActive(false);
            });
    }
}