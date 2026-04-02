using UnityEngine;

/// <summary>
/// 거래시 공통으로 사용되는 메서드들
/// </summary>
public interface ITradeStrategy
{
    public bool Trade(int tradeCount, int tradePrice);
    public int GetPrice(IStoreItem item);
    public int GetMaxCount(IStoreItem item);
    public void PlayTradeSFX();
}
