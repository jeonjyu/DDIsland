using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FishDatabaseSO", menuName = "Scriptable Objects/DataBase/FishDatabaseSO")]
public class FishDatabaseSO : TableDatabase<int, FishDataSO>
{

}
