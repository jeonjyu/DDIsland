using UnityEngine;

public class LakeMoveMask : MonoBehaviour
{
    public RectTransform target; // 따라다닐 대상 

    void LateUpdate()
    {
        if (target == null) return;

        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(target.rect.width, target.rect.height);
    }
}