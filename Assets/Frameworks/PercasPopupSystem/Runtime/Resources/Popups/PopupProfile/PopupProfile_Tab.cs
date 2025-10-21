using UnityEngine;
using UnityEngine.UI;
using Percas.UI;

namespace Percas
{
    public class PopupProfile_Tab : MonoBehaviour
    {
        [SerializeField] ButtonBase button;
        [SerializeField] int id;
        [SerializeField] Image m_buttonColor;

        private void Awake()
        {
            button.SetPointerClickEvent(ToggleTab);
        }

        private void ToggleTab()
        {
            PopupProfile.OnUpdateUI?.Invoke(id);
        }

        public void UpdateUI(Sprite spriteColor)
        {
            m_buttonColor.sprite = spriteColor;
        }
    }
}
