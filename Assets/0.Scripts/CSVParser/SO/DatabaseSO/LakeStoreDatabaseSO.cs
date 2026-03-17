using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LakeStoreDatabaseSO", menuName = "Scriptable Objects/DataBase/LakeStoreDatabaseSO")]
public class LakeStoreDatabaseSO : TableDatabase<int, LakeStoreDataSO>
{

}
