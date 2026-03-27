using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MailSlot : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _contentText;
    [SerializeField] private TMP_Text _expireDateText;
    [SerializeField] private Image _rewardImage;
    [SerializeField] private TMP_Text _rewardCount;
    [SerializeField] private Button _slotButton;

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

        if (_titleText == null|| _contentText == null) return;

        // 끌고온 우편 데이터를 각각 슬롯에 집어 넣기
        _titleText.text = _data._title;
        _contentText.text = _data._content;
        _expireDateText.text = $"만료일: {_data._expireDate}";

        //만약 아이템이 있으면 아이템을 보여주는 아이콘을 활성화하고
        if (_data._rewardItemID > 0)
        {
            _rewardImage.gameObject.SetActive(true);

            if (MailManager.Instance.IsMailClaimed(_data._mailID))
            {
                _rewardCount.text = "0";
            }
            else
            {
                _rewardCount.text = $"{_data._rewardCount}";
            }

            _rewardImage.sprite = DataManager.Instance.CurrencyDatabase.CurrencyInfoData[_data._rewardItemID].CurrencyImgPath_Sprite;
        }
        //아니면 비활성화
        else
        {
            _rewardImage.gameObject.SetActive(false);
            _rewardCount.text = $"";
        }
        _slotButton.onClick.RemoveAllListeners();
        _slotButton.onClick.AddListener(OnClickSlot);

        if (_group != null)
        {
            if (MailManager.Instance.IsMailRead(_data._mailID))
            {
                _group.alpha = 0.5f;
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
    }

    private void OnDisable()
    {
        if (MailManager.Instance != null)
            MailManager.Instance.OnMailUpdated -= RefreshSlot;
    }

}
