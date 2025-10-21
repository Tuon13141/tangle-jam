using System;
using UnityEngine;

namespace Percas.UI
{
    public class UIMarketingHandler : MonoBehaviour
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

        private void OnEnable()
        {
            Show();
        }

        private void Show()
        {
            this.gameObject.SetActive(!GameConfig.Instance.IsMarketingVersion);
        }
    }
}
