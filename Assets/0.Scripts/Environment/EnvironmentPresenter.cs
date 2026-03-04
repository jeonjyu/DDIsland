using System;
using UnityEngine;

public class EnvironmentPresenter : MonoBehaviour
{
    private EnvironmentModel _model;
    [SerializeField] private EnvironmentView _view;

    //여기서 미리 지정해줍니다
    private DateTime now = DateTime.UtcNow.AddHours(9);

    private void Awake()
    {
        _model = new EnvironmentModel();
    }

    private void OnEnable()
    {
        // 켜질 때 구독 (이벤트 연결)
        _model.OnSeasonChanged += _view.PlaySeasonParticle;
        _model.OnSeasonChanged += _view.ChangeSeasonSprite;

        _model.OnDailyChanged += _view.ChangeDayilyBackGround;
        _model.OnDailyChanged += _view.ChangeDailySprite;
        _model.OnDailyChanged += (daily) => _view.PlaySeasonParticle(_model.CurrentSeason);


        if (DataMgr.Instance != null)
            DataMgr.Instance.OnDataLoaded += SyncWithDataMgr;
    }
    private void OnDisable()
    {
        // 꺼질 때 해제 (중복 구독 및 메모리 누수 방지)
        _model.OnSeasonChanged -= _view.PlaySeasonParticle;
        _model.OnSeasonChanged -= _view.ChangeSeasonSprite;

        _model.OnDailyChanged -= _view.ChangeDayilyBackGround;
        _model.OnDailyChanged -= _view.ChangeDailySprite;
        _model.OnDailyChanged -= (daily) => _view.PlaySeasonParticle(_model.CurrentSeason);

        if (DataMgr.Instance != null)
            DataMgr.Instance.OnDataLoaded -= SyncWithDataMgr;
    }

    private void Start()
    {
       
        //처음 시작할 때 현재 시각 기억
        DateTime now = DateTime.UtcNow.AddHours(9);
        //DateTime now = new DateTime(2026,1,1).AddDays(UnityEngine.Random.Range(0, 365))
        //                       .AddHours(UnityEngine.Random.Range(0, 24))
        //                       .AddMinutes(UnityEngine.Random.Range(0, 60));
        _model.UpdateTimeSet(now);

        _view.ChangeDayilyBackGround(_model.CurrentDay);
        _view.PlaySeasonParticle(_model.CurrentSeason);

        Debug.Log($"<color=cyan>테스트 날짜: {now} 현재 시점: {_model.CurrentDay} 현재 계절: {_model.CurrentSeason}</color>");

        _view.ChangeDailySprite(_model.CurrentDay);
        _view.ChangeSeasonSprite(_model.CurrentSeason);

    }

    private void Update()
    {
        //여기서 초를 추가해 줬습니다 10분 마다요!
        
        now = now.AddSeconds(Time.deltaTime * 600f);
        _model.UpdateTimeSet(now);
        _view.TextedTimer(now);

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
        //데이터 저장하는거   
        _model.UpdateTimeSet(now);
        _view.ChangeDayilyBackGround(_model.CurrentDay);
        _view.PlaySeasonParticle(_model.CurrentSeason);

        Debug.Log($"<color=cyan>테스트 날짜: {now} 현재 시점: {_model.CurrentDay} 현재 계절: {_model.CurrentSeason}</color>");

        EnvironmentData envData = _model.SaveData(now);
        
        DataMgr.Instance._allUserData.Environment = envData;

        DataMgr.Instance.GetEnvJson();
    }
    
    public void SyncWithDataMgr()
    {
        // 데이터 꺼내오는거
        EnvironmentData env = DataMgr.Instance._allUserData.Environment;

        if (env != null)
        {
            _model._seasonDuration = env._calculation;

           //Model에서 갖고 있는 시간에 따른 변화 - 근데 이거 Update에서 하고 있는데...혹시나 몰라서 넣어놨습니다 
           // + 테스트용
           
            _view.ChangeDayilyBackGround(_model.CurrentDay);
            _view.PlaySeasonParticle(_model.CurrentSeason);

            Debug.Log($"<color=cyan> 데이터 복구 완료 {_model.CurrentSeason} / {_model.CurrentDay}</color>");
        }
    }

}
