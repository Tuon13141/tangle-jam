using System.Collections.Generic;
using UnityEngine;

namespace Percas.UI
{
    public class UIGameSettings : MonoBehaviour, IActivatable
    {
        [SerializeField] List<GameObject> goMusics;
        [SerializeField] List<GameObject> goSounds;
        [SerializeField] List<GameObject> goHaptics;

        [SerializeField] ButtonBase buttonMusic;
        [SerializeField] ButtonBase buttonSound;
        [SerializeField] ButtonBase buttonHaptics;

        public void Activate()
        {
            UpdateSettingUI();

            buttonMusic.SetPointerClickEvent(ToggleMusic);
            buttonSound.SetPointerClickEvent(ToggleSound);
            buttonHaptics.SetPointerClickEvent(ToggleVibration);
        }

        public void Deactivate() { }

        private void UpdateSettingUI()
        {
            goMusics[0].SetActive(GameSetting.MUSIC);
            goMusics[1].SetActive(!GameSetting.MUSIC);

            goSounds[0].SetActive(GameSetting.SOUND);
            goSounds[1].SetActive(!GameSetting.SOUND);

            goHaptics[0].SetActive(GameSetting.VIBRATION);
            goHaptics[1].SetActive(!GameSetting.VIBRATION);
        }

        private void ToggleMusic()
        {
            GameSetting.MUSIC = !GameSetting.MUSIC;
            AudioController.Instance.HandleMusicValue(GameSetting.MUSIC);
            UpdateSettingUI();
        }

        private void ToggleSound()
        {
            GameSetting.SOUND = !GameSetting.SOUND;
            AudioController.Instance.HandleSoundValue(GameSetting.SOUND);
            UpdateSettingUI();
        }

        private void ToggleVibration()
        {
            GameSetting.VIBRATION = !GameSetting.VIBRATION;
            UpdateSettingUI();
        }
    }
}
