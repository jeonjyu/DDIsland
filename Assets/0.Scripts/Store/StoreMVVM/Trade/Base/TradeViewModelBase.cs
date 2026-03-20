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
    public TradeViewBase view;
    int totalPrice;
    public bool isGained;
    [SerializeField] CanvasGroup canvasGroup;

    public event PropertyChangedEventHandler PropertyChanged;

    public StoreCat storeCat => StoreManager.Instance.currentCat;


    public IStoreItem Model
    {
        get => StoreManager.Instance.TradeModel;
    }

    public int Gold
    {
        get => GameManager.Instance.PlayerGold;
        set
        {
            GameManager.Instance.SetGold(value);
            OnPropertyChanged(nameof(GameManager.Instance.PlayerGold));
        }
    }

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

    public bool IsGained
    {
        get => StoreManager.Instance.IsTradeItemGained;
        set
        {
            if (isGained != value)
            {
                isGained = value;
                //Debug.Log(this.name + " 아이템 보유 여부 변경 " + isGained);
                OnPropertyChanged(nameof(IsGained));
            }
        }
    }

    //protected virtual void Awake()
    //{
    //    canvasGroup = transform.GetComponent<CanvasGroup>();
    //}

    protected virtual void OnEnable()
    {
        Debug.Log("OnEable 캔버스 그룹 설정");
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    protected virtual void OnDisable()
    {
        Debug.Log("OnDisable 캔버스 그룹 설정");
        PropertyChanged = null;
        StoreManager.Instance.ChangeDropdownByCategory();
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
    void Start()
    {
        view = GetComponent<TradeViewBase>();
    }

  

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
