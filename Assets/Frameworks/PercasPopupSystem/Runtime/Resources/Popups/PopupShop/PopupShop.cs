using System;
using UnityEngine;
using UnityEngine.UI;
using Percas.UI;
using Sonat;

namespace Percas
{
    public class PopupShop : PopupBase
    {
        [SerializeField] ButtonClosePopup buttonClosePopup;
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] RectTransform m_scroll;

        private string placement;

        protected override void Awake()
        {
            buttonClosePopup.onCompleted = Close;
        }

        private void OnStart()
        {
            m_scroll.offsetMin = new Vector2(m_scroll.offsetMin.x, GameLogic.IsNoAds ? 0f : 176f);
            scrollRect.verticalNormalizedPosition = 1f;
            if (GameLogic.IsInGame)
            {
                placement = "ingame";
            }
            else
            {
                placement = "home";
            }

            var log = new SonatLogOpenShop()
            {
                mode = PlayMode.classic.ToString(),
                level = GameLogic.CurrentLevel,
                phase = GameLogic.CurrentLevelPhase,
                placement = placement,
            };
            log.Post(logAf: true);
        }

        private void Close()
        {
            ServiceLocator.PopupScene.HidePopup(PopupName.Shop, null);
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            OnStart();
        }

        public override void Hide(Action onHidden = null)
        {
            base.Hide(onHidden);
        }
        #endregion
    }
}
