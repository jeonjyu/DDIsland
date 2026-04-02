using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestPanel : MonoBehaviour
{
    [SerializeField] private UI_QuestSlot slotPrefab;
    [SerializeField] private Transform slotParent;
    [SerializeField] private QuestType defaultCategory = QuestType.Store;

    [SerializeField] private GameObject _rewardUIPanel;
    [SerializeField] private GameObject _reward1Slot;
    [SerializeField] private GameObject _reward2Slot;
    [SerializeField] private Image _rewardIcon1;
    [SerializeField] private Image _rewardIcon2;
    [SerializeField] private TMP_Text _rewardtext1;
    [SerializeField] private TMP_Text _rewardtext2;
    [SerializeField] private Transform rewardParent;

    [SerializeField] private GameObject _StoreTabButtonClick;
    [SerializeField] private GameObject _FishingTabButtonClick;
    [SerializeField] private GameObject _GuideTabButtonClick;
    [SerializeField] private GameObject _GrowthTabButtonClick;
    [SerializeField] private GameObject _CompletedTabButtonClick;

    [SerializeField] private Button _StoreTabButton;
    [SerializeField] private Button _FishingTabButton;
    [SerializeField] private Button _GuideTabButton;
    [SerializeField] private Button _GrowthTabButton;
    [SerializeField] private Button _CompletedTabButton;

    [SerializeField] private AudioClip _questTabButtonSFX;
    [SerializeField] private AudioClip _questCloseButtonSFX;
    [SerializeField] private AudioClip _questRewardButtonSFX;

    [SerializeField] private ScrollRect _questScrollRect;
    // [SerializeField] private GameObject _GuidePanel;

    private List<UI_QuestSlot> _slots = new List<UI_QuestSlot>();
    private QuestType _currentCategory;

    private bool _isRewardPopupOpen;
    public bool IsRewardPopupOpen => _isRewardPopupOpen;

    private void OnEnable()
    {
        RefreshCategory(defaultCategory);
    }
    private void Awake()
    {
        _rewardUIPanel.SetActive(false);
    }

    public void RefreshCategory(QuestType category)
    {
        _currentCategory = category;

        ClearSlots();

        List<QuestDataSO> quests = QuestManager.Instance.GetByQuesCategory(category);

        for (int i = 0; i < quests.Count; i++)
        {
            UI_QuestSlot slot = Instantiate(slotPrefab, slotParent);
            slot.Bind(quests[i].ID, false);
            _slots.Add(slot);
        }
    }
    public void RefreshCompleted()
    {
        ClearSlots();

        var completed = QuestManager.Instance.CompletedQuests;
        //List<QuestDataSO> quests = new List<QuestDataSO>();
        foreach (var com in completed)
        {
            QuestDataSO quest = QuestManager.Instance.GetByQuestId(com);
            UI_QuestSlot slot = Instantiate(slotPrefab, slotParent);
            slot.Bind(quest.ID, true);
            _slots.Add(slot);
            //quests.Add(quest);
        }

    }
    private void ClearSlots()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i] != null)
            {
                Destroy(_slots[i].gameObject);
            }
        }
        _slots.Clear();
    }
    public void RefreshCurrentCategory()
    {
        RefreshCategory(_currentCategory);
    }
    public void OnClickStoreTab()
    {
        RefreshCategory(QuestType.Store);
        SoundManager.Instance.PlaySFX(_questTabButtonSFX);
        _StoreTabButtonClick.SetActive(true);
        _FishingTabButtonClick.SetActive(false);
        _GuideTabButtonClick.SetActive(false);
        _GrowthTabButtonClick.SetActive(false);
        _CompletedTabButtonClick.SetActive(false);

    }

    public void OnClickFishingTab()
    {
        RefreshCategory(QuestType.Fishing);
        SoundManager.Instance.PlaySFX(_questTabButtonSFX);
        _StoreTabButtonClick.SetActive(false);
        _FishingTabButtonClick.SetActive(true);
        _GuideTabButtonClick.SetActive(false);
        _GrowthTabButtonClick.SetActive(false);
        _CompletedTabButtonClick.SetActive(false);

    }

    public void OnClickGuideTab()
    {
        RefreshCategory(QuestType.Guide);
        SoundManager.Instance.PlaySFX(_questTabButtonSFX);
        _StoreTabButtonClick.SetActive(false);
        _FishingTabButtonClick.SetActive(false);
        _GuideTabButtonClick.SetActive(true);
        _GrowthTabButtonClick.SetActive(false);
        _CompletedTabButtonClick.SetActive(false);

    }

    public void OnClickGrowthTab()
    {
        RefreshCategory(QuestType.Growth);
        SoundManager.Instance.PlaySFX(_questTabButtonSFX);
        _StoreTabButtonClick.SetActive(false);
        _FishingTabButtonClick.SetActive(false);
        _GuideTabButtonClick.SetActive(false);
        _GrowthTabButtonClick.SetActive(true);
        _CompletedTabButtonClick.SetActive(false);

    }
    public void OnClickCompletedTab()  
    {
        RefreshCompleted();
        SoundManager.Instance.PlaySFX(_questTabButtonSFX);
        _StoreTabButtonClick.SetActive(false);
        _FishingTabButtonClick.SetActive(false);
        _GuideTabButtonClick.SetActive(false);
        _GrowthTabButtonClick.SetActive(false);
        _CompletedTabButtonClick.SetActive(true);

    }
    //public void OpenQuest()  //@@
    //{
    //    gameObject.SetActive(true);
    //    _GuidePanel.SetActive(false);
    //}
    //public void OpenGuide()
    //{
    //    gameObject.SetActive(false);
    //    _GuidePanel.SetActive(true);
    //}
    public void OpenRewardPanel(int id)
    {
        _isRewardPopupOpen = true;
        SoundManager.Instance.PlaySFX(_questRewardButtonSFX);
        Debug.Log($"OpenRewardPanel 호출됨, id={id}");
        QuestDataSO completedQuest = QuestManager.Instance.GetByQuestId(id);
        if (completedQuest == null) return;

        var reward1 = QuestManager.Instance.GetewardId(completedQuest.RewardItemId1);
        var reward2 = QuestManager.Instance.GetewardId(completedQuest.RewardItemId2);
        _rewardUIPanel.SetActive(true);

        foreach (var slot in _slots)
        {
            slot.RewardButtonOff();
        }

        _StoreTabButtonClick.SetActive(false);
        _FishingTabButtonClick.SetActive(false);
        _GuideTabButtonClick.SetActive(false);
        _GrowthTabButtonClick.SetActive(false);
        _CompletedTabButtonClick.SetActive(false);
        _StoreTabButton.interactable = false;
        _FishingTabButton.interactable = false;
        _GuideTabButton.interactable = false;
        _GrowthTabButton.interactable = false;
        _CompletedTabButton.interactable = false;
        _questScrollRect.enabled = false;
        _questScrollRect.StopMovement(); 
        if (reward1 != null)
        {
            _reward1Slot.SetActive(true);
            _rewardIcon1.sprite = reward1.CurrencyImgPath_Sprite;
            _rewardtext1.text = completedQuest.RewardCount1.ToString();
        }
        else
        {
            _reward1Slot.SetActive(false);
        }

        if (reward2 != null && completedQuest.RewardCount2 > 0)
        {
            _reward2Slot.SetActive(true);
            _rewardIcon2.sprite = reward2.CurrencyImgPath_Sprite;
            _rewardtext2.text = completedQuest.RewardCount2.ToString();
        }
        else
        {
            _reward2Slot.SetActive(false);
        }
    }
    public void CloseRewardPanel()
    {
        _isRewardPopupOpen = false;
        Debug.Log("CloseRewardPanel 호출됨");
        _rewardUIPanel.SetActive(false);
        RefreshCategory(_currentCategory);
        _StoreTabButton.interactable = true;
        _FishingTabButton.interactable = true;
        _GuideTabButton.interactable = true;
        _GrowthTabButton.interactable = true;
        _CompletedTabButton.interactable = true;
        _questScrollRect.enabled = true;
        switch (_currentCategory)
        {
            case QuestType.Store:
                _StoreTabButtonClick.SetActive(true);
                break;
            case QuestType.Fishing:
                _FishingTabButtonClick.SetActive(true);
                break;
            case QuestType.Guide:
                _GuideTabButtonClick.SetActive(true);
                break;
            case QuestType.Growth:
                _GrowthTabButtonClick.SetActive(true);
                break;
        }
    }
    public void OnExitdClick()
    {
        SoundManager.Instance.PlaySFX(_questCloseButtonSFX);
        if (_rewardUIPanel.activeSelf)
        {
            CloseRewardPanel();
            foreach (var slot in _slots)
            {
                slot.Refresh();
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
