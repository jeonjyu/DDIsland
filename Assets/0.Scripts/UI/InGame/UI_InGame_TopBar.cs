using TMPro;
using UnityEngine;

public class UI_InGame_TopBar : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text lpText;

    private void ShowGoldToText(int currentGold)
    {
        goldText.text = currentGold.ToString();
    }

    private void ShowLpToText(int currentLpPiece)
    {
        lpText.text = currentLpPiece.ToString();
    }

    private void OnEnable()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.OnGoldChanged += ShowGoldToText;

        if (DataManager.Instance != null && DataManager.Instance.RecordDatabase != null)
            DataManager.Instance.RecordDatabase.OnLPPieceChanged += ShowLpToText;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGoldChanged -= ShowGoldToText;

        if (DataManager.Instance != null && DataManager.Instance.RecordDatabase != null)
            DataManager.Instance.RecordDatabase.OnLPPieceChanged -= ShowLpToText;
    }
}
