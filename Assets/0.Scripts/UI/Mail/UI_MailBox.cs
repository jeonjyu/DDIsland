using System.Collections.Generic;
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
    [SerializeField] private Button _exitButton;

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

        RefreshMailList();
    }
    private void OnDisable()
    {
        MailManager.Instance.OnMailUpdated -= RefreshMailList;
    }
    public void RefreshMailList()
    {
        foreach (Transform child in _contentTransform)
        {
            Destroy(child.gameObject);
        }

        List<MailData> allMails = MailManager.Instance.GetAllMails();
        int visibleCount = 0;


        foreach (MailData mail in allMails)
        {
            if (MailManager.Instance.IsMailDeleted(mail._mailID)) continue;

            visibleCount++;

            GameObject slotObj = Instantiate(_mailSlotPrefab, _contentTransform);
            UI_MailSlot mailSlot = slotObj.GetComponent<UI_MailSlot>();

            if (mailSlot != null)
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

    public void CloseMailBox()
    {
        gameObject.SetActive(false);
    }

}

