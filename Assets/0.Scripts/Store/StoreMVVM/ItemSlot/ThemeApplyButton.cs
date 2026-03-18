using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeApplyButton : MonoBehaviour
{
    [SerializeField] Button applyBtn;
    [SerializeField] TMP_Text btnText;

    private void Awake()
    {
        applyBtn = GetComponent<Button>();
        btnText = GetComponent<TMP_Text>();
    }


}
