using System;
using UnityEngine;
using Percas.UI;
using Percas.IAR;
using Percas.Data;
using DG.Tweening;

namespace Percas
{
    public class PopupHiddenPicture_ProcessReward : MonoBehaviour, IActivatable
    {
        [SerializeField] int rewardStep;
        [SerializeField] ButtonBase buttonReward;
        [SerializeField] GameObject m_previewReward;
        [SerializeField] GameObject m_receivedIcon;
        [SerializeField] RectTransform m_rectBox;

        public static Action OnHidePreviewReward;
        public static Action<int> OnScale;

        private Tween scaleTween;

        private void Awake()
        {
            OnHidePreviewReward += HidePreviewReward;
            OnScale += ScaleLoop;
        }

        private void OnDestroy()
        {
            OnHidePreviewReward -= HidePreviewReward;
            OnScale -= ScaleLoop;
        }

        public void Activate()
        {
            buttonReward.SetPointerClickEvent(Claim);
            UpdateUI();
        }

        public void Deactivate()
        {
            scaleTween?.Kill();
        }

        private void HidePreviewReward()
        {
            m_previewReward.SetActive(false);
        }

        private void UpdateUI()
        {
            HidePreviewReward();
            m_receivedIcon.SetActive(HiddenPictureManager.Data.IsReceived(rewardStep));
        }

        private void OnClaimReward()
        {
            switch (rewardStep)
            {
                case 3:
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin(20, Vector3.zero, new LogCurrency("currency", "coin", "event_hidden_picture", "non_iap", "feature", "hidden_picture_gift_1")));
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainInfiniteLive(600, Vector3.zero, new LogCurrency("energy", "infinite_live", "event_hidden_picture", "non_iap", "feature", "hidden_picture_gift_1"))); // 600 = 10m
                    RewardGainController.OnStartGaining?.Invoke();
                    break;

                case 6:
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin(20, Vector3.zero, new LogCurrency("currency", "coin", "event_hidden_picture", "non_iap", "feature", "hidden_picture_gift_2")));
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterUndo(2, Vector3.zero, new LogCurrency("booster", $"{BoosterType.Undo}", "event_hidden_picture", "non_iap", "feature", "hidden_picture_gift_2")));
                    RewardGainController.OnStartGaining?.Invoke();
                    break;

                case 9:
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin(50, Vector3.zero, new LogCurrency("currency", "coin", "event_hidden_picture", "non_iap", "feature", "hidden_picture_gift_3")));
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterUndo(2, Vector3.zero, new LogCurrency("booster", $"{BoosterType.Undo}", "event_hidden_picture", "non_iap", "feature", "hidden_picture_gift_3")));
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterAddSlots(1, Vector3.zero, new LogCurrency("booster", $"{BoosterType.AddSlots}", "event_hidden_picture", "non_iap", "feature", "hidden_picture_gift_3")));
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterClear(1, Vector3.zero, new LogCurrency("booster", $"{BoosterType.Clear}", "event_hidden_picture", "non_iap", "feature", "hidden_picture_gift_3")));
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainInfiniteLive(900, Vector3.zero, new LogCurrency("energy", "infinite_live", "event_hidden_picture", "non_iap", "feature", "hidden_picture_gift_3"))); // 900 = 15m
                    RewardGainController.OnStartGaining?.Invoke();
                    break;
            }
        }

        private void Claim()
        {
            if (HiddenPictureManager.Data.UnlockedPieces.Count < rewardStep)
            {
                bool currentStatus = m_previewReward.activeSelf;
                OnHidePreviewReward?.Invoke();
                m_previewReward.SetActive(!currentStatus);
                return;
            }

            if (HiddenPictureManager.Data.IsReceived(rewardStep))
            {
                OnHidePreviewReward?.Invoke();
                return;
            }

            HiddenPictureManager.Data.AddReceivedRewards(rewardStep);
            UpdateUI();

            OnClaimReward();
        }

        private void ScaleLoop(int boxID)
        {
            if (boxID != rewardStep) return;
            scaleTween?.Kill();
            Vector3 targetScale = new(1.1f, 1.1f, 1.1f);
            m_rectBox.localScale = Vector3.one;
            scaleTween = m_rectBox.DOScale(targetScale, 0.25f).SetLoops(6, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                m_rectBox.localScale = Vector3.one;
            });
        }
    }
}
