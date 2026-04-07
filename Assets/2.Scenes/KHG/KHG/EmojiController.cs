using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum EmojiType
{
    None,
    Hunger,
    Tired,
    Sleep,
    Fish,
    Food
}

public class EmojiController : Singleton<EmojiController>
{
    [SerializeField] private RectTransform _rootUI;

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _iconImage;

    [SerializeField] private Sprite _sleepSprite;
    [SerializeField] private Sprite _hungerSprite;
    [SerializeField] private Sprite _tiredSprite;

    [SerializeField] private PlayerController _player;

    private Camera _cam;
    private Coroutine _tempEmojiRoutine;

    private EmojiType _currentPersistentType = EmojiType.None;
    private bool _isShowingTempEmoji = false;
    public bool isEditMode = false;

    private void Awake()
    {
        base.Awake();
        _cam = Camera.main;
        _rootUI.localScale = Vector3.zero;
        _canvasGroup.alpha = 0f;
        _rootUI.gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        FollowCharacterForEmoji();
    }
    public void FollowCharacterForEmoji()
    {
        Vector3 worldPos = _player.transform.position + new Vector3(-2f, 7f, -2f);
        Vector3 screenPos = _cam.WorldToScreenPoint(worldPos);

        if (screenPos.z <= 0f)
        {
            _rootUI.gameObject.SetActive(false);
            return;
        }

        _rootUI.gameObject.SetActive(true);
        _rootUI.position = screenPos;
    }
    public void RefreshStateEmoji(PlayerData data, bool isSleeping)
    {
        if (_isShowingTempEmoji) return;
        if (data == null) return;

        EmojiType nextType = EmojiType.None;
        Sprite nextSprite = null;

        if (isSleeping)
        {
            nextType = EmojiType.Sleep;
            nextSprite = _sleepSprite;
        }
        else if (data.Stamina <= 10)
        {
            nextType = EmojiType.Tired;
            nextSprite = _tiredSprite;
        }
        else if (data.Hunger <= 20)
        {
            nextType = EmojiType.Hunger;
            nextSprite = _hungerSprite;
        }

        if (_currentPersistentType == nextType) return;
        _currentPersistentType = nextType;

        if (nextType == EmojiType.None)
        {
            HideEmoji();
        }
        else ShowEmoji(nextSprite);
    }

    public void ShowFishEmoji(FishDataSO data)
    {
        if (data == null) return;

        _currentPersistentType = EmojiType.Fish;
        ShowTempEmoji(_currentPersistentType, data.FishImgPath_Sprite, 3f);
    }
    public void ShowFoodEmoji(FoodDataSO data)
    {
        if (data == null) return;

        _currentPersistentType = EmojiType.Food;
        ShowTempEmoji(_currentPersistentType, data.FoodImgPath_Sprite, 3f);
    }

    public void ShowTempEmoji(EmojiType type, Sprite sprite, float duration)
    {
        if (_tempEmojiRoutine != null) StopCoroutine(_tempEmojiRoutine);

        _tempEmojiRoutine = StartCoroutine(TempEmojiRoutine(type, sprite, duration));
    }

    IEnumerator TempEmojiRoutine(EmojiType type, Sprite sprite, float duration)
    {
        _isShowingTempEmoji = true;
        _currentPersistentType = type;
        ShowEmoji(sprite);

        yield return new WaitForSeconds(duration);

        _isShowingTempEmoji = false;
        _tempEmojiRoutine = null;

        RefreshStateEmoji(_player.PlayerDataOld, _player.IsResting);
    }

    private void ShowEmoji(Sprite sprite)
    {
        if (isEditMode) return;

        _rootUI.gameObject.SetActive(true);
        _iconImage.sprite = sprite;

        _rootUI.DOKill();
        _canvasGroup.DOKill();

        _rootUI.localScale = Vector3.zero;
        _canvasGroup.alpha = 0f;

        _rootUI.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        _canvasGroup.DOFade(1f, 0.15f);
    }

    public void HideEmoji()
    {
        _rootUI.DOKill();
        _canvasGroup.DOKill();

        _rootUI.DOScale(0f, 0.15f).SetEase(Ease.InBack);
        _canvasGroup.DOFade(0f, 0.15f).OnComplete(() =>
        {
            _rootUI.gameObject.SetActive(false);
        });
    }

}
