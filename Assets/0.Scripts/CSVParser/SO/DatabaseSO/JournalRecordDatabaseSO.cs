using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "JournalRecordDatabaseSO", menuName = "Scriptable Objects/DataBase/JournalRecordDatabaseSO")]
public class JournalRecordDatabaseSO : TableDatabase<int, JournalRecordDataSO>
{

}
