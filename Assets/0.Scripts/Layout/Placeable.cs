using UnityEngine;

public abstract class Placeable : MonoBehaviour
{
    protected GameObject _selectedObject;

    protected GridSystem _targetGrid;
    
    public void SelectObject(GameObject obj)
    {
        
        _selectedObject = obj;
    }
    public virtual void Placement()
    {

    }
    public abstract void VisualFeedback();
    public abstract Vector2Int ConvertedIndex();
}
