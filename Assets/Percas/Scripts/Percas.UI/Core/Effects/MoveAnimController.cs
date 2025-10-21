using UnityEngine;
using DG.Tweening;

namespace Percas.UI
{
    public class MoveAnimController : MonoBehaviour, IActivatable
    {
        [SerializeField] RectTransform rect;
        [SerializeField] MoveDirection moveDirection;
        [SerializeField] float distance = 0; // dung so am neu la tru
        [SerializeField] float duration = 0.3f;
        [SerializeField] float delayTime = 0.0f;
        [SerializeField] Ease ease = Ease.OutQuart;
        private Tween tween;

        private void Awake()
        {
            if (rect == null) rect = GetComponent<RectTransform>();
        }

        private void OnDestroy()
        {
            tween?.Kill();
        }

        private void MoveX(bool isBack)
        {
            Vector2 initPos = rect.anchoredPosition;
            if (isBack)
            {
                if (initPos.x != 0) tween = rect.DOAnchorPosX(0, duration).SetDelay(delayTime).SetEase(ease);
            }
            else
            {
                if (initPos.x != distance) tween = rect.DOAnchorPosX(distance, duration).SetDelay(delayTime).SetEase(ease);
            }
        }

        private void MoveY(bool isBack)
        {
            Vector2 initPos = rect.anchoredPosition;
            if (isBack)
            {
                if (initPos.y != 0) tween = rect.DOAnchorPosY(0, duration).SetDelay(delayTime).SetEase(ease);
            }
            else
            {
                if (initPos.y != distance) tween = rect.DOAnchorPosY(distance, duration).SetDelay(delayTime).SetEase(ease);
            }
        }

        #region Public Methods
        public void Activate() { }

        public void Deactivate()
        {
            tween?.Kill();
        }

        public void Show(bool isBack)
        {
            switch (moveDirection)
            {
                case MoveDirection.X:
                    MoveX(isBack);
                    break;

                case MoveDirection.Y:
                    MoveY(isBack);
                    break;
            }
        }
        #endregion
    }
}
