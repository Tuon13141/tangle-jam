using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine.UI;


public class SonatToolWindowFunctionEditor : EditorWindow
{
    private static string[] add_symbols = new string[]
    {
        "use_admob",
        "use_firebase",
        "use_appflyer",
        "use_iap",
        "use_facebook",
        "use_firebase_message",
    };
   

    private static string[] add_symbolsAndSpine
    {
        get
        {
            var list = add_symbols.ToList();
            list.Add("USE_SPINE");
            return list.ToArray();
        }
    }
    
    
    [MenuItem("SonatSDK/"+nameof(AddSymbol))]
    public static void AddSymbol()
    {
        var symbols =
            #if UNITY_ANDROID
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(';');
            #else 
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(';');
            #endif

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,add_symbols);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS,add_symbols);
    }
    
    [MenuItem("SonatSDK/"+nameof(AddSymbol)+"+Spine")]
    public static void AddSymbolAndSpine()
    {
        var symbols =
#if UNITY_ANDROID
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(';');
#else 
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(';');
#endif

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,add_symbolsAndSpine);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS,add_symbolsAndSpine);
    }
    
     
    [MenuItem("SonatSDK/"+nameof(DeleteAllData))]
    public static void DeleteAllData()
    {
      PlayerPrefs.DeleteAll();
    }

    
    [MenuItem("SonatSDK/"+nameof(DummyService))]
    public static void DummyService()
    {
        var symbols =
#if UNITY_ANDROID
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(';').ToList();
#else 
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(';').ToList();
#endif
        
        foreach (var symbol in symbols)
        {
            Debug.Log(symbol);
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,new[]{"dummy_log"});
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS,new[]{"dummy_log"});
    }
    
        
    [MenuItem("SonatSDK/"+nameof(DummyService) +"+USE_SPINE")]
    public static void DummyServiceUSE_SPINE()
    {
        var symbols =
#if UNITY_ANDROID
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(';').ToList();
#else 
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(';').ToList();
#endif
        
        foreach (var symbol in symbols)
        {
            Debug.Log(symbol);
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,new[]{"dummy_log;USE_SPINE"});
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS,new[]{"dummy_log;USE_SPINE"});
    }


//    [MenuItem("Tools/SonatSDK/Setting Window")]
//    static void ShowEditor()
//    {
//        EditorWindow.GetWindow(typeof(SonatTestingSettingEditor), false, "SonatTestingSettingEditor");
//    }


    void OnSelectionChange()
    {
        this.Repaint();
    }


    private Vector2 _scrollPos;


    void OnGUI()
    {
        Event current = Event.current;
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
//
//        TestingSettings.TurnOffBanner =
//            EditorGUILayout.Toggle(nameof(TestingSettings.TurnOffBanner), TestingSettings.TurnOffBanner);
//        TestingSettings.TurnOffOnFocusAds = EditorGUILayout.Toggle(nameof(TestingSettings.TurnOffOnFocusAds),
//            TestingSettings.TurnOffOnFocusAds);
//        
//        TestingSettings.SkipRewarded = EditorGUILayout.Toggle(nameof(TestingSettings.SkipRewarded),
//            TestingSettings.SkipRewarded);
//
//        if (GUILayout.Button("Delete All Data"))
//        {
//            PlayerPrefs.DeleteAll();
//        }
        EditorGUILayout.EndScrollView();
    }


    public static string ByteArrayToString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

    public static byte[] StringToByteArray(String hex)
    {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    private void DrawTexture(Texture2D texture, Rect rect)
    {
        GUI.color = Color.white;
        GUI.DrawTexture(rect, texture, ScaleMode.StretchToFill);
    }

    private void DrawTexture(Texture2D texture, Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, texture, ScaleMode.StretchToFill);
        GUI.color = Color.white;
    }

#if UNITY_EDITOR
    [MenuItem("SonatSDK/" + nameof(OpenDocumentation))]
    public static void OpenDocumentation()
    {
        Application.OpenURL("https://docs.google.com/document/d/1pkV4rT89LGEddO0rAeoMybalsnGc1wJiMNPuZJLnzD0/edit?usp=drive_link");
    }
#endif


    #region Styles

    GUIStyle _level1
    {
        get
        {
            return new GUIStyle()
            {
                padding = new RectOffset(10, 0, 0, 0),
                fontStyle = FontStyle.Bold,
            };
        }
    }

    GUIStyle _level2Title
    {
        get
        {
            return new GUIStyle()
            {
                padding = new RectOffset(20, 0, 5, 0),
                fontStyle = FontStyle.Bold
            };
        }
    }

    GUIStyle _level2
    {
        get
        {
            return new GUIStyle()
            {
                padding = new RectOffset(30, 0, 5, 0),
                //fontStyle = FontStyle.Bold
            };
        }
    }

    GUILayoutOption[] _baseLayout
    {
        get { return new GUILayoutOption[] {GUILayout.Width(100f)}; }
    }

    #endregion
}