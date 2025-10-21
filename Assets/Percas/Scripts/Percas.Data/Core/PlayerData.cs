using System;
using System.Collections.Generic;

namespace Percas.Data
{
    public class PictureQuote
    {
        public int Level;
        public int QuoteIndex;

        public PictureQuote(int level, int quote)
        {
            Level = level;
            QuoteIndex = quote;
        }
    }

    public class PlayerData
    {
        // In Game Datas
        public string UserTyper;
        public int SessionStartCount = 0;
        public int VideoWinCount = 0;
        public int VideoReviveCount = 0;
        public int HighestLevel = 0;
        public int WinStreak = 0;
        public int LoseStreak = 0;
        public int LevelAttempts = 0;
        public int Coin = 200;
        public int CoinInPiggyBank = 0;
        public int Coil = 0;
        public int Pin = 0;
        public int LuxuryBasketOpenedCount = 0;
        public string ContinueWith;
        public List<int> CollectionProgress = new();
        public List<int> PictureGained = new();
        public List<int> PictureCoinReceived = new();
        public List<string> PictureQuotes = new();

        public void AddSessionStartCount()
        {
            SessionStartCount += 1;
            PlayerDataManager.OnSave?.Invoke();
        }

        public void AddVideoWinCount()
        {
            VideoWinCount += 1;
            PlayerDataManager.OnSave?.Invoke();
        }

        public void AddVideoReviveCount()
        {
            VideoReviveCount += 1;
            PlayerDataManager.OnSave?.Invoke();
        }

        #region Player Profile
        public string PlayerProfile;
        public List<int> CoinAvatarUnlocked = new();
        public List<int> CoinFrameUnlocked = new();

        public bool IsUnlockedCoinAvatar(int avatarID)
        {
            return CoinAvatarUnlocked.Contains(avatarID);
        }

        public void AddCoinAvatar(int avatarID)
        {
            if (IsUnlockedCoinAvatar(avatarID)) return;
            CoinAvatarUnlocked.Add(avatarID);
            PlayerDataManager.OnSave?.Invoke();
        }

        public bool IsUnlockedCoinFrame(int frameID)
        {
            return CoinFrameUnlocked.Contains(frameID);
        }

        public void AddCoinFrame(int frameID)
        {
            if (IsUnlockedCoinFrame(frameID)) return;
            CoinFrameUnlocked.Add(frameID);
            PlayerDataManager.OnSave?.Invoke();
        }

        public void SavePlayerProfile(string value)
        {
            PlayerProfile = value;
            PlayerDataManager.OnSave?.Invoke();
        }

        public string GetPlayerName(string playerProfile = null)
        {
            string result;
            try
            {
                if (string.IsNullOrEmpty(playerProfile)) playerProfile = PlayerProfile;

                if (string.IsNullOrEmpty(playerProfile) || playerProfile.Length <= 6)
                {
                    string defaultPlayerProfile = $"000000Player #{UnityEngine.Random.Range(1000, 10000)}";
                    PlayerProfile = defaultPlayerProfile;
                    PlayerDataManager.OnSave?.Invoke();
                    result = PlayerProfile;
                }
                else
                {
                    result = playerProfile;
                }
            }
            catch (Exception)
            {
                result = $"000000Player #{UnityEngine.Random.Range(1000, 10000)}";
            }
            return result[6..];
        }

        public int GetAvatarID(string playerProfile = null)
        {
            int result = 0;
            try
            {
                if (string.IsNullOrEmpty(playerProfile)) playerProfile = PlayerProfile;
                result = int.Parse(playerProfile[..2]);
            }
            catch (Exception) { }
            return result;
        }

        public int GetFrameID(string playerProfile = null)
        {
            int result = 0;
            try
            {
                if (string.IsNullOrEmpty(playerProfile)) playerProfile = PlayerProfile;
                result = int.Parse(playerProfile.Substring(2, 2));
            }
            catch (Exception) { }
            return result;
        }

        public int GetBadgeID(string playerProfile = null)
        {
            int result = 0;
            try
            {
                if (string.IsNullOrEmpty(playerProfile)) playerProfile = PlayerProfile;
                result = int.Parse(playerProfile.Substring(4, 2));
            }
            catch (Exception) { }
            return result;
        }
        #endregion

        #region Tutorials
        public bool IntroToBuilding = false;
        public bool IntroToDailyReward = false;
        public bool IntroToCollectionTutorial = false;
        public bool IntroToCollectionDemo = false;
        public bool IntroToLuckySpin = false;
        public bool PreIntroToLuxuryBasket = false;
        public bool IntroToLuxuryBasket = false;
        public bool IntroToLuxuryBasketTutorial = false;
        public bool IntroToStarRushTutorial = false;
        public bool IntroToHiddenPictureHelp = false;
        public bool IntroToHiddenPictureTutorial = false;
        public bool IntroNoCoinOnBoard = false;
        public bool GameRated = false;
        public int GameRatedValue = 0;

        public void SetGameRated(int value)
        {
            GameRated = true;
            GameRatedValue = value;
            PlayerDataManager.OnSave?.Invoke();
        }

        public void SaveIntroNoCoinOnBoard()
        {
            IntroNoCoinOnBoard = true;
            PlayerDataManager.OnSave?.Invoke();
        }
        #endregion

        #region Daily Rewards
        public int DailyRewardIndex = 0;
        public int DailySpins = 0;

        public void ResetDailyRewards()
        {
            DailyRewardIndex = 0;
        }

        public bool CanSpin => DailySpins < Const.MAX_DAILY_SPINS;
        public int CurrentSpins => Math.Clamp(Const.MAX_DAILY_SPINS - DailySpins, 0, Const.MAX_DAILY_SPINS);
        public bool FreeSpin = true;

        public void ResetDailySpins()
        {
            DailySpins = 0;
        }

        public void UpdateDailySpins()
        {
            DailySpins += 1;
        }

        public int LastSetupProgress = 0;
        public int CountWatchAds = 0;
        public int[] ProgressWatchAds;
        public int GetCountRewardAds()
        {
            var lastUpdate = Static.GetDateTime(LastSetupProgress);
            if (lastUpdate.Date != DateTime.UtcNow.Date)
            {
                CountWatchAds = 0;
                ProgressWatchAds = new int[3];
            }

            return CountWatchAds;
        }
        public void UpdateCountRewardAds()
        {
            CountWatchAds++;
            switch (CountWatchAds)
            {
                case 1:
                    SetProgressWatchAds(0, 1);
                    break;
                case 3:
                    SetProgressWatchAds(1, 1);
                    break;
                case 5:
                    SetProgressWatchAds(2, 1);
                    break;
            }

            LastSetupProgress = Static.GetUnixTime();
            PlayerDataManager.OnSave?.Invoke();
        }
        public bool CheckHaveGiftReward()
        {
            try
            {
                // foreach (var processData in ProgressWatchAds)
                // {
                //     if (processData == 1) return true;
                // }
                // return false;
                
                if (GetProgressWatchAds(0) == 1 || GetProgressWatchAds(1) == 1 || GetProgressWatchAds(2) == 1) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //0: default, 1: already claim, 2: received gift
        public int GetProgressWatchAds(int index)
        {
            var lastUpdate = Static.GetDateTime(LastSetupProgress);
            if (lastUpdate.Date != DateTime.UtcNow.Date)
            {
                CountWatchAds = 0;
                ProgressWatchAds = new int[3];
            }

            if (ProgressWatchAds == null || ProgressWatchAds.Length <= index)
            {
                ProgressWatchAds = new int[Const.MAX_DAILY_WATCH_ADS];
                return 0;
            }

            return ProgressWatchAds[index];
        }

        //0: default, 1: already claim, 2: received gift
        public void SetProgressWatchAds(int index, int value)
        {
            if (ProgressWatchAds == null || ProgressWatchAds.Length <= index) return;

            ProgressWatchAds[index] = value;
            LastSetupProgress = Static.GetUnixTime();
            PlayerDataManager.OnSave?.Invoke();
        }

        #endregion

        // Player Datas
        public bool IsStarterPackPurchased = false;

        // Settings
        public bool Music = true;
        public bool Sound = true;
        public bool Vibration = true;

        public List<string> TutorialShown = new();
        public bool CanShowHeartOffers = false;

        // IAP
        public List<string> PurchaseHistory = new();

        public void UpdateMusicValue(bool value)
        {
            Music = value;
        }

        public void UpdateSoundValue(bool value)
        {
            Sound = value;
        }

        public void UpdateVibrationValue(bool value)
        {
            Vibration = value;
        }
    }
}
