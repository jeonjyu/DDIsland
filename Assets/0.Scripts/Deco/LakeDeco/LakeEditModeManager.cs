using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// 호수 꾸미기 편집 모드 전환 관리
/// 평소: 타일이 화면 하단, 격자 안 보임, 인벤/버튼 숨김
/// 편집 모드: 격자 보임, 인벤 패널 올라옴, 타일 위로 밀림, 백그라운드+상단 버튼 활성화

public class LakeEditModeManager : MonoBehaviour
{
    [Header("2D그리드 전용")]
    public RectTransform gridPanel;
    public LakeGridManager gridManager;

    [Header("UI 연결")]
    public RectTransform bottomPanel; // 꾸미기 버튼
    public RectTransform topButtonPanel; // 인벤
    public GameObject objectActionPanel; // 배치, 취소
    public Image dimBackground;  
    public LakeItemListManager itemListManager;  // 하단에 인벤 

    [Header("상단 꾸미기 버튼")]
    public Button btnSave;          // 저장
    public Button btnExit;          // 취소 (나가기)
    public Button btnRecallAll;     // 전체 회수
    public Button btnDecoMode;      // 꾸미기 모드 (편집 모드 토글) 

    [Header("하단 인벤")]
    public float animTime = 0.3f;           // 애니메이션 시간
    public float bottomPanelHeight = 240f;  // 인벤 패널 높이
    public float gridMoveUp = 240f;   // 그리드 위로 올리는 높이

    // 내부 변수
    bool isEditMode = false;
    Vector2 girdCurrentPos;    // 그리드 원래 위치 저장
    float dimAlpha = 0.5f;  // 백그라운드 배경 알파값

    //  초기화
    void Start()
    {
        // 그리드 원래 위치 저장
        if (gridPanel != null)
            girdCurrentPos = gridPanel.anchoredPosition;

        // 처음에는 편집 모드 OFF 상태로 세팅
        SetEditModeOff();

        // 버튼 연결
        if (btnDecoMode != null)
            btnDecoMode.onClick.AddListener(ToggleEditMode);

        if (btnSave != null)
            btnSave.onClick.AddListener(OnSave);

        if (btnExit != null)
            btnExit.onClick.AddListener(OnExit);

        if (btnRecallAll != null)
            btnRecallAll.onClick.AddListener(OnRecallAll);
    }

    // 편집 모드 토글
    public void ToggleEditMode()
    {
        if (isEditMode)
            ExitEditMode();
        else
            EnterEditMode();
    }

    // 편집 모드 진입
    public void EnterEditMode()
    {
        isEditMode = true;

        // 백그라운드 
        if (dimBackground != null) 
        { 
            dimBackground.gameObject.SetActive(true);
            dimBackground.color = new Color(0, 0, 0, 0);
            dimBackground.DOColor(new Color(0, 0, 0, dimAlpha), animTime);
        }
        // 격자 
        if (gridManager != null)
            gridManager.ShowGrid(2f); // 이미지에 격자가 있는게 아니라, 중간중간 띄워서 백그라운드가 보이게 하는 방식  

        // 그리드 타일 위로 밀기

        if (gridPanel != null)
        {
            gridPanel.DOAnchorPosY(girdCurrentPos.y + gridMoveUp, animTime)
            .SetEase(Ease.OutQuad);
        }

        // 인벤 패널 아래에서 위로 슥 올라오기 
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
            itemListManager.SetupTestInventory();
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
        // 격자 

        if (gridManager != null)
            gridManager.HideGrid();

        // 그리드 
        if (gridPanel != null)
        {
            gridPanel.DOAnchorPosY(girdCurrentPos.y, animTime)
            .SetEase(Ease.OutQuad);
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
        if (bottomPanel != null) bottomPanel.gameObject.SetActive(false);

        // 상단 버튼 숨기기
        if (topButtonPanel != null) topButtonPanel.gameObject.SetActive(false);

        // 오브젝트 액션 패널 숨기기 (배치, 취소)
        if (objectActionPanel != null)
            objectActionPanel.SetActive(false);
    }

    //  상단 버튼 기능
    void OnSave()
    {
        // TODO: 저장 로직 (나중에 구현)
        ExitEditMode();
    }

    void OnExit()
    {
        // TODO: 변경사항 되돌리기 확인 팝업 (나중에 구현)
        ExitEditMode();
    }

    void OnRecallAll()
    {
        // 전체 회수
        if (gridManager != null)
            gridManager.RecallAll();
    }

    //  외부에서 상태 확인용
    public bool IsEditMode()
    {
        return isEditMode;
    }
}