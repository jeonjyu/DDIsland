// 상호작용을 해야하는 오브젝트들이 가질 인터페이스
public interface IInteract
{
    // 마우스가 오브젝트에 올라왔을 때
    public void OnMouseHoverIn();

    // 마우스가 오브젝트에서 내려왔을 때
    public void OnMouseHoverOut();

    // 마우스가 오브젝트 위에서 클릭되었을 때 실행

    public void OnInteract();
}
