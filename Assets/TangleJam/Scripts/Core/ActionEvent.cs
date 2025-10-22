using System;

namespace Tuon
{
    public static class ActionEvent
    {
        public static Action<string> OnPopupOpen;
        public static Action OnPopupClose;
        public static Action OnLastPopupClose;
        public static Action OnSettingClose;
        public static Action OnCallGoogleInAppReviews;
        public static Action<int, Action> OnAutoShowInGame;
        public static Action OnLevelStart;
        public static Action OnLevelWin;
        public static Action OnLevelLose;
        public static Action OnPhaseStart;
        public static Action OnPhaseEnd;
        public static Action OnItemsMatched;
        public static Action OnCoilTap;
        public static Action OnReleasePin;
        public static Action OnNoCoinOnBoard;

        public static Action<string> OnShowToast;
        public static Action<bool> OnBlockNativeAd;

        #region Sounds
        public static Action OnPlaySFXButtonClickOn;
        public static Action OnPlaySFXButtonClickOut;

        public static Action OnPlaySFXCoilClickOn;
        public static Action OnPlaySFXCoilMerge;
        public static Action OnPlaySFXCoilHide;
        public static Action OnPlaySFXBoosterUndo;
        public static Action OnPlaySFXBoosterAddSlots;
        public static Action OnPlaySFXBoosterClear;
        public static Action OnPlaySFXGameLose;
        public static Action OnPlaySFXGameWin;
        public static Action OnPlaySFXCoinDrop;

        public static Action OnPlaySFXCollectionClickOn;
        public static Action OnPlaySFXCollectionFill;
        public static Action OnPlaySFXCollectionFillCompleted;
        public static Action OnPlaySFXCollectionCompleted;

        public static Action OnPlaySFXLuckySpin;
        #endregion
    }
}
