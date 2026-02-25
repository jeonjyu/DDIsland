using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    int playerGold = 0;
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
