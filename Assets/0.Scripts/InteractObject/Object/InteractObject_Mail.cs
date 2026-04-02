using UnityEngine;

public class InteractObject_Mail : InteractObject
{
    [Header("우편함 UI")]
    [SerializeField] private GameObject MailUI;

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

        if (MailUI != null && !MailUI.activeSelf)
        {
            MailUI.SetActive(true);

        }
    }
}
