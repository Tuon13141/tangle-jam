using System;
using UnityEngine;

namespace Elements
{
    public class RollElement : MonoBehaviour
    {
        #region PROPERTIES
        private static readonly int PropSpeedAnim = Animator.StringToHash("SpeedAnim");
        private static readonly int PropRollIn = Animator.StringToHash("RollIn");
        private static readonly int PropRollOut = Animator.StringToHash("RollOut");
        private static readonly int PropAutoStart= Animator.StringToHash("AutoStart");
        private static readonly int PropBaseColor = Shader.PropertyToID("_BaseColor");
        [SerializeField] private Animator m_Animator;
        [SerializeField] private Renderer m_Renderer;
        [SerializeField] private float m_AnimTime = 1.33f;
        private MaterialPropertyBlock _mpb;
        #endregion

        #region PUBLIC FUNCS
        public void ChangeColor(Color coilColor)
        {
            _mpb ??= new MaterialPropertyBlock();
            _mpb.SetColor(PropBaseColor, coilColor);
            m_Renderer.SetPropertyBlock(_mpb, 0);
        }
        public void SetSpeedAnim(float timePlay)
        {
            m_Animator.SetFloat(PropSpeedAnim, timePlay);
        }
        
        public void SetSpeedWithTimePlay(float timePlay)
        {
            SetSpeedAnim(m_AnimTime / timePlay);
        }
        
        public void SetAutoStart(bool auto)
        {
            m_Animator.SetBool(PropAutoStart, auto);
        }
        
        public void PlayRollIn()
        {
            m_Animator.SetTrigger(PropRollIn);
        }
        
        public void PlayRollOut()
        {
            m_Animator.SetTrigger(PropRollOut);
        }
        #endregion
    }
}