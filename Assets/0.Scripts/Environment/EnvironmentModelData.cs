using System;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentModel
{
    EnvironmentData _data;

    public event Action<Season> OnSeasonChanged;
    public event Action<DayilyCycle> OnDailyChanged;

    private Season _currentSeason;
    public Season CurrentSeason { 
        get => _currentSeason; 
        private set
        {
            if (_currentSeason != value)
            {
                _currentSeason = value;
                OnSeasonChanged?.Invoke(_currentSeason); // 계절이 바뀔 때만 실행
            }
        } }
    private DayilyCycle _currentDay;
    public DayilyCycle CurrentDay
    {
        get => _currentDay;
        private set
        {
            if (_currentDay != value)
            {
                _currentDay = value;
                OnDailyChanged?.Invoke(_currentDay); // 낮, 밤이 바뀔 때만 실행
            }
        }
    }

    public int[] _seasonDuration = new int[4];
    private int _lastMonth = -1;
    private int _lastDay = -1;

    [SerializeField] private float _dayDuration = 1200f;
    private Dictionary<Season, float[]> _seasonWeights = new()
    {
        { Season.Spring, new float[] { 1.0f, 1.0f, 1.0f } },
        { Season.Summer, new float[] { 1.5f, 1.0f, 0.5f } },
        { Season.Autumn, new float[] { 0.75f, 1.5f, 0.75f } },
        { Season.Winter, new float[] { 0.5f, 1.0f, 1.5f } }
    };


    //전체적인 시간 계산 메서드
    public void UpdateTimeSet(DateTime now)
    {
        if (now.Month != _lastMonth) //달이 바뀔 때 마다
        {
            _lastMonth = now.Month;
            SeasonalCalculation(now);
        }
        if (now.Day != _lastDay) //일이 바뀔 때 마다
        {
            _lastDay = now.Day;
            CurrentSeason = SetSeason(now);
        }
        SetDay(now); //얘는 계속 해줘야하니까
    }

    private Season SetSeason(DateTime now)
    {
        //이 부분 반복문 안쓰고 더 깔끔하게 할 수 있을거 같은데...
        for (int i = 0; i < _seasonDuration.Length; i++)
        {
            if (now.Day <= _seasonDuration[i]) //만약 아래에서 계산한 날짜별 계절 마지막날보다 적으면
            {
                return (Season)i; //Season에 있는 enum값에 따라 계절 설정
            }
        }
        return Season.Winter; //얘를 안하면 오류떠서 기본값은 마지막날인 겨울로 설정
    }
    private void SeasonalCalculation(DateTime now)
    {

        int days = DateTime.DaysInMonth(now.Year, now.Month); //현재 시간 계산
        int extra = days - 28; //29,30,31일 등 나머지 날짜

        HashSet<int> additional = new();

        while (additional.Count < extra)
        {
            additional.Add(UnityEngine.Random.Range(0, 4)); //계절이 4개니까 0,1,2,3 중 하나 선택
        }
        int currentDaySum = 0; //계절별 마지막 날짜 구하기 위한 장치
        for (int i = 0; i < additional.Count; i++)
        {
            int duration = 7; //계절별 기본 날짜
            if (additional.Contains(i))
            {
                duration++; //만약 위에서 랜덤으로 뽑힌 값이 같다면 일자 추가(랜덤)
            }

            currentDaySum += duration; //종합적인 마지막 날을 계산 ex) 봄 7, 여름 15, 가을 23, 겨울 31
            _seasonDuration[i] = currentDaySum; //위에서 계절 배열에다가 주입
        }
    }

    private void SetDay(DateTime now)
    {
        float currentSeconds = (now.Minute * 60) + now.Second; //분과 초를 종합해서 계산 ex)3600초
        float[] mult = _seasonWeights[CurrentSeason]; //딕셔너리에 있는 가중치 들고옴

        float dayEnd = _dayDuration * mult[0];
        float SunsetEnd = _dayDuration * mult[1];

        //스위치문으로 변경 시켰는데 더 깔끔해보이고 좋네요
        //그리고 굳이 Night는 조건 안줘도 되니까 그냥 없앴습니다
        CurrentDay = currentSeconds switch
        {
            _ when currentSeconds < dayEnd => DayilyCycle.Day,
            _ when currentSeconds < dayEnd + SunsetEnd => DayilyCycle.Sunset,
            _ => DayilyCycle.Night
        };
    }
    //데이터 저장 메서드
    public EnvironmentData SaveData(DateTime now)
    {
        EnvironmentData box = new()
        {
            _calculation = (int[])_seasonDuration.Clone()
        };

        Debug.Log($"계절 마지막 일자: [{box._calculation}] 저장완료</color>");

        return box;
    }
    public void LoadData(EnvironmentData box)
    {
        _data = box;
        _seasonDuration = (int[])_data._calculation.Clone();

        Debug.Log($"<color=yellow>{_lastMonth}월{_lastDay}일 로드 완료</color>");
    }
}
