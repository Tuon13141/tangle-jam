using UnityEngine;
using DG.Tweening;

namespace Percas.UI
{
    public class ScaleLoop : MonoBehaviour
    {
        [SerializeField] Vector2 targetScale = new(1.1f, 1.1f);

        [SerializeField] float duration = 0.5f;
        [SerializeField] float delayTime = 0.25f;

        [SerializeField] Ease ease = Ease.InOutSine;

        private Tween tween;

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
            tween = this.transform.DOScale(targetScale, duration).SetDelay(delayTime).SetLoops(-1, LoopType.Yoyo).SetEase(ease);
        }
    }
}
