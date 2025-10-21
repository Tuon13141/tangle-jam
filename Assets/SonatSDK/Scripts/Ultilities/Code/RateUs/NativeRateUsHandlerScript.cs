#if use_in_app_review
using Google.Play.Review;
#endif
using UnityEngine;

namespace SonatSdkUltilities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class NativeRateUsHandlerScript : SdkActionScript
    {
        public int decideRateUs = 4;
        public MonoBehaviour target;
        public override void StartAction()
        {
        }


#if use_in_app_review
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;

        public override void StartAction(int i)
        {
            if (i >= decideRateUs)
            {
                _reviewManager = new ReviewManager();
                if (target != null)
                    target.StartCoroutine(IeShowInAppReview());
                else
                    StartCoroutine(IeShowInAppReview());
                // go native
            }
            else
            {
                SdkRateUs.Instance.ShowFeedbackForm();
            }
        }

        IEnumerator IeShowInAppReview()
        {
            Debug.LogError("IeShowInAppReview");
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }

            _playReviewInfo = requestFlowOperation.GetResult();

            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            
            Debug.LogError("launchFlowOperation");

            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Debug.LogError("launchFlowOperation.Error "+launchFlowOperation.Error );
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }
            SdkRateUs.Instance.Close();
// The flow has finished. The API does not indicate whether the user
// reviewed or not, or even whether the review dialog was shown. Thus, no
// matter the result, we continue our app flow.
        }
#else
        public override void StartAction(int i)
        {
        }
#endif
    }
}