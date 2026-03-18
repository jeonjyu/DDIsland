using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestDatabaseSO", menuName = "Scriptable Objects/DataBase/QuestDatabaseSO")]
public class QuestDatabaseSO : TableDatabase<int, QuestDataSO>
{

}
