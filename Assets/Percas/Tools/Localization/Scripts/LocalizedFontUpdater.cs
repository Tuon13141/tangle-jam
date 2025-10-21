using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedFontUpdater : MonoBehaviour
{
    private TMP_Text textComponent;
    private TMP_FontAsset defaultFont;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
        defaultFont = textComponent.font; // fallback
    }

    private void OnEnable()
    {
        LocalizationManager.Instance.OnLanguageChanged += UpdateFont;
        UpdateFont();
    }

    private void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= UpdateFont;
    }

    public void UpdateFont()
    {
        if (LocalizationManager.Instance == null) return;

        // Apply font
        var font = LocalizationManager.Instance.GetFontForCurrentLanguage();
        textComponent.font = font != null ? font : defaultFont;

        // Handle RTL
        bool isRTL = LocalizationManager.Instance.IsCurrentLanguageRTL();
        textComponent.isRightToLeftText = isRTL;

        // Keep alignment smart
        if (isRTL)
        {
            if (IsLeftAligned(textComponent.alignment))
                textComponent.alignment = TextAlignmentOptions.Right;
        }
        else
        {
            if (IsRightAligned(textComponent.alignment))
                textComponent.alignment = TextAlignmentOptions.Left;
        }
    }

    private bool IsLeftAligned(TextAlignmentOptions alignment)
    {
        return alignment == TextAlignmentOptions.Left || alignment == TextAlignmentOptions.TopLeft || alignment == TextAlignmentOptions.BottomLeft;
    }

    private bool IsRightAligned(TextAlignmentOptions alignment)
    {
        return alignment == TextAlignmentOptions.Right || alignment == TextAlignmentOptions.TopRight || alignment == TextAlignmentOptions.BottomRight;
    }
}
