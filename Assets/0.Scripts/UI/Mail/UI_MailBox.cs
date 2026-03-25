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

    private void OnEnable()
    {
#if UNITY_EDITOR
        MailManager.Instance.AddTestMail();
#endif
        RefreshMailList();
    }
    private void Awake()
    {
        if (_exitButton != null)
        {
            _exitButton.onClick.AddListener(CloseMailBox);
        }
    }

    public void RefreshMailList()
    {
        foreach (Transform child in _contentTransform)
        {
            Destroy(child.gameObject);
        }

        List<MailData> allMails = MailManager.Instance.GetAllMails();

        if (allMails.Count == 0)
        {
            _emptyMailText.gameObject.SetActive(true);
        }
        else
        {
            _emptyMailText.gameObject.SetActive(false);
        }

        foreach (MailData mail in allMails)
        {
            GameObject slotObj = Instantiate(_mailSlotPrefab, _contentTransform);
            UI_MailSlot mailSlot = slotObj.GetComponent<UI_MailSlot>();

            if (mailSlot != null)
            {
                mailSlot.MailSlot(mail, _mailPopup);
            }
        }
    }

    public void CloseMailBox()
    {
        gameObject.SetActive(false);
    }

}

