using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Percas.Data;
using Percas.UI;
using Percas.IAR;
using System;
using UnityEngine.UI;
using DG.Tweening;
using NaughtyAttributes;

namespace Percas
{
    public class PopupRewardAds : PopupBase
    {
        [Header("UI")]
        [SerializeField] ButtonBase buttonClosePopup;
        [SerializeField] ButtonWatchVideoAd buttonWatchAds;
        [SerializeField] SlicedFilledImage progressWatchAds;
        [SerializeField] RewardInfo rewardInfo;

        [SerializeField] List<TupleSerialize<ButtonBase, GameObject, GameObject>> m_GiftList;

        private void OnStart()
        {
            buttonClosePopup.SetPointerClickEvent(Close);

            buttonWatchAds.skipVideo = false;
            buttonWatchAds.onStart = OnStartWatch;
            buttonWatchAds.onCompleted = OnCompletedWatch;

            UpdateUI(0);
        }


        private void UpdateUI(float usedTween = 0)
        {
            ButtonRewardAds.OnUpdateNoti?.Invoke();

            usedTween = (int)Mathf.Clamp(usedTween, 0, 1);
            var countRewardAds = PlayerDataManager.PlayerData.GetCountRewardAds();

            DOTween.To(() => progressWatchAds.fillAmount,
                        x => progressWatchAds.fillAmount = x,
                        (float)countRewardAds / Const.MAX_DAILY_WATCH_ADS,
                        0.3f * usedTween);


            for (int i = 0; i < m_GiftList.Count; i++)
            {
                var giftTuple = m_GiftList[i];
                giftTuple.Value2.SetActive(false);
                giftTuple.Value3.SetActive(false);

                switch (PlayerDataManager.PlayerData.GetProgressWatchAds(i))
                {
                    case 0:
                        break;

                    case 1:
                        giftTuple.Value2.SetActive(true);
                        giftTuple.Value2.transform.DOScale(1, 0.3f * usedTween).From(0).SetEase(Ease.OutBack);
                        break;

                    case 2:
                        giftTuple.Value1.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
                        giftTuple.Value3.SetActive(true);
                        //giftTuple.Value3.transform.DOScale(1, 0.3f * usedTween).From(0).SetEase(Ease.OutBack);

                        break;
                }
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnCompletedWatch();
            }
        }
#endif

        private void OnHide(Action callback = null)
        {
            Hide();
            //ServiceLocator.PopupScene.HidePopup(PopupName.LuxuryBasketOpen, callback);
        }

        private void Close()
        {
            OnHide();
        }

        private void OnStartWatch(Action<bool> onCallback)
        {
            if (!Kernel.Resolve<AdsManager>().IsVideoAdsReady())
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_VIDEO_NOT_READY);
                onCallback?.Invoke(false);
            }
            if (PlayerDataManager.PlayerData.GetCountRewardAds() >= Const.MAX_DAILY_WATCH_ADS)
            {
                ActionEvent.OnShowToast?.Invoke("Full Progress! Come Back Tomorrow!");
                onCallback?.Invoke(false);
            }
            else
            {
                onCallback?.Invoke(true);
            }
        }

        private void OnCompletedWatch()
        {
            UpdateUI(1);
        }

        #region Public Methods
        [Button]
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            OnStart();
        }

        public void GetReward(int index)
        {
            if (PlayerDataManager.PlayerData.GetProgressWatchAds(index) == 1)
            {
                PlayerDataManager.PlayerData.SetProgressWatchAds(index, 2);

                RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin((2 * index + 1) * 100, Vector3.zero, new LogCurrency("currency", "coin", "video_gifts", "non_iap", "ads", "rwd_ads")));
                RewardGainController.OnStartGaining?.Invoke();

                ButtonRewardAds.OnUpdateNoti?.Invoke();
            }
            else
            {
                rewardInfo.Show();
                rewardInfo.transform.position = m_GiftList[index].Value1.transform.position;
                rewardInfo.SetText((2 * index + 1) * 100);
            }

            UpdateUI(1);
        }
        #endregion
    }
}