using UnityEngine;


public enum ItemState // 건물의 상태를 나타내는 열거형
{
    Preview, // 배치 미리보기 상태 
    Placed, // 배치 된 상태
}
/// <summary>
/// 배치 가능한 오브젝트의 공통 기능을 정의하는 추상 클래스
/// </summary>
public abstract class Placeable : MonoBehaviour
{
    protected GameObject _selectedObject;

    protected GridSystem _targetGrid;

    protected ItemState _itemState = ItemState.Preview;

    public ItemState ItemState
    {
        get => _itemState;
        set => _itemState = value;
    }

    public void SelectObject(GameObject obj)
    {
        
        _selectedObject = obj;
    }
    public virtual void Placement()
    {

    }
    // 배치 가능한 오브젝트가 그리드에 배치될 때 시각적 피드백을 제공하는 추상 메서드
    public abstract void VisualFeedback();
    public abstract Vector2Int ConvertedIndex();

}
