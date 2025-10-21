using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UnityEditor;
using UnityEngine;
using LogParameter = Sonat.LogParameter;

public class SdkTrackingMonitorWindow : EditorWindow
{
    [MenuItem("SonatSDK/" + nameof(SdkTrackingMonitorWindow))]
    public static void ShowWindow()
    {
        GetWindow(typeof(SdkTrackingMonitorWindow));
    }
    
    protected Vector2 ScrollPosition;
    protected GUIStyle Style;
    protected GUIStyle Style2;
    protected GUIStyle StyleRed;
    protected int[] EnumValues;

    protected virtual void StartHorizon(Action action)
    {
        //*
        EditorGUILayout.BeginHorizontal();
        if (action != null)
            action.Invoke();
        EditorGUILayout.EndHorizontal();
        //*
    }

    
    public virtual void OnGUI()
    {
        OnTrackingMonitor();
    }
    
    private void OnTrackingMonitor()
    {
        StartHorizon(() =>
        {
            if (GUILayout.Button("Clear"))
            {
                Sonat.BaseSonatAnalyticLog.ClearEditorLog();
                Repaint();
            }
            if (GUILayout.Button("Test"))
            {
                #if UNITY_EDITOR
                new CustomSonatLog("this_is_a_test_log", new List<Sonat.LogParameter>()
                {
                    new Sonat.LogParameter("content","nothing")
                }).Post();
                #endif
                Repaint();
            }
        });

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        GUI.enabled = false;
        EditorGUILayout.TextArea(
            string.Join("\n", Sonat.BaseSonatAnalyticLog.eventsForEdiorWindow.ToArray()),
            GUILayout.ExpandHeight(true));
        GUI.enabled = true;
        EditorGUILayout.EndScrollView();
    }
    
    Vector2 scrollPosition;

    
}