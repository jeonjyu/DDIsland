using UnityEngine;

public class InteractObject_LP : InteractObject
{
    [Header("LP플레이어")]
    [SerializeField] private GameObject RecordUI;

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
        if (RecordUI == null || RecordUI.activeSelf) return;

        RecordUI.SetActive(true);
    }
}
