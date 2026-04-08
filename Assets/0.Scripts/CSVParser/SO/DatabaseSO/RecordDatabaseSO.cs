using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RecordDatabaseSO", menuName = "Scriptable Objects/DataBase/RecordDatabaseSO")]
public class RecordDatabaseSO : TableDatabase<int, RecordDataSO>
{

}
