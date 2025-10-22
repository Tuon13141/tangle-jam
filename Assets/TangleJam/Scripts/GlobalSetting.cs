using Percas;
using Percas.UI;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tuon
{
    public class GlobalSetting : MonoBehaviour
    {
        public static Action<Action> OnHomeToGame;
        public static Action<Action> OnGameToHome;

        public static bool NotiPiggyBankSeen { get; set; }
        public static bool NotiHeartOffersSeen { get; set; }
        public static bool IsFirstOpen { get; set; }
        public static bool IsJustPlay { get; set; }

        public static string ScreenName { get; set; }
        public static PlayMode PlayMode { get; set; }
        public static LevelAsset HiddenPictureLevelData { get; set; }
        public static int HiddenPictureLevelIndex { get; set; }

        private void Awake()
        {
            OnHomeToGame += HomeToGame;
            OnGameToHome += GameToHome;

#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            Input.multiTouchEnabled = false;
            Debug.unityLogger.logEnabled = GameConfig.Instance.DebugOn;
#endif
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            OnHomeToGame -= HomeToGame;
            OnGameToHome -= GameToHome;
        }

        private void HomeToGame(Action onCallback = null)
        {
            AsyncOperation operation;
            operation = SceneManager.LoadSceneAsync(Const.SCENE_GAME);
            GameLoading.OnShow?.Invoke(onCallback, false);
        }

        private void GameToHome(Action onCallback = null)
        {
            AsyncOperation operation;
            operation = SceneManager.LoadSceneAsync(Const.SCENE_HOME);
            GameLoading.OnShow?.Invoke(onCallback, true);
        }

        #region Public Methods
        public static void ToggleDebugConsole()
        {
            //
        }

        public static void SetPlayMode(PlayMode playMode)
        {
            PlayMode = playMode;
        }
        #endregion
    }
}
