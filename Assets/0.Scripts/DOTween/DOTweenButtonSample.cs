using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 테스트 용 스크립트
/// 빈 Canvas에 이 컴포넌트 붙이고, 버튼울 인스펙터에서 연결하면 끝
/// </summary>
public class DOTweenButtonSample : MonoBehaviour
{
    [Header("버튼 연결")]
    public Button[] btnPunchScale;    // 통통 튐
    public Button[] btnPunchPosition; // 좌우로 흔들림
    public Button[] btnFade;          // 사라졌다 나타남
    public Button[] btnScale;         // 커졌다 돌아옴
    public Button[] btnSizeDelta;     // 넓어졌다 돌아옴
    public Button[] btnRotate;        // 한바퀴 회전
    public Button[] btnColor;         // 색 변경
    public Button[] btnSeqColor;      // 여러번 색 변경
    public Button[] btnHoverColor;    // 미구현 

    private void Start()
    {
        // 1. PunchScale - 누르면 통통 튐
        foreach (var btn in btnPunchScale)
        {
            btn.onClick.AddListener(() =>
            {
                btn.GetComponent<RectTransform>().TweenPunchScale(0.2f, 0.3f); // 더 크게 튐 (0.2f), 더 느리게 튐 (0.3초간)
            });
        }

        // 2. PunchPosition - 누르면 좌우로 흔들림
        foreach (var btn in btnPunchPosition)
        {
            btn.onClick.AddListener(() =>
            {
                // btn.GetComponent<RectTransform>().TweenPunchPosition(new Vector2(30f, 0f), 0.3f); // 좌우로 흔들림 (0.3초간)
                
                RectTransform rt = btn.GetComponent<RectTransform>();
                var seq = DOTween.Sequence();
                seq.Append(rt.TweenPunchPosition(new Vector2(30f, 0f), 0.3f)); // 좌우로 흔들림 (0.3초간)
                seq.Append(rt.TweenPunchPosition(new Vector2(0f, 50f), 0.3f)); // 위아래로 50 흔들림

            });
        }                                       

        // 3. Fade - 누르면 사라졌다 나타남
        foreach (var btn in btnFade)
        {
            btn.onClick.AddListener(() =>
            {
                CanvasGroup cg = btn.GetComponent<CanvasGroup>();
                if (cg == null) cg = btn.gameObject.AddComponent<CanvasGroup>();

                cg.TweenFade(0f, 0.5f). // 0.5초동안 알파값이 사라짐
                OnComplete(() => cg.TweenFade(1f, 10.0f));  // 10초동안 알파값이 서서히 나타남 
                                                            //   cg.TweenFade(0.5f, 0.3f); //반투명
            });
        }

        // 4. Scale - 누르면 커졌다 돌아옴
        foreach (var btn in btnScale)
        {
            btn.onClick.AddListener(() =>
            {
                RectTransform rt = btn.GetComponent<RectTransform>();
                var seq = DOTween.Sequence();
                seq.Append(rt.TweenScale(Vector3.one * 0.5f, 0.2f));   // 0.5배로 줄어듦    
                seq.Append(rt.TweenScale(Vector3.one * 1.3f, 0.2f)) // 1.3배 커짐                       
                     .OnComplete(() => rt.TweenScale(Vector3.one, 0.2f)); // 원래 크기로 돌아옴
            });
        }
        // 5. SizeDelta - 누르면 넓어졌다 돌아옴
        foreach (var btn in btnSizeDelta)
        {
            btn.onClick.AddListener(() =>
            {
                RectTransform rt = btn.GetComponent<RectTransform>();
                Vector2 originSize = rt.sizeDelta;
                var seq = DOTween.Sequence();
                seq.Append(rt.TweenSizeDelta(originSize + new Vector2(100f, 0f), 0.3f)); // 가로만 100 커짐
                seq.Append(rt.TweenSizeDelta(originSize + new Vector2(0f, 100), 0.3f))   // 세로만 100 커짐 
                    .OnComplete(() => rt.TweenSizeDelta(originSize, 0.3f)); // 원래 크기로 돌아옴
            });
        }

        // 6. Rotate - 누르면 한바퀴 회전
        foreach (var btn in btnRotate)
        {
            btn.onClick.AddListener(() =>
            {
                RectTransform rt = btn.GetComponent<RectTransform>();
                var seq = DOTween.Sequence(); // 시퀀스 
                seq.Append(rt.TweenRotate(new Vector3(0, 0, 360f), 0.5f)); // z축으로 360도 한바퀴 회전
                seq.Append(rt.TweenRotate(new Vector3(0, 360f, 0), 0.5f)); // y축으로 회전 //3d로 뒤집어지는 느낌 
                seq.Append(rt.TweenRotate(new Vector3(360f, 0, 0), 0.5f)); // z축으로 회전 //3d로 뒤집어지는 느낌 
            });
        }

        // 7. HoverColor - 누르면 색 변경
        foreach (var btn in btnColor)
        {
            btn.onClick.AddListener(() =>
            {
                Image img = btn.GetComponent<Image>();
                img.TweenColor(new Color(0.3f, 0.8f, 1f), 0.3f); // 하늘색
            });
        }

        // 8. btnSeqColor - 시퀀스 테스트 
        foreach (var btn in btnSeqColor)
        {
            btn.onClick.AddListener(() =>
            {
                Image img = btn.GetComponent<Image>();
                var seq = DOTween.Sequence();
                seq.Append(img.TweenColor(Color.red, 0.3f)); // 빨간색  
                seq.Append(img.TweenColor(Color.blue, 0.3f)); // 파란색
                seq.Append(img.TweenColor(new Color(0.3f, 0.8f, 1f), 0.3f)); // 하늘색
                seq.Append(img.TweenColor(Color.white, 0.3f)); // 흰색 
            });
        }

        // btnHoverColor.AddHoverColor(new Color(0.3f, 0.8f, 1f), 0.2f);
    }
}