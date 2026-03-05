using TMPro;
using UnityEngine;

public class UI_InGame_TopBar : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;

    private void ShowGoldToText(int currentGold)
    {
        goldText.text = currentGold.ToString();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGoldChanged += ShowGoldToText;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGoldChanged -= ShowGoldToText;
    }
}
