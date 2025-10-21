using System;
using System.Collections;
using UnityEngine;
using Sdk.Google.NativeAds;

namespace Percas.UI
{
    public class GameLoading : MonoBehaviour
    {
        [SerializeField] GameObject gameLoading;
        [SerializeField] GameObject nativeAd;
        [SerializeField] float timeToClose = 1.25f;

        public static Action<Action, bool> OnShow;

        private void Awake()
        {
            OnShow += Show;
        }

        private void OnDestroy()
        {
            OnShow -= Show;
        }

        private void Show(Action onCompleted, bool isHome)
        {
            TrackingManager.OnTrackScreenView?.Invoke(ScreenName.GameLoading.ToString());
            GlobalSetting.ScreenName = ScreenName.GameLoading.ToString();
            gameLoading.SetActive(true);
            nativeAd.SetActive(NativeAdsManager.AdLoaded && NativeAdsManager.AdReady);
            StartCoroutine(OnClose(onCompleted, isHome));
        }

        private IEnumerator OnClose(Action onCompleted, bool isHome)
        {
            yield return new WaitForSeconds(timeToClose);
            gameLoading.SetActive(false);
            onCompleted?.Invoke();
            if (isHome) UIHomeController.OnDisplay?.Invoke(false, false);
        }
    }
}
