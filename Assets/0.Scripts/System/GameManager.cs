using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] int playerGold = 10000000;
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
}
