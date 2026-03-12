using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LakeDecoManagerV2 : MonoBehaviour
{
    [Header("호수 배경 장식물 Image")]
    [SerializeField] private Image deco1Image;   
    [SerializeField] private Image deco2Image;   

    // ID를 스프라이트로 매핑 (테스트 데이터에서 로드)
    private Dictionary<int, Sprite> spriteMap = new Dictionary<int, Sprite>();

    // 상점 연결용 이벤트 
    public UnityEvent<int, int> OnImageChanged;  // (slotIndex, itemId) 전달
    public UnityEvent<int> OnSlotReset;          // slotIndex 전달

    private void Start()
    {
        HideSlot(deco1Image);
        HideSlot(deco2Image);

        InitFromTestData();
        InitTestButtons();
    }

    // 테스트 데이터로 스프라이트 매핑 초기화
    // 나중에 실제 데이터 연결하면 이 함수만 교체
    private void InitFromTestData()
    {
        spriteMap = LakeDecoDataV2.GetSpriteMap();
    }

    // 핵심 메서드: 상점에서 이걸 호출하면 됨
    // slotIndex: 0=장식물1, 1=장식물2
    // itemId: 데이터 아이디
    //
    // 사용 예시:
    //   ChangeImage(0, 1001); 1번 슬롯에 아이템 1001 이미지 적용
    //   ChangeImage(1, 2003); 2번 슬롯에 아이템 2003 이미지 적용
    public bool ChangeImage(int slotIndex, int itemId)
    {
        Image target = GetSlotImage(slotIndex);
        if (target == null) 
            return false;
        if (!spriteMap.ContainsKey(itemId))
            return false; 

        target.sprite = spriteMap[itemId];
        ShowSlot(target); 
        OnImageChanged?.Invoke(slotIndex, itemId);
        return true;
    }
    // 상점에서 쉽게 불러오는 메서드 
    public void ResetSlot0() { ResetSlot(0); }
    public void ResetSlot1() { ResetSlot(1); }
    // 슬롯 0 (장식물 1번)
    public void ChangeSlot0_1001() { ChangeImage(0, 1001); }
    public void ChangeSlot0_1002() { ChangeImage(0, 1002); }
    public void ChangeSlot0_1003() { ChangeImage(0, 1003); }
    //public void ChangeSlot0_1004() { ChangeImage(0, 1004); }
    //public void ChangeSlot0_1005() { ChangeImage(0, 1005); }

    // 슬롯 1 (장식물 2번)
    public void ChangeSlot1_2001() { ChangeImage(1, 2001); }
    public void ChangeSlot1_2002() { ChangeImage(1, 2002); }
    public void ChangeSlot1_2003() { ChangeImage(1, 2003); }
    //public void ChangeSlot1_2004() { ChangeImage(1, 2004); }
    //public void ChangeSlot1_2005() { ChangeImage(1, 2005); }

    // 슬롯 해제  
    public void ResetSlot(int slotIndex)
    {
        Image target = GetSlotImage(slotIndex);
        if (target == null) return;

        target.sprite = null;
        HideSlot(target);
        OnSlotReset?.Invoke(slotIndex);
    }
    // 일괄 해제 
    public void ResetAll()
    {
        ResetSlot(0);
        ResetSlot(1);
    }


    private Image GetSlotImage(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0: return deco1Image;
            case 1: return deco2Image;
            default: return null;
        }
    }
    private void HideSlot(Image img)
    {
        if (img != null) img.enabled = false;
    }

    private void ShowSlot(Image img)
    {
        if (img != null) img.enabled = true;
    }


    #region 이하는 테스트용 
    [Header("테스트용 버튼")]
    [SerializeField] private Button[] slot0TestButtons; // 장식물 1번 버튼들
    [SerializeField] private Button[] slot1TestButtons; // 장식물 2번 버튼들
    [SerializeField] private Button resetButton; // 전체 리셋 
    private void InitTestButtons()
    {
        // 슬롯 0 버튼
        List<int> slot0Ids = LakeDecoDataV2.GetSlot0Ids();
        for (int i = 0; i < slot0TestButtons.Length && i < slot0Ids.Count; i++)
        {
            if (slot0TestButtons[i] == null) continue;
            int id = slot0Ids[i];
            slot0TestButtons[i].onClick.AddListener(() => ChangeImage(0, id));
        }

        // 슬롯 1 버튼
        List<int> slot1Ids = LakeDecoDataV2.GetSlot1Ids();
        for (int i = 0; i < slot1TestButtons.Length && i < slot1Ids.Count; i++)
        {
            if (slot1TestButtons[i] == null) continue;
            int id = slot1Ids[i];
            slot1TestButtons[i].onClick.AddListener(() => ChangeImage(1, id));
        }

        if (resetButton != null)
            resetButton.onClick.AddListener(() => ResetAll()); // 전체 리셋 
    }
    #endregion  
}