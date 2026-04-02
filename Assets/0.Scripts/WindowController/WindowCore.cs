using AOT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEngine;
using static WindowController;

#region 열거형, 구조체 정의
/// <summary>
/// 투명화 타입
/// </summary>
public enum TransparentType : int
{
    None = 0,       // 투명화 효과 사용 x
    Alpha = 1,      // 전체 창을 투명화
    ColorKey = 2,   // 특정 색을 투명화 (크로마키 형식)
}

public enum HitType : int
{
    None = 0,       // 마우스가 통과하지 않음
    Opacity = 1,    // 픽셀의 투명도 기준으로 통과 (알파값이 0이면 통과)
    Raycast = 2,    // 레이캐스트를 쏴서 판단하여 통과
}

[Flags]
public enum WindowStateEventType : int
{
    None = 0,
    StyleChanged = 1,
    Resized = 2,

    TopMostEnabled = 16 + 1 + 8,
    TopMostDisabled = 16 + 1,
    BottomMostEnabled = 32 + 1 + 8,
    BottomMostDisabled = 32 + 1,
    WallpaperModeEnabled = 64 + 1 + 8,
    WallpaperModeDisabled = 64 + 1,
}

[Flags]
public enum MouseButton : int
{
    None = 0,
    Left = 1,
    Right = 2,
    Middle = 4,
}

[Flags]
public enum ModifierKey : int
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Command = 8,
}

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}

/// <summary>
/// 작업표시줄 데이터를 담을 곳
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct TaskbarData
{
    public uint tbSize;                 // TaskbarData 구조체 전체 크기
    public IntPtr hWnd;                 // 작업표시줄 창 핸들 ID
    public uint uCallbackMessage;       // 콜백 메시지 ID, 작업표시줄 상태가 변했을 때(예: 크기 변경, 숨김 모드 전환 등) 알려줄 상태 ID
    public uint uEdge;                  // 작업표시줄이 모니터의 어느 방향에 위치해 있는지 (0 → 좌 / 1 → 상 / 2 → 우 / 3 → 하)
    public RECT rc;                     // 작업 표시줄의 사각형 좌표 영역 (화면상의 절대 좌표)
    public IntPtr lParam;               // 추가 메시지 매개변수, 특정 상태 설정 시 부가적인 옵션 값 등이 담김
}
#endregion

/// <summary>
/// 위젯 / 오버레이 관련 기능을 관리하는 클래스
/// </summary>
public class WindowCore : IDisposable
{
    #region 네이티브 코드들
    protected class LibWindowC
    {
        // dll 에서 C# 함수를 호출할 수 있게 하기 위해 정의
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void StringCallback([MarshalAs(UnmanagedType.LPWStr)] string returnString);

        [UnmanagedFunctionPointer((CallingConvention.Winapi))]
        public delegate void IntCallback([MarshalAs(UnmanagedType.I4)] int value);

        #region 상태 확인
        // 현재 대상 창이 유효한지
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsActive();

        // 배경이 투명한 모드인지
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsTransparent();

        // 창 테두리 제거 상태인지
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsBorderless();

        // 항상 최상단에 있는 상태인지
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsTopmost();

        // 항상 최하단에 있는 상태인지
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsBottommost();

        // 천제화면 여부 받아오기
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsMaximized();

        // 자유 위치 이동 모드인지
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsFreePositioningEnabled();
        #endregion

        #region 창 연결 / 해제
        // 현재 프로세스 창 자동 탐색 후 연결
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AttachMyWindow();

        // Owner 윈도우를 찾아 연결
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AttachMyOwnerWindow();

        // 현재 활성화중인 창을 연결
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AttachMyActiveWindow();

        // 원래 스타일로 복원 후 연결 해제
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DetachWindow();

        // 현재 게임 창 찾기
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
        #endregion

        #region 창 스타일 제어

        #region 창 투명화 관련
        // 투명 모드 on / off
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetTransparent([MarshalAs(UnmanagedType.U1)] bool bEnabled);

        // 배경 투명 타입
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern bool SetTransparentType(int type);

        // 컬러키 투명 색 지정
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetKeyColor(uint colorref);

        // 투명도 값 조절
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetAlphaValue(float alpha);
        #endregion

        #region 테두리 제거
        // 창 테두리 제거 / 복구
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetBorderless([MarshalAs(UnmanagedType.U1)] bool bEnabled);
        #endregion

        #region 창 우선순위
        // 최상단 고정 여부
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetTopmost([MarshalAs(UnmanagedType.U1)] bool bEnabled);
        #endregion

        #region 마우스 클릭 통과
        // 마우스 클릭 통과 여부
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetClickThrough([MarshalAs(UnmanagedType.U1)] bool bEnabled);
        #endregion

        #endregion

        #region 창 크기 / 위치 제어

        #region 위치
        // 창 위치 설정
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetPosition(float x, float y);

        // 창 위치 얻기
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPosition(out float x, out float y);

        // 전체화면
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetMaximized([MarshalAs(UnmanagedType.U1)] bool bZoomed);
        #endregion

        #region 크기
        // 창 크기 설정
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetSize(float x, float y);

        // 전체 창 크기
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSize(out float x, out float y);

        // 현재 창의 클라이언트 크기 (타이틀 바, 테두리 등을 제외한 게임이 그려지는 공간) 받아오기
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientSize(out float width, out float height);

        // 현재 창의 클라이언트 크기, 위치 (타이틀 바, 테두리 등을 제외한 게임이 그려지는 공간) 받아오기
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientRectangle(out float x, out float y, out float width, out float height);
        #endregion

        #endregion

        #region 모니터 관련
        // 현재 창이 속한 모니터 인덱스
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern int GetCurrentMonitor();

        // 모니터 개수
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern int GetMonitorCount();

        // 모니터 화면 해상도 크기 받아오기
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorRectangle(int index, out float x, out float y, out float width, out float height);
        #endregion

        #region 마우스 / 입력
        // 마우스 이동
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern void SetCursorPosition(float x, float y);

        // 마우스 위치 얻기
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPosition(out float x, out float y);

        // 눌린 마우스 버튼 얻기
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern int GetMouseButtons();

        // Shift / Ctrl 버튼 얻기
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern int GetModifierKeys();
        #endregion

        #region 콜백 관련
        // 모니터 환경 변경 콜백 호출
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterMonitorChangedCallback([MarshalAs(UnmanagedType.FunctionPtr)] IntCallback callback);

        // 모니터 환경 변경 콜백 해제
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterMonitorChangedCallback();

        // 윈도우 스타일 변경 콜백 호출
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterWindowStyleChangedCallback([MarshalAs(UnmanagedType.FunctionPtr)] IntCallback callback);

        // 윈도우 스타일 변경 콜백 해제
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterWindowStyleChangedCallback();
        #endregion

        #region 작업표시줄 관련
        // 작업표시줄 정보 가져오기
        [DllImport("shell32")]
        public static extern IntPtr SHAppBarMessage(uint dwMessage, ref TaskbarData pData);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        // 훅 등록
        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        // 혹 해제
        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        // 특정 윈도우 핸들 찾기
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // 윈도우 열거를 위한 델리게이트
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        #endregion

        // 내부 상태 갱신 루프
        [DllImport("LibUniWinC", CallingConvention = CallingConvention.Winapi)]
        public static extern void Update();
    }
    #endregion

    #region 필드
    private const uint ABM_GETSTATE = 0x00000004;               // 메시지 타입 (작업표시줄 숨김 여부 받아오기)
    private const uint ABM_GETTASKBARPOS = 0x00000005;          // 메시지 타입 (작업표시줄 위치, 크기 받아오기)
    private const int ABS_AUTOHIDE = 0x01;                      // 상태 값 → 작업표시줄 자동 숨김 모드 비트 플래그

    private static bool wasMonitorChanged = false;              // 모니터 관련 변경 여부
    private static bool wasWindowStyleChanged = false;          // 장 스타일 관련 변경 여부

    // 윈도우 스타일 설정 비트 플래그
    private static WindowStateEventType windowStateEventType = WindowStateEventType.None;

    // 조작 가능한 윈도우가 연결되어 있는지
    public bool IsActive { get; private set; } = false;

    // 위젯 최상단 설정 여부
    private bool isTopmost = false;
    public bool IsTopmost
    {
        get { return IsActive && isTopmost; }
    }

    // 투명화 설정 여부
    private bool isTransparent = false;
    public bool IsTransparent
    {
        get { return IsActive && isTransparent; }
    }

    // 마우스 클릭 통과 여부
    private bool isClickThrough = false;
    public bool IsClickThrough
    {
        get { return IsActive && isClickThrough; }
    }

    // 창 테두리 제거 여부
    private bool isBorderless = false;
    public bool IsBorderless
    {
        get { return IsActive && isBorderless; }
    }

    // 투명화 타입
    private TransparentType transparentType = TransparentType.Alpha;

    // 투명화, 마우스 통과 지정 색
    private Color32 keyColor = new Color32(1, 0, 1, 0);

    public IntPtr GameWindowHandle { get; private set; }    // 현재 게임 창의 주소

    private TaskbarData taskbarData;

    public IntPtr Taskbar { get; private set; }     // 현재 작업 표시줄을 담아둘 변수, 모니터가 바뀌면 따라 바뀜

    private float monitorPosX = 0;
    private float monitorPosY = 0;
    private float monitorWidth = 0;
    private float monitorHeight = 0;
    #endregion

    #region 생성자 및 소멸자
    public WindowCore()
    {
        IsActive = false;
    }

    ~WindowCore()
    {
        Dispose();
    }

    public void Dispose()
    {
        // 콜백 해제
        LibWindowC.UnregisterMonitorChangedCallback();
        LibWindowC.UnregisterWindowStyleChangedCallback();
    }
    #endregion

    #region 콜백
    /// <summary>
    /// 모니터 또는 해상도가 변했을 때의 콜백
    /// 처리를 최소한으로 하기 위해 플래그만 정의
    /// </summary>
    /// <param name="monitorCount"> 모니터 개수 </param>
    [MonoPInvokeCallback(typeof(LibWindowC.IntCallback))]
    private static void MonitorChangedCallback([MarshalAs(UnmanagedType.I4)] int monitorCount)
    {
        wasMonitorChanged = true;
    }

    /// <summary>
    /// 윈도우 스타일, 최대화, 최소화 등에서 사용되는 콜백
    /// 처리를 최소한으로 하기 위해 플래그만 정의
    /// </summary>
    /// <param name="e"> 창 스타일 </param>
    [MonoPInvokeCallback(typeof(LibWindowC.IntCallback))]
    private static void WindowStyleChangedCallback([MarshalAs(UnmanagedType.I4)] int e)
    {
        wasWindowStyleChanged = true;
        windowStateEventType = (WindowStateEventType)e;
    }
    #endregion

    #region Attach or Detach
    public void DetachWindow()
    {
#if UNITY_EDITOR
        // 에디터에서 최상단에 고정되는 것을 방지하기 위함
        EnableTopmost(false);
#endif
        LibWindowC.DetachWindow();
    }

    public bool AttachMyWindow()
    {
#if UNITY_EDITOR_WIN
        var gameView = GetGameView();
        if (gameView)
        {
            gameView.Focus();
            LibWindowC.AttachMyActiveWindow();
        }
#else
        LibWindowC.AttachMyWindow();
        GameWindowHandle = LibWindowC.GetActiveWindow();
#endif
        LibWindowC.RegisterMonitorChangedCallback(MonitorChangedCallback);
        LibWindowC.RegisterWindowStyleChangedCallback(WindowStyleChangedCallback);

        IsActive = LibWindowC.IsActive();

        FitToMonitor(LibWindowC.GetCurrentMonitor());
        return IsActive;
    }

    /// <summary>
    /// 현재 프로세스에서 활성화된 창을 선택, 에디터의 경우 포커스 시에 호출
    /// </summary>
    /// <returns></returns>
    public bool AttachMyActiveWindow()
    {
        LibWindowC.AttachMyActiveWindow();
        IsActive = LibWindowC.IsActive();
        return IsActive;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 리플렉션을 이용해 유니티 에디터의 게임뷰를 반환
    /// </summary>
    /// <returns></returns>
    public static EditorWindow GetGameView()
    {
        var assembly = typeof(EditorWindow).Assembly;
        var type = assembly.GetType("UnityEditor.GameView");
        var gameView = EditorWindow.GetWindow(type);
        return gameView;
    }
#endif
    #endregion

    public void Update()
    {
        LibWindowC.Update();
    }

    /// <summary>
    /// 투명도 설정 / 해제
    /// </summary>
    /// <param name="isTransparent"> 투명도 여부 </param>
    public void EnableTransparent(bool isTransparent)
    {
        //에디터는 투명 처리가 안되거나 테두리가 달라서 제외
#if !UNITY_EDITOR
        LibWindowC.SetTransparent(isTransparent);
        LibWindowC.SetBorderless(isTransparent);
#endif
        this.isTransparent = isTransparent;
    }

    /// <summary>
    /// 투명 배경 타입 지정
    /// </summary>
    /// <param name="type"> 투명 배경 타입 </param>
    public void SetTransparentType(TransparentType type)
    {
        LibWindowC.SetTransparentType((int)type);
        transparentType = type;
    }

    /// <summary>
    /// 윈도우 알파값 설정
    /// </summary>
    /// <param name="alpha"> 설정할 알파값 수치 </param>
    public void SetAlphaValue(float alpha)
    {
        // Windows 편집기에서는 한 번 반투명으로 만들면 표시가 업데이트되지 않으므로 비활성화한다.
#if !UNITY_EDITOR_WIN
        LibWindowC.SetAlphaValue(alpha);
#endif
    }

    /// <summary>
    /// 가장 최상단에 있을지 설정
    /// </summary>
    /// <param name="isTopmost"> 최상단 여부 </param>
    public void EnableTopmost(bool isTopmost)
    {
        LibWindowC.SetTopmost(isTopmost);
        this.isTopmost = isTopmost;
    }

    /// <summary>
    /// TransparentType이 ColorKey일 때 색을 지정해주는 메서드
    /// </summary>
    /// <param name="color"> 설정할 색 </param>
    public void SetKeyColor(Color32 color)
    {
        LibWindowC.SetKeyColor((uint)(color.b * 0x10000 + color.g * 0x100 + color.r));
        keyColor = color;
    }

    /// <summary>
    /// 클릭 통과 여부 설정
    /// </summary>
    /// <param name="isThrough"> 통과 여부 </param>
    public void EnableClickThrough(bool isThrough)
    {
        // 에디터에서는 클릭이 통과되면 조작이 불가능해질 수 있기 때문에 제외
#if !UNITY_EDITOR
        LibWindowC.SetClickThrough(isThrough);
#endif
        this.isClickThrough = isThrough;
    }

    /// <summary>
    /// 전체화면
    /// 전체화면 변경 후 크기가 변경될 수 있기 때문에 주의하여 사용 요망
    /// </summary>
    /// <param name="isZoomed"> 전체화면 여부 </param>
    public void SetZoomed(bool isZoomed)
    {
        LibWindowC.SetMaximized(isZoomed);
    }

    /// <summary>
    /// 전체화면 여부 받아오기
    /// </summary>
    /// <returns> 전체화면인지 반환할 bool 값 </returns>
    public bool GetZoomed()
    {
        return LibWindowC.IsMaximized();
    }

    /// <summary>
    /// 현재 창 위치 설정
    /// 모니터의 좌상단이 (0, 0)임 / 모니터의 경우 오른쪽(x)과 아래(y)로 갈 수록 값이 커짐
    /// 모니터가 여러개일 경우 하나의 캔버스처럼 관리됨
    /// 메인 모니터가 (0, 0)이고, 서브 모니터가 왼쪽에 있으면 x값은 그만큼 음수를 더해주고 오른쪽에 있으면 양수를 더해줌
    /// 위아래일 경우에도 y값으로 똑같이 처리해주면 됨
    /// </summary>
    /// <param name="position"> 설정할 위치 벡터값 </param>
    public void SetWindowPosition(Vector2 position)
    {
        LibWindowC.SetPosition(position.x, position.y);
    }

    /// <summary>
    /// 현재 창 위치 받아오기
    /// </summary>
    /// <returns> 현재 창 위치 벡터값 </returns>
    public Vector2 GetWindowPosition()
    {
        Vector2 pos = Vector2.zero;
        LibWindowC.GetPosition(out pos.x, out pos.y);
        return pos;
    }

    /// <summary>
    /// 현재 창 크기 설정
    /// </summary>
    /// <param name="size"> 설정할 창 크기 </param>
    public void SetWindowSize(Vector2 size)
    {
        LibWindowC.SetSize(size.x, size.y);
    }

    /// <summary>
    /// 현재 창 크기 받아오기
    /// </summary>
    /// <returns> 현재 창 크기 벡터값 </returns>
    public Vector2 GetWindowSize()
    {
        Vector2 size = Vector2.zero;
        LibWindowC.GetSize(out size.x, out size.y);
        return size;
    }

    /// <summary>
    /// 현재 창의 클라이언트 크기 (타이틀 바, 테두리 등을 제외한 게임이 그려지는 공간) 받아오기
    /// </summary>
    /// <returns> 현재 창의 클라이언트 크기 벡터값 </returns>
    public Vector2 GetClientSize()
    {
        Vector2 size = Vector2.zero;
        LibWindowC.GetClientSize(out size.x, out size.y);
        return size;
    }

    /// <summary>
    /// 현재 창의 클라이언트 크기, 위치 (타이틀 바, 테두리 등을 제외한 게임이 그려지는 공간) 받아오기
    /// </summary>
    /// <returns> 현재 창의 클라이언트 크기, 위치 벡터값 </returns>
    public Rect GetClientRectangle()
    {
        Vector2 pos = Vector2.zero;
        Vector2 size = Vector2.zero;
        LibWindowC.GetClientRectangle(out pos.x, out pos.y, out size.x, out size.y);
        return new Rect(pos.x, pos.y, size.x, size.y);
    }

    /// <summary>
    /// 모니터 또는 해상도 변경을 감지하고 플래그를 해제
    /// </summary>
    /// <returns> 모니터 또는 해상도 변경 여부 </returns>
    public bool ObserveMonitorChanged()
    {
        if (!wasMonitorChanged) return false;

        wasMonitorChanged = false;
        return true;
    }

    /// <summary>
    /// 창 스타일 변경을 감지하고 플래그를 해제
    /// </summary>
    /// <returns> 창 스타일 변경 여부 </returns>
    public bool ObserveWindowStyleChanged()
    {
        if (!wasWindowStyleChanged) return false;

        windowStateEventType = WindowStateEventType.None;
        wasWindowStyleChanged = false;
        return true;
    }

    /// <summary>
    /// 창 스타일 변경을 감지하고 플래그를 해제
    /// </summary>
    /// <param name="type"> 변경된 스타일 타입 </param>
    /// <returns> 창 스타일 변경 여부 </returns>
    public bool ObserveWindowStyleChanged(out WindowStateEventType type)
    {
        if (!wasWindowStyleChanged)
        {
            type = WindowStateEventType.None;
            return false;
        }

        type = windowStateEventType;
        windowStateEventType = WindowStateEventType.None;
        wasWindowStyleChanged = false;
        return true;
    }

    /// <summary>
    /// 마우스 위치 설정
    /// </summary>
    /// <param name="position"> 설정할 위치 벡터값 </param>
    public static void SetCursorPosition(Vector2 position)
    {
        LibWindowC.SetCursorPosition(position.x, position.y);
    }

    /// <summary>
    /// 현재 마우스 위치 반환
    /// </summary>
    /// <returns> 현재 마우스 위치 벡터값 </returns>
    public static Vector2 GetCursorPosition()
    {
        Vector2 pos = Vector2.zero;
        LibWindowC.GetCursorPosition(out pos.x, out pos.y);
        return pos;
    }

    /// <summary>
    /// 어떤 마우스 버튼이 눌렸는지 반환
    /// </summary>
    /// <returns> 눌린 마우스 버튼 종류
    /// 0 → 아무것도 안눌림
    /// 1 → 왼쪽 마우스 버튼
    /// 2 → 오른쪽 마우스 버튼
    /// 4 → 휠 버튼
    /// 여러 버튼이 눌린 경우 각 값을 더해주면 됨 ex) 왼쪽 + 오른쪽 버튼 눌림 → 1 + 2 = 3
    /// </returns>
    public static int GetMouseButtons()
    {
        return LibWindowC.GetMouseButtons();
    }

    /// <summary>
    /// 어떤 특수키 버튼이 눌렸는지 반환
    /// </summary>
    /// <returns>
    /// 눌린 특수키 버튼 종류
    /// 0 → 아무것도 안눌림
    /// 1 → Alt 키
    /// 2 → Ctrl 키
    /// 4 → Shift 키
    /// 8 → Window 키
    /// 여러 버튼이 눌린 경우 각 값을 더해주면 됨 ex) Alt + Ctrl 키 눌림 → 1 + 2 = 3
    /// </returns>
    public static int GetModifierKeys()
    {
        return LibWindowC.GetModifierKeys();
    }

    /// <summary>
    /// 현재 모니터의 번호(인덱스) 받아오기
    /// </summary>
    /// <returns> 현재 모니터 인덱스 </returns>
    public int GetCurrentMonitor()
    {
        return LibWindowC.GetCurrentMonitor();
    }

    /// <summary>
    /// 연결된 모니터 개수 받아오기
    /// </summary>
    /// <returns> 연결된 모니터 개수 </returns>
    public int GetMonitorCount()
    {
        return LibWindowC.GetMonitorCount();
    }

    /// <summary>
    /// 모니터 위치와 크기 구하기
    /// </summary>
    /// <param name="index"> 모니터 번호 </param>
    /// <param name="position"> 모니터 위치 </param>
    /// <param name="size"> 모니터 크기 </param>
    /// <returns> 정보를 잘 가져왔는지 여부 </returns>
    public bool GetMonitorRectangle(int index)
    {
        return LibWindowC.GetMonitorRectangle(index, out monitorPosX, out monitorPosY, out monitorWidth, out monitorHeight);
    }

    // 윈도우 훅 연결
    public IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags)
    {
        return LibWindowC.SetWinEventHook(eventMin, eventMax, hmodWinEventProc, lpfnWinEventProc, idProcess, idThread, dwFlags);
    }

    // 윈도우 훅 해제
    public bool UnhookWinEvent(IntPtr hWindEventHook)
    {
        return LibWindowC.UnhookWinEvent(hWindEventHook);
    }

    /// <summary>
    /// 윈도우 창 찾기
    /// </summary>
    /// <param name="lpClassName"> 찾고자 하는 창의 클래스 이름 </param>
    /// <param name="lpWindowName"> 창의 캡션 or 제목 표시줄 텍스트 </param>
    /// <returns> 조건에 부합하는 창의 고유 핸들 ID </returns>
    public IntPtr FindWindow(string lpClassName, string lpWindowName)
    {
        return LibWindowC.FindWindow(lpClassName, lpWindowName);
    }

    public void SetBorderless(bool isBorderless)
    {
        this.isBorderless = isBorderless;
        LibWindowC.SetBorderless(isBorderless);
    }

    /// <summary>
    /// 모니터에 화면 맞추기(전체화면으로 설정)
    /// </summary>
    /// <param name="monitorIndex"> 설정할 모니터 번호(인덱스) </param>
    /// <returns> 성공 여부 </returns>
    public bool FitToMonitor(int monitorIndex)
    {
        if (GetMonitorRectangle(monitorIndex))
        {
            //Debug.Log($"{monitorIndex}번째 모니터 위치: ({monitorPosX}, {monitorPosY})");
            //Debug.Log($"{monitorIndex}번째 모니터 크기: ({monitorWidth}, {monitorHeight})");

            if (!IsBorderless)
                LibWindowC.SetBorderless(true);

            // 이미 최대화 상태일 경우 최대화를 잠시 풀어줌 → 모니터를 이동해야할 경우 최대화가 풀려야 이동 가능하기 때문
            if (LibWindowC.IsMaximized())
                LibWindowC.SetMaximized(false);

            Taskbar = FindCurrentTaskbar(ref taskbarData);

            //Debug.Log($"작업 표시줄 발견: {Taskbar}");
            //Debug.Log($"작업 표시줄 이름: {taskbarData.hWnd}");

            if (IsTaskBarAutoHide())
            {
                // 작업표시줄이 숨김 상태라면
                LibWindowC.SetPosition(monitorPosX, monitorPosY + 1);
                LibWindowC.SetSize(monitorWidth, monitorHeight - 1);

                return true;
            }
            else
            {
                // 작업표시줄이 숨김 상태가 아니라면
                //taskbarData.tbSize = (uint)Marshal.SizeOf(typeof(TaskbarData));

                //LibWindowC.SHAppBarMessage(ABM_GETTASKBARPOS, ref taskbarData);

                // 작업표시줄 크기 구하기
                int tbWidth = taskbarData.rc.Right - taskbarData.rc.Left;
                int tbHeight = taskbarData.rc.Bottom - taskbarData.rc.Top;

                //Debug.Log($"작업 표시줄 위치: ({taskbarData.rc.Left}, {taskbarData.rc.Top})");
                //Debug.Log($"작업 표시줄 크기: ({tbWidth}, {tbHeight})");

                // 현재 창 크기 받아옴
                float windowWidth = monitorWidth;
                float windowHeight = monitorHeight;

                // 현재 창이 배치 될 좌표
                float windowX = monitorPosX;
                float windowY = monitorPosY;

                switch (taskbarData.uEdge)
                {
                    case 0:
                        // 작업표시줄이 왼쪽일 때
                        windowX += tbWidth;
                        windowWidth -= tbWidth;
                        break;
                    case 1:
                        // 작업표시줄이 위일 때
                        windowY += tbHeight;
                        windowHeight -= tbHeight;
                        break;
                    case 2:
                        // 작업표시줄이 오른쪽일 때
                        windowWidth -= tbWidth;
                        break;
                    case 3:
                        // 작업표시줄이 아래일 때
                        windowY += tbHeight;
                        windowHeight -= tbHeight;
                        break;
                }

                LibWindowC.SetPosition(windowX, windowY);
                LibWindowC.SetSize(windowWidth, windowHeight);

                return true;
            }
        }

        return false;
    }

    // 현재 모니터에 맞는 작업 표시줄 찾아서 캐싱하는 메서드
    public IntPtr FindCurrentTaskbar(ref TaskbarData taskbarData)
    {
        // 모든 작업 표시줄 저장해두기
        List<IntPtr> taskbarList = GetAllTaskbarHandles();

        // 변경할 모니터의 작업 표시줄 찾기
        foreach (IntPtr taskbar in taskbarList)
        {
            TaskbarData data = new TaskbarData();
            data.tbSize = (uint)Marshal.SizeOf(typeof(TaskbarData));
            data.hWnd = taskbar;

            LibWindowC.GetWindowRect(taskbar, out data.rc);

            //Debug.Log($"현재 모니터 작업 표시줄 찾는 중... {data.hWnd}");
            //Debug.Log($"후보 작업 표시줄의 위치: ({data.rc.Left}, {data.rc.Top})");
            //Debug.Log($"작업 표시줄 하단 위치 {data.rc.Bottom}");

            if (data.rc.Left >= monitorPosX && data.rc.Left < monitorPosX + monitorWidth &&
                data.rc.Top > monitorPosY && data.rc.Top <= monitorPosY + monitorHeight)
            {
                Debug.Log("현재 모니터 작업 표시줄 찾음!");
                taskbarData = data;
                return taskbar;
            }
        }

        return IntPtr.Zero;
    }

    // 받아올 수 있는 모든 모니터의 작업 표시줄을 반환
    private List<IntPtr> GetAllTaskbarHandles()
    {
        List<IntPtr> taskbarList = new List<IntPtr>();

        LibWindowC.EnumWindows((hWnd, lParam) =>
        {
            StringBuilder sb = new StringBuilder(256);
            LibWindowC.GetClassName(hWnd, sb, sb.Capacity);
            string className = sb.ToString();

            if(className == "Shell_TrayWnd" || className == "Shell_SecondaryTrayWnd")
            {
                Debug.Log($"{DateTime.UtcNow}: {className} = {hWnd}");
                taskbarList.Add(hWnd);
            }

            return true;

        }, IntPtr.Zero);

        return taskbarList;
    }

    /// <summary>
    /// 작업표시줄이 숨김 상태인지 확인, 자동숨김 설정이더라도 작업표시줄이 내려가있을 때만 true
    /// </summary>
    /// <returns> 작업표시줄 숨김 상태 여부 </returns>
    public bool IsTaskBarAutoHide()
    {
        taskbarData.tbSize = (uint)Marshal.SizeOf(typeof(TaskbarData));

        LibWindowC.SHAppBarMessage(ABM_GETTASKBARPOS, ref taskbarData);
        IntPtr state = LibWindowC.SHAppBarMessage(ABM_GETSTATE, ref taskbarData);

        LibWindowC.GetWindowRect(Taskbar, out taskbarData.rc);

        // 모니터 크기에 -5를 하는 이유는 작업표시줄이 내려가있어도 2px 정도는 걸쳐있기 때문
        return ((int)state & ABS_AUTOHIDE) == ABS_AUTOHIDE && taskbarData.rc.Top >= Mathf.RoundToInt(monitorHeight) - 5;
    }
}