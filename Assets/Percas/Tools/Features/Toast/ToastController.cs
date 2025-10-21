using UnityEngine;

namespace Percas
{
    public class ToastController : MonoBehaviour
    {
        [SerializeField] RectTransform rtToastController;
        [SerializeField] GameObject toastPanelPrefab;

        private bool IsShowing;

        private void Awake()
        {
            ActionEvent.OnShowToast += ShowToast;
        }

        private void OnDestroy()
        {
            ActionEvent.OnShowToast -= ShowToast;
        }

        private void ShowToast(string message)
        {
            if (IsShowing) return;
            IsShowing = true;

            GameObject toast = SimplePool.Spawn(toastPanelPrefab, Vector3.zero, Quaternion.identity);
            toast.transform.SetParent(rtToastController, false);

            ToastPanel toastPanel = toast.GetComponent<ToastPanel>();
            toastPanel.Show();
            toastPanel.Display(message, () =>
            {
                IsShowing = false;
            });
        }
    }
}
