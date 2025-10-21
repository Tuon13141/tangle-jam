

#if UNITY_EDITOR
using static UnityEditor.EditorPrefs;

#endif


public static class TestingSettings
{
        public static bool TurnOffBanner
    {
#if UNITY_EDITOR
        get => GetBool(nameof(TurnOffBanner));
        set => SetBool(nameof(TurnOffBanner), value);
#else
        get; set;
#endif
    }
    
    public static bool TurnOffOnFocusAds
    {
#if UNITY_EDITOR
        get => GetBool(nameof(TurnOffOnFocusAds));
        set => SetBool(nameof(TurnOffOnFocusAds), value);
#else
        get; set;
#endif
    }
    
    public static bool SkipRewarded
    {
#if UNITY_EDITOR
        get => GetBool(nameof(SkipRewarded));
        set => SetBool(nameof(SkipRewarded), value);
#else
        get; set;
#endif
    }
    
    public static bool InternetConnected
    {
#if UNITY_EDITOR
            get => GetBool(nameof(InternetConnected),true);
            set => SetBool(nameof(InternetConnected), value);
#else
        get; set;
#endif
    }
    
    public static bool DebugBreak
    {
#if UNITY_EDITOR
            get => GetBool(nameof(DebugBreak),false);
            set => SetBool(nameof(DebugBreak), value);
#else
        get; set;
#endif
    }
    
    public static bool DebugBreak1
    {
#if UNITY_EDITOR
        get => GetBool(nameof(DebugBreak1),false);
        set => SetBool(nameof(DebugBreak1), value);
#else
        get; set;
#endif
    }
    
    public static int EpochDayAdd
    {
#if UNITY_EDITOR
        get => GetInt(nameof(EpochDayAdd));
        set => SetInt(nameof(EpochDayAdd), value);
#else
        get; set;
#endif
    }
    
    public static int Param1
    {
#if UNITY_EDITOR
        get => GetInt(nameof(Param1));
        set => SetInt(nameof(Param1), value);
#else
        get; set;
#endif
    }

    
    public static int DebugLevel
    {
#if UNITY_EDITOR
            get => GetInt(nameof(DebugLevel));
            set => SetInt(nameof(DebugLevel), value);
#else
        get; set;
#endif
    }

    public static bool CopyPaste
    {
#if UNITY_EDITOR
        get => GetBool(nameof(CopyPaste));
        set => SetBool(nameof(CopyPaste), value);
#else
        get; set;
#endif
    }

    public static bool DrawPointMap
    {
#if UNITY_EDITOR
            get => GetBool(nameof(DrawPointMap), true);
            set => SetBool(nameof(DrawPointMap), value);
#else
        get; set;
#endif
    }



    public static bool UiLogEnable
    {
#if UNITY_EDITOR
            get => GetBool(nameof(UiLogEnable), true);
            set => SetBool(nameof(UiLogEnable), value);
#else
        get; set;
#endif
    }

    public static bool AlwaysNewDay
    {
#if UNITY_EDITOR
            get => GetBool(nameof(AlwaysNewDay),true);
            set => SetBool(nameof(AlwaysNewDay), value);
#else
        get; set;
#endif
    }

}
