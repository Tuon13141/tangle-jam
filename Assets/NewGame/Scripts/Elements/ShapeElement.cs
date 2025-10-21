using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.WoolSort.Element
{
    using Game.WoolSort.Data;
    using Game.WoolSort.Controller;

    public class ShapeElement : MonoBehaviour
    {
        public int id;

        [Space]
        [SerializeField] ColorManager m_ColorManager;

        [Space]
        [SerializeField] SpriteRenderer m_Renderer;
        [SerializeField] SortingGroup m_SortingGroup;
        [SerializeField] Collider2D m_Collider;
        [SerializeField] Rigidbody2D m_Rigidbody;
        [SerializeField] Transform m_Holder;

        public Transform holder => m_Holder;

        [Space]
        public ColorType color;

        public Collider2D colliderShape => m_Collider;

        private void Start()
        {
            var wools = GetComponentsInChildren<WoolElement>();
            foreach (var wool in wools)
            {
                wool.hingeJoint2D.connectedBody = m_Rigidbody;
            }

            m_Rigidbody.simulated = true;
        }

        private void Update()
        {
            if (m_Rigidbody.transform.position.y <= -20)
            {
                gameObject.SetActive(false);
                LevelController.instance.gridController.CheckLayer();
            }
        }

        private void OnValidate()
        {
            ChangeColor(color);
        }

        public void ChangeColor(ColorType color)
        {
            this.color = color;
            m_Renderer.sharedMaterial = m_ColorManager.GetShapeMaterial(color);
        }
    }
}