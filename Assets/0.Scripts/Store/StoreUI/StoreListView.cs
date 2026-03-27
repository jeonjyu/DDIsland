using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StoreListView : MonoBehaviour
{
    [SerializeField] GameObject storeListPanel;
    //[SerializeField] Button StoreButton;

    List<CategoryButton> stores = new List<CategoryButton>();

    StoreListViewModel viewModel;
    
    Color normalColor = new Color(0.831f, 0.706f, 0.600f, 1f);
    Color selectedColor = new Color(0.992f, 0.969f, 0.906f, 1f);

    public List<CategoryButton> Stores => stores;

    void Awake()
    {
        viewModel = GetComponent<StoreListViewModel>();
        viewModel.PropertyChanged += OnStoreListViewModelChanged;

        foreach (CategoryButton button in storeListPanel.transform.GetComponentsInChildren<CategoryButton>())
        {
            stores.Add(button);
        }

        SetStoreCat();
    }

    private void OnDisable()
    {
        foreach(CategoryButton button in stores)
        {
            SetSelectedCatBtnColor(button.CatBtn, false);
        }
    }

    // 스토어 버튼 설정
    void SetStoreCat()
    {
        List<string> catUiStr = DescriptionExtracter.GetEnumList<StoreCat>(Enum.GetValues(typeof(StoreCat)));
        foreach (string item in catUiStr)
        {
            int idx = catUiStr.IndexOf(item);

            // 이름 변경
            stores[idx].CatUiString.TextId = item;

            // 버튼 이벤트 추가
            // 버튼에 모델을 현재 상점 카테고리로 설정하도록 하는 로직 추가
            stores[idx].CatBtn.onClick.AddListener(() =>
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
            //Debug.Log(button.name + "선택된 색");
            colorBlock.normalColor = selectedColor;
            colorBlock.selectedColor = selectedColor;
        }
        else
        {
            //Debug.Log(button.name + "기본 색");
            colorBlock.normalColor = normalColor;
            colorBlock.selectedColor = normalColor;
        }


        button.colors = colorBlock;
    }

    private void OnStoreListViewModelChanged(object sender, PropertyChangedEventArgs e)
    {
        viewModel.Filter.filterDrop.value = 0;
        viewModel.Filter.filterDrop.interactable = viewModel.CurrentCat != StoreCat.recipe ? true : false;
        viewModel.Filter.UpdateFilter((Filter)viewModel.CurrentCat);
        
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
