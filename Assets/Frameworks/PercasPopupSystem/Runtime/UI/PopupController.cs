using System;
using System.Collections.Generic;
using UnityEngine;

namespace Percas
{
    public class PopupController : MonoBehaviour
    {
        [SerializeField] private List<PopupEntry> popupEntries;

        private readonly Dictionary<PopupName, PopupBase> _popupDict = new();
        //private readonly List<PopupBase> _activePopups = new();

        private void Awake()
        {
            foreach (var entry in popupEntries)
            {
                if (entry.popup != null && !_popupDict.ContainsKey(entry.name))
                {
                    _popupDict.Add(entry.name, entry.popup);
                }
            }
        }

        public void ShowPopup(PopupName name, object args = null, Action callback = null)
        {
            if (_popupDict.TryGetValue(name, out var popup))
            {
                ActionEvent.OnPopupOpen?.Invoke($"Popup{name}");
                TrackingManager.OnTrackScreenView?.Invoke($"Popup{name}");
                //if (!_activePopups.Contains(popup)) _activePopups.Add(popup);
                //UICurrencyManager.OnShowBalance?.Invoke(true, false, false);
                popup.Show(args, callback);
                if (popup.TryGetComponent<RectTransform>(out var rt)) rt.SetAsLastSibling();
            }
            else
            {
                Debug.LogWarning($"Popup '{name}' not found.");
            }
        }

        public void HidePopup(PopupName name, Action callback = null)
        {
            if (_popupDict.TryGetValue(name, out var popup))
            {
                ActionEvent.OnPopupClose?.Invoke();
                //if (_activePopups.Contains(popup)) _activePopups.Remove(popup);
                //if (_activePopups.Count <= 0)
                //{
                //    if (GameLogic.IsInHome) UICurrencyManager.OnShowBalance?.Invoke(true, true, true);
                //    if (GameLogic.IsInGame) UICurrencyManager.OnShowBalance?.Invoke(true, false, false);
                //}
                popup.Hide(callback);
            }
            else
            {
                Debug.LogWarning($"Popup '{name}' not found.");
            }
        }

        [Serializable]
        private struct PopupEntry
        {
            public PopupName name;
            public PopupBase popup;
        }
    }
}
