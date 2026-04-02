using TMPro;
using UnityEngine;

public class SlotMouseOver : Singleton<SlotMouseOver>
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject tooltip;
    public GameObject ToolTip => tooltip;

    public void OnMouseIn(RectTransform rt, string itemName)
    {
        tooltip.SetActive(true);
        text.text = itemName;
        tooltip.transform.position = rt.position + new Vector3(0, 50.0f, 0);
    }

    public void OnMouseOut()
    {
        tooltip.SetActive(false);
    }
}
