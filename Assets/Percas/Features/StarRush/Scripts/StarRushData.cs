using System;
using System.Collections.Generic;
using Percas.Data;
using UnityEngine;

namespace Percas
{
    public enum StarRushStatus
    {
        None,
        InProcess,
        Completed,
    }

    public class StarRushData
    {
        public StarRushStatus Status;
        public string EndTime;
        public string LastTimeScored;
        public int YourLastScore = 0;
        public bool Scored = false; // neu = true: diem gan nhat da duoc ghi nhan (YourLastScore)
        public bool CallToPlay = false;
        public List<StarRushPlayer> Players;

        public int WinCount = 0;

        private readonly List<int> rates1 = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0 };
        private readonly List<int> rates2 = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0 };
        private readonly List<int> rates3 = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 0, 0, 0, 0 };

        public bool CanStart()
        {
            return Status == StarRushStatus.None || string.IsNullOrEmpty(EndTime);
        }

        public bool IsRunning()
        {
            return Status == StarRushStatus.InProcess;
        }

        public bool IsCompleted()
        {
            try
            {
                TimeSpan remainTime = DateTime.UtcNow - TimeHelper.ParseIsoString(EndTime);
                return remainTime.TotalSeconds > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int AutoScoredCount()
        {
            try
            {
                TimeSpan remainTime = DateTime.UtcNow - TimeHelper.ParseIsoString(LastTimeScored);
                int count = (int)(remainTime.TotalSeconds / 600);
                return count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void AdjustScore()
        {
            try
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    StarRushPlayer player = Players[i];
                    if (player.IsYou)
                    {
                        player.Score += AutoScoredCount() >= 1 ? 0 : YourLastScore;
                    }
                    else
                    {
                        if (AutoScoredCount() >= 1)
                        {
                            int scoreBase = AutoScoredCount() >= 1 ? UnityEngine.Random.Range(0, 101) : YourLastScore;
                            List<int> rates = WinCount == 0 ? rates1 : WinCount <= 3 ? rates2 : rates3;
                            player.Score += (int)(scoreBase * rates[UnityEngine.Random.Range(0, rates.Count)] / 10f);

                            //for (int c = 0; c < AutoScoredCount(); c++)
                            //{
                            //}
                        }
                        else
                        {
                            int scoreBase = AutoScoredCount() >= 1 ? UnityEngine.Random.Range(0, 101) : YourLastScore;
                            List<int> rates = WinCount == 0 ? rates1 : WinCount <= 3 ? rates2 : rates3;
                            player.Score += (int)(scoreBase * rates[UnityEngine.Random.Range(0, rates.Count)] / 10f);
                        }
                    }
                }
                Scored = true;
                LastTimeScored = TimeHelper.ToIsoString(DateTime.UtcNow);
            }
            catch (Exception) { }
        }

        public void UpdateYourLastScore(int value)
        {
            YourLastScore = value;
            Scored = false;
            StarRushManager.OnSave?.Invoke();
        }

        public void UpdateWinCount()
        {
            WinCount += 1;
            StarRushManager.OnSave?.Invoke();
        }

        public void Start()
        {
            Status = StarRushStatus.InProcess;
            EndTime = TimeHelper.ToIsoString(DateTime.UtcNow.AddHours(1));
            LastTimeScored = null;
            YourLastScore = 0;
            Scored = false;
            Players = new();
            for (int i = 0; i <= 3; i++)
            {
                Players.Add(new StarRushPlayer(DataManager.Instance.GetRandomPlayerProfile(), false));
            }
            Players.Add(new StarRushPlayer(PlayerDataManager.PlayerData.PlayerProfile, true));
            StarRushManager.OnSave?.Invoke();
        }

        public void Reset()
        {
            Status = StarRushStatus.None;
            EndTime = null;
            LastTimeScored = null;
            Players = null;
            YourLastScore = 0;
            Scored = false;
            CallToPlay = false;
        }
    }

    [Serializable]
    public class StarRushPlayer
    {
        public string PlayerProfile;
        public int Score;
        public bool IsYou;

        public StarRushPlayer(string playerProfile, bool isYou)
        {
            PlayerProfile = playerProfile;
            Score = 0;
            IsYou = isYou;
        }
    }
}
