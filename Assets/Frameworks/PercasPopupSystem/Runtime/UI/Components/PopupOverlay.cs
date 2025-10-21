using UnityEngine;
using DG.Tweening;

namespace Percas
{
    public class PopupOverlay : MonoBehaviour, IActivatable
    {
        [Header("References")]
        [SerializeField] CanvasGroup m_canvasGroup;

        [Header("Configs")]
        [SerializeField] bool doFade = true;
        [SerializeField] float fadeDuration = 0.2f;

        private Tween fadeTween;

        private void Awake()
        {
            if (m_canvasGroup == null) m_canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            fadeTween?.Kill();
        }

        private void DoFade()
        {
            m_canvasGroup.alpha = 0;
            fadeTween = m_canvasGroup.DOFade(1f, fadeDuration);
        }

        #region Public Methods
        public void Activate()
        {
            if (doFade) DoFade();
        }

        public void Deactivate()
        {
            fadeTween?.Kill();
        }
        #endregion
    }
}
