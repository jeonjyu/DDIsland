using UnityEngine;

public class InteractObject_LP : InteractObject, IInterchangeableInteract
{
    [Header("LP플레이어")]
    [SerializeField] private GameObject RecordUI;

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
        if (RecordUI == null || RecordUI.activeSelf) return;

        RecordUI.SetActive(true);
    }

    public void TransferTo(GameObject newBuilding)
    {
        // 콜라이더가 있는 오브젝트에 상호작용 컴포넌트 붙이기
        Collider col = newBuilding.GetComponentInChildren<Collider>();
        GameObject target = col != null ? col.gameObject : newBuilding;

        if (!target.TryGetComponent(out InteractObject_LP newLp))
        {
            newLp = target.AddComponent<InteractObject_LP>();
        }

        newLp.RecordUI = RecordUI;

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
