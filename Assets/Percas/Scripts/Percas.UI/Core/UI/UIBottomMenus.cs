using System;
using UnityEngine;

namespace Percas.UI
{
    public class UIBottomMenus : MonoBehaviour
    {
        public static Action<bool> OnShow;

        private RectTransform rect;

        private void Awake()
        {
            OnShow += Show;

            rect = this.GetComponent<RectTransform>();
        }

        private void OnDestroy()
        {
            OnShow -= Show;
        }

        private void OnEnable()
        {
            Show(isUpdate: false);
        }

        private void Show(bool isUpdate)
        {
            // [HardCode]
            if (!isUpdate)
            {
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, !GameLogic.IsNoAds ? -334f : -510f);
            }
            else
            {
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, !GameLogic.IsNoAds ? 266f : 90f);
            }
        }
    }
}
