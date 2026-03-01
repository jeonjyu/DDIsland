using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FoodDatabaseSO", menuName = "Scriptable Objects/DataBase/FoodDatabaseSO")]
public class FoodDatabaseSO : TableDatabase<int, FoodDataSO>
{

}
