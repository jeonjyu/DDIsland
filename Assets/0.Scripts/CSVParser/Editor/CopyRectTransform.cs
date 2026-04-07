#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class CopyRectTransformOnly : MonoBehaviour
{
    static GameObject sourceObj;

    [MenuItem("GameObject/Custom/1. 레이아웃 복사", false, -10)]
    static void CopyLayout()
    {
        sourceObj = Selection.activeGameObject;
    }

    [MenuItem("GameObject/Custom/2. 레이아웃 붙여넣기", false, -9)]
    static void PasteLayout()
    {
        if (sourceObj == null)
        {
            return;
        }

        GameObject targetObj = Selection.activeGameObject;

        // Undo (Ctrl +Z) 기능
        Undo.RegisterFullObjectHierarchyUndo(targetObj, "Paste RectTransforms");

        PasteRectTransformRecursive(sourceObj.transform, targetObj.transform);

    }

    static void PasteRectTransformRecursive(Transform src, Transform dest)
    {
        //현재 오브젝트의 RectTransform 복사
        RectTransform srcRect = src.GetComponent<RectTransform>();
        RectTransform destRect = dest.GetComponent<RectTransform>();

        if (srcRect != null && destRect != null)
        {
            ComponentUtility.CopyComponent(srcRect);
            ComponentUtility.PasteComponentValues(destRect);
        }

        //자식 오브젝트들도 이름이 같다면 동일하게 덮어쓰기
        for (int i = 0; i < src.childCount; i++)
        {
            Transform srcChild = src.GetChild(i);
            Transform destChild = dest.Find(srcChild.name);

            if (destChild != null)
            {
                PasteRectTransformRecursive(srcChild, destChild);
            }
        }
    }
}
#endif