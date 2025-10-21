using UnityEngine;
using TMPro;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LanguageFontConfig", menuName = "Localization/Language Font Config")]
public class LanguageFontConfig : ScriptableObject
{
    [System.Serializable]
    public class LanguageFontPair
    {
        public string languageCode;
        public TMP_FontAsset font;
        public bool isRTL;
    }

    public List<LanguageFontPair> fonts;

    private Dictionary<string, LanguageFontPair> fontMap;

    public TMP_FontAsset GetFont(string langCode)
    {
        EnsureMap();
        return fontMap.TryGetValue(langCode, out var pair) ? pair.font : null;
    }

    public bool IsRTL(string langCode)
    {
        EnsureMap();
        return fontMap.TryGetValue(langCode, out var pair) && pair.isRTL;
    }

    private void EnsureMap()
    {
        if (fontMap != null) return;
        fontMap = new Dictionary<string, LanguageFontPair>();
        foreach (var pair in fonts)
            fontMap[pair.languageCode] = pair;
    }
}
