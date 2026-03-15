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
    #region 변수 선언
    [Header("UI 연결")]
    public RectTransform itemContent; // 슬롯들의 부모
    public Button btnArrowLeft;       // 좌측 화살표
    public Button btnArrowRight;      // 우측 화살표

    [Header("슬롯 템플릿")]
    public GameObject slotTemplate;   // 슬롯 하나 복사하여 생성

    [Header("페이지 카운트")]
    public RectTransform pageCountParent;   // 점들의 부모 오브젝트
    public GameObject NowCount;   // 현재 페이지
    public GameObject WaitCount;  // 다른 페이지

    [Header("페이지 설정")]
    public int slotsPerPage = 6; // 한 페이지에 보이는 최대 슬롯 수
    public int maxPages = 3;     // 최대 페이지 수

    [Header("SO데이터 관련")]
    public InteriorDatabaseSO _interiorDatabaseSO;

    public DecoMode currentMode = DecoMode.Lake; // 현재 편집 모드, 디폴트는 호수 


    // 내부 변수
    List<LakeInvenSlot> invenData; // invenData는 DecoInventoryManager의 리스트를 참조
    List<GameObject> slotObjects = new List<GameObject>();
    List<DecoSlotUI> slotUIs = new List<DecoSlotUI>();
    List<int> slotDataIndex = new List<int>(); // slotObjects[i]가 invenData[몇번]인지 매핑 (수량0 스킵 시 인덱스 삐끗 방지)
    List<GameObject> pageCountDots = new List<GameObject>();
    int selectedIndex = -1;  // 현재 선택된 슬롯 인덱스 (-1이면 선택 없음)
    int currentPage = 0;     // 현재 페이지 (0부터)
    int totalPages = 1;      // 전체 페이지 수
    DecoMode lastMode = DecoMode.Lake;
    List<LakeInvenSlot> lakeInvenSave = null;
    List<LakeInvenSlot> islandInvenSave = null;
    List<LakeInvenSlot> snapshotData = null;
    // 아이템 선택 이벤트
    public event Action<int, int> OnSlotPick;   // (itemId, slotIndex)
    public event Action OnSlotCancel;           // 선택 해제
    #endregion

    void Start()
    {
        // 페이지 넘김 버튼 연결
        if (btnArrowLeft != null)
            btnArrowLeft.onClick.AddListener(GoToPrevPage);

        if (btnArrowRight != null)
            btnArrowRight.onClick.AddListener(GoToNextPage);

        // 템플릿 더미 슬롯 비활성화 (복제용으로만 사용)
        if (slotTemplate != null)
            slotTemplate.SetActive(false);
    }

    // 인벤토리 데이터 넣고 슬롯 생성 (편집 모드 진입 시 호출)
    public void SetupInventory()
    {
        if (currentMode == DecoMode.Island) // 섬 인벤 
        {
            invenData = DecoInventoryManager.Instance.GetInven();
        }
        else if (currentMode == DecoMode.Lake) // 호수 인벤
        {
            invenData = LakeDecoTestData.CreateTestInventory();
        }
          
        selectedIndex = -1;
        currentPage = 0;

        ClearSlots();
        CreateSlots();
        RecalcPages();
        UpdatePageDisplay();
    }

    #region 인벤토리 세팅 

    // 기존 슬롯 전부 삭제
    void ClearSlots()
    {
        for (int i = 0; i < slotObjects.Count; i++)
        {
            Destroy(slotObjects[i]);
        }
        slotObjects.Clear();
        slotUIs.Clear();
        slotDataIndex.Clear(); 
    }

    // 인벤토리 데이터 기반으로 슬롯 동적 생성
    void CreateSlots()
    {
        for (int i = 0; i < invenData.Count; i++)
        {
            LakeInvenSlot slotData = invenData[i];

            // 수량 0이면 슬롯 생성 안함
            if (slotData.quantity <= 0) continue;

            // 템플릿 복제
            GameObject slotObj = Instantiate(slotTemplate, itemContent);
            slotObj.SetActive(false); // 페이지 전환에서 켜줌
            slotObj.name = "ItemSlot_" + i;

            // DecoSlotUI 컴포넌트에서 내용 참조 
            DecoSlotUI slotUI = slotObj.GetComponent<DecoSlotUI>();
            // 슬롯 내용 채우기
            SetupSlotUI(slotUI, slotData);
           
            SetSlotClick(slotUI, slotObj);
        
            slotObjects.Add(slotObj);
            slotUIs.Add(slotUI);
            slotDataIndex.Add(i); 
        }
    }

    // 슬롯 UI (이름/수량/이미지 세팅) 
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
            // 어짜피 GetData로 한꺼번에 가져오기 때문에 GetItemName이라는 함수는 삭제하고 GetData에서 문자열 값을 직접 들고옴
            else if (currentMode == DecoMode.Island)
            {
                try
                {
                    slotUI.nameText.text = DataManager.Instance.DecorationDatabase.InteriorData[slotData.itemId].InteriorName_String;
                }
                catch
                {
                    slotUI.nameText.text = "폴백 :" + slotData.itemId;
                }
            }
        }

        // 아이템 슬롯 이미지 
        if (slotUI.itemImage != null)
        {
            Sprite sprite = null;

            if (currentMode == DecoMode.Lake)
                sprite = LakeDecoTestData.GetIconSprite(slotData.itemId);
            else if (currentMode == DecoMode.Island)
            {
                // 상점SO에서 아이콘 가져오기 (InteriorId로 검색)
                var storeDatas = DataManager.Instance.StoreDatabase.IslandStoreData.datas;
                foreach (var sd in storeDatas)
                {
                    if (sd.InteriorId == slotData.itemId && sd.InteriorImgPath_Sprite != null)
                    {
                        sprite = sd.InteriorImgPath_Sprite;
                        break;
                    }
                }
                // 폴백
                if (sprite == null)
                    sprite = IslandDecoTestData.GetIconSprite(slotData.itemId);
            }

            if (sprite != null)
                slotUI.itemImage.sprite = sprite;

        }
    }


    #endregion

    #region 인디케이터 
    // 현재 페이지 슬롯 표시 
    void UpdatePageDisplay()
    {
        int startIndex = currentPage * slotsPerPage;
        int endIndex = startIndex + slotsPerPage;

        // 슬롯 표시/숨기기
        for (int i = 0; i < slotObjects.Count; i++)
            slotObjects[i].SetActive(i >= startIndex && i < endIndex);

        // 화살표 활성/비활성 (1페이지뿐이면 둘 다 비활성)
        //if (btnArrowLeft != null)
        //    btnArrowLeft.interactable = (currentPage > 0);

        //if (btnArrowRight != null)
        //    btnArrowRight.interactable = (currentPage < totalPages - 1);

        // 1페이지거나 마지막 페이지면  안 보여줌 
        if (btnArrowLeft != null)
            btnArrowLeft.gameObject.SetActive(currentPage > 0);

        if (btnArrowRight != null)
            btnArrowRight.gameObject.SetActive(currentPage < totalPages - 1);

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
            // 현재 프리팹 = NowCount, 나머지 프리팹 = WaitCount
            GameObject dot = Instantiate(
                (i == currentPage) ? NowCount : WaitCount,
                pageCountParent
            );
            pageCountDots.Add(dot);
        }
    }
    #endregion

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
        if (index < slotDataIndex.Count)
        {
            int dataIdx = slotDataIndex[index];  // slotDataIndex로 매핑 변환
            OnSlotPick?.Invoke(invenData[dataIdx].itemId, index); 
        }
    }

    // 선택 해제
    public void DeselectSlot()
    {
        if (selectedIndex >= 0)
            SetSlotHighlight(selectedIndex, false);

        selectedIndex = -1;
        OnSlotCancel?.Invoke();
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

    //  invenData 인덱스로 슬롯(slotObjects) 위치 찾기
    int FindSlotByDataIndex(int dataIdx)
    {
        for (int i = 0; i < slotDataIndex.Count; i++)
        {
            if (slotDataIndex[i] == dataIdx) return i;
        }
        return -1;
    }

    // 아이템 배치 성공 시 수량 차감
    public void UseItem(int itemId)
    {
        DecoInventoryManager.Instance.UseItem(itemId); // 수량 차감 

        int dataIdx = -1; // invenData 인덱스 추적
        for (int i = 0; i < invenData.Count; i++)
        {
            if (invenData[i].itemId == itemId)
            {
               // invenData[i].quantity--;
                dataIdx = i; 
                break;
            }
        }
        if (dataIdx < 0) return;

        int slotIdx = FindSlotByDataIndex(dataIdx); // 매핑으로 슬롯 찾기
        if (slotIdx < 0) return; 

        // 수량 0이면 슬롯 제거 
        if (invenData[dataIdx].quantity <= 0) //dataIdx 기준
        {
            if (selectedIndex == slotIdx) 
                DeselectSlot();
            RemoveSlot(slotIdx); // 직접 인덱스로 제거
        }
        else
        {   // 수량 UI 갱신 
            if (slotUIs[slotIdx] != null && slotUIs[slotIdx].quantityText != null)
                slotUIs[slotIdx].quantityText.text = "x" + invenData[dataIdx].quantity;
        }
    }

    // 아이템 회수 시 수량 복구
    public void RestoreItem(int itemId)
    {
    

        int dataIdx = -1;
        for (int i = 0; i < invenData.Count; i++)
        {
            if (invenData[i].itemId == itemId)
            {
                dataIdx = i;
                break;
            }
        }
        if (dataIdx < 0) return; 

        bool wasZero = (invenData[dataIdx].quantity <= 0);
        // invenData[dataIdx].quantity++;
        DecoInventoryManager.Instance.RestoreItem(itemId); // 수량 복구
        if (wasZero) // 슬롯이 없으면 다시 생성
        {
            AddSlot(invenData[dataIdx], dataIdx); 
        }
        else
        {
            int slotIdx = FindSlotByDataIndex(dataIdx); // 매핑으로 슬롯 찾기
            if (slotIdx >= 0 && slotUIs[slotIdx] != null && slotUIs[slotIdx].quantityText != null)
                slotUIs[slotIdx].quantityText.text = "x" + invenData[dataIdx].quantity;
        }
    }

    // 초기화 버튼용 (마지막 저장 시점으로 인벤 복원)   
    public void ReturnAllItems()
    {
        if (snapshotData == null) return;
        foreach (var slot in snapshotData)
        {
            DecoInventoryManager.Instance.SetItemCount(slot.itemId, slot.quantity);
        }
        LoadSnapshot();
    }
    #region 스냅샷 
    // 현재 인벤 상태 스냅샷 저장
    public void SaveSnapshot()
    {
        snapshotData = new List<LakeInvenSlot>();
        for (int i = 0; i < invenData.Count; i++)
        {
            snapshotData.Add(new LakeInvenSlot
            {
                itemId = invenData[i].itemId,
                quantity = invenData[i].quantity
            });
        }
    }

    // 스냅샷으로 인벤 복원
    public void LoadSnapshot()
    {
        if (snapshotData == null) return;

        //var restored = new List<LakeInvenSlot>();
        //for (int i = 0; i < snapshotData.Count; i++)
        //{
        //    restored.Add(new LakeInvenSlot
        //    {
        //        itemId = snapshotData[i].itemId,
        //        quantity = snapshotData[i].quantity
        //    });
        //}
        SetupInventory();
    }
    #endregion   
    // 슬롯 제거 (수량 0 됐을 때)
    void RemoveSlot(int slotIdx) 
    {
        if (slotIdx < 0 || slotIdx >= slotObjects.Count) return;

        // 리스트 동기화 
        Destroy(slotObjects[slotIdx]); // 인덱스로 제거 
        slotObjects.RemoveAt(slotIdx); // 슬롯
        slotUIs.RemoveAt(slotIdx);      // UI
        slotDataIndex.RemoveAt(slotIdx); // 매핑 제거 

        // 삭제된 슬롯 뒤쪽이면 인덱스 당기기
        if (selectedIndex == slotIdx) selectedIndex = -1; 
        else if (selectedIndex > slotIdx) selectedIndex--; 

        RecalcPages();
        UpdatePageDisplay();
    }
    // 슬롯 추가 (수량 0에서 1 이상으로 복구됐을 때)
    void AddSlot(LakeInvenSlot slotData, int dataIndex)
    {
        GameObject slotObj = Instantiate(slotTemplate, itemContent);
        slotObj.SetActive(false);
        slotObj.name = "ItemSlot_" + dataIndex;

        DecoSlotUI slotUI = slotObj.GetComponent<DecoSlotUI>();
        SetupSlotUI(slotUI, slotData);

        SetSlotClick(slotUI, slotObj);    

        slotObjects.Add(slotObj);
        slotUIs.Add(slotUI);
        slotDataIndex.Add(dataIndex); // 매핑 등록

        RecalcPages();
        UpdatePageDisplay();
    }

    // 슬롯 버튼 클릭 시 현재 인덱스 찾기 
    void SetSlotClick(DecoSlotUI slotUI, GameObject slotObj) 
    {
        if (slotUI == null || slotUI.itemButton == null) return;
       
        slotUI.itemButton.onClick.AddListener(() =>
        {
            int idx = slotObjects.IndexOf(slotObj); 
            if (idx >= 0) OnSlotClicked(idx);
        });
    }

    // 슬롯 수에 맞게 페이지 수 재계산
    void RecalcPages()
    {
        totalPages = Mathf.CeilToInt((float)slotObjects.Count / slotsPerPage);
        totalPages = Mathf.Clamp(totalPages, 1, maxPages);

        if (currentPage >= totalPages)
            currentPage = totalPages - 1;
    }

    // 현재 선택된 슬롯의 아이템 ID 반환 (-1이면 선택 없음)
    public int GetSelectedItemId()
    {
        if (selectedIndex < 0 || selectedIndex >= slotDataIndex.Count)
            return -1; 
        int dataIdx = slotDataIndex[selectedIndex]; // 매핑 변환
        return invenData[dataIdx].itemId; 
    }
    // 현재 선택된 슬롯 인덱스 반환 
    public int GetSelectedIndex()
    {
        return selectedIndex;
    }


}