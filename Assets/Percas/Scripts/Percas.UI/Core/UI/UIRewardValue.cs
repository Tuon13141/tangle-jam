using DG.Tweening;
using UnityEngine;
using TMPro;

namespace Percas.UI
{
    public class UIRewardValue : MonoBehaviour
    {
        [SerializeField] RectTransform rect;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] TMP_Text textValue;
        [SerializeField] float duration = 1.0f;
        [SerializeField] float delayTime = 0.0f;
        [SerializeField] Ease easeOut;

        public Vector2 SizeDelta { get; set; }
        public RectTransform CurrentRect => rect;

        private Tween fadeTween;
        private Tween tween;

        private void OnDestroy()
        {
            fadeTween?.Kill();
            tween?.Kill();
        }

        private void OnDisable()
        {
            fadeTween?.Kill();
            tween?.Kill();
        }

        public void Show()
        {
            this.SizeDelta = new Vector2(500f, 150f);
            CurrentRect.sizeDelta = SizeDelta;
            CurrentRect.anchorMax = new Vector2(0.5f, 0.5f);
            this.transform.localScale = Vector3.one;
            CurrentRect.anchoredPosition3D = Vector3.zero;
        }

        public void Hide()
        {
            SimplePool.Despawn(this.gameObject);
        }

        public void SetPosition(Vector2 position)
        {
            CurrentRect.anchoredPosition3D = position;
        }

        public void Display(int value)
        {
            textValue.text = $"+{value}";
            canvasGroup.alpha = 0;
            fadeTween = canvasGroup.DOFade(1, 0.5f);
            Vector2 targetPos = new(CurrentRect.anchoredPosition.x, CurrentRect.anchoredPosition.y + 100);
            tween = CurrentRect.DOAnchorPosY(targetPos.y, duration).SetEase(easeOut).SetDelay(delayTime);
            fadeTween = canvasGroup.DOFade(0, duration - 0.2f).SetDelay(delayTime + 0.2f).OnComplete(() =>
            {
                Hide();
            });
            this.transform.localScale = Vector3.one * Helpers.ScreenRatio;
        }
    }
}
