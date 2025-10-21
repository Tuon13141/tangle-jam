using UnityEngine;
using Percas.UI;

namespace Percas
{
    public enum RateStarStatus
    {
        ACTIVE,
        INACTIVE,
    }

    public class RateStar : MonoBehaviour, IActivatable
    {
        [SerializeField] private int starID;
        [SerializeField] private RateStarStatus starStatus;
        [SerializeField] private GameObject m_activeStar, m_inactiveStar;
        [SerializeField] ButtonBase buttonStar;

        private void Awake()
        {
            PopupCallToRate.OnUpdateStatus += UpdateStarDisplay;
        }

        private void OnDestroy()
        {
            PopupCallToRate.OnUpdateStatus -= UpdateStarDisplay;
        }

        public void Activate()
        {
            buttonStar.SetPointerClickEvent(OnClick);
            starStatus = RateStarStatus.INACTIVE;
            UpdateStarDisplay(0);
        }

        public void Deactivate() { }

        private void OnClick()
        {
            PopupCallToRate.CurrentStar = starID;
            if (starStatus == RateStarStatus.ACTIVE) return;
            PopupCallToRate.OnUpdateStatus?.Invoke(starID);
        }

        private void UpdateStarDisplay(int currentStarID)
        {
            m_activeStar.SetActive(currentStarID >= starID);
            m_inactiveStar.SetActive(currentStarID < starID);
            if (currentStarID >= 1) PopupCallToRate.OnShowButtonRate?.Invoke(true);
        }
    }
}
