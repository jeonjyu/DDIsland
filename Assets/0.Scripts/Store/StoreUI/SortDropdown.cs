using System;
using UnityEngine;
using System.ComponentModel;




public class SortDropdown : StoreDropdownBase
{
    void Start()
    {
        // 드롭다운 옵션 리스트 전달
        optionList = StoreManager.Instance.GetEnumList<StoreSort>(Enum.GetValues(typeof(StoreSort)));

        SetOptions();
    }

    // 드롭다운에 
}
