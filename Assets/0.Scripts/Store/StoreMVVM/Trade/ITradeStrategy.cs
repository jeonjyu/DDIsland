public interface ITradeStrategy
{
    public bool Trade(int tradeCount, StoreItem item);
    public int GetPrice(StoreItem item);
    public int GetMaxCount(StoreItem item);
}
