using System;

namespace SonatSdkUltilities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ActionScriptScaleIn : SdkActionScript
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private bool startOnEnable;

        private void OnEnable()
        {
            if (startOnEnable)
            {
                StartAction();
            }
        }

        public override void StartAction()
        {
            StartCoroutine(IeAnim());
        }

        private IEnumerator IeAnim()
        {
            transform.localScale = Create(curve.Evaluate(0));
            float t = 0;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
                transform.localScale = Create(curve.Evaluate(t / duration));
            }
        }

        private Vector3 Create(float f) => new Vector3(f, f, f);

        public override void StartAction(int i)
        {
        }
    }
}