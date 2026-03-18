using System.Collections.Generic;
using UnityEngine;
// SO 기반 업그레이드 데이터 로더
public static class UpgradeTempData
{
    public static List<UpgradeData> GetAll()
    {
        List<UpgradeData> table = new List<UpgradeData>();
 
        // DataManager.cs의 CharacterUpgradeData.datas로 SO 순회
        var soList = DataManager.Instance.CharacterDatabase.CharacterUpgradeData.datas;
        foreach (var so in soList)
        {
            table.Add(new UpgradeData
            {
                ID = so.ID,
                GroupID = so.GroupID.ToString(),
                applyType = so.applyType,
                StatType = so.statType,
                Level = so.Level,
                Cost = so.Cost,
                Value = so.Value,
                Probability = so.Probability,
                IsMax = so.IsMax
            });
        }
        return table;
    }
}
