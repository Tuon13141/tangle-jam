using System;
using System.Text;
using sonat_sdk.Scripts;
using UnityEngine;
using Random = System.Random;

namespace SonatARM
{
    public class Util {
 
        private static readonly string AUTO_ID_ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly int AUTO_ID_LENGTH = 24;
        private static readonly Random rand = new Random();
 
        public static string generateSonatARMUserID() {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();
 
            return unixTimeMilliseconds.ToString() + "-" + _AutoId();
        }
 
 
        private static string _AutoId() {
            StringBuilder builder = new StringBuilder();
            int maxRandom = AUTO_ID_ALPHABET.Length;
            for (int i = 0; i < AUTO_ID_LENGTH; i++) {
                builder.Append(AUTO_ID_ALPHABET[rand.Next(maxRandom)]);
            }
            return builder.ToString();
        }
    }
}

public static class SonatExtenstions
{
    public static void SetTestDeviceAdmob(bool remove)
    {
        UIDebugLog.Log(GetAdmobTestDeviceID().ToUpper());
        if(remove)
            PlayerPrefs.DeleteKey(PlayerPrefEnum.test_device_id.ToString());
        else
            PlayerPrefs.SetString(PlayerPrefEnum.test_device_id.ToString(),GetAdmobTestDeviceID().ToUpper());
    }
    
    public static string GetAdmobTestDeviceID()
    {
#if UNITY_ANDROID && ! UNITY_EDITOR
        // Get Android ID
        AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
 
        string android_id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
 
        // Get bytes of Android ID
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(android_id);
 
        // Encrypt bytes with md5
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
 
        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";
 
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
 
        string device_id = hashString.PadLeft(32, '0');
 
        return device_id;
        
#else 
        return "";
#endif
    }
}