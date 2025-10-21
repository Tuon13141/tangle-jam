using System;
using UnityEngine;
using DG.Tweening;

namespace Percas.UI
{
    public class ScaleAnim : MonoBehaviour, IActivatable
    {
        [SerializeField] Vector2 _initScale = Vector2.zero;
        [SerializeField] Vector2 _endScale = Vector2.one;
        [SerializeField] float _duration = 0.5f;
        [SerializeField] float _delayTime = 0.5f;
        [SerializeField] Ease ease = Ease.OutBounce;

        private Tween tween;
        private Action onCompleted;

        private void OnEnable()
        {
            OnShow();
        }

        private void OnDisable()
        {
            tween?.Kill();
        }

        private void OnShow()
        {
            transform.localScale = _initScale;
            tween = transform.DOScale(_endScale, _duration).SetDelay(_delayTime).SetEase(ease).OnComplete(() =>
            {
                onCompleted?.Invoke();
            });
        }

        #region Public Methods
        public void Activate()
        {
            OnShow();
        }

        public void Deactivate()
        {
            tween?.Kill();
        }

        public void SetOnCompleted(Action onCompleted)
        {
            this.onCompleted = onCompleted;
        }
        #endregion
    }
}
