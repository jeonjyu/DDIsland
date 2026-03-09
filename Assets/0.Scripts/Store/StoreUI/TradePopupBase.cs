using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 거래 팝업 UI 베이스
// 인테리어에서 사용, 코스튬, 낚시 등에서 변경해서 사용할 예정
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
