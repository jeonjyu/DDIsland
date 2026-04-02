using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{

    [SerializeField] Button button;
    [SerializeField] TMP_Text btnText;

    public Button CatBtn => button;
}
