using System;
using UnityEngine;
using TMPro;

namespace Percas
{
    public class UITextDailyTimeCounter : MonoBehaviour
    {
        [SerializeField] TMP_Text textTimeCounter;
        [SerializeField] string textString;

        private string textRemainTime;

        private void Awake()
        {
            TimeManager.OnTick += Counter;
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(textString)) textString = "{0}";
        }

        private void OnDestroy()
        {
            TimeManager.OnTick -= Counter;
        }

        private void Counter()
        {
            try
            {
                TimeSpan? remainTime = TimeHelper.ParseIsoString(GameLogic.NextDailyTimeReset) - DateTime.UtcNow;
                if (remainTime?.TotalSeconds > 0)
                {
                    if (remainTime?.TotalHours <= 0)
                    {
                        textRemainTime = string.Format("{0:D2}:{1:D2}", remainTime?.Minutes, remainTime?.Seconds);
                    }
                    else
                    {
                        textRemainTime = string.Format("{0:D2}:{1:D2}:{2:D2}", remainTime?.Hours, remainTime?.Minutes, remainTime?.Seconds);
                    }
                }
                else
                {
                    textRemainTime = null;
                }
            }
            catch (Exception)
            {
                textRemainTime = null;
            }
            textTimeCounter.text = textRemainTime != null ? string.Format(textString, textRemainTime) : $"WATCH A SHORT AD TO GET REWARDS!";
        }
    }
}
