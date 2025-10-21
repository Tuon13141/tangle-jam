using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class UserSegment
{
    private static PlayerPrefString campaignType;
    private static PlayerPrefString userSegmentSave;
    public static UserCampaignSegment userCampaignSegment;
    public static UserPaySegment userPaySegment;

    public static void InitOnStart()
    {
        campaignType = new PlayerPrefString("SONAT_CAMPAIGN_TYPE", "");
        userSegmentSave = new PlayerPrefString("SONAT_USER_SEGMENT", "");

        string paySegmentString = RemoteConfigKey.payer_segment.GetValueString();
        if (!string.IsNullOrEmpty(paySegmentString))
        {
            userPaySegment = Enum.Parse<UserPaySegment>(paySegmentString);
        }
        
        if(!string.IsNullOrEmpty(userSegmentSave.Value))
        {
            userCampaignSegment = Enum.Parse<UserCampaignSegment>(userSegmentSave.Value);
        }
    }

    public static void SetUserSegment(string campaign_type, string campaignFull)
    {
        campaignType.Value = campaign_type;
        string json = RemoteConfigKey.campaign_segment.GetValueString();
        if (string.IsNullOrEmpty(json))
        {
            json = Resources.Load<TextAsset>("campaign_segment").text;
        }

        if (string.IsNullOrEmpty(json)) return;
        Dictionary<UserCampaignSegment, List<string>> campaignDic =
            JsonConvert.DeserializeObject<Dictionary<UserCampaignSegment, List<string>>>(json);

        campaignFull = campaignFull.ToLower();
        foreach (var campaignCollection in campaignDic)
        {
            foreach (var camp in campaignCollection.Value)
            {
                if (campaignFull.Contains(camp.ToLower()))
                {
                    userCampaignSegment = campaignCollection.Key;
                    userSegmentSave.Value = userCampaignSegment.ToString();
                    return;
                }
            }
        }
    }

    public static void SetUserCampaignSegment(UserCampaignSegment userCampaignSegment)
    {
        userSegmentSave.Value = userCampaignSegment.ToString();
    }

    public static void SetUserPaySegment(UserPaySegment _userPaySegment)
    {
        userPaySegment = _userPaySegment;
    }
}

public enum UserCampaignSegment
{
    Organic,
    IAA,
    IAP,
    Hybrid
}

public enum UserPaySegment
{
    None,
    Minows,
    SpecialMinows,
    Dolphin,
    DolphinNoads,
    MidDolphin,
    DolphinRetarget,
    Whale,
    WhaleRetarget1,
    WhaleRetarget2,
}
