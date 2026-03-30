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
        ApplyBtn.onClick.AddListener(viewModel.ApplyTheme);
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
        // 장착중인 호수 테마인지 확인해서 isApplied 적용해주기
        bool isApplied = LakeItemManager.Instance.ThemeID == modelData.ID;
        if (viewModel.Model is null) Debug.Log("모델이 비었음");

        applyBtn.gameObject.SetActive(viewModel.Model.IsGained);
        applyBtn.interactable = !isApplied;

        // 현재 착용중인 아이템이 아니라면 버튼 텍스트 "적용"
        //if (btnText == null) btnText = applyBtn.GetComponent<TMP_Text>();

        btnText.text = isApplied ? "적용 중" : "적용";
    }

    protected override void OnViewModelPropChanged(object sender, PropertyChangedEventArgs e)
    {
        base.OnViewModelPropChanged(sender, e);
        switch(e.PropertyName)
        {
            case nameof(viewModel.IsApplied):
            SetBtn();
            break;
        } 
    }
}
