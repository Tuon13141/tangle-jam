using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Percas.UI;
using Percas.Data;

namespace Percas
{
    public class TutorialMessage : MonoBehaviour
    {
        [SerializeField] ButtonBase buttonClaim;
        [SerializeField] GameObject container, autoClaimBox;
        [SerializeField] TMP_Text textMessage, textAutoClaim;

        public static Action<TutorialDataSO> OnShow;
        public static Action OnHide;

        private void Awake()
        {
            OnShow += Show;
            OnHide += Hide;
        }

        private void OnDestroy()
        {
            OnShow -= Show;
            OnHide -= Hide;
        }

        private void Show(TutorialDataSO tutorialData)
        {
            textMessage.spriteAsset = tutorialData.spriteAsset;
            textMessage.text = tutorialData.showType == TutorialShowType.Popup ? $"{tutorialData.message}" : string.IsNullOrEmpty(tutorialData.messageWithoutImage) ? $"{tutorialData.message}" : $"{tutorialData.messageWithoutImage}";
            container.SetActive(true);
            autoClaimBox.SetActive(tutorialData.autoClose);
            if (tutorialData.autoClose) StartCoroutine(UpdateAutoClaimText(tutorialData));
            buttonClaim.gameObject.SetActive(tutorialData.showCloseButton);
            buttonClaim.SetPointerClickEvent(Claim);
        }

        private IEnumerator UpdateAutoClaimText(TutorialDataSO tutorialData)
        {
            float remainingTime = tutorialData.autoCloseIn;
            while (remainingTime > 0f)
            {
                int displayTime = Mathf.CeilToInt(remainingTime);
                textAutoClaim.text = tutorialData.isBooster ? $"Claim {displayTime}" : $"Close {displayTime}";
                remainingTime -= Time.deltaTime;
                yield return null;
            }
        }

        private void Claim()
        {
            TutorialManager.OnClaim?.Invoke();
        }

        private void Hide()
        {
            container.SetActive(false);
        }
    }
}
