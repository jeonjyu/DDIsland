using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Reward : MonoBehaviour
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TMP_Text _countText;

    // 프리팹이 생성될 때 데이터를 주입받는 함수
    public void Setup(int itemID, int count)
    {
        var data = DataManager.Instance.CurrencyDatabase.CurrencyInfoData[itemID];
        if (data != null)
        {
            _itemIcon.sprite = data.CurrencyImgPath_Sprite;
            _countText.text = count.ToString();
        }
    }
}
