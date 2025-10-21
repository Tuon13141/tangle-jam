using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Percas
{
    public class ScrollSnap : MonoBehaviour, IActivatable
    {
        public ScrollRect scrollRect;
        public RectTransform content;
        public float snapSpeed = 10f;
        public float autoSwipeInterval = 2f; // Time between auto-swipes

        private RectTransform[] childRects;
        private float[] positions;
        private int selectedIndex = 0;
        private bool isSnapping = false;
        private bool isDragging = false;
        private Coroutine autoSwipeCoroutine;

        private void InitializePositions()
        {
            // Initialize child positions
            int childCount = content.childCount;
            childRects = new RectTransform[childCount];
            positions = new float[childCount];

            for (int i = 0; i < childCount; i++)
            {
                childRects[i] = content.GetChild(i).GetComponent<RectTransform>();
                positions[i] = (float)i / (childCount - 1); // Normalized positions
            }
        }

        public void Activate()
        {
            InitializePositions();

            // Snap to the center of the nearest position when reactivating
            SnapToNearestImmediate();

            // Restart auto-swiping when reactivating
            StartAutoSwipe();
        }

        public void Deactivate()
        {
            // Stop auto-swipe when the scroll view is disabled
            StopAutoSwipe();
        }

        public void OnDragBegin()
        {
            isDragging = true;
            StopAutoSwipe();
        }

        public void OnDragEnd()
        {
            isDragging = false;

            // Snap to the nearest position after drag
            SnapToNearest();

            // Resume auto-swiping after a short delay
            StartCoroutine(ResumeAutoSwipe());
        }

        private void SnapToNearestImmediate()
        {
            float nearest = float.MaxValue;

            for (int i = 0; i < positions.Length; i++)
            {
                float distance = Mathf.Abs(scrollRect.horizontalNormalizedPosition - positions[i]);
                if (distance < nearest)
                {
                    nearest = distance;
                    selectedIndex = i;
                }
            }

            // Immediately set the scroll position to the nearest position
            scrollRect.horizontalNormalizedPosition = positions[selectedIndex];
        }

        private void SnapToNearest()
        {
            if (isSnapping) return;

            float nearest = float.MaxValue;

            for (int i = 0; i < positions.Length; i++)
            {
                float distance = Mathf.Abs(scrollRect.horizontalNormalizedPosition - positions[i]);
                if (distance < nearest)
                {
                    nearest = distance;
                    selectedIndex = i;
                }
            }

            StartCoroutine(SnapToPosition(positions[selectedIndex]));
        }

        private IEnumerator SnapToPosition(float targetPosition)
        {
            isSnapping = true;

            while (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition) > 0.001f)
            {
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                    scrollRect.horizontalNormalizedPosition,
                    targetPosition,
                    Time.deltaTime * snapSpeed
                );
                yield return null;
            }

            scrollRect.horizontalNormalizedPosition = targetPosition;
            isSnapping = false;
        }

        private IEnumerator AutoSwipe()
        {
            while (true)
            {
                yield return new WaitForSeconds(autoSwipeInterval);

                if (!isDragging && !isSnapping)
                {
                    selectedIndex = (selectedIndex + 1) % positions.Length;
                    yield return StartCoroutine(SnapToPosition(positions[selectedIndex]));
                }
            }
        }

        private void StartAutoSwipe()
        {
            if (autoSwipeCoroutine == null)
            {
                autoSwipeCoroutine = StartCoroutine(AutoSwipe());
            }
        }

        private IEnumerator ResumeAutoSwipe()
        {
            yield return new WaitForSeconds(2f); // Delay before resuming auto-swipe
            StartAutoSwipe();
        }

        private void StopAutoSwipe()
        {
            if (autoSwipeCoroutine != null)
            {
                StopCoroutine(autoSwipeCoroutine);
                autoSwipeCoroutine = null;
            }
        }
    }
}
