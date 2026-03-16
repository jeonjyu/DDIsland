using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-50), RequireComponent(typeof(InputHandler))]
public class GameManager : Singleton<GameManager>
{
    [Header("InputHandler 클래스")]
    [field: SerializeField] public InputHandler InputManager { get; private set; }

    [Header("StageControl 클래스")]
    [field: SerializeField] public StageControl StageController { get; set; }

    public UI_IslandWindow IslandWindow{ get; set; }
    public UI_WaterWindow WaterWindow { get; set; }

    public event Action<int> OnGoldChanged;

    [SerializeField] private int playerGold;
    public int PlayerGold => playerGold;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        StartCoroutine(AutoAddGold());

        IslandWindow?.Init();
        WaterWindow?.Init();
    }

    private IEnumerator AutoAddGold()
    {
        while(true)
        {
            SetGold(100);

            yield return new WaitForSeconds(5f);
        }
    }

    public void SetGold(int gold)
    {
        if(gold < 0 && playerGold < gold)
        {
            Debug.LogWarning("[GameManager] 들어온 골드가 현재 골드보다 큼");
            return;
        }
        playerGold += gold;

        OnGoldChanged?.Invoke(PlayerGold);
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        PlayerPrefs.Save();
    }
}
