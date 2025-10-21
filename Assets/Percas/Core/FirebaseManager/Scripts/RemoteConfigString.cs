using Firebase.RemoteConfig;
using UnityEngine;

namespace PercasSDK
{
    [CreateAssetMenu(menuName = "PercasSDK/Remote Configs/String")]
    public class RemoteConfigString : RemoteConfigValueSO
    {
        [Header("Default Value")]
        public string defaultValue;

        [Header("Current Value (Runtime)")]
        public string currentValue;

        private void OnEnable()
        {
            currentValue = defaultValue;
        }

        public override void UpdateValue(ConfigValue configValue)
        {
            if (!string.IsNullOrEmpty(configValue.StringValue))
            {
                currentValue = configValue.StringValue;
            }
            else
            {
                Debug.LogWarning($"[{remoteKey}] Using default string value: {defaultValue}");
                currentValue = defaultValue;
            }
        }
    }
}
