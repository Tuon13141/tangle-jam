using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Percas.UI
{
    public class UIPictureGain : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] RectTransform rect;
        [SerializeField] Image icon;
        [SerializeField] float duration = 1.0f;
        [SerializeField] float delayTime = 0.0f;
        [SerializeField] Ease easeOut;

        public Vector2 SizeDelta { get; set; }
        public RectTransform CurrentRect => rect;

        private Sequence sequence;

        private void OnDestroy()
        {
            sequence?.Kill();
        }

        private void OnDisable()
        {
            sequence?.Kill();
        }

        public void Show()
        {
            this.transform.localScale = Vector3.zero;
            this.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            canvasGroup.alpha = 0;
        }

        public void SetPicture(Sprite sprite)
        {
            icon.sprite = sprite;
        }

        public void SetPosition(Vector2 position)
        {
            CurrentRect.anchoredPosition3D = position;
        }

        public void Movement(Vector2 target, Action onCompleted)
        {
            sequence = DOTween.Sequence();
            sequence.Append(canvasGroup.DOFade(1, duration / 2f));
            sequence.AppendInterval(delayTime);
            sequence.Append(CurrentRect.DOMove(target, duration).SetEase(easeOut));
            sequence.Join(CurrentRect.DOScale(Vector2.zero, duration).SetEase(easeOut));
            sequence.AppendCallback(() =>
            {
                onCompleted?.Invoke();
            });
            sequence.Play();
        }

        public void Hide()
        {
            SimplePool.Despawn(this.gameObject);
        }
    }
}
