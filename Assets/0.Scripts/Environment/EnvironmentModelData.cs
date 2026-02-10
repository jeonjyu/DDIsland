using System;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentModel
{
    public Season CurrentSeason { get; private set; }
    public DayilyCycle CurrentDay { get; private set; }
    EnvironmentDataBox data;

    public Season SetSeason()
    {

        return Season.Spring; //아직 계절 바뀌는게 없어서 임시로 return값 넣음
    }
    public void SetDay(DateTime now)
    {
        int minutes = now.Minute;
        float[] mult = data._seasonWeight[CurrentSeason];

        float morning = 1200f * mult[0];
        float day = 1200f * mult[1];
        float night = 1200f * mult[2];


    }

}
