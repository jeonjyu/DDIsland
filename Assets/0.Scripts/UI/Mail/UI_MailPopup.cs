using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MailPopup : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _contentText;
    [SerializeField] private TMP_Text _expireDateText;
    [SerializeField] private Image _rewardImage;
    [SerializeField] private TMP_Text _rewardCountText;

    [Header("UI 버튼 연결")]
    [SerializeField] private Button _actionButton;         // 하단 중앙 버튼 (받기 or 닫기)
    [SerializeField] private TMP_Text _actionButtonText;

    private MailData _currentData;


    public void OpenPopup(MailData mail)
    {
        _currentData = mail;
        gameObject.SetActive(true);

        _titleText.text = _currentData._title;
        _contentText.text = _currentData._content;
        _expireDateText.text = $"만료일: {_currentData._expireDate}";

        MailManager.Instance.MarkAsRead(_currentData._mailID);

        if (_currentData._rewardItemID > 0)
        {
            _rewardImage.gameObject.SetActive(true);
            _rewardImage.sprite = DataManager.Instance.CurrencyDatabase.CurrencyInfoData[_currentData._rewardItemID].CurrencyImgPath_Sprite;
            if (MailManager.Instance.IsMailClaimed(_currentData._mailID))
            {
                _rewardCountText.text = "0";
            }
            else
            {
                _rewardCountText.text = $"{_currentData._rewardCount}";
            }
        }
        else
        {
            _rewardImage.gameObject.SetActive(false);
            _rewardCountText.text = "";
        }

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        _actionButton.onClick.RemoveAllListeners();

        bool isClaimed = MailManager.Instance.IsMailClaimed(_currentData._mailID);

        // 보상이 있고, 아직 수령하지 않았다면 -> '받기'
        if (_currentData._rewardItemID > 0 && !isClaimed)
        {
            _actionButtonText.text = "받기";
            _actionButton.onClick.AddListener(OnClickClaim);
        }
        // 보상이 없거나 이미 수령했다면 -> '닫기'
        else
        {
            _actionButtonText.text = "닫기";
            _actionButton.onClick.AddListener(ClosePopup);
        }
    }

    private void OnClickClaim()
    {
        MailManager.Instance.ClaimReward(_currentData);
        OpenPopup(_currentData);
        UpdateButtonState();
        ClosePopup();
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
