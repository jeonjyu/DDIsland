using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentView : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private ParticleSystem[] _particles; //반드시 봄, 여름, 가을, 겨울 순서대로 넣어줄 것
    private ParticleSystem _currentParticle;

    public float _transitionDuration = 1f; //배경 색상 전환 시간

    public void PlaySeasonParticle(Season season)
    {
        //미리 계절을 정수로 변환
        int seasonIndex = (int)season;

        //현재 재생중인 파티클이 있으면 정지
        if (_currentParticle != null)
        {
            _currentParticle.Stop();
            _currentParticle.gameObject.SetActive(false);
        }

        //해당 계절 파티클 재생
        _currentParticle = _particles[seasonIndex]; //현재 파티클을 설정
        _particles[seasonIndex].gameObject.SetActive(true);
        _particles[seasonIndex].Play();

    }
    public void ChangeDayilyBackGround(DayilyCycle dayily)
    {
        Color targetColor = dayily switch
        {
            DayilyCycle.Morning => new Color(1f, 0.9568627f, 0.8392157f), //아침 노란빛
            DayilyCycle.Day => new Color(0.5294118f, 0.8078431f, 0.9215686f), //낮 파란빛
            DayilyCycle.Night => new Color(0.05098039f, 0.05098039f, 0.2f), //밤 어두운 파란빛
            _ => _background.color
        };
    }
    private void Update()
    {
        _background.color = Color.Lerp(_background.color, _background.color, Time.deltaTime *  _transitionDuration); //배경 색상 부드럽게 변화
    }
}
