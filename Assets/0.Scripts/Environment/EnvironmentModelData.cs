using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class EnvironmentModel
{
    public Season CurrentSeason { get; private set; }
    public DayilyCycle CurrentDay { get; private set; }
    EnvironmentDataBox _data;
    private int[] _seasonDuration = new int[4];
    private int _lastMonth = -1;
    private int _lastDay = -1;
    public void UpdateTimeSet(DateTime now)
    {
        if(now.Month != _lastMonth)
        {
            _lastMonth = now.Month;
            SeasonalCalculation(now);
        }
        if(now.Day != _lastDay)
        {
            _lastDay = now.Day;
            SetSeason(now);
        }
        SetDay(now);
    }

    private Season SetSeason(DateTime now)
    {
        for(int i =0; i<_seasonDuration.Length; i++)
        {
            if(now.Day<=_seasonDuration[i])
            {
                return (Season)i;
            }
        }
        return Season.Winter;
    }
    private void SeasonalCalculation(DateTime now)
    {

        int days = DateTime.DaysInMonth(now.Year, now.Month);
        int extra = days - 28;

        HashSet<int> additional = new();

        while (additional.Count<extra)
        {
            additional.Add(UnityEngine.Random.Range(0, 4));
        }
        int currentDaySum = 0;
        for (int i = 0; i < 4; i++)
        {
            int duration = 7;
            if (additional.Contains(i))
            {
                duration++;
            }

            currentDaySum += duration;
            _seasonDuration[i] = currentDaySum;
        }
    }
    private void SetDay(DateTime now)
    {
        float currentSeconds = (now.Minute * 60) + now.Second;
        float[] mult = _data._seasonWeight[CurrentSeason];

        float morning = 1200f * mult[0];
        float day = 1200f * mult[1];
        float night = 1200f * mult[2];

        if (currentSeconds < morning)
        {
            CurrentDay = DayilyCycle.Morning;
        }
        else if (currentSeconds < (morning + day))
        {
            CurrentDay = DayilyCycle.Day;
        }
        else
        {
            CurrentDay = DayilyCycle.Night;
        }
    }
}
