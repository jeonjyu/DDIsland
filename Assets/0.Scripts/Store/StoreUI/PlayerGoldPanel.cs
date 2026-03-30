using TMPro;
using UnityEngine;

public class PlayerGoldPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;

    private void OnEnable()
    {
        goldText.text = GameManager.Instance.PlayerGold.ToString();
        GameManager.Instance.OnGoldChanged += OnGoldChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGoldChanged -= OnGoldChanged;
    }

    private void OnGoldChanged(int currentGold)
    {
        goldText.text = currentGold.ToString();
    }
}
