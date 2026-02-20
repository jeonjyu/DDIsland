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

    StoreCat currentCat;

    void Start() 
    {
        //stores = storeListPanel.gameObject.GetComponentsInChildren<GameObject>().ToList();
        SetStoreCat();

        // 초기 카테고리 설정
        currentCat = (StoreCat)1;
    }

    // 스토어 버튼 설정
    // 이름 변경
    // 버튼 이벤트 추가
    void SetStoreCat()
    {
        if(stores != null)
            stores.Clear();

        foreach (Transform child in storeListPanel.transform)
            stores.Add(child.gameObject);

        List<string> catList = GetEnumList<StoreCat>(Enum.GetValues(typeof(StoreCat)));
        foreach(string item in catList)
        {
            int idx = catList.IndexOf(item);
            stores[idx].GetComponentInChildren<TMP_Text>().text = item;
            // 버튼에 현재 상점 카테고리 설정하도록 하는 로직 추가
            stores[idx].GetComponent<Button>().onClick.AddListener(() =>
            {
                SetCurrentCat(idx);
            });
        }
    }

    public void SetCurrentCat(int catIdx)
    {
        currentCat = (StoreCat)catIdx + 1;
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
