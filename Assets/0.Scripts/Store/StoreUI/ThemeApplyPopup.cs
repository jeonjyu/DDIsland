using System;
using UnityEngine;
using UnityEngine.UI;

// 매니저에 id 넘겨주는 이벤트 , 예 누르면 이벤트 보냄
public class ThemeApplyPopup : MonoBehaviour
{
    private IStoreItem _item;

    public Button ApplyBtn;

    public Action<int> OnApplyTheme;

    private void OnEnable()
    {
        _item = StoreManager.Instance.TradeModel;
        ApplyBtn.onClick.AddListener(ApplyThisTheme);
    }

    private void OnDisable()
    {
        ApplyBtn.onClick.RemoveAllListeners();
    }

    public void ApplyThisTheme()
    {
        LakeItemManager.Instance.ChangedLakeSlot(_item);
        Debug.Log("ThemeApplyPopup 테마 적용 완료");
        
        this.gameObject.SetActive(false);
    }
}
