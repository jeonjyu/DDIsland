//using UnityEngine;

//public class PurchaseUnitViewModel : TradeUnitViewModelBase
//{
//    /// <summary>
//    ///  구매 메서드
//    ///  모델의 보유 아이템 갯수 변경
//    ///  미보유였을 경우 보유로 변경
//    /// </summary>
//    /// <param name="tradeCount">구매할 아이템 갯수</param>
//    public override void Trade(int tradeCount)
//    {
//        // 보유 금액이 총 비용보다 많은지 판별 후 true면 계쏙
//        // false면 팝업 띄운 후 리턴
//        //if(Gold)

//        if (Model.ItemCount == 0) ChangeIsGained(true);

//        // 구매 완료 팝업 띄우기

//        return;
//    }

//    public override int GetPrice()
//    {
//        return Model.PurchasePrice;
//    }

//}
