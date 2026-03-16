using UnityEngine;

public class UI_RecordUnlock : MonoBehaviour
{
    [Header("요구 음반조각 개수")]
    [SerializeField] private int lpPieceCount;

    private UI_BGMSlot currentSlot;

    // 해금 팝업창 띄우기
    public void ShowUnlockPopup(UI_BGMSlot slot)
    {
        gameObject.SetActive(true);

        currentSlot = slot;
    }

    // 미리 듣기 버튼 클릭
    public void OnClick_PreListening()
    {
        // todo: 미리 듣기 실행
    }

    // 교환 버튼 클릭
    public void OnClick_ExchangeRecord()
    {
        

        gameObject.SetActive(false);
    }
}
