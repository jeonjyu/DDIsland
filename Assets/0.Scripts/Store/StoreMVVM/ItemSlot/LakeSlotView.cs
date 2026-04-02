using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 획득한 아이템은 구매 팝업 여는 이벤트 빼고
// 장착중인 아이템은 장착 팝업 여는 이벤트 빼고

public class LakeSlotView : ItemSlotViewBase<LakeSlotViewModel>
{
    [SerializeField] Button applyBtn;
    [SerializeField] TMP_Text btnText;
    [SerializeField] UI_ReplacingButtonText uiReplacing;

    public Button ApplyBtn => applyBtn;
    public TMP_Text BtnText => btnText;


    //protected override void Awake()
    //{
    //    viewModel = GetComponent<LakeSlotViewModel>() as LakeSlotViewModel;
    //}

    private void OnEnable()
    {
        //Debug.Log("LakeSlotViewModel : OnEnable " + Model);
        viewModel.PropertyChanged += OnViewModelPropChanged;
        ApplyBtn.onClick.AddListener(viewModel.OpenThemeApply);
    }

    private void OnDisable()
    {
        viewModel.PropertyChanged -= OnViewModelPropChanged;
        ApplyBtn.onClick?.RemoveAllListeners();
    }

    public override void Init()
    {
        base.Init();
        SetBtn();

        //Debug.Log("LakeSlotView : UI 갱신 완료 - " + modelData.ItemName);
    }



    // 고려 조건: 보유여부/착용여부
    public void SetBtn()
    {
        //Debug.Log(LakeItemManager.Instance.ThemeID == modelData.ID ? viewModel.ItemName  + " 적용중": viewModel.ItemName + " 적용중 아님");

        // 장착중인 호수 테마인지 확인해서 isApplied 적용해주기
        bool isApplied = LakeItemManager.Instance.ThemeID == modelData.ID;
        if (viewModel.Model is null) Debug.Log("모델이 비었음");

        applyBtn.gameObject.SetActive(viewModel.Model.IsGained);
        applyBtn.interactable = !isApplied;
        uiReplacing.SetTextRuntime(isApplied);
    }

    protected override void OnViewModelPropChanged(object sender, PropertyChangedEventArgs e)
    {
        base.OnViewModelPropChanged(sender, e);

        //Debug.Log("[LakeSlotView] " + e.PropertyName + " 프로퍼티 바뀜");

        switch (e.PropertyName)
        {
            case nameof(viewModel.IsApplied):
                //Debug.Log("[LakeSlotView] 테마 적용 여부 변경됨");
                StoreManager.Instance.sortDropdown.ApplySortPriority();
                StoreManager.Instance.StoreListVM.LoadSlotList();
                SetBtn();
                break;
            default:
                //Debug.Log("[LakeSlotView] " + e.PropertyName + " 프로퍼티 바뀜");
                break;
        }
    }
}
