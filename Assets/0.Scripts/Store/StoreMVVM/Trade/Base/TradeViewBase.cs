using UnityEngine;

// 구매/판매 시 모델 외에 모든 요소 초기화
// 모델 이름, 설명 띄우기

public class TradeViewBase : MonoBehaviour
{
    TradeViewModelBase viewmodel;
    

    public void Start()
    {
        viewmodel = GetComponent<TradeViewModelBase>();
    }


}
