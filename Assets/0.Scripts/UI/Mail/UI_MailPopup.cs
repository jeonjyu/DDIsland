using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MailPopup : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _contentText;
    [SerializeField] private TMP_Text _expireDateText;

    [Header("UI 버튼 연결")]
    [SerializeField] private Button _actionButton;         // 하단 중앙 버튼 (받기 or 닫기)
    [SerializeField] private TMP_Text _actionButtonText;

    [Header("보상 프리팹 연결")]
    [SerializeField] private GameObject _rewardPrefab;
    [SerializeField] private Transform _rewardContent;

    private MailData _currentData;


    public void OpenPopup(MailData mail)
    {
        _currentData = mail;
        gameObject.SetActive(true);

        bool isKorean = PlayerPrefsDataManager.Language == 0;

        if (_titleText != null)
        {
            _titleText.text = isKorean ? mail._title_kr : mail._title_en;
        }

        if (_contentText != null)
        {
            _contentText.text = isKorean ? mail._content_kr : mail._content_en;
        }


        if (DateTime.TryParse(mail._expireDate, out DateTime expireTime))
        {
            TimeSpan timeLeft = expireTime - DateTime.Now;

            if (timeLeft.TotalDays >= 1)
            {
                _expireDateText.text = isKorean ?
                    $"{(int)timeLeft.TotalDays}일 {(int)timeLeft.TotalHours % 24}시간 남음 " :
                    $"{(int)timeLeft.TotalDays}d {(int)timeLeft.TotalHours % 24}h left";
            }
            else if (timeLeft.TotalHours >= 1)
            {
                _expireDateText.text = isKorean ?
                    $"{(int)timeLeft.TotalHours}시간 남음" :
                    $"{(int)timeLeft.TotalHours}h left";
            }
           
            else
            {
                _expireDateText.text = isKorean ? "만료됨" : "Expired";
            }

        }
        else
        {
            _expireDateText.text = isKorean ? "무기한" : "No Expiration";
        }

        MailManager.Instance.MarkAsRead(_currentData._mailID);

        foreach (Transform child in _rewardContent)
        {
            Destroy(child.gameObject);
        }

        // 팝업 다중 보상 아이콘 생성
        if (!string.IsNullOrEmpty(_currentData._rewardItemID))
        {
            string[] itemIDs = _currentData._rewardItemID.Split(',');
            string[] counts = _currentData._rewardCount.Split(',');

            for (int i = 0; i < itemIDs.Length; i++)
            {
                if (int.TryParse(itemIDs[i].Trim(), out int itemID) && i < counts.Length && int.TryParse(counts[i].Trim(), out int count))
                {
                    GameObject obj = Instantiate(_rewardPrefab, _rewardContent);
                    if (obj.TryGetComponent<UI_Reward>(out var rewardIcon))
                    {
                        rewardIcon.Setup(itemID, count);
                    }
                }
            }
        }

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        _actionButton.onClick.RemoveAllListeners();

        bool isClaimed = MailManager.Instance.IsMailClaimed(_currentData._mailID);

        // 보상이 있고, 아직 수령하지 않았다면 -> '받기'
        if (!string.IsNullOrEmpty(_currentData._rewardItemID) && !isClaimed)
        {
            _actionButtonText.text = LocalizationManager.Instance.GetString("InteriorPostBoxClaimBtn");
            _actionButton.onClick.AddListener(OnClickClaim);
        }
        // 보상이 없거나 이미 수령했다면 -> '닫기'
        else
        {
            _actionButtonText.text = LocalizationManager.Instance.GetString("InteriorPostBoxCloseBtn");
            _actionButton.onClick.AddListener(ClosePopup);
        }
    }

    private void RefreshPopup()
    {
        if (_currentData != null && gameObject.activeSelf)
        {
            OpenPopup(_currentData);
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

    private void OnEnable()
    {
        PlayerPrefsDataManager.OnLanguageChanged += RefreshPopup;
    }

    private void OnDisable()
    {
        PlayerPrefsDataManager.OnLanguageChanged -= RefreshPopup;
    }
}
