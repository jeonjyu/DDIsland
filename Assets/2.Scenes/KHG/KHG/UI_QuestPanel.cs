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

    [SerializeField] private GameObject _StoreTabButton;
    [SerializeField] private GameObject _FishingTabButton;
    [SerializeField] private GameObject _GuideTabButton;
    [SerializeField] private GameObject _GrowthTabButton;
    [SerializeField] private GameObject _CompletedTabButton;

    [SerializeField] private GameObject _GuidePanel;

    private List<UI_QuestSlot> _slots = new List<UI_QuestSlot>();
    private QuestType _currentCategory;

    private void OnEnable()
    {
        RefreshCategory(defaultCategory);
    }
    private void Awake()
    {
        _rewardUIPanel.SetActive(false);
    }
    void Update()
    {
       // if (Input.GetKeyDown(KeyCode.Escape))
       // {
       //     if (_rewardUIPanel.activeSelf)
       //     {
       //         CloseRewardPanel();
       //     }
       //     else
       //     {
       //         gameObject.SetActive(false);
       //     }
       // }
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
        _StoreTabButton.SetActive(true);
        _FishingTabButton.SetActive(false);
        _GuideTabButton.SetActive(false);
        _GrowthTabButton.SetActive(false);
        _CompletedTabButton.SetActive(false);

    }

    public void OnClickFishingTab()
    {
        RefreshCategory(QuestType.Fishing);
        _StoreTabButton.SetActive(false);
        _FishingTabButton.SetActive(true);
        _GuideTabButton.SetActive(false);
        _GrowthTabButton.SetActive(false);
        _CompletedTabButton.SetActive(false);

    }

    public void OnClickGuideTab()
    {
        RefreshCategory(QuestType.Guide);
        _StoreTabButton.SetActive(false);
        _FishingTabButton.SetActive(false);
        _GuideTabButton.SetActive(true);
        _GrowthTabButton.SetActive(false);
        _CompletedTabButton.SetActive(false);

    }

    public void OnClickGrowthTab()
    {
        RefreshCategory(QuestType.Growth);
        _StoreTabButton.SetActive(false);
        _FishingTabButton.SetActive(false);
        _GuideTabButton.SetActive(false);
        _GrowthTabButton.SetActive(true);
        _CompletedTabButton.SetActive(false);

    }
    public void OnClickCompletedTab()  
    {
        RefreshCompleted();
        _StoreTabButton.SetActive(false);
        _FishingTabButton.SetActive(false);
        _GuideTabButton.SetActive(false);
        _GrowthTabButton.SetActive(false);
        _CompletedTabButton.SetActive(true);

    }
    public void OpenQuest()  //@@
    {
        gameObject.SetActive(true);
        _GuidePanel.SetActive(false);
    }
    public void OpenGuide()
    {
        gameObject.SetActive(false);
        _GuidePanel.SetActive(true);
    }
    public void OpenRewardPanel(int id)
    {
        Debug.Log($"OpenRewardPanel 호출됨, id={id}");
        QuestDataSO completedQuest = QuestManager.Instance.GetByQuestId(id);
        if (completedQuest == null) return;

        var reward1 = QuestManager.Instance.GetewardId(completedQuest.RewardItemId1);
        var reward2 = QuestManager.Instance.GetewardId(completedQuest.RewardItemId2);
        _rewardUIPanel.SetActive(true);
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
        Debug.Log("CloseRewardPanel 호출됨");
        _rewardUIPanel.SetActive(false);
    }
    public void OnExitdClick()
    {
        if (_rewardUIPanel.activeSelf)
        {
            CloseRewardPanel();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
