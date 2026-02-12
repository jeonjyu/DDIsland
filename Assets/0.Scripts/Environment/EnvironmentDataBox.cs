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
    public string _lastDay; //저장 일시
    public int[] _calculation;

}
