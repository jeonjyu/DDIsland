using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// DoTween UI 트위닝 확장 클래스
/// 범용 확장 메서드 모음
/// 
/// 사용법 예시:                                          // (); 로 비우면 기본값, 끝에 동일한 0.3f 는 작동하는 시간 
///   rt.TweenPunchScale(0.5f, 0.3f);                    // 더 크게 튐, 더 느리게 튐(작동하는 시간)  
///   rt.TweenPunchPosition(new Vector2(0f, 50f), 0.3f); // 좌우로 흔들림, 위아래로 흔들림, 작동하는 시간 
///   cg.TweenFade(0f, 0.3f);                            // 알파값 (0f 투명, 1f 불투명) 
///   rt.TweenScale(Vector3.one * 1.5f, 0.3f);           // 1.5배 커짐
///   rt.TweenSizeDelta(new Vector2(300, 100), 0.3f);    // 가로/세로 넓어짐
///   rt.TweenRotate(new Vector3(0, 0, 90), 0.3f);       // z축으로 90도 회전
///   img.TweenColor(Color.red, 0.3f);                 // 빨간색으로 변경
///   
/// </summary>
public static class DOTweenExtensionsClass
{
    //  DOPunchScale - 누르면 통통 튐 // rt.TweenPunchScale(0.5f, 0.5f); // 더 크게 튐, 더 느리게 튐  
    public static Tween TweenPunchScale(this RectTransform rt,
        float punch = 0.2f, float time = 0.3f, int vibrato = 10, float elasticity = 1f)
    {
        rt.DOKill(); // 이전 트윈이 있다면 제거
        return rt.DOPunchScale(Vector3.one * punch, time, vibrato, elasticity)
            .SetUpdate(true).SetLink(rt.gameObject);
    }

    //  DOPunchPosition - 누르면 좌우로 흔들림 // rt.TweenPunchPosition(0f, 50f); // 좌우로 흔들림, 위아래로 흔들림
    public static Tween TweenPunchPosition(this RectTransform rt,
        Vector2 punch = default, float time = 0.3f, int vibrato = 10, float elasticity = 1f)
    {
        rt.DOKill();
        if (punch == default) punch = new Vector2(30f, 0f); // 기본값 설정 (좌우로 30, 위아래로 0)
        return rt.DOPunchAnchorPos(punch, time, vibrato, elasticity)
            .SetUpdate(true).SetLink(rt.gameObject);
    }

    //  DOFade - 누르면 사라졌다 나타남 // cg.TweenFade(0f, 0.3f);  // 알파값, 작동하는 시간  
    public static Tween TweenFade(this CanvasGroup cg,
        float to, float time = 0.3f, Ease ease = Ease.OutQuad)
    {
        cg.DOKill();
        return cg.DOFade(to, time).SetEase(ease)
            .SetUpdate(true).SetLink(cg.gameObject);
    }

    // DOScale - 누르면 커지거나 작아짐 // rt.TweenScale(Vector3.one * 1.5f, 0.3f); // 1.5배 커짐, 0.3초간 
    public static Tween TweenScale(this RectTransform rt,
        Vector3 to, float time = 0.3f, Ease ease = Ease.OutQuad)
    {
        rt.DOKill();
        return rt.DOScale(to, time).SetEase(ease)
            .SetUpdate(true).SetLink(rt.gameObject);
    }

    // DOSizeDelta - 누르면 넓어짐 // rt.TweenSizeDelta(new Vector2(300, 100), 0.3f); // 가로/세로 넓어짐, 0.3초간
    public static Tween TweenSizeDelta(this RectTransform rt,
        Vector2 to, float time = 0.3f, Ease ease = Ease.OutQuad)
    {
        rt.DOKill();
        return rt.DOSizeDelta(to, time).SetEase(ease)
            .SetUpdate(true).SetLink(rt.gameObject);
    }

    // DORotate - 누르면 한바퀴 회전 // rt.TweenRotate(new Vector3(0, 0, 90), 0.3f); // z축으로 90도 회전, 0.3초간
    public static Tween TweenRotate(this RectTransform rt,
        Vector3 to, float time = 0.3f, RotateMode mode = RotateMode.FastBeyond360, Ease ease = Ease.OutQuad)
    {
        rt.DOKill();
        return rt.DORotate(to, time, mode).SetEase(ease)
            .SetUpdate(true).SetLink(rt.gameObject);
    }

    // DOColor - 누르면 색 변경 // img.TweenColor(Color.red, 0.3f); // 빨간색으로 변경, 0.3초간 
    public static Tween TweenColor(this Image img,
        Color to, float time = 0.3f, Ease ease = Ease.OutQuad)
    {
        img.DOKill();
        return img.DOColor(to, time).SetEase(ease)
            .SetUpdate(true).SetLink(img.gameObject);
    }


    //public static void AddHoverColor

}