using System;
using UnityEngine;

public class EnvironmentPresenter : MonoBehaviour
{
    EnvironmentModel _model;
    EnvironmentView _view;

    Season _lastSeason;
    DayilyCycle _lastDayily;

    //데이터 받아오기
    //private void InitData()
    //{
    //데이터 받아오는 로직
    //받아온 데이터를 모델에 연결해 데이터 추가
    //}

    private void Start()
    {
        //처음 시작할 때 현재 시각 기억
        DateTime now = DateTime.UtcNow.AddHours(9);
        _model.UpdateTimeSet(now);

        //각 각 현재 시간, 현재 일 수를 기억
        _lastDayily = _model.CurrentDay;
        _lastSeason = _model.CurrentSeason;
    }

    private void Update()
    {
        DateTime now = DateTime.UtcNow.AddHours(9);
        _model.UpdateTimeSet(now);

        if (_lastDayily != _model.CurrentDay) 
        {
            //만약 바뀌면 배경 변경
            _lastDayily = _model.CurrentDay;
        }
        if (_lastSeason != _model.CurrentSeason) 
        {
            //파티클 랜덤확률로 쏘기
            _lastSeason = _model.CurrentSeason;
        }

    }


}
