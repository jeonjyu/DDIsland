using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreManager : Singleton<StoreManager>
{ 
    List<Enum> category = new List<Enum>();

    [SerializeField] GameObject storeListPanel;
    [SerializeField] GameObject buyAndSellPanel;

    List<GameObject> stores = new List<GameObject>();

    public StoreCat currentCat;

    public GameObject BuyAndSellPanel => buyAndSellPanel;

    void Start() 
    {
        // 초기 카테고리 설정
        currentCat = (StoreCat)1;
    }

    // 각 enum에 설정된 Description으로 리스트 반환
    public List<string> GetEnumList<T>(Array optionEnum) where T : Enum
    {
        List<string> lst = new List<string>(optionEnum.Length);

        foreach (T option in optionEnum)
            lst.Add(GetEnumDesc(option));

        return lst;
    }

    // 각 enum에 지정된 description 반환
    public string GetEnumDesc<T>(T value) where T : Enum
    {
        FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
        DescriptionAttribute description = fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;

        return description.Description;
    }


}
