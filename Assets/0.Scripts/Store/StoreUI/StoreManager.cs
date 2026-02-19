using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum StoreCat 
{
    [Description("인테리어")] interior = 1,
    [Description ("코스튬")] costume,
    [Description ("낚시")] fishing,
    [Description ("레시피")] recipe
}

public class StoreManager : Singleton<StoreManager>
{
    // 아이템 카탈로그 딕셔너리 모음
    List<Dictionary<int, StoreItemBase>> _totalItemData = new List<Dictionary<int, StoreItemBase>>();

    List<Enum> category = new List<Enum>();

    // 플레이어가 소유한 아이템 딕셔너리 모음
    List<Dictionary<int, StoreItemBase>> _playerItemData = new List<Dictionary<int, StoreItemBase>>();

    [SerializeField] GameObject storeListPanel;

    List<GameObject> stores = new List<GameObject>();

    public StoreCat currentCat;

    void Start() 
    {
        //stores = storeListPanel.gameObject.GetComponentsInChildren<GameObject>().ToList();

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
