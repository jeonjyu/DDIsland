using System;
using UnityEngine;
using UnityEngine.UI;

public class FixBuildingButton : MonoBehaviour
{
    [SerializeField] private Button _selectButton;
    public void Setup(int itemId, Action<int> onClickAction)
    {
        _selectButton.onClick.RemoveAllListeners();
        _selectButton.onClick.AddListener(() => onClickAction(itemId));

        gameObject.SetActive(true); // 버튼 켜기
    }
}
