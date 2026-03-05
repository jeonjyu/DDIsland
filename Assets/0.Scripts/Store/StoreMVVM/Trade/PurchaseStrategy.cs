using UnityEngine;

public class PurchaseStrategy : MonoBehaviour, ITradeStrategy
{
    public virtual bool Trade(int tradeCount, IStoreItem item)
    {
        if(GameManager.Instance.PlayerGold < tradeCount)
        {
            return false;
        }

        GameManager.Instance.SetGold(-(tradeCount * item.PurchasePrice));
        //item.ItemCount += tradeCount;
        StoreManager.Instance.ItemCountChanged(item.ItemCount + tradeCount);

        if (!item.IsGained)
        {
            item.IsGained = true;
            //ItemManager.Instance.AddToPlayerItem(StoreManager.Instance.tradeModel, StoreManager.Instance.currentCat);
        }
        return true;
    }

    public int GetPrice(IStoreItem item)
    {
        return item.PurchasePrice;
    }

    public int GetMaxCount(IStoreItem item)
    {
        return Mathf.Min(GameManager.Instance.PlayerGold / item.PurchasePrice, item.MaxCount - item.ItemCount);
    }
}
