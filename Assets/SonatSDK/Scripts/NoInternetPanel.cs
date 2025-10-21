using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoInternetPanel : MonoBehaviour
{
    public Button continueButton;
	public GameObject popup;

    // Start is called before the first frame update
    void Start()
    {
		popup.SetActive(false);
        continueButton.onClick.AddListener(OnContinueClick);
		Kernel.OnInternetDisconnected += OnNoInternet;
    }

	private void OnDestroy()
	{
		Kernel.OnInternetDisconnected -= OnNoInternet;
	}

	private void OnNoInternet()
	{
		popup.SetActive(true);
		Kernel.NeedPauseGame?.Invoke(true);
	}


	protected virtual void OnContinueClick()
	{
		popup.SetActive(false);
        Kernel.kernel.WaitCheckInternet();
		Kernel.NeedPauseGame?.Invoke(false);
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			if (popup.activeInHierarchy && Kernel.IsInternetConnection())
			{
				OnContinueClick();
			}
		}
	}

#if UNITY_ANDROID
	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			if (popup.activeInHierarchy && Kernel.IsInternetConnection())
			{
				OnContinueClick();
			}
		}
	}
#endif
}
