using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
[Serializable]
public class ResourceDefinition
{
    public string ResourceID;   
    public string PrefabPath;   
}
public static class ResourceCsvLoader
{
    public static List<ResourceDefinition> LoadFromResources(string resourcesPath)
    {
        TextAsset csv = Resources.Load<TextAsset>(resourcesPath);
        if (csv == null)
        {
            Debug.LogError($"CSV not found in Resources: {resourcesPath}");
            return new List<ResourceDefinition>();
        }

        return Parse(csv.text);
    }

    public static List<ResourceDefinition> Parse(string csvText)
    {
        var list = new List<ResourceDefinition>();
        var lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1) return list; 

        var ci = CultureInfo.InvariantCulture;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] cols = line.Split(',');

            var def = new ResourceDefinition
            {
                ResourceID = cols[0],
                PrefabPath = cols[1]
            };

            list.Add(def);
        }

        return list;
    }
}