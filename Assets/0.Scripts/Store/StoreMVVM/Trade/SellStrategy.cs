using UnityEngine;

public class SellStrategy : MonoBehaviour, ITradeStrategy
{
    public bool Trade(int tradeCount, int tradePrice)
    {
        Debug.Log("판매 로직 " + StoreManager.Instance.TradeItemCount + " 획득 여부 " + StoreManager.Instance.TradeModel.IsGained);
        GameManager.Instance.SetGold(tradePrice);
        StoreManager.Instance.ItemCountChanged(-tradeCount);

        Debug.Log("판매 로직 " + StoreManager.Instance.TradeItemCount + " 획득 여부 " + StoreManager.Instance.TradeModel.IsGained);

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
