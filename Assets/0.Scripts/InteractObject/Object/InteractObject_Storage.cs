using UnityEngine;

public class InteractObject_Storage : InteractObject
{
    [SerializeField] private UI_Storage ui_Storage;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
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
        // todo: 보관함 UI 키기

        if (ui_Storage != null && !ui_Storage.gameObject.activeSelf)
            ui_Storage.OpenStorageUI();
    }
}
