using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentView : MonoBehaviour
{
    [SerializeField] private Image _seasonImage;
    [SerializeField] private Image _dailyImage;
    [SerializeField] private ParticleSystem[] _particles; //반드시 봄, 여름, 가을, 겨울 순서대로 넣어줄 것
    [SerializeField] private TextMeshProUGUI _timeText;
    private ParticleSystem _currentParticle;
    private Color targetColor;

    [SerializeField] List<Sprite> _seasonSprite;
    [SerializeField] List<Sprite> _dayliySprite;

    [Range(0.1f,2)]
    public float _transitionDuration = 0.5f; //배경 색상 전환 시간. 숫자가 적어지면 적어질 수록 느리게 바뀜

    public void PlaySeasonParticle(Season season)
    {
        //미리 계절을 정수로 변환
        int seasonIndex = (int)season;

        int rnd = UnityEngine.Random.Range(1, 101);

        if (rnd <= 35)
        {
            //해당 계절 파티클 재생
            _currentParticle = _particles[seasonIndex]; //현재 파티클을 설정
            _particles[seasonIndex].gameObject.SetActive(true);
            _particles[seasonIndex].Play();
        }
        //만약 랜덤값이 35보다 크고 현재 재생중인 파티클이 있다면 멈추고 비활성화
        else if (_currentParticle != null)
        {
            _currentParticle.Stop();
            _currentParticle.gameObject.SetActive(false);
            _currentParticle = null; //밑에서 만약 파티클이 안나올 수 있기 때문에 null로 설정
        }
    }
    public void ChangeDayilyBackGround(DayilyCycle dayily)
    {
        targetColor = dayily switch //색상은 언제든지 변경 가능합니다
        {
            DayilyCycle.Day => new Color(1f, 0.9568627f, 0.8392157f),
            DayilyCycle.Sunset => new Color(0.5294118f, 0.8078431f, 0.9215686f), 
            DayilyCycle.Night => new Color(0.05098039f, 0.05098039f, 0.2f),
            _ => _seasonImage.color
        };
    }

    public void ChangeSeasonSprite(Season season)
    {
        int index = (int)season;
        if (index >= 0 && index < _seasonSprite.Count)
        {
            _seasonImage.sprite = _seasonSprite[index];
        }
    }

    public void ChangeDailySprite(DayilyCycle daily)
    {
        int index = (int)daily;
        if (index >= 0 && index < _dayliySprite.Count)
        {
             _dailyImage.sprite = _dayliySprite[index];
        }
    }
   
}
