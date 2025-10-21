using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonSampleCreator : MonoBehaviour
{
   public IntByLevelCollection interstitialTimeGapByLevel;
   public StringByLevelCollection rewardedAdIdByLevel;

   [MyButtonInt(nameof(LogTimeGap))] public int test1;

   public void LogTimeGap()
   {
      Debug.Log(JsonUtility.ToJson(interstitialTimeGapByLevel));
      Debug.Log(JsonUtility.ToJson(rewardedAdIdByLevel));
   }
}
