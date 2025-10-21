using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Percas.Data;
using Percas.UI;

namespace Percas
{
    public class PopupProfile_Item : MonoBehaviour, IActivatable
    {
        [SerializeField] ProfileItemType type;
        [SerializeField] int id;
        [SerializeField] ButtonBase buttonSelect;
        [SerializeField] GameObject m_active, m_locked;
        [SerializeField] Image m_avatarImage, m_frameBG, m_avatarFrame;
        [SerializeField] TMP_Text m_textUnlockedLevel;

        public static Action<int> OnUpdateUI;

        AvatarDataSO avatarDataSO;

        private void Awake()
        {
            buttonSelect.SetPointerClickEvent(Select);
            OnUpdateUI += UpdateUI;
        }

        private void OnDestroy()
        {
            OnUpdateUI -= UpdateUI;
        }

        public void Activate()
        {
            UpdateUI();
        }

        public void Deactivate() { }

        private void UpdateUI(int currentID = -1)
        {
            avatarDataSO = type == ProfileItemType.Avatar ? DataManager.Instance.GetAvatar(id) : DataManager.Instance.GetFrame(id);

            m_active.SetActive(currentID == id);

            if (GameLogic.CurrentLevel < avatarDataSO.unlockByLevel)
            {
                m_locked.SetActive(true);
                m_textUnlockedLevel.text = $"Level {avatarDataSO.unlockByLevel}";
            }
            else if (avatarDataSO.unlockByCoin > 0 && ((type == ProfileItemType.Avatar && !PlayerDataManager.PlayerData.IsUnlockedCoinAvatar(id)) || (type == ProfileItemType.Frame && !PlayerDataManager.PlayerData.IsUnlockedCoinFrame(id))))
            {
                m_locked.SetActive(true);
                m_textUnlockedLevel.text = $"{avatarDataSO.unlockByCoin}<sprite=0>";
            }
            else
            {
                m_locked.SetActive(false);
            }

            if (type == ProfileItemType.Avatar) m_avatarImage.sprite = DataManager.Instance.GetAvatarImage(id);
            if (type == ProfileItemType.Frame) m_frameBG.sprite = DataManager.Instance.GetAvatarFrameBG(id);
            if (type == ProfileItemType.Frame) m_avatarFrame.sprite = DataManager.Instance.GetAvatarFrame(id);
        }

        private async void Select()
        {
            if (GameLogic.CurrentLevel < avatarDataSO.unlockByLevel)
            {
                ActionEvent.OnShowToast?.Invoke($"Beat level {avatarDataSO.unlockByLevel} to unlock!");
                return;
            }

            if (avatarDataSO.unlockByCoin > 0 && ((type == ProfileItemType.Avatar && !PlayerDataManager.PlayerData.IsUnlockedCoinAvatar(id)) || (type == ProfileItemType.Frame && !PlayerDataManager.PlayerData.IsUnlockedCoinFrame(id))))
            {
                bool isDone = false;
                bool isSuccess = false;
                ServiceLocator.PopupScene.ShowPopup(PopupName.ConfirmUseCoin, new PopupConfirmUseCoinArgs(avatarDataSO.unlockByCoin, (useCoin) =>
                {
                    if (useCoin)
                    {
                        PlayerDataManager.OnSpendCoin?.Invoke(avatarDataSO.unlockByCoin, (done) =>
                        {
                            isDone = true;
                            isSuccess = done;
                        }, type == ProfileItemType.Avatar ? "unlock_avatar" : "unlock_frame", new LogCurrency("currency", "coin", "profile", null, "feature", "unlock_item"));
                    }
                    else
                    {
                        isDone = true;
                        isSuccess = true;
                    }
                }));
                await UniTask.WaitUntil(() => isDone);
                if (!isSuccess) return;
                if (type == ProfileItemType.Avatar) PlayerDataManager.PlayerData.AddCoinAvatar(id);
                if (type == ProfileItemType.Frame) PlayerDataManager.PlayerData.AddCoinFrame(id);
            }

            OnUpdateUI?.Invoke(id);

            int imageID = type == ProfileItemType.Avatar ? id : -1;
            int frameID = type == ProfileItemType.Frame ? id : -1;
            PlayerAvatar.OnUpdateUI?.Invoke(false, imageID, frameID);
            PopupProfile.OnUpdateProfile?.Invoke(imageID, frameID);
        }
    }
}
