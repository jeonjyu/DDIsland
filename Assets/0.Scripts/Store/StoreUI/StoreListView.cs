using System;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreListView : MonoBehaviour
{
    [SerializeField] GameObject storeListPanel;

    List<Button> stores = new List<Button>();

    StoreListViewModel viewModel;

    public List<Button> Stores => stores;

    void Awake()
    {
        viewModel = GetComponent<StoreListViewModel>();
        viewModel.PropertyChanged += OnStoreListViewModelChanged;
        SetStoreCat();
    }

    // 스토어 버튼 설정
    void SetStoreCat()
    {
        if (stores != null)
            stores.Clear();

        foreach (Button button in storeListPanel.GetComponentsInChildren<Button>())
            stores.Add(button);

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
    public void SetSelectedCatBtnColor(Button button, bool isSelected)
    {
        ColorBlock colorBlock = button.colors;

        if (isSelected)
        {
            colorBlock.normalColor = Color.lightGray;
            colorBlock.selectedColor = Color.lightGray;
        }
        else
        {
            colorBlock.normalColor = Color.white;
            colorBlock.selectedColor = Color.white;
        }


        button.colors = colorBlock;

        //Debug.Log(colorBlock.normalColor.GetHashCode());
    }

    private void OnStoreListViewModelChanged(object sender, PropertyChangedEventArgs e)
    {
        viewModel.Filter.UpdateFilter((Filter)viewModel.CurrentCat);
        switch (e.PropertyName)
        {
            case null:
            case "":
                //Debug.Log("전체 변경");
                viewModel.Filter.FilterSlots(0); // 카테고리가 변할때만 
                viewModel.Sort.SetOptions();
                viewModel.Sort.ApplySortPriority();
                break;
        }
        //viewModel.LoadSlotList();
        //Debug.Log("변경끝");
    }
}
