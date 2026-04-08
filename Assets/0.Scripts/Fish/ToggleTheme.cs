using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleTheme : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip UI")]
    public AquariumMgr _aquariumMgr;
    public GameObject _tooltipObject;
    public TMP_Text _tooltipText;

    private void Start()
    {
        _tooltipObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bool isLake = (_aquariumMgr.CurrentType == FishType.Lake);

        string tableKey = isLake ? "MainMessageChangeSea" : "MainMessageChangeLake";

        Debug.Log(tableKey + " 0");
        Debug.Log(LocalizationManager.Instance.GetString(tableKey) + " 1");

        _tooltipText.text = LocalizationManager.Instance.GetString(tableKey);

        _tooltipObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tooltipObject.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        _tooltipObject.SetActive(false);
    }

}
