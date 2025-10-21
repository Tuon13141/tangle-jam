namespace SonatSdkUltilities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class IntScript : MonoBehaviour
    {
        public enum CompareType
        {
            GreaterOrEqual,
            SmallerOrEqual
        }

        public abstract void OnChanged(int val);
    }
}