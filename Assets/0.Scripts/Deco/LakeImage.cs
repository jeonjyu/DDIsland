using System.Collections;
using UnityEngine;

public class LakeImage : MonoBehaviour
{
    [SerializeField] private Animator _lakeAnimator;
    [SerializeField] private float _transitionSpeed = 1.0f; // 전환 속도

    private int _sunsetLayerIndex;
    private int _nightLayerIndex;
    private Coroutine _transitionCoroutine;

    private float _targetSunsetWeight = 0f;
    private float _targetNightWeight = 0f;

    void Start()
    {
        _sunsetLayerIndex = _lakeAnimator.GetLayerIndex("Sunset");
        _nightLayerIndex = _lakeAnimator.GetLayerIndex("Night");
    }
    public void HandleDailyChanged(DayilyCycle newCycle)
    {
        switch (newCycle)
        {
            case DayilyCycle.Day:
                _targetSunsetWeight = 0f;
                _targetNightWeight = 0f;
                break;
            case DayilyCycle.Sunset:
                _targetSunsetWeight = 1f;
                _targetNightWeight = 0f;
                break;
            case DayilyCycle.Night:
                _targetSunsetWeight = 0f;
                _targetNightWeight = 1f;
                break;
        }

        if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
        _transitionCoroutine = StartCoroutine(SmoothTransition());
    }

    IEnumerator SmoothTransition()
    {
        if (_sunsetLayerIndex == -1 || _nightLayerIndex == -1) yield break;

        while (true)
        {
            float currentSunset = _lakeAnimator.GetLayerWeight(_sunsetLayerIndex);
            float currentNight = _lakeAnimator.GetLayerWeight(_nightLayerIndex);

            float nextSunset = Mathf.MoveTowards(currentSunset, _targetSunsetWeight, _transitionSpeed * Time.deltaTime);
            float nextNight = Mathf.MoveTowards(currentNight, _targetNightWeight, _transitionSpeed * Time.deltaTime);

            _lakeAnimator.SetLayerWeight(_sunsetLayerIndex, nextSunset);
            _lakeAnimator.SetLayerWeight(_nightLayerIndex, nextNight);

            if (Mathf.Approximately(nextSunset, _targetSunsetWeight) &&
                Mathf.Approximately(nextNight, _targetNightWeight))
                break;

            yield return null;
        }
    }
}
