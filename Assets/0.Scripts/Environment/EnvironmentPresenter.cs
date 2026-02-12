using System;
using UnityEngine;

public class EnvironmentPresenter : MonoBehaviour
{
    private EnvironmentModel _model;
    [SerializeField] private EnvironmentView _view;

    Season _lastSeason;
    DayilyCycle _lastDayily;

    private void Awake()
    {
        _model = new EnvironmentModel();
    }

    private void Start()
    {
        //처음 시작할 때 현재 시각 기억
        DateTime now = DateTime.UtcNow.AddHours(9);
        //_model.UpdateTimeSet(now);
        DateTime start = new(2026, 1, 1);

        DateTime randomDate = start.AddDays(UnityEngine.Random.Range(0, 365))
                                   .AddHours(UnityEngine.Random.Range(0, 24))
                                   .AddMinutes(UnityEngine.Random.Range(0, 60));

        _model.UpdateTimeSet(randomDate);

        //각 각 현재 시간, 현재 일 수를 기억
        if (_lastDayily != _model.CurrentDay)
        {
            _view.ChangeDayilyBackGround(_model.CurrentDay);
            _lastDayily = _model.CurrentDay;
        }
        if (_lastSeason != _model.CurrentSeason)
        {
            _lastSeason = _model.CurrentSeason;
        }
            _view.PlaySeasonParticle(_lastSeason);

        Debug.Log($"<color=cyan>테스트 날짜: {randomDate} 현재 시점: {_model.CurrentDay} 현재 계절: {_model.CurrentSeason}</color>");
    }

    private void Update()
    {
        DateTime now = DateTime.UtcNow.AddHours(9);
        //_model.UpdateTimeSet(now);
      
        if (_lastDayily != _model.CurrentDay) 
        {
            _view.ChangeDayilyBackGround(_model.CurrentDay);
            _lastDayily = _model.CurrentDay;
        }
        if (_lastSeason != _model.CurrentSeason) 
        {
            _view.PlaySeasonParticle(_model.CurrentSeason);
            _lastSeason = _model.CurrentSeason;
        }

    }
    //이 친구 인스펙터에서 우클릭 후 해당 메서드 이름 누르시면 실행되요!
    [ContextMenu("Random Date")]
    public void TestRandomDate()
    {
        DateTime start = new(2026, 1, 1);

        DateTime randomDate = start.AddDays(UnityEngine.Random.Range(0, 365))
                                   .AddHours(UnityEngine.Random.Range(0, 24))
                                   .AddMinutes(UnityEngine.Random.Range(0, 60));

        _model.UpdateTimeSet(randomDate);
        _view.ChangeDayilyBackGround(_model.CurrentDay);
        _view.PlaySeasonParticle(_model.CurrentSeason);

        Debug.Log($"<color=cyan>테스트 날짜: {randomDate} 현재 시점: {_model.CurrentDay} 현재 계절: {_model.CurrentSeason}</color>");
    }
    //데이터 받아오기
    //private void InitData()
    //{
    //데이터 받아오는 로직
    //받아온 데이터를 모델에 연결해 데이터 추가
    //}


}
