using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UnityEngine;

public interface ILogParameterValueRetriever
{
//    int Level { get; }
    int LevelDisplay { get; }
    string Mode { get; }
}

// DTODO implement this
public interface ILogParameterValueRetrieverExtended : ILogParameterValueRetriever
{
    int Phase { get; }
    string Placement { get; }
}


public class DummyLogParameterValueRetriever : ILogParameterValueRetriever
{
    public static int UserPropertyLevel
    {
        get => PlayerPrefs.GetInt("user_property_level");
        set => PlayerPrefs.SetInt("user_property_level",value);
    }
    
    public static string UserPropertyMode
    {
        get => PlayerPrefs.GetString("user_property_mode","classic");
        set => PlayerPrefs.SetString("user_property_mode",value);
    }
    
    public int Level => UserPropertyLevel;
    public int LevelDisplay => UserPropertyLevel;
    public string Mode => UserPropertyMode;
}

public static class SonatLogCreator
{
    public static SonatLogShowInterstitial CreateDefaultLogInterstitial(this string placement)
    {
        return new SonatLogShowInterstitial()
        {
            placement = placement,
            level = Kernel.ParameterValueRetriever.LevelDisplay,
            mode = Kernel.ParameterValueRetriever.Mode,
        };
    }
    
    public static SonatLogShowInterstitial CreateDefaultLogInterstitial(this ParameterValue placement)
    {
        return CreateDefaultLogInterstitial(placement.ToString());
    }
    
    public static SonatLogVideoRewarded CreateDefaultLogRewarded(string placement,string itemType,string itemId,LogParameter[] extra = null)
    {
        var log =  new SonatLogVideoRewarded()
        {
            placement = placement,
            level = Kernel.ParameterValueRetriever.LevelDisplay,
            mode = Kernel.ParameterValueRetriever.Mode,
            item_type = itemType,
            item_id = itemId,
        };
#if use_firebase
        if (extra != null)
            log.SetExtraParameter(Convert(extra).ToArray());
#endif
        return log;
    }

    private static IEnumerable<Sonat.LogParameter> Convert(LogParameter[] input)
    {
        foreach (var logParameter in input)
        {
            yield return new Sonat.LogParameter(logParameter.stringKey,
                logParameter.boolValue,
                logParameter.intValue,
                logParameter.floatValue,
                logParameter.stringValue,
                (Sonat.LogParameter.ParamType)logParameter.type);
        }
    }
    
//    public static SonatLogVideoRewarded CreateDefaultLogRewarded(this ParameterValue placement,string itemType,string itemId)
//    {
//        return CreateDefaultLogRewarded(placement.ToString(),itemType,itemId);
//    }
}