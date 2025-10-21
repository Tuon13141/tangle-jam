using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace Percas.UI
{
    public class AdLoading : MonoBehaviour
    {
        [SerializeField] GameObject adLoading;
        [SerializeField] TMP_Text textMessage;
        [SerializeField] float timeToClose = 1.25f;

        public static Action<Action, bool> OnShow;
        public static Action<float, Action> OnLoad;
        public static Action<bool> OnDisplay;

        private void Awake()
        {
            OnShow += Show;
            OnLoad += Load;
            OnDisplay += Display;
        }

        private void OnDestroy()
        {
            OnShow -= Show;
            OnLoad -= Load;
            OnDisplay -= Display;
        }

        private void Show(Action onCompleted, bool hasReward)
        {
            TrackingManager.OnTrackScreenView?.Invoke(ScreenName.AdLoading.ToString());
            GlobalSetting.ScreenName = ScreenName.AdLoading.ToString();
            adLoading.SetActive(true);
            textMessage.text = hasReward ? $"<size=64>Take a Break!</size><br><sprite=0> X5" : $"Take a break!";
            StartCoroutine(OnClose(onCompleted));
        }

        private void Load(float timeToClose, Action onCompleted)
        {
            this.timeToClose = timeToClose;
            adLoading.SetActive(true);
            textMessage.text = $"<size=48>Loading Data</size>";
            StartCoroutine(OnClose(onCompleted));
        }

        private IEnumerator OnClose(Action onCompleted)
        {
            yield return new WaitForSeconds(timeToClose);
            adLoading.SetActive(false);
            onCompleted?.Invoke();
        }

        private void Display(bool value)
        {
            if (value)
            {
                textMessage.text = $"<size=48>In Processing</size>";
                adLoading.SetActive(true);
            }
            else
            {
                adLoading.SetActive(false);
            }
        }
    }
}
