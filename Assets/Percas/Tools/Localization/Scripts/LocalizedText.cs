using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    public string key;
    public object[] dynamicArgs;

    private TMP_Text textComponent;
    private TMP_FontAsset defaultFont;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        defaultFont = textComponent.font; // cache default editor-assigned font
    }

    private void OnEnable()
    {
        LocalizationManager.Instance.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
    }

    public void SetDynamicArgs(params object[] args)
    {
        dynamicArgs = args;
        UpdateText();
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance == null) return;

        // Localize text
        string value = dynamicArgs != null && dynamicArgs.Length > 0
            ? LocalizationManager.Instance.GetFormattedValue(key, dynamicArgs)
            : LocalizationManager.Instance.GetLocalizedValue(key);

        textComponent.text = value;

        // Font: use language-specific or fallback to default
        TMP_FontAsset langFont = LocalizationManager.Instance.GetFontForCurrentLanguage();
        textComponent.font = langFont != null ? langFont : defaultFont;

        // RTL: only if font config supports it
        bool isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
        textComponent.isRightToLeftText = isRTL;
        textComponent.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
    }
}
