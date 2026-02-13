public interface IStoreItemView
{
    void Init(int price, string name, int count, bool isGained, string imgPath);
    void UpdateItemCount(int count);
    void UpdateSlotColor(bool isGained);
}