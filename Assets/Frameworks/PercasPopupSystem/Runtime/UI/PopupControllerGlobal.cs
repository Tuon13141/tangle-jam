using UnityEngine;

namespace Percas
{
    public class PopupControllerGlobal : MonoBehaviour
    {
        [SerializeField] PopupController popupController;

        private void Awake()
        {
            ServiceLocator.RegisterPopupGlobal(popupController);
        }
    }
}
