using System;
using UnityEngine;

public class UI_IslandWindow : MonoBehaviour
{
    [Header("섬 창")]
    [SerializeField] private RectTransform islandWindow;

    [Header("호수/바다 창")]
    [SerializeField] private UI_WaterWindow waterWindow;

    [Header("최소 / 최대 비율")]
    [SerializeField] private float defaultRatio = 0.67f;
    [SerializeField] private float maxRatio = 1f;
    [SerializeField] private float minRatio = 0.375f;

    public Vector2 WindowSize => islandWindow.sizeDelta;

    private void Awake()
    {
        islandWindow.localScale = new Vector3(defaultRatio, defaultRatio, defaultRatio);


    }
}
