using UnityEngine;

[RequireComponent(typeof(InputHandler))]
public class GameManager : Singleton<GameManager>
{
    [Header("InputHandler 클래스")]
    [field: SerializeField] public InputHandler InputManager { get; private set; }

    [Header("StageControl 클래스")]
    [field: SerializeField] public StageControl StageController { get; set; }

    [SerializeField] private int playerGold = 10000000;
    public int PlayerGold => playerGold;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetGold(int gold)
    {
        if(gold < 0 && playerGold < gold)
        {
            Debug.LogWarning("[GameManager] 들어온 골드가 현재 골드보다 큼");
            return;
        }
        playerGold += gold;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
