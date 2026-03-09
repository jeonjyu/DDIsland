using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(TradeViewBase))]

// 모델 받아오기
// 원래의 모델 값 변경하기
// 사용자 소유 카테고리에 저장하기
// 
// 사용자 재화 관리하기
// 재화 부족 체크하기
// 팝업 끌 때 상점 리스트 갱신, 정렬 초기화, 카테고리 유지

public class TradeViewModelBase : MonoBehaviour, INotifyPropertyChanged
{
    public IStoreItem Model
    {
        get => StoreManager.Instance.TradeModel;
        //set 
        //{  
        //    StoreManager.Instance.TradeModel = value;
        //}
    }
    public TradeViewBase view;

    public int Gold
    {
        get => GameManager.Instance.PlayerGold;
        set
        {
            GameManager.Instance.SetGold(value);
            OnPropertyChanged(nameof(GameManager.Instance.PlayerGold));
        }
    }



    int totalPrice;

    public int TotalPrice
    {
        get => totalPrice;
        set
        {
            if (totalPrice != value)
            {
                totalPrice = value;

            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    void Start()
    {
        view = GetComponent<TradeViewBase>();
        //Init(StoreManager.Instance.tradeModel);
    }

    public void OnDisable()
    {
        StoreManager.Instance.ChangeDropdownAvailability(true);
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
