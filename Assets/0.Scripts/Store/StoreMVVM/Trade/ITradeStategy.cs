public interface ITradeStategy
{
    public void Trade(int tradeCount);

    public abstract int GetPrice();

    public void ChangeCount(int tradeCount);

    public void ChangeIsGained(bool isGained);
}
