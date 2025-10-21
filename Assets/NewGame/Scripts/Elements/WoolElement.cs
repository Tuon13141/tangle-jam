using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using DG.Tweening;

namespace Game.WoolSort.Element
{
    using Game.WoolSort.Data;
    using Game.WoolSort.Controller;

    public enum WoolStatus
    {
        Enable = 0,
        Disable = 1,
        Mystery = 2,

        Hide = 6,
        Transfer = 7,
        InBar = 8,
        Completed = 9,
    }

    public class WoolElement : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] ColorManager m_ColorManager;

        [Space]
        [SerializeField] Transform m_Pivot;
        [SerializeField] SpriteRenderer m_Renderer;
        [SerializeField] Collider2D m_Collider;
        [SerializeField] HingeJoint2D m_HingeJoint;

        [Space]
        public WoolStatus status;
        public ColorType color = ColorType.Blue;
        public bool isComplete;

        [Space]
        public ShapeElement shapeElement;

        public Transform pivot => m_Pivot;
        public HingeJoint2D hingeJoint2D => m_HingeJoint;

        void Awake()
        {

        }

        public void OnValidate()
        {
            ChangeColor(color);
        }

        [ContextMenu("ResetTransform")]
        void ResetTransform()
        {
            transform.rotation = Quaternion.identity;
        }

        public void Setup()
        {

        }

        public void ChangeColor(ColorType color)
        {
            //if (this.color == color) return;

            this.color = color;

            m_Renderer.sprite = m_ColorManager.GetWoolSprite(color);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isComplete) return;

            if (CheckEnable())
            {
                AddToSlot();
            }
            else
            {
                m_Renderer.transform.DOPunchRotation(Vector3.forward * 20, 0.3f).OnComplete(() => m_Renderer.transform.DORotate(Vector3.zero, 0.1f));
            }
            //switch (status)
            //{
            //    case WoolStatus.Enable:
            //        AddToSlot();
            //        break;

            //    case WoolStatus.Disable:
            //        break;
            //    case WoolStatus.Mystery:
            //        break;
            //}
        }

        public void AddToSlot()
        {
            Debug.Log("AddToSlot");
            LevelController.instance.slotController.AddWoolToSlot(this);
        }

        public bool CheckEnable()
        {
            var group = GetComponentInParent<SortingGroup>();

            var listShapeCheck = LevelController.instance.gridController.GetAllShapeOverLayer(group.sortingOrder);
            //Debug.Log(listShapeCheck.Count);
            foreach (var shape in listShapeCheck)
            {
                if (m_Collider.Distance(shape.colliderShape).isOverlapped)
                {
                    Debug.Log(shape.gameObject.name);
                    return false;
                }
            }

            return true;
        }
    }
}