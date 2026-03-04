public interface ITradeStrategy
{
    public bool Trade(int tradeCount, IStoreItem item);
    public int GetPrice(IStoreItem item);
    public int GetMaxCount(IStoreItem item);
}
