using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class SonatHelper
{
    // Start is called before the first frame update

    public static void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();

        Type fileHandlerType = Type.GetType("FileHandler");
        if (fileHandlerType != null)
        {
            Debug.Log(fileHandlerType);
            var methodOutput = fileHandlerType.GetMethod("DeleteAll", BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, null);
        }
    }

  
}