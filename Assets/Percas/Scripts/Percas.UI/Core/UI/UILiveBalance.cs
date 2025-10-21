using System;
using UnityEngine;
using TMPro;
using Percas.Live;
using DG.Tweening;
using Sonat;

namespace Percas.UI
{
    public class UILiveBalance : MonoBehaviour
    {
        [SerializeField] ButtonBase buttonBalance;
        [SerializeField] TMP_Text textLiveBalance;
        [SerializeField] TMP_Text textLiveAmount;
        [SerializeField] GameObject iconInfiniteLive;

        private string textRemainTime;

        public static Action OnScaleLoop;

        private Tween scaleTween;

        private void Awake()
        {
            //LiveManager.OnUpdateLiveBalanceUI += UpdateLiveBalance;
            TimeManager.OnTick += UpdateLiveBalance;
            OnScaleLoop += ScaleLoop;
        }

        private void OnDestroy()
        {
            //LiveManager.OnUpdateLiveBalanceUI -= UpdateLiveBalance;
            TimeManager.OnTick -= UpdateLiveBalance;
            OnScaleLoop -= ScaleLoop;
            scaleTween?.Kill();
            transform.localScale = Vector3.one;
        }

        private void OnDisable()
        {
            scaleTween?.Kill();
            transform.localScale = Vector3.one;
        }

        private void Start()
        {
            UpdateLiveBalance();
            buttonBalance.SetPointerClickEvent(OpenRefillLives);
        }

        private void ScaleLoop()
        {
            Vector3 targetScale = new(1.1f, 1.1f, 1.1f);
            transform.localScale = Vector3.one;
            scaleTween = transform.DOScale(targetScale, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                transform.localScale = Vector3.one;
            });
        }

        private void UpdateLiveBalance()
        {
            if (GameLogic.IsInfiniteLive)
            {
                try
                {
                    TimeSpan? remainTime = LiveManager.InfiniteLivesEndTime - DateTime.UtcNow;
                    if (remainTime?.TotalSeconds > 0)
                    {
                        textLiveBalance.text = Helpers.ConvertTimeToText(remainTime);
                    }
                    else
                    {
                        textLiveBalance.text = $"00:00";
                    }
                }
                catch (Exception) { }
            }
            else
            {
                if (GameLogic.CurrentLive >= LiveManager.MaxLives)
                {
                    textLiveBalance.text = Const.LANG_KEY_FULL_LIVE;
                }
                else
                {
                    try
                    {
                        TimeSpan? remainTime = LiveManager.NextLiveRefillTime - DateTime.UtcNow;
                        if (remainTime?.TotalSeconds > 0)
                        {
                            textRemainTime = string.Format("{0:D2}:{1:D2}", remainTime?.Minutes, remainTime?.Seconds);
                        }
                        else
                        {
                            textRemainTime = $"00:00";
                        }
                    }
                    catch (Exception) { }
                    textLiveBalance.text = textRemainTime;
                }
            }
            textLiveAmount.text = $"{GameLogic.CurrentLive}";
            DisplayInfiniteIcon(GameLogic.IsInfiniteLive);
        }

        private void DisplayInfiniteIcon(bool value)
        {
            iconInfiniteLive.SetActive(value);
            textLiveAmount.gameObject.SetActive(!value);
        }

        private void OpenRefillLives()
        {
            if (UICurrencyManager.BlockBalanceButton) return;

            ServiceLocator.PopupScene.ShowPopup(PopupName.RefillLives);

            string placement;
            if (GameLogic.IsInGame)
            {
                placement = "ingame";
            }
            else
            {
                placement = "home";
            }
            var log = new SonatLogClickIconShortcut()
            {
                mode = PlayMode.classic.ToString(),
                level = GameLogic.CurrentLevel,
                placement = placement,
                shortcut = "live_refill"
            };
            log.Post(logAf: true);
        }
    }
}
