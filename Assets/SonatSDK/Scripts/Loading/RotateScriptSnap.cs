using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScriptSnap : MonoBehaviour
{
    [SerializeField] private float time = 0.1f;
    [SerializeField] private float angle = -45f;
    private void OnEnable()
    {
        StartCoroutine(Rotate());
    }
 
    private IEnumerator Rotate()
    {

        while (true)
        {
            yield return new WaitForSecondsRealtime(time);
            transform.localEulerAngles += new Vector3(0,0,angle);
        }
    }
}
