using System;
using UnityEngine;

namespace Percas.UI
{
    public class UIStarterPack : MonoBehaviour, IActivatable
    {
        public static Action OnShow;

        private void Awake()
        {
            OnShow += Show;
        }

        private void OnDestroy()
        {
            OnShow -= Show;
        }

        private void Show()
        {
            gameObject.SetActive(!GameLogic.IsStarterPackPurchased);
        }

        public void Activate()
        {
            Show();
        }

        public void Deactivate() { }
    }
}
