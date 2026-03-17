using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LayoutValue
{
    public LayoutValue(GridLayoutGroup gridLayout)
    {
        grid = gridLayout;
        constraintCount = gridLayout.constraintCount;
        padding = gridLayout.padding;
        cellSize = gridLayout.cellSize;
        alignorder = gridLayout.childAlignment;
    }
    public GridLayoutGroup grid;
    public RectOffset padding;
    public Vector2 cellSize;
    public int constraintCount;
    public TextAnchor alignorder;
}

