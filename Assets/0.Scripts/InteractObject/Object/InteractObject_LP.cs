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
        if (!newBuilding.TryGetComponent(out InteractObject_LP newLp))
        {
            newLp = newBuilding.AddComponent<InteractObject_LP>();
        }

        newLp.RecordUI = RecordUI;
    }
}
