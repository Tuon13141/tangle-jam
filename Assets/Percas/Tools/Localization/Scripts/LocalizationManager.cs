using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

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

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    public string defaultLanguage = "en";

    private string currentLanguage;
    private Dictionary<string, string> localizedText = new();

    public event Action OnLanguageChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLocalizedText(GetSystemLanguageCode());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string GetSystemLanguageCode()
    {
        SystemLanguage sysLang = Application.systemLanguage;
        string langCode = LanguageToCode(sysLang);

        // Check if file exists, else fallback to default
        if (Resources.Load<TextAsset>($"{langCode}") != null)
        {
            return langCode;
        }

        return defaultLanguage;
    }

    private string LanguageToCode(SystemLanguage lang)
    {
        return lang switch
        {
            SystemLanguage.French => "fr",
            SystemLanguage.German => "de",
            SystemLanguage.Spanish => "es",
            SystemLanguage.Italian => "it",
            SystemLanguage.Russian => "ru",
            SystemLanguage.Japanese => "ja",
            SystemLanguage.Korean => "ko",
            SystemLanguage.ChineseSimplified => "zh",
            SystemLanguage.ChineseTraditional => "zh-TW",
            SystemLanguage.Portuguese => "pt",
            _ => "en"
        };
    }

    public void LoadLocalizedText(string languageCode)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"{languageCode}");
        if (textAsset == null)
        {
            Debug.LogWarning($"Localization file not found: {languageCode}, falling back to {defaultLanguage}");
            textAsset = Resources.Load<TextAsset>($"{defaultLanguage}");
        }

        LocalizationData data = JsonUtility.FromJson<LocalizationData>(textAsset.text);
        localizedText.Clear();
        foreach (var item in data.items)
            localizedText[item.key] = item.value;

        currentLanguage = languageCode;
        OnLanguageChanged?.Invoke();
    }

    public string GetLocalizedValue(string key)
    {
        return localizedText.TryGetValue(key, out string value) ? value : key;
    }

    public void SetLanguage(string languageCode)
    {
        LoadLocalizedText(languageCode);
    }

    public string GetCurrentLanguage() => currentLanguage;

    public LanguageFontConfig fontConfig;

    public TMP_FontAsset GetFontForCurrentLanguage()
    {
        return fontConfig != null ? fontConfig.GetFont(currentLanguage) : null;
    }

    public bool IsCurrentLanguageRTL()
    {
        return fontConfig != null && fontConfig.IsRTL(currentLanguage);
    }

    public string GetFormattedValue(string key, params object[] args)
    {
        string raw = GetLocalizedValue(key);
        try
        {
            return string.Format(raw, args);
        }
        catch (FormatException e)
        {
            Debug.LogWarning($"Localization format error for key '{key}': {e.Message}");
            return raw;
        }
    }
}
