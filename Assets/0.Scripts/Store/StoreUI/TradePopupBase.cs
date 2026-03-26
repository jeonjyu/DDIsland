using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 거래 완료 팝업
public class TradePopupBase : MonoBehaviour
{
    [SerializeField] Button exitButton;
    [SerializeField] TMP_Text goldText;
    CanvasGroup canvasGroup;

    private void OnEnable()
    {
        exitButton.onClick.AddListener(() => { this.gameObject.SetActive(false); });
        goldText.text = GameManager.Instance.PlayerGold.ToString();
        canvasGroup = StoreManager.Instance.BuyAndSellPanel.CanvasGroup;
        if(StoreManager.Instance.BuyAndSellPanel)
        {
            Debug.Log(StoreManager.Instance.BuyAndSellPanel.MyCanvasGroup.gameObject.name);
            StoreManager.Instance.BuyAndSellPanel.MyCanvasGroup.interactable = false;
            StoreManager.Instance.BuyAndSellPanel.MyCanvasGroup.blocksRaycasts = false;
        }
    }

    private void OnDisable()
    {
        StoreManager.Instance.BuyAndSellPanel.MyCanvasGroup.interactable = true;
        StoreManager.Instance.BuyAndSellPanel.MyCanvasGroup.blocksRaycasts = true;
        exitButton.onClick.RemoveAllListeners();
    }
}
