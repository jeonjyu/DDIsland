using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-50)]
public class WindowController : Singleton<WindowController>
{
    private WindowCore core = null;

    #region 작업표시줄
    // 작업 표시줄 변경 메서드 형식 정의
    public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;    // 위치 변경 이벤트
    private const uint WINEVENT_OUTOFCONTEXT = 0;

    private IntPtr hook;
    private WinEventDelegate procDelegate;

    private bool isTaskbarAlreadyChanged = false;

    private Coroutine taskbarCoroutine;
    #endregion

    private Camera currentCamera;
    private CameraClearFlags originalCameraClearFlags;
    private Color originalCameraBackground;

    private PointerEventData pointerEventData;      // 레이캐스트에서 사용할 마우스 이벤트 정보
    private int hitLayerMask;                       // 레이캐스트에서 사용할 레이어 정보
    [SerializeField] private float hitTick = 0.1f;  // 마우스 클릭 통과 여부 판단 주기
    private WaitForSecondsRealtime hitTime;


    private HitType hitType = HitType.None;
    private Coroutine hitMouseCoroutine;

    public Vector2 WindowPosition
    {
        get { return core != null ? core.GetWindowPosition() : Vector2.zero; }
        set { core.SetWindowPosition(value); }
    }

    public Vector2 WindowSize
    {
        get { return core != null ? core.GetWindowSize() : Vector2.zero; }
        set { core.SetWindowSize(value); }
    }

    [SerializeField] private bool isTransparent;
    public bool IsTransparent
    {
        get { return isTransparent; }
        set
        {
            core?.SetTransparentType(TransparentType.Alpha);
            core?.EnableTransparent(value);
            hitType = value ? HitType.Raycast : HitType.None;
            isTransparent = value;
        }
    }

    [SerializeField] private bool isClickThrough;
    public bool IsClickThrough
    {
        get { return isClickThrough; }
        set
        {
            core?.EnableClickThrough(value);
            isClickThrough = value;

            if (value)
            {
                if (hitMouseCoroutine != null)
                {
                    StopCoroutine(hitMouseCoroutine);
                    hitMouseCoroutine = null;
                }

                hitMouseCoroutine = StartCoroutine(HitMouse());
            }
        }
    }

    [SerializeField] private bool isTopmost;
    public bool IsTopmost
    {
        get { return isTopmost; }
        set
        {
            core?.EnableTopmost(value);
            isTopmost = value;
        }
    }

    [SerializeField] private bool isBorderless;
    public bool IsBorderless
    {
        get { return isBorderless; }
        set
        {
            core.SetBorderless(value);
            isBorderless = value;
        }
    }

    [SerializeField] private int targetFrame = 144;

    private bool onObject;      // 현재 마우스가 오브젝트 위에 있는지 판단하는 변수

    // 현재 모니터 인덱스 반환
    public int GetCurrentMonitor() => core.GetCurrentMonitor();

    // 모니터 개수 반환
    public int GetMonitorCount() => core.GetMonitorCount();

    // 모니터 변경
    public void ChangeMonitor(int monitorIndex) => core.FitToMonitor(monitorIndex);

    protected override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = targetFrame;
        currentCamera = Camera.main;

        if (currentCamera)
        {
            originalCameraClearFlags = currentCamera.clearFlags;
            originalCameraBackground = currentCamera.backgroundColor;
        }

        // 창 제어 클래스 인스턴스화
        core = new WindowCore();
        core.AttachMyWindow();

        // 마우스 이벤트 정보
        pointerEventData = new PointerEventData(EventSystem.current);

        // Ignore Raycast를 제외한 모든 레이어를 검출
        hitLayerMask = ~LayerMask.GetMask("Ignore Raycast", "HoverOnly");

        // 마우스 통과 여부 체크 로직 주기 초기화
        hitTime = hitTick == 0 ? new WaitForSecondsRealtime(0.1f) : new WaitForSecondsRealtime(hitTick);
    }

    private void Start()
    {
#if !UNITY_EDITOR
        core.SetAlphaValue(1.0f);

        currentCamera.clearFlags = CameraClearFlags.SolidColor;
        currentCamera.backgroundColor = Color.clear;

        IsTransparent = isTransparent;
        //IsClickThrough = isClickThrough;
        IsTopmost = isTopmost;

        // 이벤트를 받을 함수 받기
        procDelegate = new WinEventDelegate(WinEventProc);

        // 훅 연결
        hook = core.SetWinEventHook(EVENT_OBJECT_LOCATIONCHANGE, EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, procDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
#endif
        IsClickThrough = isClickThrough;
    }

    private void Update()
    {
#if !UNITY_EDITOR
        core.Update();
#endif
        UpdateClickThrough();
    }

    /// <summary>
    /// 마우스 위치 좌표에 오브젝트나 UGUI가 있는지 검사
    /// </summary>
    private void UpdateClickThrough()
    {
        if (hitType == HitType.None) return;

        if (isClickThrough)
        {
            // 마우스 통과 상태일 때 오브젝트 위에 있다면 클릭을 막아둠
            if (onObject)
            {
                IsClickThrough = false;
            }
        }
        else
        {
            // 투명 배경 + 마우스 위치에 오브젝트가 없을 경우 클릭 통과
            if (IsTransparent && !onObject)
            {
                IsClickThrough = true;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator HitMouse()
    {
        while (Application.isPlaying)
        {
            yield return hitTime;

            HitByRaycast();
        }

        yield break;
    }

    /// <summary>
    /// 레이캐스트를 이용한 마우스 통과 여부 판단
    /// </summary>
    private void HitByRaycast()
    {
        Vector2 position = GetClientCursorPosition();

        // 마우스 위치에 UGUI가 있는지 검사
        var raycastResults = new List<RaycastResult>();
        pointerEventData.position = position;
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        foreach (var result in raycastResults)
        {
            if (((1 << result.gameObject.layer) & hitLayerMask) > 0)
            {
                onObject = true;
                return;
            }
        }

        // 마우스 위치에 게임 오브젝트가 있는지 검사
        if (currentCamera != null && currentCamera.isActiveAndEnabled)
        {
            Ray ray = currentCamera.ScreenPointToRay(position);

            // 3D 오브젝트 검사
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                onObject = true;

                if (GameManager.Instance.StageController != null)
                {
                    // 상호작용 오브젝트 검사
                    if (hit.collider.TryGetComponent<IInteract>(out var interactObj))
                    {
                        GameManager.Instance.StageController.InteractObj = interactObj;
                    }
                    else
                    {
                        GameManager.Instance.StageController.InteractObj = null;
                    }
                }

                return;
            }
            else if (GameManager.Instance.StageController != null)
            {
                if (GameManager.Instance.StageController.InteractObj != null)
                {
                    GameManager.Instance.StageController.InteractObj = null;
                }
            }

            // 2D 오브젝트 검사
            var rayHit2D = Physics2D.GetRayIntersection(ray);
            if (rayHit2D.collider != null)
            {
                onObject = true;
                return;
            }
        }
        else
        {
            currentCamera = Camera.main;
        }

        onObject = false;
    }

    /// <summary>
    /// 화면상의 마우스 좌표를 유니티 좌표로 변환 후 반환
    /// </summary>
    /// <returns> 마우스 좌표값 </returns>
    private Vector2 GetClientCursorPosition()
    {
        // 현재 마우스 좌표
        Vector2 mousePos = WindowCore.GetCursorPosition();

        // 현재 창 위치 좌표
        Vector2 winPos = WindowPosition;

        // 작업 공간 영역 좌표 (타이블바, 테두리 등)
        Rect clientRect = core.GetClientRectangle();

        // 유니티 화면상의 좌표로 변환
        Vector2 unityPos = new Vector2(
            (mousePos.x - winPos.x - clientRect.x) * Screen.width / clientRect.width,
            (mousePos.y - winPos.y - clientRect.y) * Screen.height / clientRect.height
            );

#if UNITY_EDITOR
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.mousePosition;
#elif UNITY_EDITOR && ENABLE_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
            return unityPos;
#endif
#else
            return unityPos;
#endif
    }

    private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        // 변경된 정보가 작업표시줄일 경우 실행
        if (hwnd != IntPtr.Zero && hwnd == core.Taskbar && !isTaskbarAlreadyChanged)
        {
            // 작업표시줄 설정이 변경되었을 때 실행할 로직
            if (taskbarCoroutine != null)
            {
                StopCoroutine(taskbarCoroutine);
                taskbarCoroutine = null;
            }

            taskbarCoroutine = StartCoroutine(FitMonitorToTaskbar());
        }

        // 게임 창의 상태가 변했을 때 (단 게임 내의 오브젝트 상태가 변했을 때는 실행X)
        if(hwnd == core.GameWindowHandle && idObject != 0)
        {
            int newMonitor = core.GetCurrentMonitor();
            Debug.Log("단축키로 모니터 위치 변경");
        }
    }

    private IEnumerator FitMonitorToTaskbar()
    {
        isTaskbarAlreadyChanged = true;

        yield return new WaitForSeconds(0.5f);
        core.FitToMonitor(core.GetCurrentMonitor());
        isTaskbarAlreadyChanged = false;
    }

    public void OnClick_Quit()
    {
        Application.Quit();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (taskbarCoroutine != null)
        {
            StopCoroutine(taskbarCoroutine);
            taskbarCoroutine = null;
        }

        if (hitMouseCoroutine != null)
        {
            StopCoroutine(hitMouseCoroutine);
            hitMouseCoroutine = null;
        }

        // 훅 해제
        if (hook != IntPtr.Zero)
            core.UnhookWinEvent(hook);
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        if (taskbarCoroutine != null)
        {
            StopCoroutine(taskbarCoroutine);
            taskbarCoroutine = null;
        }

        if (hitMouseCoroutine != null)
        {
            StopCoroutine(hitMouseCoroutine);
            hitMouseCoroutine = null;
        }

        // 훅 해제
        if (hook != IntPtr.Zero)
            core.UnhookWinEvent(hook);
    }
}