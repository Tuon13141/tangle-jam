using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Percas.Data;
using Sdk.Google.NativeAds;
using PercasSDK;

namespace Percas
{
    public class LoadingController : MonoBehaviour
    {
        [SerializeField] TMP_Text textLoadingValue;
        [SerializeField] GameObject bg1, bg2;

        private bool loading = true;

        private void Start()
        {
            StartCoroutine(LoadSceneAsync());
        }

        private IEnumerator LoadSceneAsync()
        {
            bg1.SetActive(true);
            bg2.SetActive(false);

            // wait to load all datas
            yield return new WaitUntil(() => PlayerDataManager.LoadDataDone
                && Live.LiveManager.LoadDataDone
                && BoosterManager.LoadDataDone
                && GameConfig.LoadDataDone
                && HiddenPictureManager.LoadDataDone
                && IAA.IAAManager.LoadDataDone
                && IAR.RewardManager.LoadDataDone
                && StarRushManager.LoadDataDone
                && RemoteConfigManager.LoadDataDone);

            TrackingManager.OnTrackScreenView?.Invoke(ScreenName.SceneLoading.ToString());
            GlobalSetting.ScreenName = ScreenName.SceneLoading.ToString();

            float timeOut = 3f;
            AsyncOperation operation;
            IAA.IAAManager.CanShowAppOpen = true;

            Debug.Log($"abcabc: {Static.isActiveNewMode}");
            if (Static.isActiveNewMode && !Static.isPlayDoneNewMode)
            {
                operation = SceneManager.LoadSceneAsync("GamePlayScene");
            }

            else if (GameLogic.CurrentLevel < GameLogic.LevelUnlockHome)
            {
                GlobalSetting.IsFirstOpen = true;
                PlayerDataManager.SetContinueWith(null);
                GlobalSetting.SetPlayMode(PlayMode.classic);
                operation = SceneManager.LoadSceneAsync(Const.SCENE_GAME);
            }
            else
            {
                GlobalSetting.IsFirstOpen = true;
                operation = SceneManager.LoadSceneAsync(Const.SCENE_HOME);
            }
            operation.allowSceneActivation = false;
            while (!operation.isDone)
            {
                int value = 0;
                while (value <= 101)
                {
                    value++;
                    if (value >= 101) loading = false;
                    if (loading) OnUpdate(value);
                    yield return new WaitForSeconds(0.01f);
                }
                bg1.SetActive(false);
                bg2.SetActive(true);
#if use_admob
                yield return new WaitUntil(() => Kernel.Resolve<AdsManager>().IsAppOpenAdsReady() || (timeOut -= Time.deltaTime) <= 0);
                try
                {
                    NativeAdsManager.Instance.LoadNative();
                }
                catch (Exception) { }
#endif

#if use_max
                yield return new WaitUntil(() => IAA.IAAManager.IsAppOpenReady || (timeOut -= Time.deltaTime) <= 0);
#endif
                AudioController.Instance.PlayGameMusic();
                TimeManager.OnResetDailyTime?.Invoke();
                PlayerDataManager.PlayerData.AddSessionStartCount();
                operation.allowSceneActivation = true;
            }
        }

        private void OnUpdate(int progressValue)
        {
            textLoadingValue.text = $"Loading {Mathf.Clamp(progressValue, 1, 100)}%";
        }
    }
}
