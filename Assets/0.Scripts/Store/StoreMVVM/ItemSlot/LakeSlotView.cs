using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 획득한 아이템은 구매 팝업 여는 이벤트 빼고
// 장착중인 아이템은 장착 팝업 여는 이벤트 빼고

public class LakeSlotView : ItemSlotViewBase
{
    [SerializeField] Button applyBtn;
    [SerializeField] TMP_Text btnText;

    public Button ApplyBtn => applyBtn;
    public TMP_Text BtnText => btnText;


    protected override void Awake()
    {
        viewModel = GetComponent<LakeSlotViewModel>();
    }

    public override void Init()
    {
        base.Init();
        //Debug.Log("LakeSlotView : Init");

        modelData = viewModel.Model;

        if (modelData is null)
        {
            Debug.Log("LakeSlotView : 모델이 없음");

            ResetSlot();
            return;
        }
        _itemImage.sprite = modelData.ImgSprite;
        SetBtn();

        //Debug.Log("LakeSlotView : UI 갱신 완료 - " + modelData.ItemName);
    }



    // 고려 조건: 보유여부/착용여부
    public void SetBtn()
    {
        // 장착중인 호수 테마인지 확인해서 isApplied 적용해주기
        bool isApplied = false;

        applyBtn.gameObject.SetActive(viewModel.Model.IsGained);
        applyBtn.interactable = !isApplied;

    // 현재 착용중인 아이템이 아니라면 버튼 텍스트 "적용"
        if (btnText == null) btnText = applyBtn.GetComponent<TMP_Text>();
            
        btnText.text = isApplied ? "적용 중" : "적용";

    }


}
