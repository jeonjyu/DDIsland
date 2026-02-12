using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// DoTween UI 트위닝 확장 클래스
/// 범용 확장 메서드 모음
/// 확장 메서드 담으려면 무조건 static 클래스여야 함
/// 사용법: (); 로 비우면 기본값 
///   rt.TweenPunchScale(0.5f, 0.5f); // a: 더 크게 튐, b: 더 느리게 튐  
///   rt.TweenPunchPosition(new Vector2(0f, 50f)); // a: 좌우로 흔들림, b: 위아래로 흔들림
///   cg.TweenFade(0f, 0.3f); // a: 알파값, b: 사라지는 시간  
///   rt.TweenScale(Vector3.one * 1.5f, 0.3f);
///   rt.TweenSizeDelta(new Vector2(300, 100), 0.3f);
///   rt.TweenRotate(new Vector3(0, 0, 90), 0.3f);
///   image.TweenColor(Color.red, 0.3f);
///   
/// </summary>
public static class DOTweenExtensionsClass
{
    //  DOPunchScale - 누르면 통통 튐 // rt.TweenPunchScale();
    public static Tween TweenPunchScale(this RectTransform rt,
        float punch = 0.2f, float time = 0.3f, int vibrato = 10, float elasticity = 1f)
    {
        rt.DOKill();
        return rt.DOPunchScale(Vector3.one * punch, time, vibrato, elasticity)
            .SetUpdate(true).SetLink(rt.gameObject);
    }

    //  DOPunchPosition - 누르면 좌우로 흔들림 // rt.TweenPunchPosition();
    public static Tween TweenPunchPosition(this RectTransform rt,
        Vector2 punch = default, float time = 0.3f, int vibrato = 10, float elasticity = 1f)
    {
        rt.DOKill();
        if (punch == default) punch = new Vector2(30f, 0f);
        return rt.DOPunchAnchorPos(punch, time, vibrato, elasticity)
            .SetUpdate(true).SetLink(rt.gameObject);
    }

    //  DOFade - 누르면 사라졌다 나타남 // cg.TweenFade(0f, 0.3f);
    public static Tween TweenFade(this CanvasGroup cg,
        float to, float time = 0.3f, Ease ease = Ease.OutQuad)
    {
        cg.DOKill();
        return cg.DOFade(to, time).SetEase(ease)
            .SetUpdate(true).SetLink(cg.gameObject);
    }

    // DOScale - 누르면 커졌다 돌아옴 // rt.TweenScale(Vector3.one * 1.5f, 0.3f);
    public static Tween TweenScale(this RectTransform rt,
        Vector3 to, float time = 0.3f, Ease ease = Ease.OutQuad)
    {
        rt.DOKill();
        return rt.DOScale(to, time).SetEase(ease)
            .SetUpdate(true).SetLink(rt.gameObject);
    }

    // DOSizeDelta - 누르면 넓어졌다 돌아옴 // rt.TweenSizeDelta(new Vector2(300, 100), 0.3f);
    public static Tween TweenSizeDelta(this RectTransform rt,
        Vector2 to, float time = 0.3f, Ease ease = Ease.OutQuad)
    {
        rt.DOKill();
        return rt.DOSizeDelta(to, time).SetEase(ease)
            .SetUpdate(true).SetLink(rt.gameObject);
    }

    // DORotate - 누르면 한바퀴 회전 // rt.TweenRotate(new Vector3(0, 0, 90), 0.3f);
    public static Tween TweenRotate(this RectTransform rt,
        Vector3 to, float time = 0.3f, RotateMode mode = RotateMode.FastBeyond360, Ease ease = Ease.OutQuad)
    {
        rt.DOKill();
        return rt.DORotate(to, time, mode).SetEase(ease)
            .SetUpdate(true).SetLink(rt.gameObject);
    }

    // DOColor - 누르면 색 변경 // image.TweenColor(Color.red, 0.3f);
    public static Tween TweenColor(this Image img,
        Color to, float time = 0.3f, Ease ease = Ease.OutQuad)
    {
        img.DOKill();
        return img.DOColor(to, time).SetEase(ease)
            .SetUpdate(true).SetLink(img.gameObject);
    }

    // 시퀀스 = 여러 트윈을 순서대로 or 동시에 묶는 것
//    var seq = DOTween.Sequence();
//    seq.Append(rt.DOScale(0.95f, 0.1f));   // 1단계
//    seq.Append(rt.DOScale(1.05f, 0.1f));   // 2단계
//    seq.Append(rt.DOScale(1f, 0.1f));      // 3단계
 
    //public static void AddHoverColor

}