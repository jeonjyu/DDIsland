using UnityEngine;

public class InteractObject_Storage : InteractObject, IInterchangeableInteract
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

    public void TransferTo(GameObject newBuilding)
    {
        // 콜라이더가 있는 오브젝트에 상호작용 컴포넌트 붙이기
        Collider col = newBuilding.GetComponentInChildren<Collider>();
        GameObject target = col != null ? col.gameObject : newBuilding;

        if (!target.TryGetComponent(out InteractObject_Storage newStorage))
        {
            newStorage = target.AddComponent<InteractObject_Storage>();
        }

        newStorage.ui_Storage = ui_Storage;

        // 기존 Outline 설정 복사
        Outline oldOutline = GetComponent<Outline>();
        Outline newOutline = target.GetComponent<Outline>();
        if (oldOutline != null && newOutline != null)
        {
            newOutline.OutlineMode = oldOutline.OutlineMode;
            newOutline.OutlineColor = oldOutline.OutlineColor;
            newOutline.OutlineWidth = oldOutline.OutlineWidth;
            newOutline.enabled = false;
        }
    }
}
