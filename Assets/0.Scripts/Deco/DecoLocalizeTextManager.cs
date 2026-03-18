using UnityEngine;
using TMPro;

/// <summary>
/// 꾸미기 모드 텍스트 로컬라이징 (임시 하드코딩)
/// TODO: 스트링테이블 추가되면 LocalizationManager.GetString()으로 교체
/// </summary>
public class DecoTextManager : MonoBehaviour
{
    [Header("꾸미기 진입")]
    [SerializeField] private TextMeshProUGUI decoModeText;
    [SerializeField] private TextMeshProUGUI islandDecoText;

    [Header("상단 버튼")]
    [SerializeField] private TextMeshProUGUI recallAllText;
    [SerializeField] private TextMeshProUGUI resetText;
    [SerializeField] private TextMeshProUGUI saveText;

    [Header("상호 작용")]
    [SerializeField] private TextMeshProUGUI recallText;
    [SerializeField] private TextMeshProUGUI moveText;
    [SerializeField] private TextMeshProUGUI cancelText;
    [SerializeField] private TextMeshProUGUI rotateText;

    [Header("나가기 팝업")]
    [SerializeField] private TextMeshProUGUI popupMsgText;
    [SerializeField] private TextMeshProUGUI saveExitText;
    [SerializeField] private TextMeshProUGUI noSaveExitText;
    [SerializeField] private TextMeshProUGUI popupCancelText;

    private void OnEnable()
    {
        Apply();
        PlayerPrefsDataManager.OnLanguageChanged += Apply;
    }

    private void OnDisable()
    {
        PlayerPrefsDataManager.OnLanguageChanged -= Apply;
    }

    private void Apply()
    {
        bool kr = PlayerPrefsDataManager.Language == 0;

        // 모드 전환
        Set(decoModeText, kr ? "꾸미기 모드 ▼" : "Deco Mode ▼");
        Set(islandDecoText, kr ? "섬 꾸미기" : "Island Deco");
        // 상단 버튼
        Set(recallAllText, kr ? "전체 회수" : "Recall All");
        Set(resetText, kr ? "초기화" : "Reset");
        Set(saveText, kr ? "저장" : "Save");
        // 상호 작용 
        Set(recallText, kr ? "회수" : "Recall");
        Set(moveText, kr ? "이동" : "Move");
        Set(cancelText, kr ? "취소" : "Cancel");
        Set(rotateText, kr ? "회전" : "Rotate");
        // 나가기 팝업 
        Set(popupMsgText, kr ? "아직 배치가 저장되지 않았습니다\n저장 하시고 나가시겠습니까?"
                                 : "Changes not saved.\nSave before leaving?");
        Set(saveExitText, kr ? "저장하고 나가기" : "Save & Exit");
        Set(noSaveExitText, kr ? "저장하지 않고 나가기" : "Exit without Save");
        Set(popupCancelText, kr ? "취소" : "Cancel");
    }

    private void Set(TextMeshProUGUI tmp, string text)
    {
        if (tmp != null) tmp.text = text;
    }
}