using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceUpdate : MonoBehaviour
{
    public static ForceUpdate instan;
    public GameObject popupForceUpdate;
    private PlayerPrefInt lastLevelCheck;
    private ForceUpdateData data;
    private Action callback;
    private bool updated;
    private const string AndroidAppURI = "http://play.google.com/store/apps/details?id={0}";
    private const string iOSAppURI = "https://apps.apple.com/app/id{0}";
    [Tooltip("iOS App ID (number), example: 1122334455")]
    public string iOSAppID = "";
    public GameObject closeBtn;
    public bool autoCheckOnStart;

    private void Awake()
	{
        instan = this;
	}

	// Start is called before the first frame update
	void Start()
    {
        popupForceUpdate.SetActive(false);
        lastLevelCheck = new PlayerPrefInt("Force_Update_Last_Level_Check");
        if (!FireBaseController.FireBaseRemoteReady)
        {
            if (SharedRemoteConfigController.OnInitialized != null)
                SharedRemoteConfigController.OnInitialized.Action += data => CheckForceUpdate();
        }
		else
		{
            CheckForceUpdate();
		}
    }

    private void CheckForceUpdate()
	{
        if(data == null)
		{
            string json = RemoteConfigKey.force_update.GetValueString();
            if (string.IsNullOrEmpty(json)) return;
            data = JsonConvert.DeserializeObject<ForceUpdateData>(json);
		}

        if (!data.set || !autoCheckOnStart) return;
        string currentVersion = Application.version;
        if (Version.Parse(currentVersion) < Version.Parse(data.version))
        {
            popupForceUpdate.SetActive(true);
            closeBtn.SetActive(data.canClose && data.interval != 0);
        }
        else
        {
            updated = true;
		}
	}

    public void OnStartLevel(int level, Action callback = null)
	{
        if (data != null || !data.set || updated) return;

        string currentVersion = Application.version;
        if (Version.Parse(currentVersion) >= Version.Parse(data.version))
        {
            updated = true;
            return;
        }

        if (popupForceUpdate.activeInHierarchy)
        {
            lastLevelCheck.Value = level;
            return;
        }

        if (data.interval != 0 && (level - lastLevelCheck.Value) % data.interval != 0) return;
        lastLevelCheck.Value = level;
        this.callback = callback;
        popupForceUpdate.SetActive(true);
        closeBtn.SetActive(data.canClose && data.interval != 0);
	}


    public void OnContinueClick()
    {
        if (data != null && data.interval != 0)
        {
            popupForceUpdate.SetActive(false);
            callback?.Invoke();
            callback = null;
        }
        string url = "";

#if UNITY_IOS
        if (!string.IsNullOrEmpty(iOSAppID))
        {
            url = string.Format(iOSAppURI, iOSAppID);
        } 
        else
        {
            Debug.LogError("Please set iOSAppID variable");
        }
#elif UNITY_ANDROID
            url = string.Format(AndroidAppURI, Application.identifier);
#endif

        Application.OpenURL(url);
    }

    public void OnCloseClick()
	{
        popupForceUpdate.SetActive(false);
        callback?.Invoke();
        callback = null;
	}

	//private void OnApplicationFocus(bool focus)
	//{
	//	if (focus && !updated && data != null && data.set && data.interval == 0)
	//	{
 //           string currentVersion = Application.version;
 //           if (Version.Parse(currentVersion) < Version.Parse(data.version))
 //           {
 //               popupForceUpdate.SetActive(true);
 //               closeBtn.SetActive(false);
 //               return;
 //           }
	//		else
	//		{
 //               popupForceUpdate.SetActive(false);
 //               updated = true;
	//		}
 //       }
	//}
}


public class ForceUpdateData
{
    public bool set;
    public int interval;
    public string version;
    public bool canClose;
}