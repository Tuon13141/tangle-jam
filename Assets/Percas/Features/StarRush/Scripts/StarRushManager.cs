using System;
using UnityEngine;
using Newtonsoft.Json;

namespace Percas
{
    public class StarRushManager : MonoBehaviour
    {
        public static StarRushData Data = new();

        private readonly string dataKey = Const.KEY_STAR_RUSH_DATA;

        public static bool LoadDataDone { get; private set; }

        #region Unity Methods
        private void Awake()
        {
            OnSave += Save;

            InitData();
        }

        private void OnDestroy()
        {
            OnSave -= Save;
        }

        private void OnApplicationQuit()
        {
            Save();
        }
        #endregion

        #region Data Controller
        private void InitData()
        {
            if (!PlayerPrefs.HasKey(dataKey))
            {
                Data.Reset();
                Save();
            }
            else
            {
                Load();
            }
            LoadDataDone = true;
        }

        private void Load()
        {
            try
            {
                string encryptedJson = PlayerPrefs.GetString(dataKey);
                string jsonData = encryptedJson.Decrypt();
                StarRushData data = JsonConvert.DeserializeObject<StarRushData>(jsonData);
                Data = data;
            }
            catch (Exception)
            {
                Data = new();
            }
            finally
            {
                Save();
            }
        }

        private void Save()
        {
            string jsonData = JsonConvert.SerializeObject(Data);
            string encryptedJson = jsonData.Encrypt();
            PlayerPrefs.SetString(dataKey, encryptedJson);
            PlayerPrefs.Save();
            Helpers.WriteToLocal(dataKey, jsonData);
        }
        #endregion

        #region Actions
        public static Action OnSave;
        #endregion
    }
}
