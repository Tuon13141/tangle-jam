using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Percas.Data;
using Percas.UI;
using TMPro;
using UnityEngine;

namespace Percas
{
    public class PopupProfile : PopupBase
    {
        [SerializeField] ButtonClosePopup buttonClosePopup;
        [SerializeField] ButtonBase buttonEdit, buttonSave;
        [SerializeField] PopupProfile_Tab buttonTabAvatar, buttonTabFrame;
        [SerializeField] GameObject m_contentAvatar, m_contentFrame;
        [SerializeField] TMP_InputField m_inputDisplayName;

        [Header("Refs")]
        [SerializeField] List<Sprite> buttonColors;

        public static Action<int> OnUpdateUI;
        public static Action<int, int> OnUpdateProfile;

        private int currentTab = 0;
        private int currentAvatarID = 0;
        private int currentFrameID = 0;

        private bool _canSave = false;

        private readonly Regex validNameRegex = new("^[a-zA-Z0-9 #]{3,20}$");
        private readonly List<string> bannedWords = new()
        {
            "damn",
            "hell",
            "shit",
            "fuck",
            "ass",
            "bitch",
            "bastard",
            "damnation",
            "sex",
            "nude",
            "xxx",
            "porn",
            "horny",
            "slut",
            "whore",
            "nazi"
        };

        protected override void Awake()
        {
            RegisterButtons();
        }

        private void RegisterButtons()
        {
            buttonClosePopup.onCompleted = Close;
            buttonEdit.SetPointerClickEvent(() =>
            {
                m_inputDisplayName.Select();
                m_inputDisplayName.ActivateInputField();
            });
            buttonSave.SetPointerClickEvent(Save);
        }

        protected override void OnSubscribeEvents()
        {
            OnUpdateUI += UpdateUI;
            OnUpdateProfile += UpdateProfile;
        }

        protected override void OnUnsubscribeEvents()
        {
            OnUpdateUI -= UpdateUI;
            OnUpdateProfile -= UpdateProfile;
        }

        private void InitUI()
        {
            _canSave = false;
            buttonEdit.gameObject.SetActive(!_canSave);
            buttonSave.gameObject.SetActive(_canSave);

            UpdateUI(currentTab);
            m_inputDisplayName.text = $"{PlayerDataManager.PlayerData.GetPlayerName()}";
            currentAvatarID = PlayerDataManager.PlayerData.GetAvatarID();
            currentFrameID = PlayerDataManager.PlayerData.GetFrameID();
        }

        private void OnHide(Action onCompleted = null)
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.Profile, () =>
            {
                onCompleted?.Invoke();
            });
        }

        private void Close()
        {
            OnHide();
        }

        private void UpdateUI(int tab)
        {
            if (currentTab == tab) return;

            currentTab = tab;

            m_contentAvatar.SetActive(currentTab == 0);
            m_contentFrame.SetActive(currentTab == 1);

            bool isActiveAvatar = currentTab == 0;
            bool isActiveFrame = currentTab == 1;

            buttonTabAvatar.UpdateUI(buttonColors[isActiveAvatar ? 0 : 1]);
            buttonTabFrame.UpdateUI(buttonColors[isActiveFrame ? 0 : 1]);
        }

        private void UpdateProfile(int avatarID, int frameID)
        {
            if (avatarID >= 0) currentAvatarID = avatarID;
            if (frameID >= 0) currentFrameID = frameID;
            _canSave = currentAvatarID != PlayerDataManager.PlayerData.GetAvatarID() || currentFrameID != PlayerDataManager.PlayerData.GetFrameID();
            buttonEdit.gameObject.SetActive(!_canSave);
            buttonSave.gameObject.SetActive(_canSave);
        }

        public void OnSelect()
        {
            buttonClosePopup.gameObject.SetActive(false);
            buttonEdit.gameObject.SetActive(false);
            buttonSave.gameObject.SetActive(true);
        }

        private bool CanSave()
        {
            if (!m_inputDisplayName.text.Trim().Equals(PlayerDataManager.PlayerData.GetPlayerName())) return true;
            return false;
        }

        public void OnDeselect()
        {
            if (!CanSave())
            {
                buttonClosePopup.gameObject.SetActive(true);
                buttonEdit.gameObject.SetActive(true);
                buttonSave.gameObject.SetActive(false);
                return;
            }
            buttonClosePopup.gameObject.SetActive(true);
            buttonEdit.gameObject.SetActive(false);
            buttonSave.gameObject.SetActive(true);
            _canSave = true;
        }

        private bool IsValidDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName) || !validNameRegex.IsMatch(displayName)) return false;
            foreach (var word in bannedWords)
            {
                if (displayName.ToLower().Contains(word))
                {
                    return false;
                }
            }
            return true;
        }

        private void Save()
        {
            if (!_canSave)
            {
                OnHide();
                return;
            }

            if (!IsValidDisplayName(m_inputDisplayName.text.Trim()))
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_INVALID_DISPLAY_NAME);
                return;
            }

            string playerProfile = string.Format("{0:00}{1:00}{2:00}{3}", currentAvatarID, currentFrameID, 0, m_inputDisplayName.text.Trim());
            PlayerDataManager.PlayerData.SavePlayerProfile(playerProfile);
            OnHide(() =>
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_PROFILE_UPDATED);
                PlayerAvatar.OnUpdateUI?.Invoke(true, currentAvatarID, currentFrameID);
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            InitUI();
        }
        #endregion
    }
}
