using System;
using System.IO;
using UnityEngine;
using DG.Tweening;

namespace Percas
{
    public static class Helpers
    {
        /// <summary>
        /// A very simple (and not very secure) "encryption" using Base64 encoding.
        /// </summary>
        public static string Encrypt(string input)
        {
            try
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
                return Convert.ToBase64String(bytes);
            }
            catch (Exception)
            {
                return input;
            }
        }

        /// <summary>
        /// Decrypts the Base64-encoded string.
        /// </summary>
        public static string Decrypt(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static void WriteToLocal(string fileName, string fileData)
        {
#if UNITY_EDITOR
            File.WriteAllText($"Assets/Percas/Game/Datas/{fileName}.txt", fileData);
#endif
        }

        public static string ConvertTimeToText(TimeSpan? timeSpan)
        {
            string tmp;
            if (timeSpan?.Days >= 1)
            {
                tmp = string.Format("{0:D2}d {1:D2}h", timeSpan?.Days, timeSpan?.Hours);
            }
            else if (timeSpan?.Hours >= 1)
            {
                tmp = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan?.Hours, timeSpan?.Minutes, timeSpan?.Seconds);
            }
            else
            {
                tmp = string.Format("{0:D2}:{1:D2}", timeSpan?.Minutes, timeSpan?.Seconds);
            }
            return tmp;
        }

        public static string ConvertSecondToText(float seconds)
        {
            string result;

            if (seconds < 3600)
            {
                result = $"{(int)(seconds / 60)} mins";
            }
            else
            {
                result = $"{(int)(seconds / 3600)} hrs";
            }

            return result;
        }

        public static Tweener ChangeValueInt(int startValue, int endValue, float speed, float delay, Action<int> onUpdate)
        {
            return DOTween.To(() => startValue, x => startValue = x, endValue, speed).OnUpdate(delegate
            {
                onUpdate?.Invoke(startValue);
            }).SetDelay(delay);
        }

        private static float screenRatio = 0;
        public static float ScreenRatio
        {
            get
            {
                if (screenRatio == 0)
                {
                    float stdRatio = 1920f / 1080f;
                    screenRatio = (float)Screen.height / Screen.width;
                    screenRatio /= stdRatio;
                }
                return screenRatio < 1 ? 1 : screenRatio;
            }
        }

        #region Int Extensions
        public static void Increase(ref this int value)
        {
            value += 1;
        }

        public static void IncreaseBy(ref this int value, int amount)
        {
            value += amount;
        }

        public static void Set(ref this int value, int defaultValue = 0)
        {
            value = defaultValue;
        }
        #endregion

        public static DateTime StartOfDay(this DateTime theDate)
        {
            return theDate.Date;
        }

        public static DateTime EndOfDay(this DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }
    }
}
