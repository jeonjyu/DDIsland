using UnityEngine;

public class SellStrategy : MonoBehaviour, ITradeStrategy
{
    public bool Trade(int tradeCount, StoreItem item)
    {
        GameManager.Instance.SetGold(tradeCount * item.SellPrice);
        item.ItemCount -= tradeCount;
        if(item.ItemCount == 0 && item.IsGained == true)
            item.IsGained = false;
        return true;
    }

    public int GetPrice(StoreItem item)
    {
        return item.SellPrice;
    }

    public int GetMaxCount(StoreItem item)
    {
        return item.ItemCount;
    }
}
