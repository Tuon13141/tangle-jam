using Firebase.RemoteConfig;
using UnityEngine;

namespace PercasSDK
{
    public abstract class RemoteConfigValueSO : ScriptableObject
    {
        [Header("Remote Config Key")]
        [Tooltip("This key is used to look up the remote value in Firebase Remote Config.")]
        public string remoteKey;

        /// <summary>
        /// Update this asset’s value based on the Firebase Remote Config value.
        /// </summary>
        public abstract void UpdateValue(ConfigValue configValue);
    }
}
