using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 도감 슬롯 하나의 UI 처리
/// 고정 슬롯에 이미지 교체 방식
/// IsUnlocked에 따라 컬러/실루엣 전환
/// </summary>
public class JournalSlot : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private Image itemImage;          // 아이템 이미지
    [SerializeField] private Image backgroundImage;    // 슬롯 배경
    [SerializeField] private TextMeshProUGUI nameText; // 아이템 이름
    [SerializeField] private Button slotButton;        // 클릭 버튼

    [Header("실루엣 셰이더")]
    [SerializeField] private Material silhouetteMaterial; // 검은색 실루엣 머테리얼

    // 기획서 색상 코드
    private readonly Color UNLOCKEDBGCOLOR = new Color32(0xFD, 0xF7, 0xE7, 0xFF); // #FDF7E7
    private readonly Color LOCKEDBGCOLOR = new Color32(0x98, 0x8F, 0x73, 0xFF);    // #988F73

    private JournalDataLoader.JournalItemData itemData;
    private Material defaultMaterial; // 원본 머테리얼 저장용


    // 슬롯 클릭 시 발생하는 이벤트, 해금된 아이템만 호출됨
    public event Action<JournalDataLoader.JournalItemData> OnSlotClicked;

    private void Awake()
    {
        if (itemImage != null)
            defaultMaterial = itemImage.material;

        if (slotButton != null)
            slotButton.onClick.AddListener(HandleClick);
    }


    // 슬롯 데이터 세팅 (dataLoader에서 받은 JournalItemData로 이미지, 이름 교체)
    public void Setup(JournalDataLoader.JournalItemData data)
    {
        itemData = data;

        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        // 스프라이트 로드
        if (itemImage != null && data.SlotSprite != null)
        {
            itemImage.sprite = data.SlotSprite;
        }

        if (data.IsUnlocked)
        {
            SetUnlockedState();
        }
        else
        {
            SetLockedState();
        }
    }


    // 해금 상태 UI 적용 
    private void SetUnlockedState()
    {
        if (backgroundImage != null) // 뒷배경 
            backgroundImage.color = UNLOCKEDBGCOLOR;

        if (itemImage != null) // 이미지 
          //  itemImage.material = defaultMaterial; // 원본 머테리얼 
          itemImage.color = Color.white;
        if (nameText != null) // 이름 
            nameText.text = itemData.ItemName;
    }

    // 미해금 상태 UI 적용
    private void SetLockedState()
    {
        if (backgroundImage != null) // 뒷배경 
            backgroundImage.color = LOCKEDBGCOLOR;

        if (itemImage != null && silhouetteMaterial != null) // 이미지 
           // itemImage.material = silhouetteMaterial; // 실루엣 셰이더
            itemImage.color = Color.black;
       
        if (nameText != null) // 이름 폴백 
            nameText.text = "???";
    }

   
    // 아이템 팝업창 전용 슬롯 클릭 처리
    private void HandleClick()
    {
        if (itemData == null) return;

        // 미해금이면 무시
        if (!itemData.IsUnlocked) return;
        // 해금된 아이템만 팝업창 호출
        OnSlotClicked?.Invoke(itemData);
    }

    private void OnDestroy()
    {
        if (slotButton != null)
            slotButton.onClick.RemoveListener(HandleClick);
    }
}
