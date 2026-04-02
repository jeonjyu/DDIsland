using DG.Tweening;
using UnityEngine;
using System;
public class RewardCollectItem : MonoBehaviour
{
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Play(Vector3 targetPos, Action onComplete)
    {
        _rect.DOKill();
        _rect.localScale = Vector3.zero;

        Vector2 startPos = _rect.anchoredPosition;
        Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
        Vector2 scatterPos = startPos + randomDir * UnityEngine.Random.Range(80f, 140f);

        float scatterDuration = UnityEngine.Random.Range(0.2f, 0.3f);
        float collectDelay = UnityEngine.Random.Range(0.05f, 0.15f);
        float collectDuration = UnityEngine.Random.Range(0.35f, 0.5f);

        Sequence seq = DOTween.Sequence();

        seq.Append(_rect.DOScale(1f, 0.15f).SetEase(Ease.OutBack));
        seq.Join(_rect.DOAnchorPos(scatterPos, scatterDuration).SetEase(Ease.OutQuad));
        seq.AppendInterval(collectDelay);
        seq.Append(_rect.DOAnchorPos(targetPos, collectDuration).SetEase(Ease.InBack));
        seq.Join(_rect.DOScale(0.3f, collectDuration).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
    private void OnDisable()
    {
        transform.DOKill();
    }
}
