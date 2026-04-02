using UnityEngine;
using UnityEngine.UI;

public class HelpButton : MonoBehaviour
{
    [Header("UI에 해당하는 도움말 버튼 열기")]
    public HelpLocation myLocation;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => HelpManager.Instance.OpenHelp(myLocation));
    }
}
