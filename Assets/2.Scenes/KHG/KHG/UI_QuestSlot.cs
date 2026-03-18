using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestSlot : MonoBehaviour
{
    [SerializeField] private UI_QuestPanel _questPanel;
    [SerializeField] private Image _rewardIcon1;
    [SerializeField] private Image _rewardIcon2;
    [SerializeField] private TMP_Text _questNameText;
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private Button _rewardButton;
    [SerializeField] private TMP_Text _rewardButtonText;
    [SerializeField] private Slider _questSlider;

    private int _questId;
    private bool _isCompletedView;

    public void OnEnable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestAddValue += Refresh;
    }
    private void OnDisable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestAddValue -= Refresh;
    }
    public void Bind(int questId, bool isCompletedView = false)
    {
        _questId = questId;
        _isCompletedView = isCompletedView;
        Refresh();
    }
    public void Refresh()
    {
        QuestDataSO quest = QuestManager.Instance.GetByQuestId(_questId);
        if (quest == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        string title = quest.QuestName_String;
        if (string.IsNullOrEmpty(title)) _questNameText.text = "";
        else _questNameText.text = title.Replace("{0}", quest.Requirement.ToString());
       
        Debug.Log($"QuestName key/result: {quest.QuestName_String}");
        var reward1 = QuestManager.Instance.GetewardId(quest.RewardItemId1);
        var reward2 = QuestManager.Instance.GetewardId(quest.RewardItemId2);

        if (reward1 != null) _rewardIcon1.sprite = reward1.CurrencyImgPath_Sprite;

        if (reward2 != null)
        {
            _rewardIcon2.gameObject.SetActive(true);
            _rewardIcon2.sprite = reward2.CurrencyImgPath_Sprite;
        }
        else
        {
            _rewardIcon2.gameObject.SetActive(false);
        }

        QuestStateType state = QuestManager.Instance.GetQuestState(_questId);
        int current = QuestManager.Instance.GetQuestCurrentProgress(_questId);
        int requirement = quest.Requirement;

        _questSlider.maxValue = requirement;

        if (_isCompletedView)
        {
            _rewardButton.gameObject.SetActive(false);
            _questSlider.value = requirement;
            _progressText.text = "100%";
            return;
        }

        if (state == QuestStateType.Completed)
        {
            gameObject.SetActive(false);
            return;
        }

        _rewardButton.gameObject.SetActive(true);
        _rewardButton.interactable = (state == QuestStateType.Claimable);
        _rewardButtonText.text = state == QuestStateType.Claimable ? "받기" : "진행중";

        _questSlider.value = current;
        _progressText.text = $"{current}/{requirement}";
    }
    public void OnClickReward()
    {
        bool success = QuestManager.Instance.GiveQuestReward(_questId);
        if (success)
        {
            _questPanel.OpenRewardPanel(_questId);
            _questPanel.RefreshCurrentCategory();
        }
    }
}
