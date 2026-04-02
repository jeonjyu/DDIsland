using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenuArrow : MonoBehaviour
{
    [Header("UI_MainMenu 스크립트")]
    [SerializeField] private UI_MainMenu mainMenu;

    [Header("화살표 방향 관련 이미지 / 스프라이트")]
    [SerializeField] private Image arrowImg;
    [SerializeField] private Sprite showArrow;
    [SerializeField] private Sprite hideArrow;

    private void SetArrowSprite(bool isShow)
    {
        Debug.Log("화살표");
        arrowImg.sprite = isShow ? showArrow : hideArrow;
    }

    private void OnEnable()
    {
        mainMenu.OnMenuStateChanged += SetArrowSprite;
    }

    private void OnDisable()
    {
        mainMenu.OnMenuStateChanged -= SetArrowSprite;
    }
}
