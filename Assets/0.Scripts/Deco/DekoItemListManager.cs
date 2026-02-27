using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

/// [꾸미기 모드] 인벤 관리 
/// 호수, 섬 둘다 SetupInventory()로 데이터만 넣어주면 됨
public enum DecoMode { Lake = 0, Island = 1 }

public class DecoItemListManager : MonoBehaviour
{
    [Header("UI 연결")]
    public RectTransform itemContent; // 슬롯들의 부모
    public Button btnArrowLeft;       // 좌측 화살표
    public Button btnArrowRight;      // 우측 화살표

    [Header("슬롯 템플릿")]
    public GameObject slotTemplate;   // 슬롯 하나 복사하여 생성

    [Header("페이지 카운트")]
    public RectTransform pageCountParent;   // 점들의 부모 오브젝트
    public Color pageCountOn = Color.white;               // 현재 페이지 ●
    public Color pageCountOff = new Color(1, 1, 1, 0.3f); // 다른 페이지 ○

    [Header("페이지 설정")]
    public int slotsPerPage = 6; // 한 페이지에 보이는 최대 슬롯 수
    public int maxPages = 3;     // 최대 페이지 수

    public DecoMode currentMode = DecoMode.Lake; // 현재 편집 모드, 디폴트는 호수 


    // 내부 변수
    List<LakeInvenSlot> inventoryData = new List<LakeInvenSlot>();
    List<GameObject> slotObjects = new List<GameObject>();
    List<DecoSlotUI> slotUIs = new List<DecoSlotUI>();
    List<GameObject> pageCountDots = new List<GameObject>();
    int selectedIndex = -1;  // 현재 선택된 슬롯 인덱스 (-1이면 선택 없음)
    int currentPage = 0;     // 현재 페이지 (0부터)
    int totalPages = 1;      // 전체 페이지 수

    // 아이템 선택 이벤트
    public event Action<int, int> OnItemSelected;   // (itemId, slotIndex)
    public event Action OnItemDeselected;           // 선택 해제

    void Start()
    {
        // 화살표 버튼 연결
        if (btnArrowLeft != null)
            btnArrowLeft.onClick.AddListener(GoToPrevPage);

        if (btnArrowRight != null)
            btnArrowRight.onClick.AddListener(GoToNextPage);

        // 템플릿 슬롯 비활성화 (복제용으로만 사용)
        if (slotTemplate != null)
            slotTemplate.SetActive(false);
    }

    // 테스트용 (데이터 연결 전까지 빈 인벤으로 세팅)
    public void SetupTestInventory()
    {
        // 테스트용 (데이터 연결 전까지 LakeDecoTestData의 더미 인벤 사용)
        if (currentMode == DecoMode.Lake)
            SetupInventory(LakeDecoTestData.CreateTestInventory());
        // TODO: 외부에서 SetupInventory() 직접 호출 
        // else if (currentMode == DecoMode.Island)
        //     SetupInventory(IslandDecoTestData.CreateTestInventory());
        else
            SetupInventory(new List<LakeInvenSlot>()); // 없으면 빈 인벤

    }

    // 인벤토리 세팅 
    // 인벤토리 데이터 넣고 슬롯 생성 (편집 모드 진입 시 호출)
    public void SetupInventory(List<LakeInvenSlot> inventory)
    {
        inventoryData = inventory;
        selectedIndex = -1;
        currentPage = 0;

        ClearSlots();
        CreateSlots();
        RecalcPages();
        UpdatePageDisplay();
    }

    // 기존 슬롯 전부 삭제
    void ClearSlots()
    {
        for (int i = 0; i < slotObjects.Count; i++)
        {
            Destroy(slotObjects[i]);
        }
        slotObjects.Clear();
        slotUIs.Clear();
    }

    // 인벤토리 데이터 기반으로 슬롯 동적 생성 (수량 0인건 안 만듦)
    void CreateSlots()
    {
        for (int i = 0; i < inventoryData.Count; i++)
        {
            LakeInvenSlot slotData = inventoryData[i];

            // 수량 0이면 생성 안함
            if (slotData.quantity <= 0) continue;

            // 템플릿 복제
            GameObject slotObj = Instantiate(slotTemplate, itemContent);
            slotObj.SetActive(false); // 페이지 전환에서 켜줌
            slotObj.name = "ItemSlot_" + i;

            // DecoSlotUI 컴포넌트에서 내용 참조 
            DecoSlotUI slotUI = slotObj.GetComponent<DecoSlotUI>();
            // 슬롯 내용 채우기
            SetupSlotUI(slotUI, slotData);

            // 클릭 이벤트 연결용
            int slotIndex = i; // 클로저 캡처용
            if (slotUI != null && slotUI.itemButton != null)
                slotUI.itemButton.onClick.AddListener(() => OnSlotClicked(slotIndex));

            slotObjects.Add(slotObj);
            slotUIs.Add(slotUI);
        }
    }

    // 슬롯 UI 
    void SetupSlotUI(DecoSlotUI slotUI, LakeInvenSlot slotData)
    {
        if (slotUI == null) return;
        // 수량 텍스트
        if (slotUI.quantityText != null)
            slotUI.quantityText.text = "x" + slotData.quantity;

        // 아이템 이름
        if (slotUI.nameText != null)
        {
            if (currentMode == DecoMode.Lake)
                slotUI.nameText.text = LakeDecoTestData.GetItemName(slotData.itemId);
            // else if (currentMode == DecoMode.Island)
            // slotUI.nameText.text = IslandDecoTestData.GetItemName(slotData.itemId);
        }

        // 오브젝트 이미지 
        if (slotUI.itemButton != null)
        {
            Sprite sprite = null;

            if (currentMode == DecoMode.Lake)
                sprite = LakeDecoTestData.GetIconSprite(slotData.itemId);
            // else if (currentMode == DecoMode.Island)
            //     sprite = IslandDecoTestData.GetIconSprite(slotData.itemId);

            if (sprite != null)
                slotUI.itemImage.sprite = sprite;

        }
    }


    // 페이지 표시 
    void UpdatePageDisplay()
    {
        int startIndex = currentPage * slotsPerPage;
        int endIndex = startIndex + slotsPerPage;

        // 슬롯 표시/숨기기
        for (int i = 0; i < slotObjects.Count; i++)
            slotObjects[i].SetActive(i >= startIndex && i < endIndex);

        // 화살표 활성/비활성 (1페이지뿐이면 둘 다 비활성)
        if (btnArrowLeft != null)
            btnArrowLeft.interactable = (currentPage > 0);

        if (btnArrowRight != null)
            btnArrowRight.interactable = (currentPage < totalPages - 1);

        // 페이지 수 갱신
        UpdateIndicators();
    }

    // 페이지수 변경 
    void GoToPrevPage()
    {
        if (currentPage <= 0) return;
        currentPage--;
        UpdatePageDisplay();
    }

    void GoToNextPage()
    {
        if (currentPage >= totalPages - 1) return;
        currentPage++;
        UpdatePageDisplay();
    }



    // 페이지 수 점 생성/갱신
    void UpdateIndicators()
    {
        if (pageCountParent == null) return;

        // 기존 점 삭제
        for (int i = 0; i < pageCountDots.Count; i++)
            Destroy(pageCountDots[i]);
        pageCountDots.Clear();

        // 1페이지뿐이면 안 보여줌
        if (totalPages <= 1) return;

        // 페이지 수만큼 점 생성
        for (int i = 0; i < totalPages; i++)
        {
            GameObject dot = new GameObject("Dot_" + i);
            dot.transform.SetParent(pageCountParent, false);

            RectTransform rt = dot.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(12f, 12f);

            Image img = dot.AddComponent<Image>();
            img.color = (i == currentPage) ? pageCountOn : pageCountOff;
            img.raycastTarget = false;

            pageCountDots.Add(dot);
        }
    }

    // 슬롯 선택 
    void OnSlotClicked(int index)
    {
        // 같은 슬롯 다시 클릭 시 선택 해제
        if (selectedIndex == index)
        {
            DeselectSlot();
            return;
        }

        // 이전 선택 해제
        if (selectedIndex >= 0)
            SetSlotHighlight(selectedIndex, false);

        // 새 슬롯 선택
        selectedIndex = index;
        SetSlotHighlight(index, true);

        // 이벤트 발생
        if (index < inventoryData.Count)
            OnItemSelected?.Invoke(inventoryData[index].itemId, index);
    }

    // 선택 해제
    public void DeselectSlot()
    {
        if (selectedIndex >= 0)
            SetSlotHighlight(selectedIndex, false);

        selectedIndex = -1;
        OnItemDeselected?.Invoke();
    }

    // 슬롯 선택 하이라이트 표시
    void SetSlotHighlight(int index, bool highlighted)
    {
        if (index < 0 || index >= slotObjects.Count) return;

        Image bg = slotObjects[index].GetComponent<Image>();
        if (bg == null) return;

        if (highlighted)
            bg.color = new Color(0.3f, 0.8f, 0.4f, 0.9f); // 초록 하이라이트
        else
            bg.color = new Color(0.18f, 0.18f, 0.22f, 0.92f); // 원래 색상
    }

    // 배치 후 수량 갱신 
    // 아이템 배치 성공 시 수량 차감
    public void ConsumeItem(int itemId)
    {
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if (inventoryData[i].itemId == itemId)
            {
                inventoryData[i].quantity--;

                // 수량 UI 갱신
                if (i < slotUIs.Count && slotUIs[i] != null && slotUIs[i].quantityText != null)
                    slotUIs[i].quantityText.text = "x" + inventoryData[i].quantity;

                // 수량 0이면 슬롯 제거
                if (inventoryData[i].quantity <= 0)
                {
                    if (selectedIndex == i)
                        DeselectSlot();
                    RemoveSlot(i);
                }
                break;
            }
        }
    }

    // 아이템 회수 시 수량 복구
    public void RestoreItem(int itemId)
    {
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if (inventoryData[i].itemId == itemId)
            {
                bool wasZero = (inventoryData[i].quantity <= 0);
                inventoryData[i].quantity++;

                if (wasZero)
                {
                    AddSlot(inventoryData[i], i);
                }
                else if (i < slotObjects.Count)
                {
                    slotUIs[i].quantityText.text = "x" + inventoryData[i].quantity;
                }
                break;
            }
        }
    }

    // 슬롯 제거 (수량 0 됐을 때)
    void RemoveSlot(int dataIndex)
    {
        for (int i = 0; i < slotObjects.Count; i++)
        {
            if (slotObjects[i].name == "ItemSlot_" + dataIndex)
            {
                Destroy(slotObjects[i]);
                slotObjects.RemoveAt(i);
                slotUIs.RemoveAt(i);
                break;
            }
        }
        RecalcPages();
        UpdatePageDisplay();
    }

    // 슬롯 추가 
    void AddSlot(LakeInvenSlot slotData, int dataIndex)
    {
        GameObject slotObj = Instantiate(slotTemplate, itemContent);
        slotObj.SetActive(false);
        slotObj.name = "ItemSlot_" + dataIndex;

        DecoSlotUI slotUI = slotObj.GetComponent<DecoSlotUI>();
        SetupSlotUI(slotUI, slotData);

        int slotIndex = dataIndex;
        if (slotUI != null && slotUI.itemButton != null)
            slotUI.itemButton.onClick.AddListener(() => OnSlotClicked(slotIndex));

        slotObjects.Add(slotObj);
        slotUIs.Add(slotUI);

        RecalcPages();
        UpdatePageDisplay();
    }

    // 페이지 수 재계산
    void RecalcPages()
    {
        totalPages = Mathf.CeilToInt((float)slotObjects.Count / slotsPerPage);
        totalPages = Mathf.Clamp(totalPages, 1, maxPages);

        if (currentPage >= totalPages)
            currentPage = totalPages - 1;
    }

    public int GetSelectedItemId()
    {
        if (selectedIndex < 0 || selectedIndex >= inventoryData.Count)
            return -1;
        return inventoryData[selectedIndex].itemId;
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }


}