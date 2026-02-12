using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

[Serializable]
public class CharacterDefinition
{
    public int CharID;
    public string Name;

    public float BaseHunger;
    public float BaseStamina;
    public float BaseMoveSpeed;
    public float BaseFishingSpeed;
    public int BaseDoongDoongStat;

    public int BaseVisualGroupID;
    public int BaseUpgradGroupID;
}
public static class CharacterCsvLoader
{
    public static List<CharacterDefinition> LoadFromResources(string resourcesPath)
    {
        // Data/character
        TextAsset csv = Resources.Load<TextAsset>(resourcesPath);
        if (csv == null)
        {
            Debug.LogError($"CSV not found in Resources: {resourcesPath}");
            return new List<CharacterDefinition>();
        }

        return Parse(csv.text);
    }

    public static List<CharacterDefinition> Parse(string csvText)
    {
        var list = new List<CharacterDefinition>();
        var lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1) return list; 

        var ci = CultureInfo.InvariantCulture;

        for (int i = 1; i < lines.Length; i++) 
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] cols = line.Split(',');

            var def = new CharacterDefinition
            {
                CharID = int.Parse(cols[0]),
                Name = cols[1],
                BaseHunger = float.Parse(cols[2], ci),
                BaseStamina = float.Parse(cols[3], ci),
                BaseMoveSpeed = float.Parse(cols[4], ci),
                BaseFishingSpeed = float.Parse(cols[5], ci),
                BaseDoongDoongStat = int.Parse(cols[6]),
                BaseVisualGroupID = int.Parse(cols[7]),
                BaseUpgradGroupID = int.Parse(cols[8])

            };

            list.Add(def);
        }

        return list;
    }
}