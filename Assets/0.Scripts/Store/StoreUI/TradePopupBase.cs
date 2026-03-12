using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 거래 완료 팝업
public class TradePopupBase : MonoBehaviour
{
    [SerializeField] Button exitButton;
    [SerializeField] TMP_Text goldText;

    private void OnEnable()
    {
        exitButton.onClick.AddListener(() => { this.gameObject.SetActive(false); });
        goldText.text = GameManager.Instance.PlayerGold.ToString();
    }
    private void OnDisable()
    {
        exitButton.onClick.RemoveAllListeners();
    }
}
