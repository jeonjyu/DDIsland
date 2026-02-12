using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentView : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private ParticleSystem[] _particles; //반드시 봄, 여름, 가을, 겨울 순서대로 넣어줄 것
    private ParticleSystem _currentParticle;
    private Color targetColor;

    [Range(0.01f,2)]
    public float _transitionDuration = 0.5f; //배경 색상 전환 시간. 숫자가 적어지면 적어질 수록 느리게 바뀜

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
        targetColor = dayily switch //색상은 언제든지 변경 가능합니다
        {
            DayilyCycle.Morning => new Color(1f, 0.9568627f, 0.8392157f), 
            DayilyCycle.Day => new Color(0.5294118f, 0.8078431f, 0.9215686f), 
            DayilyCycle.Night => new Color(0.05098039f, 0.05098039f, 0.2f), 
            _ => _background.color
        };
    }
    private void Update()
    {
        _background.color = Color.Lerp(_background.color, targetColor, Time.deltaTime *  _transitionDuration); //배경 색상 부드럽게 변화
    }
}
