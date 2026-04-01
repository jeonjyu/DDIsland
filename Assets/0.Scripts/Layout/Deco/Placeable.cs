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
    [SerializeField] protected GridSystem _targetGrid;

    [SerializeField] protected ItemState _itemState = ItemState.Preview;

    [SerializeField] protected Vector2Int _pivot;

    public ItemState ItemState
    {
        get => _itemState;
        set => _itemState = value;
    }
    
    public virtual void Placement(AudioClip audio)
    {

    }
    // 배치 가능한 오브젝트가 그리드에 배치될 때 시각적 피드백을 제공하는 추상 메서드
    public abstract void VisualFeedback();
    public abstract Vector2Int ConvertedIndex();

}
