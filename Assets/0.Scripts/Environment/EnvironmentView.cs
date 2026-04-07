using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentView : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _particles; //반드시 봄, 여름, 가을, 겨울 순서대로 넣어줄 것
    private ParticleSystem _currentParticle;
    private Color targetColor;

    [Header("UI 이미지 연결")]
    [SerializeField] private Image _seasonImageUI;
    [SerializeField] private Image _dailyImageUI;
    [SerializeField] List<Sprite> _seasonSprite;
    [SerializeField] List<Sprite> _dayliySprite;

    [Range(0.1f,2)]
    public float _transitionDuration = 0.5f; //배경 색상 전환 시간. 숫자가 적어지면 적어질 수록 느리게 바뀜

    public void PlaySeasonParticle(Season season, bool isPlaying)
    {
        //미리 계절을 정수로 변환
        int seasonIndex = (int)season;
        if (seasonIndex < 0 || seasonIndex >= _particles.Length) return;

        if (isPlaying)
        {
            // 이미 켜져 있으면 무시
            if (_currentParticle == _particles[seasonIndex] && _currentParticle.isPlaying) return;

            StopCurrentParticle();

            _currentParticle = _particles[seasonIndex];
            _currentParticle.gameObject.SetActive(true);
            _currentParticle.Play();
        }
        else
        {
            StopCurrentParticle();
        }
    }
    private void StopCurrentParticle()
    {
        if (_currentParticle != null)
        {
            _currentParticle.Stop();
            _currentParticle = null;
        }
    }

    public void ChangeSeasonImage(Season season)
    {
        int index = (int)season;
        if (index >= 0 && index < _seasonSprite.Count)
        {
            _seasonImageUI.sprite = _seasonSprite[index];
        }
    }

    public void ChangeDailyImage(DayilyCycle daily)
    {
        int index = (int)daily;
        if (index >= 0 && index < _dayliySprite.Count)
        {
            _dailyImageUI.sprite = _dayliySprite[index];
        }
    }

}
