using UnityEngine;
using TMPro;

/// 꾸미기 모드 텍스트 로컬라이징 
public class DecoLocalizeTempText : MonoBehaviour
{
    //[Header("꾸미기 진입")]
    //[SerializeField] private TextMeshProUGUI decoModeText;
    //[SerializeField] private TextMeshProUGUI islandDecoText;

    //[Header("상단 버튼")]
    //[SerializeField] private TextMeshProUGUI recallAllText;
    //[SerializeField] private TextMeshProUGUI resetText;
    //[SerializeField] private TextMeshProUGUI saveText;

    //[Header("상호 작용")]
    //[SerializeField] private TextMeshProUGUI recallText;
    //[SerializeField] private TextMeshProUGUI moveText;
    //[SerializeField] private TextMeshProUGUI cancelText;
    //[SerializeField] private TextMeshProUGUI rotateText;

    //[Header("나가기 팝업")]
    //[SerializeField] private TextMeshProUGUI popupMsgText;
    //[SerializeField] private TextMeshProUGUI saveExitText;
    //[SerializeField] private TextMeshProUGUI noSaveExitText;
    //[SerializeField] private TextMeshProUGUI popupCancelText;

    [Header("확인 팝업")]                                  
    [SerializeField] private TextMeshProUGUI confirmMsgText;
    //    [SerializeField] private TextMeshProUGUI confirmYesText; 


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
    //private void OnEnable()
    //{
    //    Apply();
    //    PlayerPrefsDataManager.OnLanguageChanged += Apply;
    //}

    //private void OnDisable()
    //{
    //    PlayerPrefsDataManager.OnLanguageChanged -= Apply;
    //}

    //private void Apply()
    //{
    //    bool kr = PlayerPrefsDataManager.Language == 0;

    //    // 모드 전환
    //    Set(decoModeText, kr ? "꾸미기 모드 ▼" : "Deco Mode ▼");
    //    Set(islandDecoText, kr ? "섬 꾸미기" : "Island Deco");
    //    // 상단 버튼
    //    Set(recallAllText, kr ? "전체 회수" : "Recall All");
    //    Set(resetText, kr ? "초기화" : "Reset");
    //    Set(saveText, kr ? "저장" : "Save");
    //    // 상호 작용 
    //    Set(recallText, kr ? "회수" : "Recall");
    //    Set(moveText, kr ? "이동" : "Move");
    //    Set(cancelText, kr ? "취소" : "Cancel");
    //    Set(rotateText, kr ? "회전" : "Rotate");
    //    // 나가기 팝업 
    //    Set(popupMsgText, kr ? "아직 배치가 저장되지 않았습니다\n저장 하시고 나가시겠습니까?"
    //                             : "Changes not saved.\nSave before leaving?");
    //    Set(saveExitText, kr ? "저장하고 나가기" : "Save & Exit");
    //    Set(noSaveExitText, kr ? "저장하지 않고 나가기" : "Exit without Save");
    //    Set(popupCancelText, kr ? "취소" : "Cancel");

    //    Set(confirmYesText, kr ? "예" : "Yes"); // 확인창 공용               
    //}

    //private void Set(TextMeshProUGUI tmp, string text)
    //{
    //    if (tmp != null) tmp.text = text;
    //}


    // 편집모드매니저에서 팝업 열 때 호출
    // 0=저장, 1=전체회수, 2=초기화
    public void SetConfirmMsg(int type)
    {
        //bool kr = PlayerPrefsDataManager.Language == 0;

        string msg = type switch
        {
            //0 => kr ? "배치 내용을 저장하시겠습니까?" : "Save the current layout?",
            //1 => kr ? "초기화 진행 시 배치된 인테리어는\n전부 회수됩니다. 계속하시겠습니까?"
            //        : "All placed interiors will be recalled.\nContinue?",
            //2 => kr ? "마지막 저장 상태로 되돌리시겠습니까?" : "Revert to last saved state?",

            // 0 => Get("Interior_Global_SaveLayout_Btn_Text"),  // 배치 내용을 저장하시겠습니까?
            0 => GetDirect("InteriorGlobalSaveLayoutBtnText"),
            1 => GetDirect("InteriorGlobalRemoveAllBtnText"), // TODO: StringUI 매핑 추가되면 Get()으로 통일
            2 => GetDirect("InteriorGlobalResetLayoutBtnText"),
            // 2 => Get("Interior_Global_ResetLayout_Btn_Text"), // 마지막 저장 상태로 되돌리시겠습니까?
            _ => ""
        };

        // Set(confirmMsgText, msg);
        if (confirmMsgText != null) confirmMsgText.text = msg;
    }
  
}