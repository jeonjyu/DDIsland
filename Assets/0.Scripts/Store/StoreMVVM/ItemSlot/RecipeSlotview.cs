using UnityEngine;
using UnityEngine.UI;

public class RecipeSlotview : ItemSlotView
{
    [SerializeField] Image MainIngrImage;
    [SerializeField] Image SubIngrImage;
    
    public override void Init()
    {
        base.Init();
        if(modelData == null) return;

        if (modelData.MainIngSprite == null)
            MainIngrImage.gameObject.SetActive(false);
        else
        {
            MainIngrImage.gameObject.SetActive(true);
            MainIngrImage.sprite = modelData.MainIngSprite;
        }

        if (modelData.SubIngSprite == null)
            SubIngrImage.gameObject.SetActive(false);
        else
        {
            SubIngrImage.gameObject.SetActive(true);
            SubIngrImage.sprite = modelData.SubIngSprite;
        }
    }
}
