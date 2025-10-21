using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GogoGaga.OptimizedRopesAndCables;
using DG.Tweening;

[ExecuteAlways]
public class RopeElement : MonoBehaviour
{
    [SerializeField] ColorManager m_ColorManager;
    [SerializeField] Transform m_StartPoint;
    [SerializeField] Transform m_EndPoint;

    [SerializeField] Rope m_Rope;
    [SerializeField] RopeMesh m_RopeMesh;
    [SerializeField] RopeWindEffect m_RopeEffect;

    [SerializeField] ParticleSystem m_Particle;
    [SerializeField] List<Renderer> m_Renderers;

    public Transform startPoint => m_StartPoint;
    public Transform endPoint => m_EndPoint;
    public ParticleSystem particle => m_Particle;

    [HorizontalLine(2, EColor.Blue)]
    [OnValueChanged("ChangeColor")]
    public Color coilColor;


    private void Awake()
    {
        Setup();
    }

    public float rateRopeLengh = 1.5f;
    private void Update()
    {
        if (startPoint != null && endPoint != null)
        {
            m_Rope.ropeLength = Vector3.Distance(startPoint.position, endPoint.position) * rateRopeLengh;
        }
    }

    public void Setup()
    {
        ChangeColor();
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }
    public void RevivePool()
    {
        if (PoolingManager.instance?.ropePool != null)
        {
            PoolingManager.instance.ropePool.Release(this);
        }
        //gameObject.SetActive(false);
        //GameObject.Destroy(gameObject);
    }

    [OnValueChanged("ChangeColor")]
    public float timeAnimation = 1f;
    public float offsetWindForce = 500f;
    public float offsetWindDirection = 50f;
    Sequence tweenRopeFill;
    public void PlayAnimation(bool value)
    {
        if (value)
        {
            tweenRopeFill = DOTween.Sequence();
            tweenRopeFill.Append(DOVirtual.Float(-1, 1, timeAnimation / 2f, x => m_RopeEffect.windDirectionDegrees = x * offsetWindDirection).SetLoops(2, LoopType.Yoyo));
            tweenRopeFill.Insert(0f, DOVirtual.Float(0, 1, timeAnimation / 4f, x => m_RopeEffect.windForce = x * offsetWindForce).SetLoops(4, LoopType.Yoyo));
            tweenRopeFill.SetLoops(-1, LoopType.Restart);
        }
        else
        {
            tweenRopeFill?.Kill();
            m_RopeEffect.windDirectionDegrees = 0;
            m_RopeEffect.windForce = 0;
        }
    }

    MaterialPropertyBlock mpb;
    public void ChangeColor()
    {
        if (mpb == null) mpb = new MaterialPropertyBlock();

        //var color = m_ColorManager.GetCoilColor(coilColor);
        mpb.SetColor("_BaseColor", coilColor);

        foreach (var renderer in m_Renderers)
        {
            renderer.SetPropertyBlock(mpb, 0);
        }
    }

    public void ChangeColor(Color color)
    {
        coilColor = color;
        ChangeColor();
    }

    public void ChangeMesh()
    {
        m_Rope.RecalculateRope();
        m_RopeMesh.GenerateMesh();
    }
}
