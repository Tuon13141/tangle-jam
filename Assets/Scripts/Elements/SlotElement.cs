using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flexalon;

public class SlotElement : MonoBehaviour
{
    [SerializeField] PrefabManager m_PrefabManager;
    //[SerializeField] SpriteRenderer m_SpriteRenderer;
    [SerializeField] FlexalonLerpAnimator m_FlexalonLerpAnimator;

    [HorizontalLine(2, EColor.Blue)]
    [SerializeField] CoilElement m_CoilElement;
    public CoilElement coilElement
    {
        get
        {
            if (m_CoilElement == null)
            {
                SpawnCoil();
            }

            return m_CoilElement;
        }

        private set
        {
            m_CoilElement = value;
        }
    }

    public bool isLock;
    public bool coilElementInactive => coilElement.coilStatus == CoilStatus.Hide;
    //public SpriteRenderer spriteRenderer => m_SpriteRenderer;

    public void SpawnCoil()
    {
        CoilElement coilElementSpawn;

        coilElementSpawn = PoolingManager.instance.coilElementPool.Get();

        coilElementSpawn.transform.SetParent(transform, false);

        var color = Color.clear;
        coilElementSpawn.ChangeColor(color);
        coilElementSpawn.ChangeStatus(CoilStatus.Hide);

        coilElement = coilElementSpawn;
    }

    public void EnableFlexalonAnimator(bool value)
    {
        m_FlexalonLerpAnimator.enabled = value;
    }
}
