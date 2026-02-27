using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


[Serializable]
public class FishDefinition
{
    public int ID;
    public string FishName_String;        
    public string FishDesc_String;        

    public int FishType;                  
    public int Grade;                     
    public int ArriveSeason;              

    public bool IsSpecial;
    public bool UseCrowdingAlgorithm;

    public float MinLength;
    public float MaxLength;
    public float Measure;

    public int Price;
    public string FishImgPath_Sprite;   
}

public static class FishCsvLoader
{
    public static List<FishDefinition> LoadFromResources(string resourcesPath)
    {
        TextAsset csv = Resources.Load<TextAsset>(resourcesPath);
        if (csv == null)
        {
            Debug.LogError($"CSV not found in Resources: {resourcesPath}");
            return new List<FishDefinition>();
        }
        return Parse(csv.text);
    }

    public static List<FishDefinition> Parse(string csvText)
    {
        var list = new List<FishDefinition>();
        if (string.IsNullOrWhiteSpace(csvText)) return list;

        var lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0) return list;

        var ci = CultureInfo.InvariantCulture;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            // 주석 줄(#)은 스킵
            if (line.StartsWith("#")) continue;

            string[] cols = line.Split(',');
            if (cols.Length < 14) continue;


            if (!int.TryParse(cols[0].Trim(), out int id))
                continue;

            var def = new FishDefinition
            {
                ID = id,

                FishName_String = cols[2].Trim(),
                FishDesc_String = cols[3].Trim(),

                FishType = ToInt(cols[4]),
                Grade = ToInt(cols[5]),
                ArriveSeason = ToInt(cols[6]),

                IsSpecial = ToBool(cols[7]),
                UseCrowdingAlgorithm = ToBool(cols[8]),

                MinLength = ToFloat(cols[9], ci),
                MaxLength = ToFloat(cols[10], ci),
                Measure = ToFloat(cols[11], ci),

                Price = ToInt(cols[12]),
                FishImgPath_Sprite = cols[13].Trim(),
            };

            list.Add(def);
        }

        return list;
    }

    private static int ToInt(string s)
    {
        s = (s ?? "").Trim();
        return int.TryParse(s, out int v) ? v : 0;
    }

    private static float ToFloat(string s, CultureInfo ci)
    {
        s = (s ?? "").Trim();
        return float.TryParse(s, NumberStyles.Float, ci, out float v) ? v : 0f;
    }

    private static bool ToBool(string s)
    {
        s = (s ?? "").Trim();
        if (s.Equals("TRUE", StringComparison.OrdinalIgnoreCase)) return true;
        if (s.Equals("FALSE", StringComparison.OrdinalIgnoreCase)) return false;
        if (s == "1") return true;
        if (s == "0") return false;
        return bool.TryParse(s, out bool b) && b;
    }
}