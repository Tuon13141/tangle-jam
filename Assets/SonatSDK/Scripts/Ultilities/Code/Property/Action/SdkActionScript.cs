namespace SonatSdkUltilities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class SdkActionScript : MonoBehaviour
    {
        public abstract void StartAction();
        public abstract void StartAction(int i);
    }
}