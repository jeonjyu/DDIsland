using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.InputSystem;

/// [꾸미기 편집 모드 전환 관리]
public class DecoEditModeManager : MonoBehaviour
{
    #region 변수 
    [Header("2D그리드 호수 전용")]
    public RectTransform gridPanel;
    public LakeGridManager gridManager;
    public GameObject objectActionPanel; // 회수, 이동, 취소
    public AquariumMgr aquariumMgr; // 물고기 안보이게 
    public RectTransform lakeBackground; // 호수 이미지 
    public GameObject decoMaskArea; // v2 바닥재+장식물 배경
    public RectTransform floorPlane; // 바닥재 두트윈용?
   
    [Header("3D배치템 상호작용 버튼")] 
    public Button btnObjRecall;   // 회수
    public Button btnObjMove;     // 이동
    public Button btnObjRotate;   // 회전 (3D 전용)
    public Button btnObjCancel;   // 취소
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
 
    [Header("꾸미기 진입 버튼")]
    public Button btnDecoMode;
    public GameObject dropdownPanel;
    public GameObject dropdownBlocker; // 드롭다운 열었을때 취소용 
    public Button btnLakeDecoMode;      // 호수 꾸미기 진입
    public Button btnIslandDecoMode;    // 섬 꾸미기 진입

    [Header("하단 인벤")]
    public float animTime = 0.3f;           // 애니메이션 시간
    public float bottomPanelHeight = 240f;  // 인벤 패널 높이
    public float gridMoveUp = 240f;   // 그리드 위로 올리는 높이
    public float lakeBgMoveUp = 240f; // 호수 위로 올리는 높이
    [Header("나가기 팝업")]
    public GameObject exitPopupPanel; 
    public Button btnExitSave;        // 저장하고 나가기
    public Button btnExitNoSave;      // 저장하지 않고 나가기
    public Button btnExitCancel;      // 취소 
    [Header("확인 팝업")]                        
    public GameObject confirmPopupPanel;        
    public Button btnConfirmYes;      // 확인          
    public Button btnConfirmClose;    // 취소           
    public DecoLocalizeTempText decoTempText; // 스크립트 연결   
    [Header("플레이어(투명화)")]
    public GameObject playerObject;
    [Header("편집모드 들어가면 안보일 날씨 파티클들 부모")]
    public Transform weatherParticleParent;
    // 내부 변수
    bool isEditMode = false;
    DecoMode currentMode = DecoMode.Lake;   // 현재 편집 모드
    Vector2 gridOriginPos;    // 그리드 원래 위치 저장
    Vector2 lakeOriginPos; // 호수 배경 원래 위치 저장
    float dimAlpha = 0.5f;  // 백그라운드 배경 알파값
  
    int holdItemId = -1;   // 들고 있어서 배치 대기 중인 아이템 ID
    BuildingManager buildingMgr;
 
    bool isChanged = false; // 저장 안 한 변경이 있었는지 
    Vector2 placedObjOriginPos; // 배치템 원래 위치 
    Vector2 floorOriginPos; // 바닥재 원래 위치 
    Placeable3D selectedIslandTarget; // 3d 오브젝트 선택
    Vector2 islandDecoBtnOriginPos; // 두트윈 복귀  
    int UnlockCount = 0;
    float UnlockFirstTime;
    bool isUnlocked = false;
    System.Action pendingConfirmAction; // 상단 확인 팝업창용
    private ParticleSystem cachedParticle; // 파티클 상태 복원용, 편집모드 진입 시 재생 중이던 파티클 캐싱

    // FixGroup별 기본 아이템 ID (교체 시 이전 템 복구용)
    //static readonly Dictionary<FixGroup, int> DefaultFixItems = new()
    //{
    //    { FixGroup.House, 20001 },
    //    { FixGroup.Box, 20005 },
    //    { FixGroup.LpPlayer, 20012 },
    //    { FixGroup.Bed, 20032 },
    //};
    #endregion

    // 버튼들 초기화   
    void Start()
    {
        // 그리드 원래 위치 저장
        if (gridPanel != null)
            gridOriginPos = gridPanel.anchoredPosition;
        if (gridManager != null && gridManager.placedObjectsParent != null) // 배치템 원래 위치 저장
            placedObjOriginPos = gridManager.placedObjectsParent.GetComponent<RectTransform>().anchoredPosition;
        // 호수와 바닥재 원래 위치 저장
        if (lakeBackground != null)
            lakeOriginPos = lakeBackground.anchoredPosition;
        if (floorPlane != null) 
            floorOriginPos = floorPlane.anchoredPosition;
        if (btnIslandDecoMode != null) // 버튼 
            islandDecoBtnOriginPos = btnIslandDecoMode.GetComponent<RectTransform>().anchoredPosition;


        if (aquariumMgr == null) // 인스펙터 할당된게 없으면 매니저 찾아서 물고기 숨기는용
            aquariumMgr = FindObjectOfType<AquariumMgr>();

        // 처음에는 편집 모드 OFF 상태로 세팅
        SetEditModeOff();

        // 드롭다운 내부 버튼 
        if (btnLakeDecoMode != null)
            btnLakeDecoMode.onClick.AddListener(OnLakeDecoClicked);
        if (btnIslandDecoMode != null)
            btnIslandDecoMode.onClick.AddListener(OnIslandDecoClicked);

        // 상단 버튼
        if (btnSave != null)
            btnSave.onClick.AddListener(OnSaveClicked);       
        if (btnRecallAll != null)
            btnRecallAll.onClick.AddListener(OnRecallAllClicked);
        if (btnReset != null)
            btnReset.onClick.AddListener(OnResetClicked);        
        if (btnExit != null)
            btnExit.onClick.AddListener(OnExit);

        // 드롭다운 편집모드 진입버튼 
        if (btnDecoMode != null)
            btnDecoMode.onClick.AddListener(ToggleDropdown);
        if (dropdownPanel != null)
            dropdownPanel.SetActive(false); // 초기화
        if (dropdownBlocker != null) // 풀스크린 패널
        {
            dropdownBlocker.SetActive(false);
            dropdownBlocker.GetComponent<Button>().onClick.AddListener(OnBlockerClicked);
        }
        if (btnLakeDecoMode != null)
            btnLakeDecoMode.gameObject.SetActive(false);
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
            buildingMgr.OnFixReverted += OnFixReverted; // 저장 안하고 나가면 배치템 회귀  
        }

        // 이벤트 구독 (섬 3d 오브젝트 선택/해제)
        if (PlacementMgr.Instance != null)
        {
            PlacementMgr.Instance.OnBuildingPick += ShowIslandActionPanel;
            PlacementMgr.Instance.OnBuildingDrop += HideIslandActionPanel;
            // 고정물 클릭 이벤트 구독 
            PlacementMgr.Instance.OnFixedBuildingPick += OnFixBuildingClicked;

            // 그리드에 배치된 Fix 아이템 클릭 이벤트 구독 
            // 집/침대/보관함 등 Placeable3D로 배치된 Fix 아이템용
            PlacementMgr.Instance.OnGridFixBuildingPick += OnGridFixBuildingClicked;

            // PlacementMgr.Instance.OnEmptyClick += OnEmptySpaceClicked;  
            // PlacementMgr의 오브젝트 전부 클릭무시 메서드, 자유물은 허용, UI는 무시 따로 처리해야해서 논의 필요 
        }
        //  Fix 슬롯 선택 이벤트 구독
        if (itemListManager != null)
            itemListManager.OnFixSlotPick += OnFixSlotSelected;
 

        // 3d 오브젝트 액션 버튼   
        if (btnObjRecall != null)
            btnObjRecall.onClick.AddListener(OnIslandRecall);
        if (btnObjMove != null)
            btnObjMove.onClick.AddListener(OnIslandMove);
        if (btnObjRotate != null)
            btnObjRotate.onClick.AddListener(OnIslandRotate);
        if (btnObjCancel != null)
            btnObjCancel.onClick.AddListener(OnIslandActionCancel);


        // 나가기 관련 버튼
        if (btnExitSave != null)
            btnExitSave.onClick.AddListener(OnExitWithSave);
        if (btnExitNoSave != null)
            btnExitNoSave.onClick.AddListener(OnExitWithoutSave);
        if (btnExitCancel != null)
            btnExitCancel.onClick.AddListener(OnExitPopupCancel);
        if (exitPopupPanel != null)
            exitPopupPanel.SetActive(false);

        // 확인 팝업 버튼 (공용)
        if (btnConfirmYes != null)                             
            btnConfirmYes.onClick.AddListener(OnConfirmYes);
        if (btnConfirmClose != null)
            btnConfirmClose.onClick.AddListener(OnConfirmClose);
        if (confirmPopupPanel != null)                         
            confirmPopupPanel.SetActive(false);

       
    }

    // TODO : 블로커 풀스크린 패널 ui를 안쓰고 빈 공간클릭 메서드 이벤트 구독 구조로 변경할거면 
    //void OnEmptySpaceClicked() 
    //{
    //    if (itemListManager != null && itemListManager.IsFixFilterMode)
    //        itemListManager.ExitFilterMode();
    //}


    #region 섬 전용 
    // 핸들러
    void OnFixReverted(int restoredId, int removedId)
    {
        // 원래 템은 인벤에서 차감 (되돌려 끼웠으니까)
        DecoInventoryManager.Instance.UseItem(restoredId);
        // 떼어낸 템은 인벤으로 복구
        DecoInventoryManager.Instance.RestoreItem(removedId);
    }
    // 섬 전용 오브젝트 액션패널
    void ShowIslandActionPanel(Placeable3D target)
    {
        if (currentMode != DecoMode.Island) return;
        // Fix 필터 모드 중이면 액션패널 안 띄움
        if (itemListManager != null && itemListManager.IsFixFilterMode)
            return;
        
         // TODO : 옵젝클릭무시 메서드 받아올때 자유물 클릭하면 필터해제 
        // Fix 필터 모드 중이면 해제하고 메인 인벤 복귀
        //if (itemListManager != null && itemListManager.IsFixFilterMode)
        //{
        //    itemListManager.ExitFilterMode();
        //    if (dropdownBlocker != null)
        //        dropdownBlocker.SetActive(false);
        //}


        selectedIslandTarget = target;

        if (objectActionPanel != null)
        {
            objectActionPanel.SetActive(true);
            Vector2 mousePos = Mouse.current.position.ReadValue();
            objectActionPanel.GetComponent<RectTransform>().position = mousePos + new Vector2(0, 60f);
        }
        if (btnObjRotate != null) // 회전 버튼 표시 
            btnObjRotate.gameObject.SetActive(true);
    }
   
    // 3D 오브젝트 선택 해제
    void HideIslandActionPanel()
    {
        selectedIslandTarget = null;
        if (objectActionPanel != null)
            objectActionPanel.SetActive(false);
     
        if (itemListManager != null && itemListManager.IsFixFilterMode)
            itemListManager.ExitFilterMode(); 
    }

    // 섬전용 전체회수 
    void OnIslandRecall()
    {
        if (selectedIslandTarget == null) return;

        int itemId = selectedIslandTarget.GetItemId();
        if (buildingMgr != null)
            buildingMgr.DeleteBuilding(selectedIslandTarget);

        if (itemId >= 0 && itemListManager != null)
            itemListManager.RestoreItem(itemId);

        isChanged = true;
        if (PlacementMgr.Instance != null)
            PlacementMgr.Instance.CloseEditMenu();
    }

    // 섬전용 이동
    void OnIslandMove()
    {
        if (selectedIslandTarget == null) return;

        if (PlacementMgr.Instance != null)
            PlacementMgr.Instance.OnClickMove();

        isChanged = true;
    }

    // 섬전용 회전
    void OnIslandRotate()
    {
        if (selectedIslandTarget == null) return;

        if (PlacementMgr.Instance != null)
            PlacementMgr.Instance.OnClickRotate();
    }

    // 섬전용 취소 (패널만 닫음)
    void OnIslandActionCancel()
    {
        if (PlacementMgr.Instance != null)
            PlacementMgr.Instance.CloseEditMenu();
    }

    // 섬 전용 인벤 템 보이기
    void OnIslandPick(int itemId, int slotIndex)
    {
        if (currentMode != DecoMode.Island) return;


        InteriorDataSO data = DataManager.Instance.DecorationDatabase.InteriorData[itemId];
        if (data == null) return;

        holdItemId = itemId; // 배치 대기 아이템 저장

        // PlacementMgr.cs 싱글톤으로 프리팹 전달
        if (PlacementMgr.Instance != null)
            PlacementMgr.Instance.OnClickConstructionButton(data.InteriorID);
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
    void OnIslandClearAll(List<int> destroyedIds)
    {
        if (currentMode != DecoMode.Island) return;
        //if (itemListManager != null)
        //    itemListManager.ReturnAllItems();

        foreach (int itemId in destroyedIds)                   
        {                                                      
            DecoInventoryManager.Instance.RestoreItem(itemId); 
        }                                                      

        if (itemListManager != null)                           
            itemListManager.SetupInventory();                  
    }

    //고정물(FixedBuilding) 클릭 시 인벤 필터 모드 진입 
    void OnFixBuildingClicked(FixedBuilding target)
    {
        if (currentMode != DecoMode.Island) return;
        if (target == null) return;

        // 액션패널 닫기 (회수/이동 버튼 안 보이게)
        HideIslandActionPanel();

        // 해당 고정물의 FixGroup으로 인벤 필터링
        FixGroup group = target.LocationID;

        if (group == FixGroup.None) return; // None이면 교체 불가

        if (itemListManager != null)
            itemListManager.SetupFilteredInventory(group, target);
        if (dropdownBlocker != null)
            dropdownBlocker.SetActive(true);
    }

    // 그리드에 배치된 Fix 고정물 배치템(Placeable3D) 클릭 시
    void OnGridFixBuildingClicked(Placeable3D target)
    {
        if (currentMode != DecoMode.Island) return;
        if (target == null) return;

        var fixBuilding = target.GetComponent<FixedBuilding>(); 
        if (fixBuilding == null)
            fixBuilding = target.GetComponentInChildren<FixedBuilding>();
        if (fixBuilding == null) return;

        OnFixBuildingClicked(fixBuilding);
    }
  
    // Fix 슬롯에서 아이템 선택 시 프리팹 교체 실행
    void OnFixSlotSelected(int newItemId, FixedBuilding target)
    {
        if (target == null || buildingMgr == null) return;

        int oldItemId = target.CurrentItemID;
      
        // CurrentItemID가 0이면 기본 아이템 ID로 복구
        //if (oldItemId <= 0 && DefaultFixItems.ContainsKey(target.LocationID))
        //    oldItemId = DefaultFixItems[target.LocationID];

        DecoInventoryManager.Instance.UseItem(newItemId); // 인벤에서 수량 차감

        if (oldItemId > 0) // 이전 아이템 인벤으로 복구
            DecoInventoryManager.Instance.RestoreItem(oldItemId);

        buildingMgr.SwapFixBuilding(target, newItemId); // 프리팹 교체

        // 필터 모드 해제 시 일반 인벤으로 복귀
        if (itemListManager != null)
            itemListManager.ExitFilterMode();
       
        // 블로커 비활성화 
        if (dropdownBlocker != null)
            dropdownBlocker.SetActive(false);
        
        // 편집 메뉴 닫기
        if (PlacementMgr.Instance != null)
            PlacementMgr.Instance.CloseEditMenu();

        isChanged = true;
    }
  
    #endregion

    void OnIslandDecoClicked() // 섬 
    {
        EnterEditMode(DecoMode.Island);
        HideDropdown();
    }

    void OnLakeDecoClicked() // 호수
    {
        EnterEditMode(DecoMode.Lake);
        HideDropdown();
    }

    #region 드롭다운중 취소전용  
    // 드롭다운 토글 
    void ToggleDropdown()
    {
        if (dropdownPanel == null) return;
        dropdownPanel.SetActive(!dropdownPanel.activeSelf);
        if (dropdownBlocker != null)
            dropdownBlocker.SetActive(dropdownPanel.activeSelf);
    }
    // 드롭다운 닫기
    void HideDropdown()
    {
        if (dropdownPanel != null)
            dropdownPanel.SetActive(false);
        if (dropdownBlocker != null) dropdownBlocker.SetActive(false);
    }
    #endregion


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

        // 호수 전용 편집모드 입장
        if (currentMode == DecoMode.Lake)
        {
            if (btnObjRotate != null) // 회전버튼 숨기기
                btnObjRotate.gameObject.SetActive(false); 

            if (gridPanel != null) // 2d그리드 다시 켜기
                gridPanel.gameObject.SetActive(true);
            if (gridManager != null)
                gridManager.ShowGrid();  // 이미지에 격자가 있는게 아니라, 중간중간 띄워서 백그라운드가 보이게 하는 방식

            // 그리드 타일 위로 밀기
            if (gridPanel != null)
            {
                gridPanel.DOAnchorPosY(gridOriginPos.y + gridMoveUp, animTime)
                    .SetEase(Ease.OutQuad);
            }
            if (aquariumMgr != null) aquariumMgr.HideFish(); // 물고기 숨기기

            if (lakeBackground != null) // 뒤에 호수도 같이 두트윈으로 띄어올리기 
            {
                lakeBackground.DOAnchorPosY(lakeOriginPos.y + lakeBgMoveUp, animTime)
                .SetEase(Ease.OutQuad);
            }
            if (floorPlane != null) // 바닥재 띄우기 
            {
                floorPlane.DOAnchorPosY(floorOriginPos.y + gridMoveUp, animTime)
                    .SetEase(Ease.OutQuad);
            }
            // 장식물 띄우기 
            if (gridManager != null && gridManager.placedObjectsParent != null)
            {
                gridManager.placedObjectsParent.GetComponent<RectTransform>()
                    .DOAnchorPosY(placedObjOriginPos.y + gridMoveUp, animTime)
                    .SetEase(Ease.OutQuad);
            }
        }

        // 섬 전용 편집모드 입장
        if (currentMode == DecoMode.Island)
        {
            // 섬모드면 호수 2d그리드 끄기 
            if (gridPanel != null)
                gridPanel.gameObject.SetActive(false);
            if (gridManager != null)
                gridManager.HideGrid();

            if (lakeBackground != null) // 호수 숨기기
                lakeBackground.GetComponent<CanvasGroup>().alpha = 0f;
            if (decoMaskArea != null)  // 호수 배경 숨기기              
                decoMaskArea.SetActive(false);

            // TODO: 섬 전용 로직, 섬 3d그리드 켜기 
            // 3D 그리드 편집모드 진입
            if (PlacementMgr.Instance != null)
            {
                // PlacementMgr가 View상태일때만 편집모드로 전환
                if (PlacementMgr.Instance.CurrentState == PlacementState.View)
                    PlacementMgr.Instance.ToggleEditMode();
            }
            if (aquariumMgr != null) aquariumMgr.HideFish(); // 물고기 숨기기
            if (playerObject != null)
            {
                foreach (var renderer in playerObject.GetComponentsInChildren<Renderer>())
                    renderer.enabled = false;  // 곰 숨기기
                foreach (var col in playerObject.GetComponentsInChildren<Collider>(true)) 
                    col.enabled = false; // 콜라이더 끄기
            }
        }
        HideWeatherParticle();
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
            itemListManager.SetupInventory(); // 인벤호출
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

    // 편집 모드 나가기
    public void ExitEditMode()
    {
        isEditMode = false;
        // Fix 필터 모드 해제
        if (itemListManager != null && itemListManager.IsFixFilterMode)
            itemListManager.ExitFilterMode();

        // 블로커 비활성화 
        if (dropdownBlocker != null)
            dropdownBlocker.SetActive(false);
     
        // 백그라운드 
        if (dimBackground != null)
        {
            dimBackground.DOColor(new Color(0, 0, 0, 0), animTime)
            .OnComplete(() => dimBackground.gameObject.SetActive(false));
        }

        // 호수 전용 편집모드 퇴장
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
            if (aquariumMgr != null) aquariumMgr.ShowFish(); // 물고기 다시 보이기

            if (lakeBackground != null) // 뒤에 호수 위치 복귀
            {
                lakeBackground.DOAnchorPosY(lakeOriginPos.y, animTime)
                 .SetEase(Ease.OutQuad);
            }
            if (floorPlane != null) // 바닥재 복귀 
            {
                floorPlane.DOAnchorPosY(floorOriginPos.y, animTime)
                    .SetEase(Ease.OutQuad);
            }
            // 장식물 위치 복귀 
            if (gridManager != null && gridManager.placedObjectsParent != null)
            {
                gridManager.placedObjectsParent.GetComponent<RectTransform>()
                    .DOAnchorPosY(placedObjOriginPos.y, animTime)
                    .SetEase(Ease.OutQuad);
            }
        }

        // 섬 전용 편집모드 퇴장
        if (currentMode == DecoMode.Island)
        {
            if (gridPanel != null) // 호수 2d그리드 복구 
                gridPanel.gameObject.SetActive(true);
            if (lakeBackground != null) // 호수 다시 보이기
                lakeBackground.GetComponent<CanvasGroup>().alpha = 1f;
            if (decoMaskArea != null) // 호수 배경 보이기            
                decoMaskArea.SetActive(true);

            // TODO: 섬 보이는거 비활성화 
            if (PlacementMgr.Instance != null)
            {
                // PlacementMgr가 Edit상태일때만 편집모드로 전환
                if (PlacementMgr.Instance.CurrentState == PlacementState.Edit)
                    PlacementMgr.Instance.ToggleEditMode();
            }
            holdItemId = -1; // 나갈때 배치 대기 아이템 초기화 
            if (aquariumMgr != null) aquariumMgr.ShowFish(); // 물고기 다시 보이기
            if (playerObject != null) // 곰 다시 보이기
            {
                foreach (var renderer in playerObject.GetComponentsInChildren<Renderer>())
                    renderer.enabled = true;
                foreach (var col in playerObject.GetComponentsInChildren<Collider>(true))
                    col.enabled = true;
            }
            if (UnlockCount == 0 || Time.unscaledTime - UnlockFirstTime > 10f)
            { UnlockCount = 0; UnlockFirstTime = Time.unscaledTime; }
            if (++UnlockCount >= 10 && btnLakeDecoMode != null)
                isUnlocked = true;
        }
        RestoreWeatherParticle(); // 날씨 파티클 복원
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
            btnLakeDecoMode.gameObject.SetActive(isUnlocked);
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
    void OnSaveClicked() { ShowConfirmPopup(0, () => OnSave()); }          
    void OnRecallAllClicked() { ShowConfirmPopup(1, () => OnRecallAll()); }
    void OnResetClicked() { ShowConfirmPopup(2, () => OnReset()); }
    //    {
    //  //  if (!isChanged) return;
    //    ShowConfirmPopup(2, () => OnReset());
    //}

    void ShowConfirmPopup(int type, System.Action onYes) 
    {
        pendingConfirmAction = onYes;
        if (decoTempText != null) decoTempText.SetConfirmMsg(type);
        if (confirmPopupPanel != null)
        { confirmPopupPanel.SetActive(true); LockTopButtons(false); }
        else
            onYes?.Invoke();
    }

    void OnConfirmYes() 
    {
        pendingConfirmAction?.Invoke();
        pendingConfirmAction = null;
        if (confirmPopupPanel != null) confirmPopupPanel.SetActive(false);
        LockTopButtons(true);
    }

    void OnConfirmClose() 
    {
        pendingConfirmAction = null;
        if (confirmPopupPanel != null) confirmPopupPanel.SetActive(false);
        LockTopButtons(true);
    }
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
            PlacementMgr.Instance?.OnClickConfirmSession(); // 모든 변경사항 확정 후 저장
          
            //if (buildingMgr != null)
            //    buildingMgr.ConfirmAll();

            if (itemListManager != null)
                itemListManager.SaveSnapshot(); 
            isChanged = false;
        }
    }
    void OnReset() // 초기화, 저장한 상태로 불러오기
    {
        if (!isChanged) return; // 변경사항 없으면 무시 // TODO: 리턴이 없으면 초기화버튼 2번 누르면 전체회수 버그 발생하는지 확인필요  

        // TODO:  (파베 연결 후에 나중에 구현)
        if (currentMode == DecoMode.Lake) // 호수 모드
        {
            // 인벤을 마지막 저장 시점으로 복구
            if (itemListManager != null)
                itemListManager.LoadSnapshot();
            // 배치를 마지막 저장 시점으로 복구
            if (gridManager != null)
                gridManager.LoadSnapshot();
            isChanged = false;
        }
        else if (currentMode == DecoMode.Island) // 섬모드 
        {
            PlacementMgr.Instance?.OnClickCancelSession(); // 모든 변경사항 취소
          
            if (itemListManager != null)
                itemListManager.ReturnAllItems();
            //if (buildingMgr != null)
            //{
            //    buildingMgr.RevertAll();          // 되돌리기
            //    buildingMgr.CancelCurrentAction(); // 들고있는 거 취소
            //}
            isChanged = false; 
        }
    } 
    void OnRecallAll()  // 전체회수 : 배치된 거 전부 인벤으로 회수 
    {
        // Fix 필터 모드 해제 
        if (itemListManager != null && itemListManager.IsFixFilterMode)
            itemListManager.ExitFilterMode();

        if (currentMode == DecoMode.Lake) // 호수모드 
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
        else if (currentMode == DecoMode.Island) // 섬모드 
        {
            PlacementMgr.Instance?.OnClickAllDelete(); // 전체회수 버튼클릭 

            //if (buildingMgr != null)
            //{
            //    buildingMgr.ClearAll();
            //}

            // ClearAll이 전부 파괴하니까 인벤도 처음 상태로 복원
            //if (itemListManager != null)
            //{
            //    itemListManager.ReturnAllItems();
            //}
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
            if (itemListManager != null)
                itemListManager.ReturnAllItems();
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
        {
            itemListManager.OnSlotPick -= OnIslandPick;
            // Fix 슬롯 이벤트 해제 
            itemListManager.OnFixSlotPick -= OnFixSlotSelected;
        }
        if (buildingMgr != null)
        {
            buildingMgr.OnPlaceSuccess -= OnIslandPlaceSuccess;
            buildingMgr.OnPlaceCancel -= OnIslandPlaceCancel;
            buildingMgr.OnConfirm -= OnIslandConfirm;          
            buildingMgr.OnRevert -= OnIslandRevert;            
            buildingMgr.OnClearAll -= OnIslandClearAll;
            buildingMgr.OnFixReverted -= OnFixReverted;
        }

        if (PlacementMgr.Instance != null)
        {
            PlacementMgr.Instance.OnBuildingPick -= ShowIslandActionPanel;
            PlacementMgr.Instance.OnBuildingDrop -= HideIslandActionPanel;
            // 고정물 이벤트 해제
            PlacementMgr.Instance.OnFixedBuildingPick -= OnFixBuildingClicked;
            // 그리드 Fix 이벤트 해제 
            PlacementMgr.Instance.OnGridFixBuildingPick -= OnGridFixBuildingClicked;
          
            // TODO: 나중에 구조변경시 이벤트도 해제 
            // PlacementMgr.Instance.OnEmptyClick -= OnEmptySpaceClicked;
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
    // 블로커 공용 클릭 핸들러
    void OnBlockerClicked()
    {
        if (dropdownPanel != null && dropdownPanel.activeSelf)
        {
            HideDropdown();
        }

        // 고정물 배치템 필터 모드면 해제
        else if (itemListManager != null && itemListManager.IsFixFilterMode)
        {
            itemListManager.ExitFilterMode();
            if (dropdownBlocker != null)
                dropdownBlocker.SetActive(false);
        }
    }
    
    #region 날씨 파티클 숨기기/복원 메서드
    // 편집모드 진입 시: 현재 재생 중인 파티클을 캐싱하고 정지+비활성화
    void HideWeatherParticle()
    {
        if (weatherParticleParent == null) return;  
        
        var particles = weatherParticleParent.GetComponentsInChildren<ParticleSystem>(true);
        cachedParticle = null;

        foreach (var ps in particles)
        {
            if (ps.isPlaying)
            {
                cachedParticle = ps; // 나갈 때 복원하기 위해 기억
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.gameObject.SetActive(false);
                break; // 계절은 하나만 끄게 
            }
        }
    }

    /// 편집모드 퇴장 시: 캐싱해둔 파티클 복원
    void RestoreWeatherParticle()
    {
        if (cachedParticle != null)
        {
            cachedParticle.gameObject.SetActive(true);
            cachedParticle.Play();
            cachedParticle = null;
        }
    }
    #endregion
}