using UnityEngine;
using DG.Tweening;
using TMPro;

namespace Percas
{
    public class UIHomeQuote : MonoBehaviour
    {
        [SerializeField] RectTransform textRect;
        [SerializeField] RectTransform viewportRect;
        [SerializeField] RectTransform iconSpeaker;
        [SerializeField] TMP_Text textContent;
        [SerializeField] float scrollDuration = 8f;
        [SerializeField] float rightGap = 40f;
        [SerializeField] float leftGap = 80f;

        private float startPosX;
        private float endPosX;

        private Tween moveTween;
        private Tween scaleTween;

        private void OnDestroy()
        {
            moveTween?.Kill();
            scaleTween?.Kill();
        }

        private void Start()
        {
            CalculatePositions();
            DisplayNextMessage();
        }

        private void CalculatePositions()
        {
            startPosX = viewportRect.rect.width - rightGap;
            endPosX = -textRect.rect.width + leftGap;
        }

        private void DisplayNextMessage()
        {
            SetTextContent();
            Canvas.ForceUpdateCanvases();
            scaleTween = iconSpeaker.DOScale(Vector3.one * 1.25f, 0.2f).SetLoops(10, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                iconSpeaker.localScale = Vector3.one;
            });
            textRect.anchoredPosition = new Vector2(startPosX, textRect.anchoredPosition.y);
            moveTween = textRect.DOAnchorPosX(endPosX, scrollDuration).SetEase(Ease.Linear).OnComplete(() =>
            {
                DisplayNextMessage();
            });
        }

        private void SetTextContent()
        {
            string randomName = DataManager.Instance.GetRandomPlayerName();

            if (GameLogic.CurrentLevel < GameConfig.Instance.MaxLevel)
            {
                textContent.text = $"{randomName} beated level {Mathf.Clamp(Random.Range(GameLogic.CurrentLevel + 1, GameLogic.CurrentLevel + 4), GameLogic.CurrentLevel, GameConfig.Instance.MaxLevel)}!";
            }
            else
            {
                textContent.text = $"{randomName} beated level {Mathf.Clamp(Random.Range(GameLogic.CurrentLevel - 1, GameLogic.CurrentLevel - 4), GameLogic.CurrentLevel - 4, GameLogic.CurrentLevel)}!";
            }
        }
    }
}
