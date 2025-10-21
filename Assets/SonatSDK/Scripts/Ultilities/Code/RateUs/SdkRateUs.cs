using UnityEngine.UI;

namespace SonatSdkUltilities
{
    using System;
    using UnityEngine;

    public class SdkRateUs : MonoBehaviour
    {
        public static SdkRateUs Instance;
        
        
        public ReactiveInt star = new ReactiveInt();
        [SerializeField] private int starOnEnable = 0;

        [SerializeField] private IntScript[] starScripts;
        [SerializeField] private SdkActionScript[] onRateUs;
        [SerializeField] private GameObject rateUs;
        [SerializeField] private GameObject feedBack;

        public Action onCloseAction;
        private void Awake()
        {
            Instance = this;
            star.Subscribe(data =>
            {
                foreach (var starScript in starScripts)
                    starScript.OnChanged(data);
            });
            if(deactivateOnAwake)
                gameObject.SetActive(false);
        }

        [SerializeField] private bool showRateUsOnEnable;
        [SerializeField] private bool deactivateOnAwake = true;
        private void OnEnable()
        {
            star.Value = starOnEnable;
            if(showRateUsOnEnable)
                ShowRateUs();
        }


        protected void HandlerCloseClick()
        {
        }

        public void ChooseStar(int nStar)
        {
            star.Value = nStar;
        }

        public void RateUs()
        {
            foreach (var sdkActionScript in onRateUs)
                sdkActionScript.StartAction(star.Value);
        }

        public void ShowFeedbackForm()
        {
           rateUs.gameObject.SetActive(false);
           feedBack.gameObject.SetActive(true);
        }
        
        [ContextMenu("Show rate us")]
        public void ShowRateUs()
        {
            gameObject.SetActive(true);
            rateUs.gameObject.SetActive(true);
            feedBack.gameObject.SetActive(false);
        }

        [SerializeField] private Button additionalCloseButton;
        public void Close()
        {
            rateUs.gameObject.SetActive(false);
            feedBack.gameObject.SetActive(false);
            if(deactivateOnAwake)
                gameObject.SetActive(false);
            onCloseAction?.Invoke();
            if(additionalCloseButton != null)
                additionalCloseButton.onClick.Invoke();
        }
    }
}