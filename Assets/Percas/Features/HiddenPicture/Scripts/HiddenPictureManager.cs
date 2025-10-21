using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Percas
{
    public class HiddenPictureManager : MonoBehaviour
    {
        public static HiddenPictureData Data = new();

        private readonly string dataKey = Const.KEY_HIDDEN_PICTURE_DATA;

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
                HiddenPictureData data = JsonConvert.DeserializeObject<HiddenPictureData>(jsonData);
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
