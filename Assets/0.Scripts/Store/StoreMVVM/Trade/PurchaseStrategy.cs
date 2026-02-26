using UnityEngine;

public class PurchaseStrategy : MonoBehaviour, ITradeStrategy
{
    public virtual bool Trade(int tradeCount, StoreItem item)
    {
        if(GameManager.Instance.PlayerGold < tradeCount)
        {
            return false;
        }

        GameManager.Instance.SetGold(-(tradeCount * item.PurchasePrice));
        item.ItemCount += tradeCount;

        if(!item.IsGained) item.IsGained = true;

        return true;
    }

    public int GetPrice(StoreItem item)
    {
        return item.PurchasePrice;
    }

    public int GetMaxCount(StoreItem item)
    {
        return GameManager.Instance.PlayerGold / item.PurchasePrice;
    }
}
