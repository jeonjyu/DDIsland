using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _contentText;
    [SerializeField] private TMP_Text _cancelText;
    [SerializeField] private TMP_Text _confirmText;

    [Header("UI 버튼 연결")]
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Button _confirmButton;

    private Action _onConfirmAction;

    public void OpenPopup(string titleKey, string contentKey, string confirmKey, string cancleKey, Action onConfirm)
    {
        gameObject.SetActive(true);

        // 제목과 본문 내용을 매니저에서 가져와서 설정
        _titleText.text = LocalizationManager.Instance.GetString(titleKey);
        _contentText.text = LocalizationManager.Instance.GetString(contentKey);

        _cancelText.text = LocalizationManager.Instance.GetString(confirmKey);
        _confirmText.text = LocalizationManager.Instance.GetString(cancleKey);

        _onConfirmAction = onConfirm;

        _cancelButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.AddListener(ClosePopup);

        _confirmButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.AddListener(OnClickConfirm);
    }

    private void OnClickConfirm()
    {
        _onConfirmAction?.Invoke();

        ClosePopup();
    }

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
