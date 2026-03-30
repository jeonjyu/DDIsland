using TMPro;
using UnityEngine;

public class PlayerGoldPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;

    private void OnEnable()
    {
        goldText.text = GameManager.Instance.PlayerGold.ToString();
        GameManager.Instance.OnGoldChanged += Instance_OnGoldChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGoldChanged -= Instance_OnGoldChanged;
    }

    private void Instance_OnGoldChanged(int currentGold)
    {
        goldText.text = currentGold.ToString();
    }
}
