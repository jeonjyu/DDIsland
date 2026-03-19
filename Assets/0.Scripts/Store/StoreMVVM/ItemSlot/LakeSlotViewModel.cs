using System;
using UnityEngine;

public class LakeSlotViewModel : ItemSlotViewModelBase
{
    LakeSlotView view;

    private void Awake()
    {
        view = GetComponent<LakeSlotView>();
    }

    private void OnEnable()
    {
        //Debug.Log("LakeSlotViewModel : OnEnable " + Model);
        view.ApplyBtn.onClick.AddListener(ApplyTheme);
    }

    private void OnDisable()
    {
        view.ApplyBtn.onClick?.RemoveListener(ApplyTheme);
    }

    public override void SetModel(IStoreItem model)
    {
        base.SetModel(model);
        Debug.Log(gameObject.name + " LakeSlotViewModel : SetModel " + model);

        if (view == null) view = GetComponent<LakeSlotView>();
        if(view != null) view.Init();

        Debug.Log(gameObject.name + " LakeSlotViewModel : SetModel " + model);
    }

    // 호수 아이템 적용
    // 적용 버튼이 눌리면 적용
    public void ApplyTheme()
    {
        Debug.Log("[LakeSlotView] ApplyTheme 팝업 열기");
        StoreManager.Instance.TradeModel = Model;
        LakeItemManager.Instance.themeApplyPopup.gameObject.SetActive(true);
    }
}
