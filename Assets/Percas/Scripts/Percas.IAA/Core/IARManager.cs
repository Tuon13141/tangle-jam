using System.Collections;
using UnityEngine;
#if UNITY_ANDROID
using Google.Play.Review;
#endif
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Percas
{
    public class IARManager : MonoBehaviour
    {
#if UNITY_ANDROID
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;

        private void Awake()
        {
            ActionEvent.OnCallGoogleInAppReviews += CallInAppReviews;
        }

        private void OnDestroy()
        {
            ActionEvent.OnCallGoogleInAppReviews -= CallInAppReviews;
        }

        private void CallInAppReviews()
        {
            StartCoroutine(AndroidRequestReviews());
        }

        IEnumerator AndroidRequestReviews()
        {
            _reviewManager = new ReviewManager();

            // Request a ReviewInfo object
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }
            _playReviewInfo = requestFlowOperation.GetResult();

            // Launch the in-app review flow
            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }
            // The flow has finished. The API does not indicate whether the user
            // reviewed or not, or even whether the review dialog was shown. Thus, no
            // matter the result, we continue our app flow.
        }
#endif

#if UNITY_IOS
        private bool reviewRequested = false;

        private void Awake()
        {
            ActionEvent.OnCallGoogleInAppReviews += iOSRequestReviews;
        }

        private void OnDestroy()
        {
            ActionEvent.OnCallGoogleInAppReviews -= iOSRequestReviews;
        }

        private void iOSRequestReviews()
        {
            if (!reviewRequested)
            {
                bool popupShown = Device.RequestStoreReview();
                if (popupShown)
                {
                    Debug.Log("Review popup displayed.");
                    reviewRequested = true;
                }
                else
                {
                    Debug.Log("Review popup not displayed. Ensure StoreKit is linked and iOS version is supported.");
                }
            }
        }
#endif
    }
}
