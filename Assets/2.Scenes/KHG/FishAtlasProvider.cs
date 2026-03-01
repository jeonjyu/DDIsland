using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
/*
 * FishAtlasProvider
 *
 * 역할
 * - FishDefinition.FishImgPath_Sprite 에 들어있는 스프라이트 이름으로
 *   SpriteAtlas에서 Sprite를 찾아 반환한다.
 *
 * 규칙
 * - CSV의 FishImgPath_Sprite 컬럼에는 Sprite의 실제 이름이 들어가야 한다.
 *   (PORT_SeaFish_Anchovy_001 같은 것)
 */
public class FishAtlasProvider : MonoBehaviour
{
    [SerializeField] private SpriteAtlas[] _atlases;

    public Sprite GetFishSprite(FishDefinition def)
    {
        if (def == null) return null;

        string spriteName = def.FishImgPath_Sprite;

        if (string.IsNullOrEmpty(spriteName))
        {
            Debug.LogWarning("[Atlas] spriteName empty");
            return null;
        }

        for (int i = 0; i < _atlases.Length; i++)
        {
            var atlas = _atlases[i];
            if (atlas == null) continue;

            var sp = atlas.GetSprite(spriteName);
            if (sp != null)
            {
                Debug.Log($"[Atlas] 찾아버림 '{spriteName}' in atlas='{atlas.name}'");
                return sp;
            }
        }

        Debug.LogWarning($"[Atlas] 못찾음 '{spriteName}' in any atlas. atlasesCount={_atlases?.Length ?? 0}");
        return null;
    }
}