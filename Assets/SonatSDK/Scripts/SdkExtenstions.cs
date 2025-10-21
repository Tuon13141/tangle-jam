using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SdkExtenstions 
{
    public static bool HasDuplicate(this IList<int> arr)
    {
        for (var i = 0; i < arr.Count; i++)
        for (int j = i+1; j < arr.Count; j++)
            if (arr[i] == arr[j])
                return true;

        return false;
    }

    public static void SetDirty(this UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(obj);
#endif
    }

    public static string LowercaseFirstLetter(this string str)
    {
        return char.ToLower(str[0]) + str.Substring(1);
    }
    
    public static int GetEpochDate(DateTime date)
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0);
        return (int) (date - epochStart).TotalDays;
    }
    
    
    public static int GetEpochDate(Vector3Int date)
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0);
        return (int) (new  DateTime(date.z,date.y,date.x) - epochStart).TotalDays;
    }
    public static int GetEpochDate()
    {
        return GetEpochDate(DateTime.Today);
    }

}
