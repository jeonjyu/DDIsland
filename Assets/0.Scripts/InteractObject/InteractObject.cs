using UnityEngine;

// 상호작용할 오브젝트를 제어하는 부모 클래스
[RequireComponent(typeof(Outline))]
public abstract class InteractObject : MonoBehaviour, IInteract
{
    protected Outline outline;

    protected virtual void Awake()
    {
        outline = GetComponent<Outline>();
    }

    protected void Start()
    {
        outline.enabled = false;
    }

    public virtual void OnMouseHoverIn()
    {
        if (!GameManager.Instance.InputManager.InputStateType.HasFlag(InputState.ObjectInteraction)) return;

        if(!outline.enabled)
            outline.enabled = true;

        GameManager.Instance.InputManager.RemoveInputState(InputState.UserInteraction);
    }

    public virtual void OnMouseHoverOut()
    {
        if (outline.enabled)
            outline.enabled = false;

        GameManager.Instance.InputManager.AddInputState(InputState.UserInteraction);
    }

    public abstract void OnInteract();
}
