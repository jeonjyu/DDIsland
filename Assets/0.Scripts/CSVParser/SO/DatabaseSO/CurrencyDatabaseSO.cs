using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CurrencyDatabaseSO", menuName = "Scriptable Objects/DataBase/CurrencyDatabaseSO")]
public class CurrencyDatabaseSO : TableDatabase<int, CurrencyDataSO>
{

}
