using System;
using System.Collections;
using Firebase;
using Firebase.Analytics;
using Percas;
using UnityEngine;

namespace PercasSDK
{
    public class FirebaseManager : MonoBehaviour
    {
        /// <summary>
        /// https://developers.google.com/unity/archive#firebase
        /// </summary>
        [SerializeField] RemoteConfigManager remoteConfigManager;

        public static Action<string, Parameter[]> OnLogEvent;
        public static Action<string, string> OnSetUserProperty;

        public static bool IsInitialized { get; private set; }
        public static RemoteConfigManager RemoteConfig;

        private void Start()
        {
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
            yield return new WaitUntil(() => dependencyTask.IsCompleted);
            if (dependencyTask.Result != DependencyStatus.Available)
            {
                Debug.LogError("Firebase dependencies not available: " + dependencyTask.Result);
                // GameConfig.Instance.LoadRemoteLevelData();
                GameConfig.Instance.LoadRemoteData();
                yield break;
            }
            IsInitialized = true;
            RemoteConfig = remoteConfigManager;
            RemoteConfig.Initialize();
        }
    }
}
