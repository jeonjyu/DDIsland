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
    private void OnEnable()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
            DataManager.Instance.Hub.OnRequestSave += SyncGoldSave;
    }

    private void OnDisable()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            DataManager.Instance.Hub.OnRequestSave -= SyncGoldSave;
        }
    }

    private void Start()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            if (DataManager.Instance.Hub.IsLoaded)
            {
                SyncGoldLoad();
            }
            else
            {
                DataManager.Instance.Hub.OnDataLoaded += SyncGoldLoad;
            }
        }

        StartCoroutine(AutoAddGold());

        IslandWindow?.Init();
        WaterWindow?.Init();

        Debug.Log($"GameManager");
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

    private void SyncGoldSave()
    {
        var userData = DataManager.Instance.Hub._allUserData.Currency;
        if (userData != null)
        {
            userData._gold = playerGold;
        }
    }

    private void SyncGoldLoad()
    {
        DataManager.Instance.Hub.OnDataLoaded -= SyncGoldLoad;

        var userData = DataManager.Instance.Hub._allUserData.Currency;
        if (userData != null)
        {
            playerGold = userData._gold;
            OnGoldChanged?.Invoke(playerGold);
        }
    }
}
