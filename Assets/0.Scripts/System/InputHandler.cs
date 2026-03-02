using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Flags]
public enum InputState : int
{
    None,
    ObjectInteraction = 1 << 0,     // 상호작용 오브젝트 클릭이 가능한 상태
    UserInteraction = 1 << 1,       // 유저 개입 상호작용이 가능한 상태
}

[RequireComponent(typeof(PlayerInput))]
public class InputHandler : MonoBehaviour
{
    [Header("인풋 상태")]
    [field: SerializeField] public InputState InputStateType { get; private set; }

    #region 인풋 시스템 매핑
    private PlayerInput playerInput;        // 플레이어 인풋

    private InputAction leftClickAction;    // 마우스 좌클릭에 관한 액션
    #endregion

    #region 인풋 시스템 이벤트 정의
    public event Action<InputAction.CallbackContext> OnLeftClick_Started;              // 마우스 좌클릭 이벤트

    private Action onInputMappingClear;     // 매핑된 인풋 액션 클리어
    #endregion

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        leftClickAction = playerInput.actions["LeftMouseClick"];
    }

    private void OnMapping_LeftClick()
    {
        OnMapping(leftClickAction, "LeftMouseClick", startedAction: OnLeftClick_Started);
    }

    #region 인풋 시스템 매핑 및 해제
    private void OnMapping(InputAction inputAction, string actionName,
        Action<InputAction.CallbackContext> startedAction = null,
        Action<InputAction.CallbackContext> performedAction = null,
        Action<InputAction.CallbackContext> cancledAction = null)
    {
        inputAction = playerInput.actions[actionName];

        if(startedAction != null)
            inputAction.started += startedAction;

        if (performedAction != null)
            inputAction.performed += performedAction;

        if (cancledAction != null)
            inputAction.canceled += cancledAction;

        onInputMappingClear += () =>
        {
            if (startedAction != null)
                inputAction.started -= startedAction;

            if (performedAction != null)
                inputAction.performed -= performedAction;

            if (cancledAction != null)
                inputAction.canceled -= cancledAction;
        };
    }

    private void ClearMapping()
    {
        onInputMappingClear?.Invoke();
        onInputMappingClear = null;
    }
    #endregion

    #region 인풋 상태 관련 메서드 정의
    /// <summary>
    /// 인풋 타입 추가
    /// </summary>
    /// <param name="states"></param>
    public void AddInputState(params InputState[] states)
    {
        foreach(var state in states)
        {
            InputStateType |= state;
        }
    }

    /// <summary>
    /// 인풋 타입 제거
    /// </summary>
    /// <param name="states"></param>
    public void RemoveInputState(params InputState[] states)
    {
        foreach(var state in states)
        {
            InputStateType &= ~state;
        }
    }
    #endregion

    private void OnEnable()
    {
        OnMapping_LeftClick();
    }

    private void OnDisable()
    {
        ClearMapping();
    }
}
