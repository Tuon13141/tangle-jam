using UnityEngine;

namespace Percas
{
    public class PopupControllerScene : MonoBehaviour
    {
        [SerializeField] PopupController popupController;

        private void Awake()
        {
            ServiceLocator.RegisterPopupScene(popupController);
        }
    }
}
