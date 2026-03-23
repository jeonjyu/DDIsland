using System;
using UnityEngine;

public class EnvironmentPresenter : MonoBehaviour
{
    private EnvironmentModel _model;
    [SerializeField] private EnvironmentView _view;
    [SerializeField] private LakeImage _lakeImage;

    //여기서 미리 지정해줍니다
    private DateTime now = DateTime.UtcNow.AddHours(9);

    public EnvironmentModel Model => _model;
    
    private void Awake()
    {
        _model = new EnvironmentModel();

        if (FishManager.Instance != null)
        {
            FishManager.Instance.SetEnvironment(_model);
        }
    }

    private void OnEnable()
    {
        // 켜질 때 구독 (이벤트 연결)
        _model.OnSeasonChanged += _view.ChangeSeasonSprite;
        _model.OnDailyChanged += _lakeImage.HandleDailyChanged;

        _model.OnDailyChanged += _view.ChangeDayilyBackGround;
        _model.OnDailyChanged += _view.ChangeDailySprite;
        _model.OnWeatherChanged += _view.PlaySeasonParticle;


        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            DataManager.Instance.Hub.OnRequestSave += SyncEnvironmentDataSave;
        }
    }
    private void OnDisable()
    {
        // 꺼질 때 해제 (중복 구독 및 메모리 누수 방지)
        _model.OnSeasonChanged -= _view.ChangeSeasonSprite;
        _model.OnDailyChanged -= _lakeImage.HandleDailyChanged;

        _model.OnDailyChanged -= _view.ChangeDayilyBackGround;
        _model.OnDailyChanged -= _view.ChangeDailySprite;
        _model.OnWeatherChanged -= _view.PlaySeasonParticle;
        

        if (DataManager.Instance != null &&DataManager.Instance.Hub != null)
        {
            DataManager.Instance.Hub.OnRequestSave -= SyncEnvironmentDataSave;
        }
    }

    private void Start()
    {
        if (DataManager.Instance?.Hub != null)
        {
            if (DataManager.Instance.Hub.IsLoaded)
            {
                SyncEnvironmentDataLoad();
            }
            else
            {
                DataManager.Instance.Hub.OnDataLoaded += SyncEnvironmentDataLoad;
            }
        }

        //처음 시작할 때 현재 시각 기억
        now = DateTime.UtcNow.AddHours(9);
        //DateTime now = new DateTime(2026,1,1).AddDays(UnityEngine.Random.Range(0, 365))
        //                       .AddHours(UnityEngine.Random.Range(0, 24))
        //                       .AddMinutes(UnityEngine.Random.Range(0, 60));


        RefreshAll();

    }

    private void Update()
    {
        _model.UpdateTimeSet(now);

    }

   
    public void SyncEnvironmentDataSave()
    {
        //데이터 저장하는거   
        _model.UpdateTimeSet(now);

        //Debug.Log($"<color=cyan>테스트 날짜: {now} 현재 시점: {_model.CurrentDay} 현재 계절: {_model.CurrentSeason}</color>");

        Environment_Data envData = _model.SaveData();
        
        DataManager.Instance.Hub._allUserData.Environment = envData;
    }
    
    public void SyncEnvironmentDataLoad()
    {
        if (DataManager.Instance != null && DataManager.Instance.Hub != null)
        {
            DataManager.Instance.Hub.OnDataLoaded -= SyncEnvironmentDataLoad;
        }
        // 데이터 꺼내오는거
        Environment_Data env = DataManager.Instance.Hub._allUserData.Environment;

        if (env != null && env._calculation != null)
        {
            _model.LoadData(env);

            //Model에서 갖고 있는 시간에 따른 변화 - 근데 이거 Update에서 하고 있는데...혹시나 몰라서 넣어놨습니다 
            // + 테스트용

            RefreshAll();

        }
    }

    private void RefreshAll()
    {
        _model.UpdateTimeSet(now);

        _view.ChangeDayilyBackGround(_model.CurrentDay);
        _view.ChangeDailySprite(_model.CurrentDay);
        _view.ChangeSeasonSprite(_model.CurrentSeason);
        _lakeImage.HandleDailyChanged(_model.CurrentDay);
    }

}
