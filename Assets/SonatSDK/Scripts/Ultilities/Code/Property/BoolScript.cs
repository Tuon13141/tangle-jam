namespace SonatSdkUltilities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class BoolScript : MonoBehaviour
    {
        public abstract void OnChanged(int val);
    }
}