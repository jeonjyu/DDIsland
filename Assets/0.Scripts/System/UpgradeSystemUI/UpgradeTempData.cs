using System.Collections.Generic;

// 임시 더미 데이터 테이블 (나중에 CSV 파서로 교체)
public static class UpgradeTempData
{
    public static List<UpgradeData> GetAll()
    {
        List<UpgradeData> table = new List<UpgradeData>();

        // 포만감 (Add, StatType=1, 레벨 10, Value=증가량 +10)
        AddRange(table, (ApplyType)1, (StatType)1, 200010101,
            new int[] { 300, 450, 650, 900, 1200, 1550, 2000, 2500, 3050, 3700 },
            10f);

        // 스태미너 (Add, StatType=2, 레벨 10, Value=증가량 +10)
        AddRange(table, (ApplyType)1, (StatType)2, 200010201,
            new int[] { 300, 450, 650, 900, 1200, 1550, 2000, 2500, 3050, 3700 },
            10f);

        // 이동속도 (Set, StatType=3, 레벨 10, Value=최종값)
        AddRange(table, (ApplyType)2, (StatType)3, 200010301,
            new int[] { 300, 450, 650, 900, 1200, 1550, 2000, 2500, 3050, 3700 },
            0f, new float[] { 1.01f, 1.02f, 1.03f, 1.04f, 1.05f, 1.06f, 1.07f, 1.08f, 1.09f, 1.10f });

        // 낚시 숙련도 (Set, StatType=4, 레벨 10, Value=최종값)
        AddRange(table, (ApplyType)2, (StatType)4, 200010401,
            new int[] { 300, 450, 650, 900, 1200, 1550, 2000, 2500, 3050, 3700 },
            0f, new float[] { 1.01f, 1.02f, 1.03f, 1.04f, 1.05f, 1.06f, 1.07f, 1.08f, 1.09f, 1.10f });

        // 휴식속도 (Set, StatType=5, 레벨 3, Value=최종값)
        AddRange(table, (ApplyType)2, (StatType)5, 200010501,
            new int[] { 5000, 15000, 50000 },
            0f, new float[] { 0.03f, 0.04f, 0.05f });

        return table;
    }

    // 헬퍼: 레벨 데이터 한번에 추가
    static void AddRange(List<UpgradeData> table, ApplyType applyType, StatType statType,
        int startID, int[] costs, float fixedValue, float[] levelValues = null)
    {
        int totalLevels = costs.Length;
        for (int i = 0; i < totalLevels; i++)
        {
            table.Add(new UpgradeData
            {
                ID = startID + i,
                GroupID = "501",
                applyType = applyType,
                StatType = statType,
                Level = i + 1,
                Cost = costs[i],
                Value = (levelValues != null) ? levelValues[i] : fixedValue,
                Probability = 1f,
                IsMax = (i == totalLevels - 1)
            });
        }
    }
  
}
