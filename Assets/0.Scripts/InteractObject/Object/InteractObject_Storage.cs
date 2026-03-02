using UnityEngine;

public class InteractObject_Storage : InteractObject
{
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

        Debug.Log("보관함 열기");
    }
}
