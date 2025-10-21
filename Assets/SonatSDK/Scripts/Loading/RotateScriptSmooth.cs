using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScriptSmooth : MonoBehaviour
{
    private Transform _transform;

    [MyButton(nameof(Start))] [SerializeField]
    private int abc;

    public Vector3 rotateVector = new Vector3(0, 0,120);

    public void Start()
    {
        _transform = transform;
    }

    private void OnEnable()
    {
        _transform = transform;
        //   _transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        _transform.localEulerAngles += Time.unscaledDeltaTime * rotateVector;
    }
}
