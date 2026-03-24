using System;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StoreListView : MonoBehaviour
{
    [SerializeField] GameObject storeListPanel;
    [SerializeField] Button StoreButton;

    List<Button> stores = new List<Button>();

    StoreListViewModel viewModel;
    //Color selectedColor; 
    //Color normalColor;

    [Header("카테고리가 선택되지 않았을 때 색상")]
    public Color normalColor;
    [Header("카테고리가 선택되었을 때 색상")]
    public Color selectedColor;

    public List<Button> Stores => stores;

    void Awake()
    {
        viewModel = GetComponent<StoreListViewModel>();
        viewModel.PropertyChanged += OnStoreListViewModelChanged;
        //normalColor = StoreButton.colors.normalColor;
        //selectedColor = StoreButton.colors.pressedColor;

        foreach (Button button in storeListPanel.transform.GetComponentsInChildren<Button>())
            stores.Add(button);

        SetStoreCat();
    }

    private void OnDisable()
    {
        foreach(Button button in stores)
        {
            SetSelectedCatBtnColor(button, false);
        }
    }

    // 스토어 버튼 설정
    void SetStoreCat()
    {
        List<string> catList = DescriptionExtracter.GetEnumList<StoreCat>(Enum.GetValues(typeof(StoreCat)));
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

    // 버튼 선택되었을 때 색상 변경
    public void SetSelectedCatBtnColor(Button button, bool isSelected)
    {
        ColorBlock colorBlock = button.colors;

        if (isSelected)
        {
            //Debug.Log("선택된 색");
            colorBlock.normalColor = selectedColor;
            colorBlock.selectedColor = selectedColor;
        }
        else
        {
            //Debug.Log("기본 색");
            colorBlock.normalColor = normalColor;
            colorBlock.selectedColor = normalColor;
        }


        button.colors = colorBlock;

        //Debug.Log(colorBlock.normalColor.GetHashCode());
    }

    private void OnStoreListViewModelChanged(object sender, PropertyChangedEventArgs e)
    {
        if(viewModel.CurrentCat != StoreCat.recipe)
        {
            viewModel.Filter.filterDrop.interactable = true;
            viewModel.Filter.UpdateFilter((Filter)viewModel.CurrentCat);
        }
        else
            viewModel.Filter.filterDrop.interactable = false;

        switch (e.PropertyName)
        {
            case null:
            case "":
                //Debug.Log("전체 변경");
                if (viewModel.CurrentCat != StoreCat.recipe)
                    viewModel.Filter.FilterSlots(0); // 카테고리가 변할때만 
                viewModel.Sort.SetOptions();
                viewModel.Sort.ApplySortPriority();
                break;
        }
        //viewModel.LoadSlotList();
        //Debug.Log("변경끝");
    }
}
