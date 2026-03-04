using UnityEngine;
using UnityEngine.InputSystem;

public class StageControl : MonoBehaviour
{
    [Header("현재 씬이 시작할 때 재생할 bgm")]
    [SerializeField] private AudioClip stageBgmClip;

    private IInteract interactObj;      // 현재 마우스가 올라가있는 상호작용 오브젝트 (null이면 호버중인 오브젝트가 없는 상태)
    public IInteract InteractObj
    {
        get { return interactObj; }
        set
        {
            if (interactObj == value) return;

            // 기존의 상호작용 오브젝트가 있었을 경우 빠져나가는 메서드 실행
            if (interactObj != null)
            {
                interactObj.OnMouseHoverOut();
            }

            interactObj = value;

            // 새로운 상호작용 오브젝트 진입 메서드 실행
            if (interactObj != null)
            {
                interactObj.OnMouseHoverIn();
            }
        }
    }

    private void Start()
    {
        SoundManager.Instance.PlayBGM(stageBgmClip);

        GameManager.Instance.StageController = this;
    }

    private void OnInteractObject(InputAction.CallbackContext ctx)
    {
        if (interactObj == null || !GameManager.Instance.InputManager.InputStateType.HasFlag(InputState.ObjectInteraction)) return;

        InteractObj.OnInteract();
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.InputManager.OnLeftClick_Started += OnInteractObject;
    }

    private void OnDisable()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.InputManager.OnLeftClick_Started -= OnInteractObject;
    }
}
