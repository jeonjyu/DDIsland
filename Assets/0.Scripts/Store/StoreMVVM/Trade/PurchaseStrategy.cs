using UnityEngine;

public class PurchaseStrategy : MonoBehaviour, ITradeStrategy
{
    //public virtual bool Trade(int tradeCount, IStoreItem item)
    public virtual bool Trade(int tradeCount, int tradePrice)
    {
        if(GameManager.Instance.PlayerGold < tradePrice)
            return false;        

        StoreManager.Instance.ItemCountChanged(tradeCount);
        GameManager.Instance.SetGold(-tradePrice);

        //Debug.Log("구매 로직 " + StoreManager.Instance.TradeItemCount);
        return true;
    }

    public int GetPrice(IStoreItem item)
    {
        return item.PurchasePrice;
    }

    // 현재 골드로 구매 가능한 아이템의 개수
    // 최대 보유 가능 개수 - 현재 보유 개수
    // 둘 중 작은 수 리턴
    public int GetMaxCount(IStoreItem item)
    {
        if(item.PurchasePrice == 0)
        {
            Debug.Log("아이템의 구매 가격이 0원이라 골드 고려 안함");
            return item.MaxCount - item.ItemCount;
        }
        return Mathf.Min(GameManager.Instance.PlayerGold / item.PurchasePrice, item.MaxCount - item.ItemCount);
    }
}
