using Firebase.RemoteConfig;
using UnityEngine;

namespace PercasSDK
{
    [CreateAssetMenu(menuName = "PercasSDK/Remote Configs/Int")]
    public class RemoteConfigInt : RemoteConfigValueSO
    {
        [Header("Default Value")]
        public int defaultValue;

        [Header("Current Value (Runtime)")]
        public int currentValue;

        private void OnEnable()
        {
            // Initialize with the default value.
            currentValue = defaultValue;
        }

        public override void UpdateValue(ConfigValue configValue)
        {
            if (int.TryParse(configValue.StringValue, out int value))
            {
                currentValue = value;
            }
            else
            {
                Debug.LogWarning($"[{remoteKey}] Using default int value: {defaultValue}");
                currentValue = defaultValue;
            }
        }
    }
}
