using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Percas.IAA;
using Random = UnityEngine.Random;

namespace Percas
{
    public class DataManager : SingletonMonoBehaviour<DataManager>
    {
        [SerializeField] List<HiddenPictureDataSO> hiddenPictureDatas = new();
        [SerializeField] List<IAASalePackDataSO> salePackDatas = new();
        [SerializeField] List<int> luxuryBasketTargets = new();
        [SerializeField] List<string> quoteDatas = new();
        [SerializeField] List<string> playerNameDatas = new();

        [Header("Profile")]
        [SerializeField] List<AvatarDataSO> avatars = new();
        [SerializeField] List<AvatarDataSO> frames = new();
        [SerializeField] List<Sprite> frameBackgrounds = new();

        private static List<IAASalePackDataSO> SalePackDatas;

        public List<int> LuxuryBasketTargets => luxuryBasketTargets;

        //readonly string quoteRaw = @"";

        //protected override void Awake()
        //{
        //    base.Awake();
        //    quoteDatas = new List<string>(quoteRaw.Split('\n'));
        //}

        private void OnEnable()
        {
            SalePackDatas = salePackDatas;
        }

        public void WriteToLocal(string fileName, string fileData)
        {
#if UNITY_EDITOR
            File.WriteAllText($"Assets/Game/Data/{fileName}.txt", fileData);
#endif
        }

        public static IAASalePackDataSO GetPackData(IAASalePackID packID)
        {
            return SalePackDatas.Find((item) => item.packID == packID);
        }

        #region Home Quote
        public int GetQuoteCount()
        {
            return quoteDatas.Count;
        }

        public int GetRandomQuoteIndex()
        {
            return Random.Range(0, quoteDatas.Count - 1);
        }

        public string GetQuoteByIndex(int index)
        {
            string quote = quoteDatas[0];
            try
            {
                quote = quoteDatas[index];
            }
            catch (Exception) { }
            return quote;
        }
        #endregion

        #region Player Avatar
        public string GetRandomPlayerProfile()
        {
            string randomPlayerName = GetRandomPlayerName();

            if (randomPlayerName.Contains("Player #"))
            {
                return $"000000{randomPlayerName}";
            }

            string playerProfile = string.Format("{0:00}{1:00}{2:00}{3}", GetRandomAvatarID(), GetRandomFrameID(), 0, randomPlayerName);
            return playerProfile;
        }

        public string GetRandomPlayerName()
        {
            string randomName = $"Player #{Random.Range(1000, 10000)}";
            try
            {
                if (Random.Range(0, 3) == 0)
                {
                    string fullName = playerNameDatas[Random.Range(0, playerNameDatas.Count - 1)];
                    string[] names = fullName.Split(' ');
                    randomName = names[Random.Range(0, names.Length)];
                }
            }
            catch (Exception) { }
            return randomName;
        }

        public int GetRandomAvatarID()
        {
            return Random.Range(0, avatars.Count);
        }

        public AvatarDataSO GetAvatar(int index)
        {
            AvatarDataSO result = null;
            try
            {
                result = avatars[index];
            }
            catch (Exception) { }
            return result;
        }

        public Sprite GetAvatarImage(int index)
        {
            Sprite result = null;
            try
            {
                result = avatars[index].sprite;
            }
            catch (Exception) { }
            return result;
        }

        public int GetRandomFrameID()
        {
            return Random.Range(0, frames.Count);
        }

        public AvatarDataSO GetFrame(int index)
        {
            AvatarDataSO result = null;
            try
            {
                result = frames[index];
            }
            catch (Exception) { }
            return result;
        }

        public Sprite GetAvatarFrame(int index)
        {
            Sprite result = null;
            try
            {
                result = frames[index].sprite;
            }
            catch (Exception) { }
            return result;
        }

        public Sprite GetAvatarFrameBG(int index)
        {
            Sprite result = null;
            try
            {
                result = frameBackgrounds[index];
            }
            catch (Exception) { }
            return result;
        }
        #endregion

        #region Hidden Picture
        private HiddenPictureDataSO GetRandomUncollectedHiddenPictureID()
        {
            List<HiddenPictureDataSO> uncollectedEvents = hiddenPictureDatas.Where(eventData => !HiddenPictureManager.Data.CollectedEvents.Contains(eventData.ID)).ToList();
            if (uncollectedEvents.Count == 0) return null;
            return uncollectedEvents[Random.Range(0, uncollectedEvents.Count)];
        }

        public List<HiddenPictureDataSO> GetSortedHiddenPictureDatas()
        {
            if (hiddenPictureDatas.Count <= 1)
            {
                return hiddenPictureDatas;
            }

            HiddenPictureDataSO lastData = hiddenPictureDatas.Last();
            HiddenPictureDataSO currentData = GetCurrentHiddenPictureData();
            if (hiddenPictureDatas.Count >= 2 && lastData.ID == currentData.ID)
            {
                return hiddenPictureDatas;
            }

            List<HiddenPictureDataSO> results = hiddenPictureDatas;
            results.Remove(currentData);
            results.Add(currentData);
            return results;
        }

        public int GetRandomHiddenPictureID()
        {
            HiddenPictureDataSO randEvent = GetRandomUncollectedHiddenPictureID();
            return randEvent == null ? -1 : randEvent.ID;
        }

        public HiddenPictureDataSO GetHiddenPictureDataByIndex(int index)
        {
            try
            {
                return hiddenPictureDatas[index];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public HiddenPictureDataSO GetCurrentHiddenPictureData()
        {
            return GetHiddenPictureDataByIndex(HiddenPictureManager.Data.EventID);
        }

        public int GetActiveHiddenPictureEventID()
        {
            try
            {
                return GetCurrentHiddenPictureData().ID;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public int GetLastestCollectedHiddenPictureEventID()
        {
            try
            {
                return HiddenPictureManager.Data.CollectedEvents.Last();
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public string GetCurrentHiddenPictureName()
        {
            string result = "HIDDEN PICTURE";
            try
            {
                HiddenPictureDataSO data = GetHiddenPictureDataByIndex(HiddenPictureManager.Data.EventID);
                return data.EventName;
            }
            catch (Exception) { }
            return result;
        }

        public Sprite GetCurrentHiddenPictureIcon()
        {
            try
            {
                HiddenPictureDataSO data = GetHiddenPictureDataByIndex(HiddenPictureManager.Data.EventID);
                return data.EventIcon;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Sprite GetCurrentHiddenPicture()
        {
            try
            {
                HiddenPictureDataSO data = GetHiddenPictureDataByIndex(HiddenPictureManager.Data.EventID);
                return data.HiddenPicture;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Sprite GetCurrentHiddenPictureBackground()
        {
            try
            {
                HiddenPictureDataSO data = GetHiddenPictureDataByIndex(HiddenPictureManager.Data.EventID);
                return data.HiddenBackground;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Sprite GetCurrentHiddenPicturePiece(int index)
        {
            try
            {
                HiddenPictureDataSO data = GetHiddenPictureDataByIndex(HiddenPictureManager.Data.EventID);
                //return Static.GetSpriteHiddenPicture(data.LevelDatas[index]);
                return data.PiecePictures[index];
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}
