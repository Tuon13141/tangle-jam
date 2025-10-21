using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorEffect : MonoBehaviour
{
    public enum Axis
    {
        Y = 0,
        X = 1,
    }

    [SerializeField] Renderer m_Renderer;
    [SerializeField] Vector4 titling;

    public float maxTitlingZ = 3f;
    public float speed = 3f;
    public Axis axis;

    Vector4 rootTitling;

    private void Awake()
    {
        titling = rootTitling = m_Renderer.sharedMaterial.GetVector("_BaseMap_ST");
    }

    private void Update()
    {
        titling += (axis == Axis.Y) ? new Vector4(0, 0, 0, Time.deltaTime * speed) : new Vector4(0, 0, Time.deltaTime * speed, 0);
        if (Mathf.Abs(titling.z) >= maxTitlingZ) titling = new Vector4(1, 1, 0, 0);
        m_Renderer.sharedMaterial.SetVector("_BaseMap_ST", titling);
    }

    private void OnDestroy()
    {
        m_Renderer.sharedMaterial.SetVector("_BaseMap_ST", rootTitling);
    }
}
