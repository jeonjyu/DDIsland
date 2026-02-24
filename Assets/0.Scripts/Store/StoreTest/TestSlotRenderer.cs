using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestSlotRenderer : MonoBehaviour
{
    [Tooltip("슬롯을 채워줄 아이템 그리드")]
    [SerializeField] GameObject itemContents;

    [SerializeField] List<StoreItem> itemList = new List<StoreItem>();
    //List<StoreItemPresenter> storeItemPresenters = new List<StoreItemPresenter>();

    void Start()
    {
        Debug.Log("[TestSlotRenderer] Start");
    }


    // itemList 변경되면 그에 맞춰 초기화

    // storeItemPresenters에 있는 model들 null로 만들고
    // 오브젝트 풀로 반납 > 반납 메서드에서 비활성화
    public void ResetSlotList()
    {
        //foreach (var item in storeItemPresenters)
        //{
        //    item.Reset();
        //}

        Debug.Log("[TestSlotRenderer] ResetSlotList | 슬롯 반납 완료");
    }
}
