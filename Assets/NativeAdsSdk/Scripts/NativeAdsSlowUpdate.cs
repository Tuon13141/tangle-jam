using System;
using UnityEngine;

namespace Sdk.Google.NativeAds
{
    [Serializable]
    public class NativeAdsSlowUpdate
    {
        public float curTime;
        public float maxTime;
        public bool isBusy;
        private Action onComplete;
        [SerializeField] private bool unscaledTime;

        public NativeAdsSlowUpdate(float curTime, float maxTime, Action onComplete, bool unscaledTime = false)
        {
            this.curTime = curTime;
            this.maxTime = maxTime;
            this.onComplete = onComplete;
            this.unscaledTime = unscaledTime;
        }

        public void Update()
        {
            if (curTime >= 0)
            {
                if (unscaledTime)
                {
                    curTime -= Time.unscaledDeltaTime;
                }
                else
                {
                    curTime -= Time.deltaTime;
                }
                isBusy = true;
                if (curTime < 0)
                {
                    isBusy = false;
                    onComplete?.Invoke();
                }
            }
        }

        public void ResetTime()
        {
            curTime = maxTime;
        }

        public void Cancel()
        {
            curTime = -1;
            isBusy = false;
        }
    }
}
