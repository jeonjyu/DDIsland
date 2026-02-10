using System;
using System.Collections.Generic;
using UnityEngine;
public enum Season
{
    Spring, Summer, Autumn, Winter
}
public enum DayilyCycle
{
    Morning, Day, Night
}
[Serializable]
public class EnvironmentDataBox
{
    public string _season;
    public string _day;
    
    public Dictionary<Season, float[]> _seasonWeight = new()
    {
        { Season.Spring, new float[] { 1.0f, 1.0f, 1.0f } },
        { Season.Summer, new float[] { 1.5f, 1.0f, 0.5f } },
        { Season.Autumn, new float[] { 0.75f, 1.5f, 0.75f } },
        { Season.Winter, new float[] { 0.5f, 1.0f, 1.5f } }
    };
}
