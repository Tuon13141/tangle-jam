namespace Percas
{
    public static class Const
    {
        #region Game
        public static string APP_NAME = "Thread Frenzy";
        #endregion

        #region Scene
        public static string SCENE_LOADING = "Loading";
        public static string SCENE_HOME = "Home";
        public static string SCENE_GAME = "GamePlay";
        public static string SCENE_GAME_TEST = "GameTest";
        #endregion

        #region Default Value
        public const string USER_TYPE_NEW = "new_user";
        public const string USER_TYPE_EXP = "exp_user";
        public const int MAX_DAILY_SPINS = 3;
        public const int MAX_DAILY_WATCH_ADS = 5;
        #endregion

        #region Data Key
        public static string KEY_PLAYER_DATA = "KEY_PLAYER_DATA";
        public static string KEY_REWARD_DATA = "KEY_REWARD_DATA";
        public static string KEY_IAA_DATA = "KEY_IAA_DATA";
        public static string KEY_BOOSTER_DATA = "KEY_BOOSTER_DATA";
        public static string KEY_LIVE_DATA = "KEY_LIVE_DATA";
        public static string KEY_CONFIG_DATA = "KEY_CONFIG_DATA";
        public static string KEY_DAILY_TIME_RESET = "KEY_DAILY_TIME_RESET";
        public static string KEY_STAR_RUSH_DATA = "KEY_STAR_RUSH_DATA";
        public static string KEY_HIDDEN_PICTURE_DATA = "KEY_HIDDEN_PICTURE_DATA";
        #endregion

        #region Mobile Notifications 2.4.0
        /// <summary>
        /// Need to change Tile City to the name of game
        /// </summary>
        public const string MN_CHANNEL_GROUP_ID = "ThreadFrenzy_Channel_Group_ID";
        public const string MN_CHANNEL_GROUP_NAME = "Thread Frenzy Notifications";
        public const string MN_CHANNEL_ID = "ThreadFrenzy_Channel_ID";
        public const string MN_CHANNEL_NAME = "Thread Frenzy Channel";
        public const string MN_CHANNEL_DESC = "Thread Frenzy Channel Notifications";
        public const string MN_IOS_ID = "threadfrenzy_ios_notification_01";
        public const string MN_IOS_CATEGORY_ID = "threadfrenzy_ios_category_a";
        public const string MN_IOS_THREAD_ID = "threadfrenzy_ios_thread1";
        #endregion

        #region Localization
        public const string LANG_KEY_COMING_SOON = "Coming soon!";
        public const string LANG_KEY_BTN_PLAY = "PLAY {0}";
        public const string LANG_KEY_FULL_LIVE = "FULL";
        public const string LANG_KEY_LACK_COIL = "Lack coils!";
        public const string LANG_KEY_NOT_ENOUGH_COINS = "Not enough coins!";
        public const string LANG_KEY_NOT_ENOUGH_KEY = "Not enough key!";
        public const string LANG_KEY_NOT_FULL_PB = "The Piggy Bank is not full! Keep playing!";
        public const string LANG_KEY_NO_INTERNET = "No Internet Connection!";
        public const string LANG_KEY_WAIT_FOR_SHORT_TIME = "Wait for a short time!";
        public const string LANG_KEY_VIDEO_NOT_READY = "Video is not ready.";
        public const string LANG_KEY_WATCH_TILL_END = "Watch till the end to get rewards!";
        public const string LANG_KEY_SOMETHING_WRONG = "Something went wrong. Try again later!";
        public const string LANG_KEY_UNLOCK_AT_LEVEL = "Unlock at Level {0}";
        public const string LANG_KEY_REVIVE_UNDO = "Undo your last 3 moves and keep playing!";
        public const string LANG_KEY_REVIVE_ADD_SLOTS = "Add a temporary slot and keep playing!";
        public const string LANG_KEY_CANNOT_MOVE = "No way to move!";
        public const string LANG_INVALID_DISPLAY_NAME = "Display name is not valid!";
        public const string LANG_PROFILE_UPDATED = "Profile was updated!";
        public const string LANG_OUT_OF_SPINS = "Out of today spins!";

        // Piggy Bank
        public const string LANG_KEY_FULL_PIGGY_BANK = "It's full! Break it now and collect your savings!";
        public const string LANG_KEY_NOT_FULL_PIGGY_BANK = "Win levels to fill it!";

        // Boosters
        public const string LANG_KEY_CANNOT_UNDO = "No coil to undo!";
        #endregion

        #region Sounds
        public const string SFX_BUTTON_CLICK_ON = "SFX_Click_Forward";
        public const string SFX_BUTTON_CLICK_OUT = "SFX_Click_Backward";
        public const string SFX_COIL_CLICK_ON = "SFX_Tap_Coils";
        public const string SFX_COIL_MERGE = "SFX_Merge";
        public const string SFX_COIL_HIDE = "SFX_Coil_Disappear";
        public const string SFX_BOOSTER_UNDO = "SFX_Thread_Move";
        public const string SFX_BOOSTER_ADD_SLOTS = "SFX_Booster_Add_Clear";
        public const string SFX_BOOSTER_CLEAR = "SFX_Booster_Add_Clear";
        public const string SFX_GAME_LOSE = "SFX_Lose_Screen";
        public const string SFX_GAME_WIN = "SFX_Victory_Screen";
        public const string SFX_COIN_DROP = "SFX_Coin_Drop";
        public const string SFX_COLLECTION_CLICK_ON = "SFX_Thread_Move";
        public const string SFX_COLLECTION_FILL = "SFX_Fill_Collection_Item";
        public const string SFX_COLLECTION_FILL_COMPLETED = "SFX_Fill_Collection_Item_Done";
        public const string SFX_COLLECTION_COMPLETED = "SFX_Fill_Full_Collection";
        public const string SFX_LUCKY_SPIN = "SFX_Lucky_Spin";
        #endregion
    }
}
