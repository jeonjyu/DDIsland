using UnityEngine;

public abstract class Placeable : MonoBehaviour
{
    GameObject _selectedObject;

    protected GridSystem _targetGrid;


    public void SelectObject(GameObject obj)
    {
        _selectedObject = obj;
    }
    public void Placement()
    {

    }

    public abstract Vector2Int ConvertedIndex();
}
