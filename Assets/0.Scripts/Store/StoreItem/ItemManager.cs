using System.Collections.Generic;

public class ItemManager : Singleton<ItemManager>
{
    // 아이템 카탈로그 딕셔너리 모음
    List<Dictionary<int, StoreItemBase>> _totalItemData = new List<Dictionary<int, StoreItemBase>>();

    // 플레이어가 소유한 아이템 딕셔너리 모음
    List<Dictionary<int, StoreItemBase>> _playerItemData = new List<Dictionary<int, StoreItemBase>>();
}
