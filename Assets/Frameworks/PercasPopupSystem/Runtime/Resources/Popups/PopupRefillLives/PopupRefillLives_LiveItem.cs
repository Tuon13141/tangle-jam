using UnityEngine;

namespace Percas
{
    public class PopupRefillLives_LiveItem : MonoBehaviour, IActivatable
    {
        [SerializeField] int index = 0;
        [SerializeField] GameObject active, inactive;

        private void Awake()
        {
            TimeManager.OnTick += HandleDisplay;
        }

        private void OnDestroy()
        {
            TimeManager.OnTick -= HandleDisplay;
        }

        public void Activate()
        {
            HandleDisplay();
        }

        public void Deactivate() { }

        private void HandleDisplay()
        {
            active.SetActive(GameLogic.CurrentLive >= index + 1);
            inactive.SetActive(GameLogic.CurrentLive < index + 1);
        }
    }
}
