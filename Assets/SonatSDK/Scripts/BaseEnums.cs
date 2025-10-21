using System;

namespace sonat_sdk.Scripts
{
    public enum StateCompareType
    {
        HasKey,
        Exact,
    }

    [Flags]
    public enum SetDataType : byte
    {
        None = 0,
        Set = 1,
        Add = 2,
        Clear = 4,
        ClearAll = 8,
        Tween = 16,
        SetThenTween = 17,
        AddThenTween = 18,
        ClearThenTween = 20,
    }

    public enum PlayerPrefEnum
    {
        test_device_id = 0,
        cache_consent_admob = 1,
        level_difficulty = 2,
        played = 3,
        current_placement = 4,
        passed_level = 5,
    }

    public enum PlayerDataStock
    {
        Block
    }

    public enum BaseFrameworkSharedData
    {
        Zero = 0,
        ProductQuantity = 1,
        ProductIndex = 2,
        ProductAmount = 3,
        ProductParameter1 = 4,
        ProductParameter2 = 5,
        ProgressStart = 6,
        ProgressEnd = 7,
        ProgressFrom = 8,
        ProgressTo = 9,
        Star = 10,
        RewardQuantity = 11,
    }

    public enum BaseFrameworkSharedObject
    {
        Product1 = 0,
        Product2 = 1,
        Product3 = 2,
        ProductAmount = 3,
        ProductParameter1 = 4,
        ProductParameter2 = 5,
        ProgressStart = 6,
        ProgressEnd = 7,
        ProgressFrom = 8,
        ProgressTo = 9,
        Star = 10,
        RewardQuantity = 11,
    }




    [Flags]
    public enum RewardTypeFlag
    {
        // Decimal     // Binary
        UseDefault = 0,
        ClaimInstantInPopup = 1,
        ClaimMultipleWithAdsPopup = 2,
        ClaimQuietly = 4,
        IsReward = 8,
        IsOpenBox = 16,
        IsCurrencyAnim = 32,
        CustomClaimScript = 64,
        ClaimInDialogAddCurrency = 128,
    }


    public enum BaseGlobalError
    {
        PlayFabNameTaken = 0
    }

    [Flags]
    public enum RewardState : byte
    {
        NotAvailable = 0, // 000000
        InProgress = 1, // 000001
        Ready = 2, // 000010
        Taken = 4, // 000100
    }

    public enum LogEachGameEnum
    {
        time_play = 0,
        moves = 1,
        video_count = 2,
        interstitial_count = 3,
        continue_times = 4

//    view_incentive_ads = 5,
//    move_count_by_video_ads = 5,
//    move_count_by_interstial_ads = 5,
    }

    public enum LogAllTimeEnum
    {
        video_count = 0,
        interstitial_count = 1,
        paid_ad_count = 2,
    }


    public enum BaseCustomSessionDataProperty
    {
        ShopTab = 0,
        QuestTab = 1,
        CharacterInfoTab = 2,
        UpgradeTab = 3,
    }

    public enum BaseCustomSessionParameterProperty
    {
        ArrayParameter0 = 0,
        ArrayParameter1 = 1,
        ArrayParameter2 = 2,
    }

    public enum BaseQuestTabEnum
    {
        DailyQuest = 0,
        Achievement = 1,
    }

    public enum StockType
    {
        None,
        Daily,
        LifeTime,
        Custom,
        Subscription,
        Weekly
    }

    public enum BaseLogItemType
    {
        undefined = 0,
        rewarded_ads = 1,
        iap = 2,
        exchange = 3,
        gold = 4,
        diamond = 5,
        daily_gift = 6,
        achievement = 7,
        quest = 8,
        complete_level = 9,
        subscription = 10,
        heart = 11,
        spin = 12,
        booster = 13,
    }

    public enum BaseLogPlacement
    {
        Undefined = 0,
        Shop = 1,
        Spin = 2,
        DailyReward = 3,
        Achievement = 4,
        Quest = 5,
        CompleteLevel = 6,
        Subscription = 7,
        Home = 8,
        GamePlay = 9,
        Event = 10,
    }

    public enum BaseGlobalGameEffect
    {
        None = 0,
        ShakeCamera = 1,
    }

    public enum ScreenEnum
    {
        ByName = -1,
        Home = 0,
        Loading = 1,
        PlayScreen = 2,
        LevelSelect = 3,
        ShopScreen = 4,
    }

    [Flags]
    public enum LevelSelectState
    {
        // Decimal     // Binary
        None = 0, // 000000
        Locked = 1, // 000001
        Unlocked = 2, // 000010
        Passed = 4, // 000100
        Current = 8, // 001000
        Indicate = 16, // 010000,
        ReadyToUnlock = 32,
    }

    [Flags]
    public enum UnlockEquipState
    {
        // Decimal     // Binary
        None = 0, // 000000
        Locked = 1, // 000001
        Unlocked = 2, // 000010
        Equipped = 4, // 000100
        Current = 8, // 001000
        Indicate = 16, // 010000
        ReadyToUnlock = 32,
    }

    public enum baseUnlockEquipDataEnum
    {
        Skin = 0,
        Environment = 1,
        Upgrade = 2,
        Other = 3,
    }

    public enum UnlockEquipConditionEnum
    {
        None = -1,
        GameControllerCalculate = 0,
        FinishLevel = 1,
        AlwaysUnlocked = 2,
        Shopping = 3,
        BuilderFinish = 4,
    }

    public enum ForRunType
    {
        ForEachAll,
        StopWhenTrue,
        StopWhenFalse,
    }

    public enum LogType
    {
        Bypass,
        Ads,
        Service,
        PopupAndScreen,
    }

    public enum CustomEvent
    {
        DialogAddGemOpen,
    }

    public enum Quantity
    {
        None = -1,
        Gold = 0,
        Diamond = 1,
        Energy = 3,
        Spin = 4,
        Medal = 5,
        Item = 6,
        CustomProperty = 7,
        CustomProduct = 8,
        CustomDailyProperty = 9,
        CustomTimeTick = 10,
        EquipData = 11,
        EventItem = 12,
        Stock = 13,

//
        WatchAds = 20,
        IapBuying = 21,
        Subscription = 22,
        Achievement = 23,
        Quest = 24,

//
        Free = 100,
        Random = 101,

        // event from 200
        SystemAction = 200,
        GameAction = 201,
        SystemGameAction = 202,
        TutorialSkip = 300,
        CustomQuantity = 301,
    }

    public enum BaseCustomQuantity
    {
        DailyGift,
        WatchAds,
    }

    public enum EventDayType
    {
        Default = 0,
        Special = 1,
    }

    public enum BaseCustomTimeTick
    {
        CountDownGiveEnergy,
        CountDownUnlimitedLives,
        CountDownDoubleReward,
    }


    public enum ProductSystemAction
    {
        NoAds = 0,
        DoubleReward = 1,
        Spin = 2,
        OpenBox = 3,
    }

    public enum BaseQuestEnum
    {
        WatchAds = 0,
        SpendGold = 1,
        SpendGem = 2,
        CompleteQuest = 3,
        AcquiredStar = 4,
        FinishLevel = 5,
        CollectStar = 6,
    }

    public enum BaseShopItemKey
    {
        NoAds = 0,
    }

    public enum InScreenPopupEnum
    {
        Close = -1,
        Pause = 0,
        Setting = 1,
        Shop = 2,
        Policy = 3,
        RateUs = 4,
        DailyQuest = 5,
        DailyReward = 6,
        Profile = 7,
        LeaderBoard = 8,
        SkinPopup = 9,
        Achievement = 10,
        Spin = 11,
        ReceiveReward = 12,
        OpenBox = 13,
        ThankYouRateUs = 14,
        GameOver = 15,
        VIP = 16,
        Quest = 17,
        Test = 18,
        CustomByIndex = 19,
        BoosterEquip = 20,
        Pack = 21,
        QuitGame = 22,
        NoAds = 23,
        ComingSoon = 24,
        Challenger = 25,
        Inform = 26,
        Congratulation = 27,
        PopupScreen = 28,
        Win = 29,
    }

    [Flags]
    public enum ProgressState
    {
        // Decimal     // Binary
        NotActive = 0, // 000000
        InProgress = 1, // 000001
        Ready = 2, // 000010
        Taken = 4, // 000100
        CompleteAll = 8 // 001000
    }


    [Flags]
    public enum QuestState
    {
        // Decimal     // Binary
        None = 0, // 000000
        Waiting = 1, // 000001
        InProgress = 2, // 000010
        Complete = 4, // 000100
        Taken = 8 // 001000
    }


    public enum PointType
    {
        Square,
        HexaHorizontal,
        HexaVertical,
        Triangle,
        HexaHorizontal2,
        HexaVertical2,
    }

    public enum ScrollAlign
    {
        TopDown,
        BottomUp,
        LeftRight,
    }


    public enum PointAlign
    {
        Center,
        BottomLeft,
    }

    public enum PointMoveType
    {
        Instant,
        FromCurrentPosition,
        Normal,
    }

    /// 1 == top, 2 == right, 4 == bottom, 8 == left
    [Flags]
    public enum PointDirection : byte
    {
        None = 0,
        Top = 1,
        Right = 2,
        Bottom = 4,
        Left = 8,

        Vertical = 5,
        Horizontal = 10,
        TopRight = 3,
        BottomRight = 6,
        TopLeft = 9,
        BottomLeft = 12,
        All4Direction = 15,
    };

    public enum PointDirectionBase
    {
        Undefined = -1,
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3,
    };


    public enum MyEnumCondition
    {
        Greater,
        GreaterOrEqual,
        Equal,
        LessOrEqual,
        Less,
        True,
        False,
        NotNull,
        Null,
        Match,
        Mod,
        DivideFloorToInt
    }
}