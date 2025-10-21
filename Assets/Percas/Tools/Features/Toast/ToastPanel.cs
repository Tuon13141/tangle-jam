using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Percas
{
    public class ToastPanel : MonoBehaviour
    {
        [SerializeField] RectTransform rectTransform;
        [SerializeField] TMP_Text textMessage;
        [SerializeField] CanvasGroup canvasGroup;
        [Header("Tween Config")]
        [SerializeField] float duration = 0.5f;
        [SerializeField] float delayTime = 0.5f;
        [SerializeField] Ease easeOut;

        private Vector2 SizeDelta { get; set; }

        public void Hide()
        {
            SimplePool.Despawn(this.gameObject);
        }

        public void Show()
        {
            this.SizeDelta = new Vector2(1080f, 132f);
            rectTransform.sizeDelta = SizeDelta;
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            this.transform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;
        }

        public void Display(string message, Action onCompleted)
        {
            textMessage.text = message;
            canvasGroup.alpha = 1;
            transform.localScale = Vector3.zero;
            transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
            rectTransform.DOAnchorPos(new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + 200f), duration).SetEase(easeOut).SetDelay(delayTime);
            canvasGroup.DOFade(0, duration - 0.2f).SetDelay(duration + 0.2f).OnComplete(() =>
            {
                Hide();
                onCompleted?.Invoke();
            });
        }
    }
}
