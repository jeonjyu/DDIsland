using UnityEngine;

public class UI_RecordUnlock : MonoBehaviour
{
    [Header("요구 음반조각 개수")]
    [SerializeField] private int requireLpPieceCount = 3;

    [Header("교환 결과 팝업창")]
    [SerializeField] private GameObject resultPopup;

    private UI_BGMSlot currentSlot;

    // 해금 팝업창 띄우기
    public void ShowUnlockPopup(UI_BGMSlot slot)
    {
        if (slot == null) return;

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
        if (DataManager.Instance.RecordDatabase.LpPieceCount >= requireLpPieceCount)
        {
            // 음반 교환 성공시
            currentSlot.UnlockRecord();
            gameObject.SetActive(false);
            resultPopup.SetActive(true);
        }
        else
        {
            // 음반 교환 실패시
            resultPopup.SetActive(true);
        }
    }
}
