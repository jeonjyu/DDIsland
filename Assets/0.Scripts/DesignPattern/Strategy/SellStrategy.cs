using UnityEngine;

public class SellStrategy : MonoBehaviour, ITradeStrategy
{
    // 판매
    public bool Trade(int tradeCount, int tradePrice)
    {
        GameManager.Instance.SetGold(tradePrice);
        StoreManager.Instance.ItemCountChanged(-tradeCount);

        return true;
    }

    public int GetPrice(IStoreItem item)
    {
        return item.SellPrice;
    }

    // 현재 보유하고 있는 아이템의 개수
    public int GetMaxCount(IStoreItem item)
    {
        return item.ItemCount;
    }
}
