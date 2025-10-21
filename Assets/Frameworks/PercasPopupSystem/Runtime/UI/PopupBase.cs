using System;
using UnityEngine;
using DG.Tweening;

namespace Percas
{
    public abstract class PopupBase : MonoBehaviour
    {
        [Tooltip("For Smoother Display")]
        [SerializeField] CanvasGroup m_canvasGroup;

        private Tween fadeTween;

        #region Unity Methods
        protected virtual void Awake() { }

        protected virtual void OnEnable()
        {
            OnSubscribeEvents();
        }

        protected virtual void OnDisable()
        {
            OnUnsubscribeEvents();
        }

        protected virtual void OnSubscribeEvents() { }
        protected virtual void OnUnsubscribeEvents() { }

        protected virtual void OnPopupActivated()
        {
            foreach (var a in GetComponentsInChildren<IActivatable>(true)) a.Activate();
        }

        protected virtual void OnPopupDeactivated()
        {
            foreach (var a in GetComponentsInChildren<IActivatable>(true)) a.Deactivate();
        }
        #endregion

        #region Public Methods
        public virtual void Show(object args = null, Action onShown = null)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            if (m_canvasGroup != null)
            {
                m_canvasGroup.interactable = true;
                m_canvasGroup.blocksRaycasts = true;
                fadeTween = m_canvasGroup.DOFade(1.0f, 0.2f);
                //m_canvasGroup.alpha = 1;
                //m_canvasGroup.interactable = true;
                //m_canvasGroup.blocksRaycasts = true;
            }
            OnPopupActivated();
            onShown?.Invoke();
        }

        public virtual void Hide(Action onHidden = null)
        {
            fadeTween?.Kill();
            OnPopupDeactivated();
            if (m_canvasGroup == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                m_canvasGroup.alpha = 0;
                m_canvasGroup.interactable = false;
                m_canvasGroup.blocksRaycasts = false;
            }
            onHidden?.Invoke();
        }
        #endregion
    }
}
