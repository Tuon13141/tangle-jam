using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using System;
using System.Linq;
using Unity.EditorCoroutines.Editor;

public class GoogleSheetLocalizationTool : EditorWindow
{
    private string sheetURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRdqnGy3LJ-oLHaYM-CWXir0uxaQ1f_LSTL6EqGhmtgGUgh7Yo5BsNdADeEy5rlMBN8iwle0J6k41CA/pub?output=csv";
    private string saveFolder = "Assets/Percas/Tools/Localization/Resources";

    [MenuItem("Tools/Localization/Download From Google Sheets")]
    public static void ShowWindow()
    {
        GetWindow<GoogleSheetLocalizationTool>("Localization Downloader");
    }

    void OnGUI()
    {
        GUILayout.Label("Google Sheet Localization Downloader", EditorStyles.boldLabel);
        sheetURL = EditorGUILayout.TextField("Sheet CSV URL", sheetURL);
        saveFolder = EditorGUILayout.TextField("Save Folder", saveFolder);

        if (GUILayout.Button("Download and Save JSONs"))
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(DownloadSheet());
        }
    }

    private IEnumerator<UnityWebRequestAsyncOperation> DownloadSheet()
    {
        UnityWebRequest request = UnityWebRequest.Get(sheetURL);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download sheet: " + request.error);
            yield break;
        }

        string csv = request.downloadHandler.text;
        var data = ParseCSV(csv);

        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        foreach (var lang in data.Keys)
        {
            var langData = new LocalizationData
            {
                items = data[lang].Select(kv => new LocalizationItem { key = kv.Key, value = kv.Value }).ToArray()
            };

            string json = JsonUtility.ToJson(langData, true);
            File.WriteAllText(Path.Combine(saveFolder, $"{lang}.json"), json);
        }

        AssetDatabase.Refresh();
        Debug.Log("Localization JSONs saved successfully.");
    }

    private Dictionary<string, Dictionary<string, string>> ParseCSV(string csv)
    {
        var result = new Dictionary<string, Dictionary<string, string>>();
        var lines = csv.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');
            string key = values[0];

            for (int j = 1; j < headers.Length && j < values.Length; j++)
            {
                string lang = headers[j];
                if (!result.ContainsKey(lang))
                    result[lang] = new Dictionary<string, string>();

                result[lang][key] = values[j];
            }
        }

        return result;
    }

    [Serializable]
    public class LocalizationItem
    {
        public string key;
        public string value;
    }

    [Serializable]
    public class LocalizationData
    {
        public LocalizationItem[] items;
    }
}
