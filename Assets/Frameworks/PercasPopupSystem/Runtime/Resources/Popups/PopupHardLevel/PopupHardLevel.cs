using System;
using System.Collections;
using UnityEngine;
using Percas.UI;

namespace Percas
{
    public class PopupHardLevel : PopupBase
    {
        private IEnumerator Close()
        {
            yield return new WaitForSeconds(1.25f);
            ServiceLocator.PopupScene.HidePopup(PopupName.HardLevelWarning, () =>
            {
                UIGameManager.CanShowTutorial = true;
            });
        }

        #region Public Methods
        public override void Show(object args = null, Action callback = null)
        {
            base.Show(args, callback);
            StartCoroutine(Close());
        }
        #endregion
    }
}
