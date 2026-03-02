using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic; 
/// [호수 꾸미기 편집 모드 전환 관리]
public class DecoEditModeManager : MonoBehaviour
{
    #region
    [Header("2D그리드 전용")]
    public RectTransform gridPanel;
    public LakeGridManager gridManager;
    public GameObject objectActionPanel; // 회수, 이동, 취소

    [Header("UI 연결")]
    public Image dimBackground;
    public RectTransform topButtonPanel;
    public RectTransform bottomPanel;
    public DecoItemListManager itemListManager;  // 하단에 인벤 (호수/섬 공용)

    [Header("상단 버튼")]
    public Button btnRecallAll; // 전체 회수
    public Button btnReset;     // 초기화 
    public Button btnSave;      // 저장
    public Button btnExit;      // 나가기

    [Header("꾸미기 버튼")]
    public Button btnDecoMode;
    public GameObject dropdownPanel;
    public Button btnLakeDecoMode;      // 꾸미기 모드 (편집 모드 토글) 
    public Button btnIslandDecoMode;    // 섬 꾸미기 진입

    [Header("하단 인벤")]
    public float animTime = 0.3f;           // 애니메이션 시간
    public float bottomPanelHeight = 240f;  // 인벤 패널 높이
    public float gridMoveUp = 240f;   // 그리드 위로 올리는 높이

    [Header("나가기 팝업")]
    public GameObject exitPopupPanel; 
    public Button btnExitSave;        // 저장하고 나가기
    public Button btnExitNoSave;      // 저장하지 않고 나가기
    public Button btnExitCancel;      // 취소 

    // 내부 변수
    bool isEditMode = false;
    DecoMode currentMode = DecoMode.Lake;   // 현재 편집 모드
    Vector2 gridOriginPos;    // 그리드 원래 위치 저장
    float dimAlpha = 0.5f;  // 백그라운드 배경 알파값

    int holdItemId = -1;   // 배치 대기 중인 아이템 ID
    BuildingManager buildingMgr;

    bool isChanged = false; // 저장 안 한 변경이 있었는지 
    #endregion

    //  초기화   
    void Start()
    {
        // 그리드 원래 위치 저장
        if (gridPanel != null)
            gridOriginPos = gridPanel.anchoredPosition;

        // 처음에는 편집 모드 OFF 상태로 세팅
        SetEditModeOff();

        // 드롭다운 내부 버튼 
        if (btnLakeDecoMode != null)
            btnLakeDecoMode.onClick.AddListener(OnLakeDecoClicked);
        if (btnIslandDecoMode != null)
            btnIslandDecoMode.onClick.AddListener(OnIslandDecoClicked);

        if (btnSave != null)
            btnSave.onClick.AddListener(OnSave);
        if (btnExit != null)
            btnExit.onClick.AddListener(OnExit);
        if (btnRecallAll != null)
            btnRecallAll.onClick.AddListener(OnRecallAll);
        if (btnReset != null)
            btnReset.onClick.AddListener(OnReset);

        // 드롭다운 메인 버튼 
        if (btnDecoMode != null)
            btnDecoMode.onClick.AddListener(ToggleDropdown);
        if (dropdownPanel != null)
            dropdownPanel.SetActive(false); // 초기화

        if (itemListManager != null) // 섬 편집모드 인벤 템 클릭 
            itemListManager.OnSlotPick += OnIslandPick;

        // 빌딩매니저의 이벤트 구독 (섬 배치 성공/취소)
        buildingMgr = FindFirstObjectByType<BuildingManager>();
        if (buildingMgr != null)
        {
            buildingMgr.OnPlaceSuccess += OnIslandPlaceSuccess; // 배치 성공
            buildingMgr.OnPlaceCancel += OnIslandPlaceCancel; // 배치 취소
            buildingMgr.OnConfirm += OnIslandConfirm;   // 저장      
            buildingMgr.OnRevert += OnIslandRevert;     // 전체회수
            buildingMgr.OnClearAll += OnIslandClearAll; // 초기화
        }
        // 나가기 관련 버튼
        if (btnExitSave != null)
            btnExitSave.onClick.AddListener(OnExitWithSave);
        if (btnExitNoSave != null)
            btnExitNoSave.onClick.AddListener(OnExitWithoutSave);
        if (btnExitCancel != null)
            btnExitCancel.onClick.AddListener(OnExitPopupCancel);
        if (exitPopupPanel != null)
            exitPopupPanel.SetActive(false);
   
    }

    // 섬 전용 인벤 템 보이기
    void OnIslandPick(int itemId, int slotIndex)
    {
        if (currentMode != DecoMode.Island) return;

        GameObject prefab = IslandDecoTestData.GetPrefab(itemId);
        if (prefab == null) return;

        holdItemId = itemId; // 배치 대기 아이템 저장

        // PlacementMgr.cs 싱글톤으로 프리팹 전달
        if (PlacementMgr.Instance != null)
            PlacementMgr.Instance.OnClickConstructionButton(prefab);
    }
    // 배치 성공 시 수량 차감
    void OnIslandPlaceSuccess(GameObject obj)
    {
        if (holdItemId >= 0 && itemListManager != null)
        {
            itemListManager.UseItem(holdItemId);
            holdItemId = -1;
        }
        if (itemListManager != null)
            itemListManager.DeselectSlot();
        LockTopButtons(true);
        isChanged = true;
    }

    // 배치 취소 시 차감 안 했으니 초기화만
    void OnIslandPlaceCancel(GameObject obj)
    {
        holdItemId = -1;
        if (itemListManager != null)
            itemListManager.DeselectSlot();
        LockTopButtons(true);
    }

    // 섬 저장 
    void OnIslandConfirm()
    {
        if (currentMode != DecoMode.Island) return;
        if (itemListManager != null)
            itemListManager.SaveSnapshot();
    }

    // 섬 전체회수 
    void OnIslandRevert()
    {
        if (currentMode != DecoMode.Island) return;
        if (itemListManager != null)
            itemListManager.LoadSnapshot();
    }

    // 섬 초기화 
    void OnIslandClearAll()
    {
        if (currentMode != DecoMode.Island) return;
        if (itemListManager != null)
            itemListManager.ReturnAllItems();
    }
    void OnLakeDecoClicked()
    {
        EnterEditMode(DecoMode.Lake);
        HideDropdown();
    }

    void OnIslandDecoClicked()
    {
        EnterEditMode(DecoMode.Island);
        HideDropdown();
    }
    // 드롭다운 
    void ToggleDropdown()
    {
        if (dropdownPanel != null)
            dropdownPanel.SetActive(!dropdownPanel.activeSelf);
    }
    void HideDropdown()
    {
        if (dropdownPanel != null)
            dropdownPanel.SetActive(false);
    }
    // 배치 모드 중 상단버튼 잠금 
    public void LockTopButtons(bool active)
    {
        if (btnSave != null) btnSave.interactable = active;
        if (btnReset != null) btnReset.interactable = active;
        if (btnRecallAll != null) btnRecallAll.interactable = active;
        if (btnExit != null) btnExit.interactable = active;
    }
    // 편집 모드 토글
    public void ToggleEditMode()
    {
        if (isEditMode)
            ExitEditMode();
        else
            EnterEditMode(currentMode); // 현재 모드로 진입 
    }

    // 편집 모드 진입
    public void EnterEditMode(DecoMode mode)
    {
        currentMode = mode; // 받은 모드 저장
        isEditMode = true;
        isChanged = false;

        // 공용 백그라운드 
        if (dimBackground != null)
        {
            dimBackground.gameObject.SetActive(true);
            dimBackground.color = new Color(0, 0, 0, 0);
            dimBackground.DOColor(new Color(0, 0, 0, dimAlpha), animTime);
        }

        // 호수전용 : 2d 그리드 타일 격자 
        if (currentMode == DecoMode.Lake)
        {
            if (gridPanel != null) // 2d그리드 다시 켜기
                gridPanel.gameObject.SetActive(true);
            if (gridManager != null)
                gridManager.ShowGrid(2f);  // 이미지에 격자가 있는게 아니라, 중간중간 띄워서 백그라운드가 보이게 하는 방식

            // 그리드 타일 위로 밀기
            if (gridPanel != null)
            {
                gridPanel.DOAnchorPosY(gridOriginPos.y + gridMoveUp, animTime)
                    .SetEase(Ease.OutQuad);
            }
        }

        // 섬 전용: 나중에 필요한 초기화 추가
        if (currentMode == DecoMode.Island)
        {
            // 섬모드면 호수 2d그리드 끄기 
            if (gridPanel != null)
                gridPanel.gameObject.SetActive(false);
            if (gridManager != null)
                gridManager.HideGrid();

            // TODO: 섬 전용 로직, 섬 3d그리드 켜기 
            // 3D 그리드 편집모드 진입
            if (PlacementMgr.Instance != null)
            {
                // PlacementMgr가 View상태일때만 편집모드로 전환
                if (PlacementMgr.Instance.CurrentState == PlacementState.View)
                    PlacementMgr.Instance.ToggleEditMode();
            }
        }

        // 이하 공용 
        //  인벤 패널 아래에서 위로 슥 올라오기 
        if (bottomPanel != null)
        {
            bottomPanel.gameObject.SetActive(true);
            bottomPanel.anchoredPosition = new Vector2(bottomPanel.anchoredPosition.x, -bottomPanelHeight);
            bottomPanel.DOAnchorPosY(0f, animTime)
            .SetEase(Ease.OutQuad);
        }
        // 상단 버튼 활성화 + 위에서 내려오기
        if (topButtonPanel != null)
        {
            topButtonPanel.gameObject.SetActive(true);
            topButtonPanel.anchoredPosition = new Vector2(topButtonPanel.anchoredPosition.x, 60f);
            topButtonPanel.DOAnchorPosY(-20f, animTime).SetEase(Ease.OutQuad);
        }
        // 아이템 리스트 세팅(테스트용 더미, 나중에 실제 인벤과 연결 필요)
        if (itemListManager != null)
        {
            itemListManager.currentMode = currentMode; // 모드를 받아서 세팅후 
            itemListManager.SetupTestInventory(); // 인벤호출
            itemListManager.SaveSnapshot();
        }
        // 호수 그리드 스냅샷 저장
        if (currentMode == DecoMode.Lake && gridManager != null)
        {
            gridManager.SaveSnapshot();
        }
     
        // 편집 모드 들어가면 꾸미기 버튼 숨기기
        if (btnLakeDecoMode != null)
            btnLakeDecoMode.gameObject.SetActive(false);
        if (btnIslandDecoMode != null)
            btnIslandDecoMode.gameObject.SetActive(false);
        // 드롭다운 버튼 숨기기 
        if (btnDecoMode != null)
            btnDecoMode.gameObject.SetActive(false);
    }

    //  편집 모드 나가기
    public void ExitEditMode()
    {
        isEditMode = false;

        // 백그라운드 
        if (dimBackground != null)
        {
            dimBackground.DOColor(new Color(0, 0, 0, 0), animTime)
            .OnComplete(() => dimBackground.gameObject.SetActive(false));
        }

        // 호수 전용: 격자, 그리드 
        if (currentMode == DecoMode.Lake)
        {
            // 나가면 들고있는 오브젝트 정리
            var placeController = FindFirstObjectByType<LakePlaceController>();
            if (placeController != null)
                placeController.ForceCancel();

            if (gridManager != null)
                gridManager.HideGrid();

            if (gridPanel != null)
            {
                gridPanel.DOAnchorPosY(gridOriginPos.y, animTime)
                    .SetEase(Ease.OutQuad);
            }
        }

        // 섬 전용 편집모드 퇴장
        if (currentMode == DecoMode.Island)
        {
            if (gridPanel != null) // 섬 편집 모드 나갈때, 호수 2d그리드 복구 
                gridPanel.gameObject.SetActive(true);

            // TODO: 섬 보이는거 비활성화 
            if (PlacementMgr.Instance != null)
            {
                // PlacementMgr가 Edit상태일때만 편집모드로 전환
                if (PlacementMgr.Instance.CurrentState == PlacementState.Edit)
                    PlacementMgr.Instance.ToggleEditMode();
            }
            holdItemId = -1; // 나갈때 배치 대기 아이템 초기화 
        }

        // 인벤 
        if (bottomPanel != null)
        {
            bottomPanel.DOAnchorPosY(-bottomPanelHeight, animTime)
            .SetEase(Ease.InQuad)
            .OnComplete(() => bottomPanel.gameObject.SetActive(false));
        }
        // 상단 버튼 
        if (topButtonPanel != null)
        {
            topButtonPanel.DOAnchorPosY(60f, animTime)
            .SetEase(Ease.InQuad)
            .OnComplete(() => topButtonPanel.gameObject.SetActive(false));
        }
        // obj 액션 패널  (배치, 취소)
        if (objectActionPanel != null)
            objectActionPanel.SetActive(false);

        // 편집 모드 나가면 꺼진 불도 다시보기
        if (btnLakeDecoMode != null)
            btnLakeDecoMode.gameObject.SetActive(true);
        if (btnIslandDecoMode != null)
            btnIslandDecoMode.gameObject.SetActive(true);
        if (btnDecoMode != null)
            btnDecoMode.gameObject.SetActive(true);
        isChanged = false; 
    }

    // 처음 시작시 off 
    void SetEditModeOff()
    {
        isEditMode = false;

        // 백그라운드 숨기기
        if (dimBackground != null)
        {
            dimBackground.color = new Color(0, 0, 0, 0);
            dimBackground.gameObject.SetActive(false);
        }
        // 하단 패널 숨기기
        if (bottomPanel != null)
            bottomPanel.gameObject.SetActive(false);

        // 상단 버튼 숨기기
        if (topButtonPanel != null)
            topButtonPanel.gameObject.SetActive(false);

        // 오브젝트 액션 패널 숨기기 (배치, 취소)
        if (objectActionPanel != null)
            objectActionPanel.SetActive(false);

    }

    #region 상단 버튼 기능

    void OnSave() // 저장
    {
        // TODO: 파베에 저장
        if (currentMode == DecoMode.Lake)
        {
            if (itemListManager != null)
                itemListManager.SaveSnapshot(); // 인벤을 스냅샷 저장
            if (gridManager != null)
                gridManager.SaveSnapshot(); // 배치도 스냅샷으로 저장
            isChanged = false;
        }
        else if (currentMode == DecoMode.Island)
        {
            // PlacementMgr.Instance?.OnClickConfirmSession();
            if (buildingMgr != null)
                buildingMgr.ConfirmAll();
            isChanged = false;
        }
    }
    void OnReset() // 초기화, 저장한 상태로 불러오기
    {
        // TODO:  (파베 연결 후에 나중에 구현)
        if (currentMode == DecoMode.Lake)
        {
            // 인벤을 마지막 저장 시점으로 복구
            if (itemListManager != null)
                itemListManager.LoadSnapshot();
            // 배치를 마지막 저장 시점으로 복구
            if (gridManager != null)
                gridManager.LoadSnapshot();
            isChanged = false;
        }
        else if (currentMode == DecoMode.Island)
        {
            // PlacementMgr.Instance?.OnClickAllDelete();
            if (buildingMgr != null)
            {
                buildingMgr.RevertAll();          // 되돌리기
                buildingMgr.CancelCurrentAction(); // 들고있는 거 취소
            }
        }
    } 
    void OnRecallAll()  // 전체회수 : 배치된 거 전부 인벤으로 회수 
    {
        if (currentMode == DecoMode.Lake)
        {
            // 배치된 모든 오브젝트를 인벤에 복구
            var placedList = gridManager.GetPlacedObjects();
            foreach (var p in placedList)
            {
                if (itemListManager != null)
                    itemListManager.RestoreItem(p.itemId);
            }

            gridManager.RecallAll();  // 그리드에서 전부 제거
            isChanged = true;
        }
        else if (currentMode == DecoMode.Island)
        {
            //  PlacementMgr.Instance?.OnClickCancelSession();
            if (buildingMgr != null)
            {
                buildingMgr.ClearAll();
            }
            isChanged = true;
        }
    }
    void OnExit() // 나가기
    {
        if (isChanged)  // 미저장 변경사항이 있으면 팝업 표시
        {
            ShowExitPopup();
        }
        else // 변경사항 없으면 바로 나가기
        {
            isChanged = false;
            ExitEditMode();
        }
    }
    #endregion
    #region 나가기 팝업
    void ShowExitPopup()
    {
        if (exitPopupPanel != null)
        {
            exitPopupPanel.SetActive(true);
            // 상단 버튼 잠금 (팝업 뒤에서 못 누르게)
            LockTopButtons(false);
        }
        else // 임시로 팝업 UI가 없으면 그냥 나가기
        {  
            ExitEditMode();
        }
    } // 팝업 패널 
    void OnExitWithSave()  // 저장하고 나가기
    {
        OnSave(); // 저장 먼저
        if (exitPopupPanel != null)
            exitPopupPanel.SetActive(false);
        LockTopButtons(true);
        ExitEditMode();
    }
    void OnExitWithoutSave()  // 저장하지 않고 나가기 (마지막 저장 시점으로 복구 후 나가기)
    {
        // 마지막 저장 상태로 되돌리고 나가기
        if (currentMode == DecoMode.Lake)
        {
            if (itemListManager != null)
                itemListManager.LoadSnapshot();
            if (gridManager != null)
                gridManager.LoadSnapshot();
        }
        else if (currentMode == DecoMode.Island)
        {
            // PlacementMgr.Instance?.OnClickCancelSession();
            if (buildingMgr != null)
            {
                buildingMgr.RevertAll();          // 되돌리기
                buildingMgr.CancelCurrentAction(); // 들고있는 거 취소
            }
        }

        if (exitPopupPanel != null)
            exitPopupPanel.SetActive(false);
        LockTopButtons(true);
        ExitEditMode();
    }
    void OnExitPopupCancel() // 팝업만 닫기 
    {
        if (exitPopupPanel != null)
            exitPopupPanel.SetActive(false);
        LockTopButtons(true);
    } 
    #endregion
   
    public void SetChanged()
    {
        isChanged = true;
    }

    void OnDestroy() // 이벤트 해제 
    {
        if (itemListManager != null)
            itemListManager.OnSlotPick -= OnIslandPick;

        if (buildingMgr != null)
        {
            buildingMgr.OnPlaceSuccess -= OnIslandPlaceSuccess;
            buildingMgr.OnPlaceCancel -= OnIslandPlaceCancel;
            buildingMgr.OnConfirm -= OnIslandConfirm;          
            buildingMgr.OnRevert -= OnIslandRevert;            
            buildingMgr.OnClearAll -= OnIslandClearAll;        
        }
    }

    //  외부에서 상태 확인용
    public bool IsEditMode()
    {
        return isEditMode;
    }

    public DecoMode GetCurrentMode()
    {
        return currentMode;
    }
}