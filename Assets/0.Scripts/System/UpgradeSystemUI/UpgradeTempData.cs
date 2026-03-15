using System.Collections.Generic;
using UnityEngine;
// SO 기반 업그레이드 데이터 로더
public static class UpgradeTempData
{
    public static List<UpgradeData> GetAll()
    {
        List<UpgradeData> table = new List<UpgradeData>();
 
        // TODO: 0레벨 하드코딩, 데이터테이블에 추가되면 삭제 예정 
        table.Add(new UpgradeData { ID = 200010100, GroupID = "501", applyType = (ApplyType)1, StatType = (StatType)1, Level = 0, Cost = 1, Value = 0f, Probability = 1f, IsMax = false });
        table.Add(new UpgradeData { ID = 200010200, GroupID = "501", applyType = (ApplyType)1, StatType = (StatType)2, Level = 0, Cost = 2, Value = 0f, Probability = 1f, IsMax = false });
        table.Add(new UpgradeData { ID = 200010300, GroupID = "501", applyType = (ApplyType)2, StatType = (StatType)3, Level = 0, Cost = 3, Value = 1.00f, Probability = 1f, IsMax = false });
        table.Add(new UpgradeData { ID = 200010400, GroupID = "501", applyType = (ApplyType)2, StatType = (StatType)4, Level = 0, Cost = 4, Value = 1.00f, Probability = 1f, IsMax = false });
        table.Add(new UpgradeData { ID = 200010500, GroupID = "501", applyType = (ApplyType)2, StatType = (StatType)5, Level = 0, Cost = 5, Value = 0.02f, Probability = 1f, IsMax = false });

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
