using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{

    [SerializeField] Button button;
    [SerializeField] UI_StoreCategoryText catUiString;
    [SerializeField] TMP_Text btnText;

    public Button CatBtn => button;
    public UI_StoreCategoryText CatUiString => catUiString;
}
