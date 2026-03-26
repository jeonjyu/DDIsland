
using DG.Tweening;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RewardEffect : Singleton<RewardEffect>
{
    [SerializeField] private RewardPool _rewardPool;
    [SerializeField] private RewardCollectItem _goldWorldPrefab;
    [SerializeField] private RewardCollectItem _lpWorldPrefab;

    [SerializeField] private RectTransform _goldUI;
    [SerializeField] private RectTransform _lpUI;

    [SerializeField] private Transform _SellrewardSpawn;
    [SerializeField] private Transform _QuestrewardSpawn;

    [SerializeField] private RectTransform _effectRoot; 
    [SerializeField] private Canvas _canvas;

    [SerializeField] private int _goldEffectCount = 10;
    [SerializeField] private int _lpEffectCount = 10;
    private void Awake()
    {
        base.Awake();
    }
    private Vector2 WorldToCanvasPosition(Transform worldPoint)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPoint.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_effectRoot,screenPos,null,out Vector2 localPos);

        return localPos;
    }
    private Vector2 UIToCanvasPosition(RectTransform targetUI)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, targetUI.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_effectRoot,screenPos,null,out Vector2 localPos);

        return localPos;
    }
    private void PlayEffect(RewardCollectItem prefab, Transform spawnPoint, RectTransform targetUI, int count)
    {
        Vector2 startPos = WorldToCanvasPosition(spawnPoint);
        Vector2 targetPos = UIToCanvasPosition(targetUI);

        for (int i = 0; i < count; i++)
        {
            var item = _rewardPool.Get(prefab);

            RectTransform rect = item.GetComponent<RectTransform>();
            rect.SetParent(_effectRoot, false);
            rect.anchoredPosition = startPos;
            rect.localScale = Vector3.one;

            item.Play(targetPos, () =>
            {
                _rewardPool.Release(item);
            });
        }
    }

    public void PlaySellGoldEffect()
    {
        PlayEffect(_goldWorldPrefab, _SellrewardSpawn, _goldUI, _goldEffectCount);
    }

    public void PlaySellLpEffect()
    {
        PlayEffect(_lpWorldPrefab, _SellrewardSpawn, _lpUI, _lpEffectCount);
    }

    public void PlayQuestGoldEffect()
    {
        PlayEffect(_goldWorldPrefab, _QuestrewardSpawn, _goldUI, _goldEffectCount);
    }

    public void PlayQuestLpEffect()
    {
        PlayEffect(_lpWorldPrefab, _QuestrewardSpawn, _lpUI, _lpEffectCount);
    }
}

