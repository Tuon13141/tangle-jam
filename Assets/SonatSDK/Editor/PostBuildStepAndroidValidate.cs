using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

#if UNITY_ANDROID
#if UNITY_2018_1_OR_NEWER
public class PostBuildStepAndroidValidate : IPreprocessBuildWithReport
#else
public class PostBuildStepAndroidValidate : IPreprocessBuild
#endif
{

    public int callbackOrder { get { return 1; } }

#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild(BuildReport report)
#else
    public void OnPreprocessBuild(BuildTarget target, string path)
#endif
    {
        ValidateAutomaticReporting();
    }

    private void ValidateAutomaticReporting()
    {
        var asset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Plugins/Android/AndroidManifest.xml");
        if (asset != null)
        {
            var disable_automatic_reporting =
                asset.text.Contains("google_analytics_automatic_screen_reporting_enabled");

            if (!disable_automatic_reporting)
            {
                Debug.LogError("SONAT ERROR : add google_analytics_automatic_screen_reporting_enabled to AndroidManifest");
                Debug.LogError("<meta-data android:name=\"google_analytics_automatic_screen_reporting_enabled\" android:value=\"false\" />");
                throw new BuildFailedException("SONAT : Build was canceled because not disable_automatic_reporting.");
            }
            else
            {
                Debug.Log("good");
            }
        }
    }
}
#endif