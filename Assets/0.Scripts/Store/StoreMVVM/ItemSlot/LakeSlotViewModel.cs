using System;
using UnityEngine;

public class LakeSlotViewModel : ItemSlotViewModelBase
{
    private bool _isApplied;

    public bool IsApplied
    {
        get => _isApplied;
        set
        {
            if(_isApplied != value)
            {
                _isApplied = value;
                OnPropertyChanged(nameof(IsApplied));
            }
        }
    }

    // 호수 아이템 적용
    // 적용 버튼이 눌리면 적용
    public void ApplyTheme()
    {
        LakeItemManager.Instance.themeApplyPopup.OnApplyTheme += () => IsApplied = true;
        Debug.Log("[LakeSlotView] ApplyTheme 팝업 열기");
        StoreManager.Instance.TradeModel = Model;
        LakeItemManager.Instance.themeApplyPopup.gameObject.SetActive(true);
    }
}
