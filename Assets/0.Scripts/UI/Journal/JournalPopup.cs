using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 도감 아이템창 팝업
public class JournalPopup : MonoBehaviour
{
    [Header("팝업창 UI")]
    [SerializeField] private Image itemImage;          // 이미지
    [SerializeField] private TextMeshProUGUI nameText; // 이름
    [SerializeField] private TextMeshProUGUI descText; // 설명
    [SerializeField] private TextMeshProUGUI infoText; // 정보
    [SerializeField] private Button exitButton;        // 닫기 

    private void Awake()
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(Hide);

        Hide();
    }

    
    // 팝업창 보이게 (JournalSlot에서 클릭 이벤트로 호출됨)
    public void Show(JournalDataLoader.JournalItemData data)
    {
        if (data == null || !data.IsUnlocked) return;

        gameObject.SetActive(true);

        if (nameText != null)
            nameText.text = data.ItemName;
        if (descText != null)
            descText.text = data.Description;
        if (itemImage != null && data.ItemSprite != null)
            itemImage.sprite = data.ItemSprite;
        if (infoText != null)
            infoText.text = BuildSpecialInfoText(data);
    }

    // 팝업 닫기
    public void Hide()
    {
        gameObject.SetActive(false);
    }

  
    // 카테고리별 팝업창 텍스트 생성
    private string BuildSpecialInfoText(JournalDataLoader.JournalItemData data)
    {
        if (data.SpecialInfo == null || data.SpecialInfo.Count == 0)
            return "";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        switch (data.Category)
        {
            case JournalCategory.Fish:
                AppendInfo(sb, data, "등급");
                AppendInfo(sb, data, "서식지");
                AppendInfo(sb, data, "계절");
                AppendInfo(sb, data, "최고 기록");
                break;

            case JournalCategory.Costume:
                AppendInfo(sb, data, "장착 파츠");
                break;

            case JournalCategory.Interior:
                AppendInfo(sb, data, "배치 공간");
                break;

            case JournalCategory.Food:
                AppendInfo(sb, data, "등급");
                AppendInfo(sb, data, "배고픔 지수");
                AppendInfo(sb, data, "둥둥 지수");
                break;

            case JournalCategory.Album:
                // TODO: JournalRecordDataSO 들어오면 아래 주석 해제
                // AppendInfo(sb, data, "테마");
                // AppendInfo(sb, data, "아티스트");
                // AppendInfo(sb, data, "재생 길이");
                break;
        }

        return sb.ToString().TrimEnd('\n');
    }


    // "항목명 : 값" 한 줄 추가
    private void AppendInfo(System.Text.StringBuilder sb, JournalDataLoader.JournalItemData data, string key)
    {
        if (data.SpecialInfo.TryGetValue(key, out string value))
        {
            sb.AppendLine($"{key} : {value}");
        }
    }

    private void OnDestroy()
    {
        if (exitButton != null)
            exitButton.onClick.RemoveListener(Hide);
    }
}