using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

public class OnScreenDebugLogForUIDebugLog : ConsoleBase
{
    public static OnScreenDebugLogForUIDebugLog Instance;


    public bool  IsShow;

    public bool ShowButton = true;
    public bool autoHideOnDevice;
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        #if !UNITY_EDITOR
        if(autoHideOnDevice)
            gameObject.SetActive(false);
#endif
    }


    public static void Clear(string message)
    {
        if (Instance != null)
            UIDebugLog.Clear();
     
    }

    public void Show()
    {
        if (Instance != null)
            Instance.IsShow = !Instance.IsShow;
    }

	
    protected void OnGUI()
    {
        if (IsShow)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (this.Button("Clear"))
            {
                UIDebugLog.Clear();
            }
            if (this.Button("Close"))
            {
                IsShow = false;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (this.Button("Size +"))
            {
                DpiScalingFactorEditor = DpiScalingFactorEditor + 10;
                DpiScalingFactor =DpiScalingFactor + 10;
                FontScale = FontScale + 0.2f;
                FontScaleEditor = FontScaleEditor + 0.2f;
                Clear();
            }
            if (this.Button("Size -"))
            {
                DpiScalingFactorEditor = DpiScalingFactorEditor - 10;
                DpiScalingFactor =DpiScalingFactor - 10;
                FontScale = FontScale - 0.2f;
                FontScaleEditor = FontScaleEditor - 0.2f;
                Clear();

            }
            if (this.Button("Color"))
            {
                Nextcolor();
                Clear();
            }
            GUILayout.EndHorizontal();


#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 scrollPosition = this.ScrollPosition;
                scrollPosition.y += Input.GetTouch(0).deltaPosition.y;
                this.ScrollPosition = scrollPosition;
            }
#endif
            this.ScrollPosition = GUILayout.BeginScrollView(
                this.ScrollPosition,
                GUILayout.MinWidth(ConsoleBase.MainWindowFullWidth));

            GUI.enabled = false;

            GUILayout.TextArea(
                string.Join("\n", UIDebugLog.GetLogs()),
                this.TextStyle,
                GUILayout.ExpandHeight(true),
                GUILayout.MaxWidth(ConsoleBase.MainWindowWidth));
            GUI.enabled = true;

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        else if(ShowButton)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (this.Button("Show"))
            {
                IsShow = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}

