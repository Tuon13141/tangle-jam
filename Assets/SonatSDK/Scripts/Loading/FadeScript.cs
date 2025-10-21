using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    [SerializeField] private Image img;
    [SerializeField] private float delay;
    [SerializeField] private float time = 1;
    [SerializeField] private Vector2 fromTo = new Vector2(0.1f,1f);
    [SerializeField] private Vector2 delayEach = new Vector2(0.2f,.2f);
    
    private void OnEnable()
    {
        var color = img.color;
        color = new Color(color.r,color.g,color.b,0);
        img.color = color;
        StartCoroutine(Fade());
    }
 
    private IEnumerator Fade()
    {
        var color = img.color;

        yield return new WaitForSecondsRealtime(delay);
        float t = 0f;
        bool reverse = false;
        while (t < time)
        {
            t += Time.deltaTime;
            if (t >= time)
            {
                t -= time;
                reverse = !reverse;
                yield return new WaitForSecondsRealtime(reverse ? delayEach.x : delayEach.y);
            }

            var dt = reverse ? 1 - t / time : t / time;
            color = new Color(color.r,color.g,color.b, Mathf.Lerp(fromTo.x,fromTo.y,dt));
            img.color = color;
            yield return null;
        }
    }
}
