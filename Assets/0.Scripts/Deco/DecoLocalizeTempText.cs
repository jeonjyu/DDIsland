using UnityEngine;
using TMPro;

/// 꾸미기 모드 텍스트 로컬라이징 
public class DecoLocalizeTempText : MonoBehaviour
    //[SerializeField] private TextMeshProUGUI saveText;
{ 
    [Header("확인 팝업")]                                  
    [SerializeField] private TextMeshProUGUI confirmMsgText;
 
    // StringUI 테이블에서 가져오는 헬퍼
    private static string Get(string key)
    {
        try
        {
            return DataManager.Instance.StringUIDatabase.StringUIInfoData[key].ID_String;
        }
        catch
        {
            return key;
        }
    }
    // StringUI 매핑 없는 키는 String 테이블 직접
    private static string GetDirect(string key)
    {
        try
        {
            return LocalizationManager.Instance.GetString(key);
        }
        catch
        {
            return key;
        }
    }

    // 편집모드매니저에서 팝업 열 때 호출
    // 0=저장, 1=전체회수, 2=초기화
    public void SetConfirmMsg(int type)
    {
        string msg = type switch
        {
            // TODO: StringUI 매핑 추가되면 Get()으로 통일
            // 0 => Get("Interior_Global_SaveLayout_Btn_Text"), 
            0 => GetDirect("InteriorGlobalSaveLayoutBtnText"),    // 저장 
            1 => GetDirect("InteriorGlobalRemoveAllBtnText"),     // 전체회수 
            2 => GetDirect("InteriorGlobalResetLayoutPopupText"), // 초기화
            // 2 => Get("Interior_Global_ResetLayout_Btn_Text"), 
            _ => "null" // 폴백
        };

        if (confirmMsgText != null) confirmMsgText.text = msg;
    }
  
}