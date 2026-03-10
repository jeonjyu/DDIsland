using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// [호수 편집 모드] 
public class LakePlaceController : MonoBehaviour
{
    [Header("2d 그리드 배치 전용")]
    public LakeGridManager gridManager;
    public DecoItemListManager itemListManager;
    public DecoEditModeManager editModeManager;
    public RectTransform gridArea; // GridPanel

    [Header("액션 패널 (회수/이동)")]
    public GameObject objectActionPanel; 
    public Button btnRecall;  // 회수            
    public Button btnMove;    // 이동            
    public Button btnCancel;  // 취소
    [Header("미리보기 오브젝트 클릭시 띄우는 높이")]
    public float previewOffsetY = 30f; // 미리보기 띄우는 높이

    bool isPlacing = false; // 지금 배치 모드인지
    GameObject previewObj;  // 마우스 따라다니는 프리뷰 오브젝트
    int currentItemId = -1; // 현재 선택된 아이템 ID
    int currentSizeX = 1;  
    int currentSizeY = 1;
    bool skipNextClick = false; // 슬롯 클릭한 프레임에 좌클릭 씹히는거 방지
   
    string selectedObjectId = ""; // 현재 선택된 배치 오브젝트 ID


    void Start()
    {
        // 아이템 리스트 이벤트 연결
        if (itemListManager != null)
        {
            itemListManager.OnSlotPick += OnSlotPick;
            itemListManager.OnSlotCancel += OnSlotCancel;
        }

        // 액션 버튼 연결
        if (btnRecall != null) // 옵젝 회수
            btnRecall.onClick.AddListener(OnRecallClicked);
        if (btnMove != null)  // 옵젝 이동 
            btnMove.onClick.AddListener(OnMoveClicked);
        if (btnCancel != null) // 패널 닫기 
            btnCancel.onClick.AddListener(HideActionPanel); 
    }

    void OnDestroy()
    {
        // 이벤트 해제 (메모리 누수 방지)
        if (itemListManager != null)
        {
            itemListManager.OnSlotPick -= OnSlotPick;
            itemListManager.OnSlotCancel -= OnSlotCancel;
        }
    }

    void Update()
    {
        // 편집 모드 아니면 파업
        if (editModeManager != null && !editModeManager.IsEditMode())
            return;
        // 호수가 아니면 파업
        if (editModeManager != null && editModeManager.GetCurrentMode() != DecoMode.Lake)
            return;

        if (isPlacing)
        {

            if (skipNextClick)
            {
                skipNextClick = false;
                return;
            }

            // 마우스 위치가 그리드 좌표
            Vector2Int gridPos = gridManager.MouseToGridPos();

            // 좌클릭 시 배치 시도
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {   
                // 그리드 밖이면 클릭취소 
                if (gridManager.IsOutOfBounds(gridPos.x, gridPos.y))
                {
                   // CancelPlacing(); // 그리드 밖에서 좌클로 취소 되게 
                    return;
                }
                TryPlace(gridPos);
                return; // 배치 시도 후 이번 프레임은 끝
            }
            // todo: 코루틴 
            // 우클릭 시 취.소.
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                CancelPlacing();
                return;
            }
            
            // 업데이트 순서를 클릭 안했을때만
            // 미리보기 색상 표시 (초록/빨강)
            // 배치 예상도 중앙 정령 
            int cx = gridPos.x - (currentSizeX / 2);
            int cy = gridPos.y - (currentSizeY / 2);
            // 클램프로 배치 오브젝트가 그리드 밖으로 안 나가게 제한
            cx = Mathf.Clamp(cx, 0, gridManager.GetGridWidth() - currentSizeX);
            cy = Mathf.Clamp(cy, 0, gridManager.GetGridHeight() - currentSizeY);
            gridManager.ShowPreview(cx, cy, currentSizeX, currentSizeY);
            // 오브젝트를 마우스 위치로 이동 // 자유 자재로 움직이는 마우스 커서 
            //if (previewObj != null)
            //{
            //    RectTransform rt = previewObj.GetComponent<RectTransform>();
            //    Vector2 mousePos = Mouse.current.position.ReadValue();
            //    rt.position = mousePos + new Vector2(0, 40f); // 마우스 위치 위로 살짝 띄움 
            //}

            // 그리드에 스냅 (뚝뚝 끊기는 방식) // todo: 미리보기 오브젝트는 마우스의 중앙위치에 오도록
            if (previewObj != null)
            {
                RectTransform rt = previewObj.GetComponent<RectTransform>();
                rt.SetParent(gridManager.transform, false); // 그리드 기준 좌표 사용
                rt.anchoredPosition = gridManager.GridToSnapPos(cx, cy, currentSizeX, currentSizeY)
                    + new Vector2(0, previewOffsetY); 
            } //todo: 코루틴 

        }
        else // 배치모드가 아닐때 
        {
            // 배치된 오브젝트 클릭 감지 
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                
                // 액션패널이 열려있으면 클릭 무시 (버튼 클릭 우선)
                if (objectActionPanel != null && objectActionPanel.activeSelf)
                    return;

                string objectId = gridManager.GetObjectIdAtMouse();

                if (string.IsNullOrEmpty(objectId) || objectId == "fixed")
                {
                    HideActionPanel();
                    return;
                }

                selectedObjectId = objectId;
                ShowActionPanel();
            }
        }

    }
    #region 액션 패널(회수/이동)
    void ShowActionPanel()
    {
        if (objectActionPanel == null) return;
        objectActionPanel.SetActive(true);

        RectTransform rt = objectActionPanel.GetComponent<RectTransform>();
        Vector2 mousePos = Mouse.current.position.ReadValue();
        rt.position = mousePos + new Vector2(0, 60f);

        if (gridManager != null && !string.IsNullOrEmpty(selectedObjectId))
        {
            GameObject visual = gridManager.GetVisual(selectedObjectId);
            if (visual != null)
            {    // 두트윈으로 올리기 
                RectTransform vrt = visual.GetComponent<RectTransform>();
                vrt.DOKill();  // 중복 방지
                vrt.DOAnchorPosY(vrt.anchoredPosition.y + 20f, 0.2f) 
                    .SetEase(Ease.OutBack);
            }
        }
    }
  
    // 액션 패널 숨기기
    void HideActionPanel()
    {
        // 띄운 오브젝트 내리기
        if (gridManager != null && !string.IsNullOrEmpty(selectedObjectId))
        {
            GameObject visual = gridManager.GetVisual(selectedObjectId);
            if (visual != null)
            {
                RectTransform vrt = visual.GetComponent<RectTransform>();
                vrt.DOKill();
                vrt.DOAnchorPosY(vrt.anchoredPosition.y - 20f, 0.15f);
            }
        }

        selectedObjectId = "";
        if (objectActionPanel != null)
            objectActionPanel.SetActive(false);
    }
  
    // 회수 버튼
    void OnRecallClicked()
    {
        if (string.IsNullOrEmpty(selectedObjectId)) return;

        int itemId = gridManager.GetItemIdByObjectId(selectedObjectId);
        gridManager.RecallObject(selectedObjectId);

        if (itemId >= 0 && itemListManager != null)
            itemListManager.RestoreItem(itemId); // 복원
    
        if (editModeManager != null)
            editModeManager.SetChanged();   

        HideActionPanel();
    }

    // 이동 버튼
    void OnMoveClicked()
    {
        if (string.IsNullOrEmpty(selectedObjectId)) return;

        int itemId = gridManager.GetItemIdByObjectId(selectedObjectId);
        if (itemId < 0) return;

        // 인벤에 복원 
        if (itemListManager != null)
            itemListManager.RestoreItem(itemId);

        // 타일에서 제거
        gridManager.RecallObject(selectedObjectId);

        // 다시 배치 모드
        currentItemId = itemId;
        Vector2Int size = LakeDecoTestData.GetItemSize(itemId);
        currentSizeX = size.x;
        currentSizeY = size.y;

        isPlacing = true;
        skipNextClick = true;
        CreatePreviewObject();
        HideActionPanel();

        if (editModeManager != null)
            editModeManager.SetChanged();
    }
    #endregion


    // 아이템 리스트에서 슬롯 클릭했을 때
    void OnSlotPick(int itemId, int slotIndex)
    {
        // 이미 배치 중이면 이전 것 정리
        if (isPlacing)
            CleanupPreview();

        // 아이템 정보 세팅
        currentItemId = itemId;
        Vector2Int size = LakeDecoTestData.GetItemSize(itemId);
        currentSizeX = size.x;
        currentSizeY = size.y;

        // 배치 모드 시작
        isPlacing = true;
        skipNextClick = true; 
        CreatePreviewObject();
        
        // 상단 버튼 클릭 막기 
        if (editModeManager != null)
            editModeManager.LockTopButtons(false);
    }
  
    void OnSlotCancel()
    {
        CancelPlacing();
    }

    // 배치 시도
    void TryPlace(Vector2Int gridPos)
    {
        gridManager.ClearPreview(); // 먼저 프리뷰 클리어 (색상 초기화 위해)
        
        // 중앙 기준
        int cx = gridPos.x - (currentSizeX / 2);
        int cy = gridPos.y - (currentSizeY / 2);
        // 클램프로 배치 오브젝트가 그리드 밖으로 안 나가게 제한
        cx = Mathf.Clamp(cx, 0, gridManager.GetGridWidth() - currentSizeX);
        cy = Mathf.Clamp(cy, 0, gridManager.GetGridHeight() - currentSizeY);
        bool success = gridManager.PlaceObject(currentItemId, cx, cy, currentSizeX, currentSizeY);
        if (success)
        {
            // 배치 성공 시 수량 차감
            if (itemListManager != null)
                itemListManager.UseItem(currentItemId);

            // 프리뷰 정리
            CleanupPreview();
            isPlacing = false;

            // 선택 해제
            if (itemListManager != null)
                itemListManager.DeselectSlot();
            
            // 상단 버튼 복원
            if (editModeManager != null)
                editModeManager.LockTopButtons(true);

            if (editModeManager != null)
                editModeManager.SetChanged();
        }
        else
        {
            // 실패하면 아무것도 안 함 (계속 배치 모드 유지)
        }
    }

    // 배치 취소 
    void CancelPlacing()
    {
        if (!isPlacing) return;

        isPlacing = false;
        currentItemId = -1;
        gridManager.ClearPreview();
        CleanupPreview();

        // 슬롯 선택 해제
        if (itemListManager != null)
            itemListManager.DeselectSlot();

        // 상단 버튼 다시 활성화
        if (editModeManager != null)
            editModeManager.LockTopButtons(true);
    }

    // 마우스 따라다니는 미리보기 오브젝트 생성
    void CreatePreviewObject()
    {
        previewObj = new GameObject("PlacementPreview");
        previewObj.transform.SetParent(gridArea, false);

        RectTransform rt = previewObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(
            gridManager.GetTileWidth() * currentSizeX,
            gridManager.GetTileHeight() * currentSizeY
        );
        rt.pivot = new Vector2(0.5f, 0.5f);

        Image img = previewObj.AddComponent<Image>();
        Sprite sprite = LakeDecoTestData.GetIconSprite(currentItemId);
        if (sprite != null)
            img.sprite = sprite;
        img.color = new Color(0.3f, 0.8f, 1f, 0.5f);  // 반투명 하늘색
        img.raycastTarget = false;  // 클릭 방해 안 하게

        // TODO: 나중에 실제 아이템 스프라이트로 교체
    }

    // 프리뷰 오브젝트 삭제
    void CleanupPreview()
    {
        if (previewObj != null)
        {
            Destroy(previewObj);
            previewObj = null;
        }
    }

    // 헬퍼 함수 
    // 외부에서 배치 중인지 확인
    public bool IsPlacing()
    {
        return isPlacing;
    }

    // 편집 모드 나갈 때 정리 (LakeEditModeManager.cs에서 호출)
    public void ForceCancel()
    {
        if (isPlacing)
        {
            isPlacing = false;
            currentItemId = -1;
            gridManager.ClearPreview();
            CleanupPreview();
        }
    }
}