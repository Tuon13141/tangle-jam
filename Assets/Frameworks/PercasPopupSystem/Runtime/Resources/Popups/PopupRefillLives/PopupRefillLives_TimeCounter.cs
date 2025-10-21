using System;
using UnityEngine;
using TMPro;
using Percas.Live;

namespace Percas
{
    public class PopupRefillLives_TimeCounter : MonoBehaviour, IActivatable
    {
        [SerializeField] TMP_Text textTimeCounter;

        private string textRemainTime;

        private void Awake()
        {
            TimeManager.OnTick += UpdateTimeCounter;
        }

        private void OnDestroy()
        {
            TimeManager.OnTick -= UpdateTimeCounter;
        }

        public void Activate()
        {
            UpdateTimeCounter();
        }

        public void Deactivate() { }

        private void UpdateTimeCounter()
        {
            if (GameLogic.CurrentLive >= LiveManager.MaxLives)
            {
                textTimeCounter.text = Const.LANG_KEY_FULL_LIVE;
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
                catch (Exception)
                {
                    textRemainTime = $"---";
                }
                textTimeCounter.text = textRemainTime;
            }
        }
    }
}
