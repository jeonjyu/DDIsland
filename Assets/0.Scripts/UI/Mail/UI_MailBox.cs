using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MailBox : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject _mailSlotPrefab;
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private TMP_Text _emptyMailText;
    [SerializeField] private UI_MailPopup _mailPopup;

    [Header("UI 버튼 연결")]
    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_Text _popupDelete;
    [SerializeField] private TMP_Text _popupClaim;

    [Header("팝업 연결")]
    [SerializeField] private UI_Popup _commonPopup;

    private void Awake()
    {
        if (_exitButton != null)
        {
            _exitButton.onClick.AddListener(CloseMailBox);
        }
    }

    private void OnEnable()
    {
        MailManager.Instance.OnMailUpdated += RefreshMailList;
        PlayerPrefsDataManager.OnLanguageChanged += RefreshLocalization;

        RefreshMailList();
        RefreshLocalization();
    }
    private void OnDisable()
    {
        MailManager.Instance.OnMailUpdated -= RefreshMailList;
        PlayerPrefsDataManager.OnLanguageChanged -= RefreshLocalization;
    }
    public void RefreshMailList()
    {
        foreach (Transform child in _contentTransform)
        {
            Destroy(child.gameObject);
        }

        List<MailData> allMails = MailManager.Instance.GetAllMails().ToList();
        allMails.Reverse();
        int visibleCount = 0;

        foreach (MailData mail in allMails)
        {
            if (MailManager.Instance.IsMailDeleted(mail._mailID)) continue;

            if (System.DateTime.TryParse(mail._expireDate, out System.DateTime expireDate))
            {
                if (System.DateTime.Now > expireDate) continue;
            }

            visibleCount++;

            GameObject slotObj = Instantiate(_mailSlotPrefab, _contentTransform);

            if (slotObj.TryGetComponent<UI_MailSlot>(out var mailSlot))
            {
                mailSlot.MailSlot(mail, _mailPopup);
            }
        }
        if (visibleCount == 0)
        {
            _emptyMailText.gameObject.SetActive(true);
        }
        else
        {
            _emptyMailText.gameObject.SetActive(false);
        }
    }

    private void RefreshLocalization()
    {
        if (_popupDelete != null)
            _popupDelete.text = LocalizationManager.Instance.GetString("InteriorPostBoxRemoveAllBtn");
        if (_popupClaim != null)
            _popupClaim.text = LocalizationManager.Instance.GetString("InteriorPostBoxClaimAllBtn");
        if (_emptyMailText != null)
            _emptyMailText.text = LocalizationManager.Instance.GetString("InteriorPostBoxMailEmpty");
    }

    public void CloseMailBox()
    {
        gameObject.SetActive(false);
    }

    public void OnClickDeleteAllButton()
    {
        _commonPopup.OpenPopup(
            "InteriorPostBoxDeleteAll",
            "InteriorPostBoxDeleteAllConfirmDesc",
            "InteriorPostBoxCancelBtn",
            "InteriorPostBoxRemoveAllBtn",
            () =>
            {
                // [확인]을 눌렀을 때 실행될 내용
                MailManager.Instance.DeleteAllReadMails(); // (실제 삭제 함수로 변경하세요)
                RefreshMailList();
            }
        );
    }

    public void OnClickAllClaim()
    {
        _commonPopup.OpenPopup(
            "InteriorPostBoxTitle",
            "InteriorPostBoxClaimAllConfirmDesc",
            "InteriorPostBoxCancelBtn",
            "InteriorPostBoxClaimAllBtn",
            () => { MailManager.Instance.ClaimAllRewards(); }
        );
    }
}

