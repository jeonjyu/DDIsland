using UnityEngine;

public class InteractObject_Storage : InteractObject
{
    [SerializeField] private UI_Storage ui_Storage;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnMouseHoverIn()
    {
        base.OnMouseHoverIn();
    }

    public override void OnMouseHoverOut()
    {
        base.OnMouseHoverOut();
    }

    public override void OnInteract()
    {
        if (PlacementMgr.Instance != null && PlacementMgr.Instance.CurrentState == PlacementState.Edit)
            return;

        if (ui_Storage != null && !ui_Storage.gameObject.activeSelf)
            ui_Storage.OpenStorageUI();
    }
}
