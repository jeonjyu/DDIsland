using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MailSlot : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _contentText;
    [SerializeField] private TMP_Text _expireDateText;
    [SerializeField] private Button _slotButton;
    [Header("보상 프리팹 연결")]
    [SerializeField] private GameObject _rewardPrefab;
    [SerializeField] private Transform _rewardContent;

    private MailData _data;
    private UI_MailPopup _popup;
    private CanvasGroup _group;

    private void Awake()
    {
        _group = GetComponent<CanvasGroup>();
    }

    public void MailSlot(MailData mail, UI_MailPopup popup)
    {
        // 우편 데이터 매개변수로 끌고오기
        _data = mail;
        _popup = popup;

        if (_titleText == null || _contentText == null) return;

        bool isKorean = PlayerPrefsDataManager.Language == 0;

        // 끌고온 우편 데이터를 각각 슬롯에 집어 넣기
        if (isKorean) 
        {
            _titleText.text = _data._title_kr;
            _contentText.text = _data._content_kr;
        }
        else
        {
            _titleText.text = _data._title_en;
            _contentText.text = _data._content_en;
        }
        if (DateTime.TryParse(_data._expireDate, out DateTime expireTime))
        {
            TimeSpan timeLeft = expireTime - DateTime.Now;

            if (timeLeft.TotalDays >= 1)
            {
                _expireDateText.text = isKorean ?
                    $"{(int)timeLeft.TotalDays}일 {(int)timeLeft.TotalHours%24}시간 남음 " :
                    $"{(int)timeLeft.TotalDays}d {(int)timeLeft.TotalHours%24}h left";
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

        foreach (Transform child in _rewardContent)
        {
            Destroy(child.gameObject);
        }

        //만약 아이템이 있으면 아이템을 보여주는 아이콘을 활성화하고
        if (!string.IsNullOrEmpty(_data._rewardItemID))
        {
            string[] itemIDs = _data._rewardItemID.Split(',');
            string[] counts = _data._rewardCount.Split(',');

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

        _slotButton.onClick.RemoveAllListeners();
        _slotButton.onClick.AddListener(OnClickSlot);

        if (_group != null)
        {
            if (MailManager.Instance.IsMailRead(_data._mailID))
            {
                if (!string.IsNullOrEmpty(_data._rewardItemID) && !MailManager.Instance.IsMailClaimed(_data._mailID))
                {
                    _group.alpha = 1f;
                }
                else
                {
                    _group.alpha = 0.5f;
                }
            }
            else
            {
                _group.alpha = 1.0f;
            }
        }
    }

    // 만약 이미 받은 우편이라면
    private void OnClickSlot()
    {
        if (_popup != null)
        {
            _popup.OpenPopup(_data);
        }
    }

    private void RefreshSlot()
    {
        if (_data != null)
        {
            MailSlot(_data, _popup);
        }
    }

    private void OnEnable()
    {
        MailManager.Instance.OnMailUpdated += RefreshSlot;
        PlayerPrefsDataManager.OnLanguageChanged += RefreshSlot;
    }

    private void OnDisable()
    {
        if (MailManager.Instance != null)
            MailManager.Instance.OnMailUpdated -= RefreshSlot;

        PlayerPrefsDataManager.OnLanguageChanged -= RefreshSlot;
    }

}
