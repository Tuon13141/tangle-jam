using System;

namespace SonatSdkUltilities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class StopTime : MonoBehaviour
    {
        // Start is called before the first frame update
        private float _last;

        private void OnEnable()
        {
            _last = Time.timeScale;
            Time.timeScale = 0;
        }
        
        private void OnDisable()
        {
            Time.timeScale = _last;
        }
    }
}