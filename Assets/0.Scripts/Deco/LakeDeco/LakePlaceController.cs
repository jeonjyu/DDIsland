using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

/// [호수 편집 모드] 
public class LakePlaceController : MonoBehaviour
{
    [Header("2d 그리드 배치 전용")]
    public LakeGridManager gridManager;
    public LakeItemListManager itemListManager;
    public LakeEditModeManager editModeManager;
    public RectTransform gridArea; // GridPanel

    // 내부 변수
    bool isPlacing = false; // 지금 배치 모드인지
    GameObject previewObj;  // 마우스 따라다니는 프리뷰 오브젝트
    int currentItemId = -1; // 현재 선택된 아이템 ID
    int currentSizeX = 1;  
    int currentSizeY = 1;  

    void Start()
    {
        // 아이템 리스트 이벤트 연결
        if (itemListManager != null)
        {
            itemListManager.OnItemSelected += OnItemSelected;
            itemListManager.OnItemDeselected += OnItemDeselected;
        }
    }

    void OnDestroy()
    {
        // 이벤트 해제 (메모리 누수 방지)
        if (itemListManager != null)
        {
            itemListManager.OnItemSelected -= OnItemSelected;
            itemListManager.OnItemDeselected -= OnItemDeselected;
        }
    }

    void Update()
    {
        // 편집 모드 아닐 때는 아무것도 안 함
        if (editModeManager != null && !editModeManager.IsEditMode())
            return;

        if (!isPlacing) return;

        // 마우스 위치 시 그리드 좌표
        Vector2Int gridPos = gridManager.MouseToGridPos();

        // 미리보기 색상 표시 (초록/빨강)
        gridManager.ShowPreview(gridPos.x, gridPos.y, currentSizeX, currentSizeY);

        // 프리뷰 오브젝트를 마우스 위치로 이동
        if (previewObj != null)
        {
            RectTransform rt = previewObj.GetComponent<RectTransform>();
            rt.position = Mouse.current.position.ReadValue();
        }

        // 좌클릭 시 배치 시도
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPlace(gridPos);
        }

        // 우클릭 시 취소
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacing();
        }
    }

    // 아이템 리스트에서 슬롯 클릭했을 때
    void OnItemSelected(int itemId, int slotIndex)
    {
        // 이미 배치 중이면 이전 것 정리
        if (isPlacing)
            CleanupPreview();

        // 아이템 정보 세팅
        currentItemId = itemId;
        Vector2Int size = GetItemSize(itemId);
        currentSizeX = size.x;
        currentSizeY = size.y;

        // 배치 모드 시작
        isPlacing = true;
        CreatePreviewObject();
    }

    void OnItemDeselected()
    {
        CancelPlacing();
    }

    // 배치 시도
    void TryPlace(Vector2Int gridPos)
    {
        bool success = gridManager.PlaceObject(currentItemId, gridPos.x, gridPos.y, currentSizeX, currentSizeY);

        if (success)
        {
            // 배치 성공 시 수량 차감
            if (itemListManager != null)
                itemListManager.ConsumeItem(currentItemId);

            // 프리뷰 정리
            CleanupPreview();
            isPlacing = false;
            gridManager.ClearPreview();

            // 선택 해제
            if (itemListManager != null)
                itemListManager.DeselectSlot();
        }
        // 실패하면 아무것도 안 함 (계속 배치 모드 유지)
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
    }

    // 프리뷰 오브젝트 
    // 마우스 따라다니는 반투명 프리뷰 생성
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


    // 더미 데이터
    Vector2Int GetItemSize(int itemId)
    {
        switch (itemId)
        {
            case 10001: return new Vector2Int(1, 1);
            case 10002: return new Vector2Int(1, 1);
            case 10003: return new Vector2Int(1, 1);
            case 10004: return new Vector2Int(2, 1);
            case 10005: return new Vector2Int(2, 1);
            case 10006: return new Vector2Int(2, 1);
            case 10007: return new Vector2Int(4, 2);
            case 10008: return new Vector2Int(1, 1);
            case 10009: return new Vector2Int(1, 1);
            case 10010: return new Vector2Int(2, 1);

            default: return new Vector2Int(1, 1);
        }
    }

    // 헬퍼 함수 
    // 외부에서 배치 중인지 확인
    public bool IsPlacing()
    {
        return isPlacing;
    }

    // 편집 모드 나갈 때 강제 정리 (LakeEditModeManager.cs에서 호출)
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