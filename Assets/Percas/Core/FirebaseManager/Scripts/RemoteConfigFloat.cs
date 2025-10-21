using Firebase.RemoteConfig;
using UnityEngine;

namespace PercasSDK
{
    [CreateAssetMenu(menuName = "PercasSDK/Remote Configs/Float")]
    public class RemoteConfigFloat : RemoteConfigValueSO
    {
        [Header("Default Value")]
        public float defaultValue;

        [Header("Current Value (Runtime)")]
        public float currentValue;

        private void OnEnable()
        {
            currentValue = defaultValue;
        }

        public override void UpdateValue(ConfigValue configValue)
        {
            if (float.TryParse(configValue.StringValue, out float value))
            {
                currentValue = value;
            }
            else
            {
                Debug.LogWarning($"[{remoteKey}] Using default float value: {defaultValue}");
                currentValue = defaultValue;
            }
        }
    }
}
