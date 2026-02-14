using System;
using Unity.VisualScripting;
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
        if (DataMgr.Instance != null)
        {
            DataMgr.Instance.OnDataLoaded += SyncWithDataMgr;
        }
        //처음 시작할 때 현재 시각 기억
        DateTime now = DateTime.UtcNow.AddHours(9);
        //DateTime now = new DateTime(2026,1,1).AddDays(UnityEngine.Random.Range(0, 365))
        //                       .AddHours(UnityEngine.Random.Range(0, 24))
        //                       .AddMinutes(UnityEngine.Random.Range(0, 60));
        _model.UpdateTimeSet(now);


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

        Debug.Log($"<color=cyan>테스트 날짜: {now} 현재 시점: {_model.CurrentDay} 현재 계절: {_model.CurrentSeason}</color>");
    }

    private void Update()
    {

        DateTime now = DateTime.UtcNow.AddHours(9);
        _model.UpdateTimeSet(now);

        if (_lastDayily != _model.CurrentDay)
        {
            _view.ChangeDayilyBackGround(_model.CurrentDay);
            _view.PlaySeasonParticle(_model.CurrentSeason);
            _lastDayily = _model.CurrentDay;
        }
        if (_lastSeason != _model.CurrentSeason)
        {
            _lastSeason = _model.CurrentSeason;
        }

    }
    //이 친구 인스펙터에서 우클릭 후 해당 메서드 이름 누르시면 실행돼요!
    //현 기능은 JSON 저장용입니다
    [ContextMenu("JSONSave")]
    public void SaveMenu()
    {
        DateTime now = DateTime.UtcNow.AddHours(9);
        //DateTime now = new DateTime(2026, 1, 1).AddDays(UnityEngine.Random.Range(0, 365))
        //                      .AddHours(UnityEngine.Random.Range(0, 24))
        //                      .AddMinutes(UnityEngine.Random.Range(0, 60));
        Save(now);
    }
    public void Save(DateTime now)
    {
        
        _model.UpdateTimeSet(now);
        _view.ChangeDayilyBackGround(_model.CurrentDay);
        _view.PlaySeasonParticle(_model.CurrentSeason);

        Debug.Log($"<color=cyan>테스트 날짜: {now} 현재 시점: {_model.CurrentDay} 현재 계절: {_model.CurrentSeason}</color>");

        EnvironmentData envData = _model.SaveData(now);
        
        DataMgr.Instance._allUserData.Environment = envData;

        DataMgr.Instance.GetEnvJson();
    }
    private void OnDisable()
    {
        if (DataMgr.Instance != null)
        {
            DataMgr.Instance.OnDataLoaded -= SyncWithDataMgr;
        }
    }
    public void SyncWithDataMgr()
    {
        // 데이터 꺼내오는거
        EnvironmentData env = DataMgr.Instance._allUserData.Environment;

        if (env != null && !string.IsNullOrEmpty(env._lastDay))
        {
            //굳이 끝내는 시간을 저장해야하는지는 의문
            DateTime savedTime = DateTime.Parse(env._lastDay);
            _model._seasonDuration = env._calculation;

           //Model에서 갖고 있는 시간에 따른 변화 - 근데 이거 Update에서 하고 있는데...혹시나 몰라서 넣어놨습니다 
           // + 테스트용
            _model.UpdateTimeSet(savedTime);

            //모델에서 가지고 있는 현재 시간, 현재 계절을 Presenter에 갖고옴
            _lastDayily = _model.CurrentDay;
            _lastSeason = _model.CurrentSeason;
            //앞에있는 현재 시간, 계절에 따라 시각적 업데이트를 진행
            _view.ChangeDayilyBackGround(_model.CurrentDay);
            _view.PlaySeasonParticle(_model.CurrentSeason);

            Debug.Log($"<color=cyan>데이터 복구 완료: {savedTime} / {_lastSeason} / {_lastDayily}</color>");
        }
    }

}
