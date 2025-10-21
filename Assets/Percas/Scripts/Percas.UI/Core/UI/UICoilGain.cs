using System;
using DG.Tweening;
using UnityEngine;

namespace Percas.UI
{
    public class UICoilGain : MonoBehaviour
    {
        [SerializeField] RectTransform rect;
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
            this.SizeDelta = Vector2.zero;
            CurrentRect.sizeDelta = SizeDelta;
            CurrentRect.anchorMax = new Vector2(0.5f, 0.5f);
            this.transform.localScale = Vector3.one;
            CurrentRect.anchoredPosition3D = Vector3.zero;
        }

        public void SetPosition(Vector2 position)
        {
            CurrentRect.anchoredPosition3D = position;
        }

        public void Movement(Vector2 target, Action onCompleted)
        {
            sequence = DOTween.Sequence();
            sequence.AppendInterval(delayTime);
            sequence.Append(CurrentRect.DOMove(target, duration).SetEase(easeOut));
            sequence.Join(CurrentRect.DOScale(new Vector2(0.5f, 0.5f), duration).SetEase(easeOut));
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
