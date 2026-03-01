using UnityEngine;

public class SellStrategy : MonoBehaviour, ITradeStrategy
{
    public bool Trade(int tradeCount, StoreItem item)
    {
        GameManager.Instance.SetGold(tradeCount * item.SellPrice);
        //item.ItemCount -= tradeCount;
        StoreManager.Instance.ItemCountChanged(item.ItemCount - tradeCount);

        if (item.ItemCount == 0 && item.IsGained == true)
        {
            item.IsGained = false;
            //ItemManager.Instance.RemoveFromPlayerItem(StoreManager.Instance.tradeModel, StoreManager.Instance.currentCat);

        }
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
