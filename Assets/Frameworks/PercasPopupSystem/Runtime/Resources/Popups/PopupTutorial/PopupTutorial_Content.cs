using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Percas.Data;
using Spine.Unity;

namespace Percas
{
    public class PopupTutorial_Content : MonoBehaviour
    {
        [SerializeField] Image tutorialImage;
        [SerializeField] TMP_Text tutorialMessage;
        [SerializeField] SkeletonGraphic tutorialAnimation;

        public void SetContent(TutorialDataSO tutorialData)
        {
            tutorialMessage.spriteAsset = tutorialData.spriteAsset;
            tutorialMessage.text = string.IsNullOrEmpty(tutorialData.messageWithoutImage) ? $"{tutorialData.message}" : $"{tutorialData.messageWithoutImage}";
            //tutorialImage.sprite = tutorialData.image;

            if (tutorialData.skeletonData != null)
            {
                tutorialAnimation.skeletonDataAsset = tutorialData.skeletonData;
                tutorialAnimation.Initialize(true);
                tutorialAnimation.AnimationState.SetAnimation(0, "idle", true);
            }
            else
            {
                tutorialImage.gameObject.SetActive(true);
                tutorialAnimation.gameObject.SetActive(false);
                tutorialImage.sprite = tutorialData.image;
            }
        }
    }
}
