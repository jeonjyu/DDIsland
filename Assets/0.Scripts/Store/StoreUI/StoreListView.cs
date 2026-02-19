using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreListView : MonoBehaviour
{
    [SerializeField] GameObject storeListPanel;

    List<GameObject> stores = new List<GameObject>();

    StoreListViewModel viewModel;


    void Start()
    {
        viewModel = GetComponent<StoreListViewModel>();

        SetStoreCat();
    }

    // 스토어 버튼 설정
    void SetStoreCat()
    {
        if (stores != null)
            stores.Clear();

        foreach (Transform child in storeListPanel.transform)
            stores.Add(child.gameObject);

        List<string> catList = StoreManager.Instance.GetEnumList<StoreCat>(Enum.GetValues(typeof(StoreCat)));
        foreach (string item in catList)
        {
            int idx = catList.IndexOf(item);
    // 이름 변경
            stores[idx].GetComponentInChildren<TMP_Text>().text = item;
    // 버튼 이벤트 추가
            // 버튼에 모델을 현재 상점 카테고리로 설정하도록 하는 로직 추가
            stores[idx].GetComponent<Button>().onClick.AddListener(() =>
            {
                viewModel.UpdateCurrentCat(idx);
            });
        }
    }
}
