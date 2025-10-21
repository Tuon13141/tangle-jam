using Percas.Data;

namespace Percas
{
    public static class GameSetting
    {
        public static bool MUSIC
        {
            get { return PlayerDataManager.PlayerData.Music; }
            set
            {
                PlayerDataManager.PlayerData.UpdateMusicValue(value);
                PlayerDataManager.OnSave?.Invoke();
            }
        }

        public static bool SOUND
        {
            get { return PlayerDataManager.PlayerData.Sound; }
            set
            {
                PlayerDataManager.PlayerData.UpdateSoundValue(value);
                PlayerDataManager.OnSave?.Invoke();
            }
        }

        public static bool VIBRATION
        {
            get { return PlayerDataManager.PlayerData.Vibration; }
            set
            {
                PlayerDataManager.PlayerData.UpdateVibrationValue(value);
                PlayerDataManager.OnSave?.Invoke();
            }
        }
    }
}
