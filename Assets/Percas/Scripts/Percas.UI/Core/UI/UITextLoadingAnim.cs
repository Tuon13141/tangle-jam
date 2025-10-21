using UnityEngine;
using System.Collections;
using TMPro;

namespace Percas.UI
{
    public class LoadingTextAnimator : MonoBehaviour
    {
        [Header("UI Settings")]
        [SerializeField] TMP_Text loadingText;
        [SerializeField] string baseText = "Loading";

        [Header("Animation Settings")]
        [SerializeField] float updateInterval = 0.5f;
        [SerializeField] int maxDots = 3;

        private int currentDotCount = 0;
        private Coroutine animationCoroutine;
        private bool isAnimating = false;

        private void Awake()
        {
            if (loadingText == null)
            {
                loadingText = GetComponent<TMP_Text>();
            }
        }

        private void OnEnable()
        {
            StartAnimating();
        }

        private void OnDisable()
        {
            StopAnimating();
        }

        public void StartAnimating()
        {
            if (!isAnimating && loadingText != null)
            {
                isAnimating = true;
                animationCoroutine = StartCoroutine(AnimateLoadingText());
            }
        }

        public void StopAnimating()
        {
            if (isAnimating && animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                isAnimating = false;
            }
        }

        private IEnumerator AnimateLoadingText()
        {
            float timer = 0f;
            while (isAnimating)
            {
                timer += Time.deltaTime;
                if (timer >= updateInterval)
                {
                    timer = 0f;
                    currentDotCount = (currentDotCount % maxDots) + 1;
                    string dots = new('.', currentDotCount);
                    loadingText.text = $"{baseText}{dots}";
                }
                yield return null;
            }
        }
    }
}
