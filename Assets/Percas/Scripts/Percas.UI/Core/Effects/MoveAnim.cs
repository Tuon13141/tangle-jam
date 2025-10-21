using System;
using UnityEngine;
using DG.Tweening;

namespace Percas.UI
{
    public enum MoveDirection
    {
        X,
        Y,
        XY,
    }

    public class MoveAnim : MonoBehaviour, IActivatable
    {
        [SerializeField] RectTransform rect;
        [SerializeField] MoveDirection moveDirection;
        [SerializeField] float distance = 0; // dung so am neu la tru
        [SerializeField] float duration = 0.5f;
        [SerializeField] float delayTime = 0.5f;
        [SerializeField] Ease ease = Ease.OutBounce;

        private Action onCompleted;
        private Tween tween;

        private void Awake()
        {
            if (rect == null) rect = GetComponent<RectTransform>();
        }

        private void OnDestroy()
        {
            tween?.Kill();
        }

        private void OnEnable()
        {
            Show();
        }

        private void OnDisable()
        {
            tween?.Kill();
        }

        private void MoveX()
        {
            Vector2 initPos = rect.anchoredPosition;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + distance, rect.anchoredPosition.y);
            tween = rect.DOAnchorPosX(initPos.x, duration).SetDelay(delayTime).SetEase(ease).OnComplete(() => onCompleted?.Invoke());
        }

        private void MoveY()
        {
            Vector2 initPos = rect.anchoredPosition;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y + distance);
            tween = rect.DOAnchorPosY(initPos.y, duration).SetDelay(delayTime).SetEase(ease).OnComplete(() => onCompleted?.Invoke());
        }

        #region Public Methods
        public void Activate()
        {
            Show();
        }

        public void Deactivate()
        {
            tween?.Kill();
        }

        public void Show()
        {
            switch (moveDirection)
            {
                case MoveDirection.X:
                    MoveX();
                    break;

                case MoveDirection.Y:
                    MoveY();
                    break;
            }
        }

        public void SetOnCompleted(Action onCompleted)
        {
            this.onCompleted = onCompleted;
        }
        #endregion
    }
}
