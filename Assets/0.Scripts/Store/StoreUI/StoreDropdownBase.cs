using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreDropdownBase : MonoBehaviour
{
    [SerializeField] protected TMP_Dropdown dropdown;
    [SerializeField] protected StoreListViewModel storeListViewModel;

    protected List<string> optionList = new List<string>();

    public int SelectedOption => dropdown.value;

    public void OnEnable()
    {
        dropdown.onValueChanged.AddListener(OnDropdownValueChagned);
    }

    public virtual void SetOptions()
    {
        //Debug.Log("[StoreDropdownBase] SetOptions");

        if (dropdown != null)
        {
            dropdown.ClearOptions();
        }

        foreach (var option in optionList)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(option.ToString()));
        }
    }

    // 드롭다운 값이 변경될 경우 실행되어야 할 메서드
    public virtual void OnDropdownValueChagned(int index)
    {
        dropdown.value = index;

    }

    private void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(OnDropdownValueChagned);
    }
}
