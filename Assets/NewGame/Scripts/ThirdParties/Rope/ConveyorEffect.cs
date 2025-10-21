using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.WoolSort
{
    public class ConveyorEffect : MonoBehaviour
    {
        public enum Axis
        {
            Y = 0,
            X = 1,
        }

        [SerializeField] Renderer m_Renderer;
        Vector4 titling;

        public float maxTitlingZ = 3f;
        public float speed = 3f;
        public Axis axis;

        MaterialPropertyBlock mpb;
        private void Awake()
        {
            titling = new Vector4(1, 1, 0, 0);

            mpb = new MaterialPropertyBlock();
        }

        private void Update()
        {
            titling += (axis == Axis.Y) ? new Vector4(0, 0, 0, Time.deltaTime * speed) : new Vector4(0, 0, Time.deltaTime * speed, 0);
            if (Mathf.Abs(titling.z) >= maxTitlingZ) titling = new Vector4(1, 1, 0, 0);

            mpb.SetVector("_BaseMap_ST", titling);

            m_Renderer.SetPropertyBlock(mpb, 0);
        }
    }
}