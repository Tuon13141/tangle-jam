#if UNITY_EDITOR && use_iron_source
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IronSourceTestAds : MonoBehaviour
{
    public static IronSourceTestAds instan;
    public GameObject bannerObj;
    public GameObject interObj;
    public GameObject rewardObj;
    public delegate void IronSourceAdCallback(IronSourcePlacement a, IronSourceAdInfo b);
    public delegate void IronSourceAdInfoCallback(IronSourceAdInfo b);

    private IronSourceAdInfoCallback onInterShowing, onInterClosed, onRewardShowing, onRewardClosed;
    private IronSourceAdCallback onRewardReceived;


    private void Awake()
	{
        DontDestroyOnLoad(this.gameObject);
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }


    public static void Create(IronSourceAdInfoCallback interShow, IronSourceAdInfoCallback interClose, IronSourceAdInfoCallback rewardShow
        , IronSourceAdInfoCallback rewardClose, IronSourceAdCallback rewardReceive)
	{
        if (instan != null) return;
        GameObject pref = Resources.Load<GameObject>("IronSourceTestAds");
        instan = Instantiate(pref).GetComponent<IronSourceTestAds>();
        instan.onInterShowing = interShow;
        instan.onInterClosed = interClose;
        instan.onRewardShowing = rewardShow;
        instan.onRewardClosed = rewardClose;
        instan.onRewardReceived = rewardReceive;
	}

    public void ShowInter()
	{
        interObj.SetActive(true);
        onInterShowing?.Invoke(null);
	}

    public void CloseInter()
	{
        interObj.SetActive(false);
        onInterClosed?.Invoke(null);
	}

    public void ShowBanner()
	{
        bannerObj.SetActive(true);
	}

    public void HideBanner()
	{
        bannerObj.SetActive(false);
	}

    public void DestroyBanner()
	{
        bannerObj.SetActive(false);
	}

    public void ShowReward()
	{
        rewardObj.SetActive(true);
        onRewardShowing?.Invoke(null);
	}

    public void CloseReward()
	{
        rewardObj.SetActive(false);
        onRewardClosed?.Invoke(null);
	}

    public void ClaimReward()
	{
        onRewardReceived?.Invoke(null, null);
        CloseReward();
        

    }
    
}

#endif