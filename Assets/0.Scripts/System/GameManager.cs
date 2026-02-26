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
        if (gold < 0)
            playerGold += playerGold > gold ? gold : 0;
        else
            playerGold += gold;
    }
}
