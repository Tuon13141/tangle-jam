using Cysharp.Threading.Tasks;
using DG.Tweening;
using Flexalon;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using UnityEngine;

[DefaultExecutionOrder(200)]
public class SlotController : Kit.Common.Singleton<SlotController>
{
    [SerializeField] private Transform m_ParentSlotExtra;
    [SerializeField] private Transform m_ParentSlot;

    [SerializeField] private List<SlotElement> m_SlotElements;
    [SerializeField] private List<SlotElement> m_SlotElementsExtra;
    [SerializeField] private List<SpriteRenderer> m_SlotSpriteRenderers;

    [SerializeField] private AnimationSupport m_WarningAnimation;
    [SerializeField] private Sprite m_SlotHardLevel;
    [SerializeField] private SlotElement m_SlotElementPrefab;
    public Transform parentSlotExtra => m_ParentSlotExtra;

    public static float sizeControler = 7.15f;
    
    public int maxSlots = 7;
    [ReadOnly] public int currentMaxSlots;
    
    private void OnDrawGizmos()
    {
        Vector3 center = transform.position + Vector3.forward * sizeControler / 2;
        Vector3 direction = Vector3.up;
        Vector2 size = new Vector2(9, sizeControler);

        Kit.GizmosExtend.DrawSquare(center, direction, size, 0, Color.blue);
    }

    protected override void Awake()
    {
        base.Awake();
        SetSlotSprite();
        currentMaxSlots = maxSlots;
    }

    public void SetSlotSprite(bool isHardLevel = false)
    {
        foreach (var slot in m_SlotSpriteRenderers)
        {
            if (isHardLevel) slot.sprite = m_SlotHardLevel;
        }
    }

    public void PlayWarningAnimation(bool value)
    {
        if (m_WarningAnimation == null) return;

        Debug.Log($"###PlayWarningAnimation: {value}");

        if (value) m_WarningAnimation.Play("SlotWarning");
        else if (m_WarningAnimation.animationComponent.isPlaying)
        {
            m_WarningAnimation.Stop();
            m_WarningAnimation.SetupFirstFrameAnimation("SlotWarning");
        }
    }

    public void ResetSlot()
    {
        m_SlotElements.ForEach(x =>
        {
            x.coilElement.ChangeStatus(CoilStatus.Hide);
            x.coilElement.ChangeColor(Color.clear);
        });

        m_SlotElementsExtra.ForEach(x =>
        {
            x.coilElement.ChangeStatus(CoilStatus.Hide);
            x.coilElement.ChangeColor(Color.clear);
        });
    }
    
    public void ReArrangeSlotWithColor(SlotElement slotAdd, CoilElement blockElementAdd)
    {
        var defaultOrder = slotAdd.transform.GetSiblingIndex();
        var order = defaultOrder;
        var orderedSlots = m_SlotElements.OrderBy(x => x.transform.GetSiblingIndex());
        foreach (var slot in orderedSlots)
        {
            if (slot == slotAdd || slot.coilElement == null) continue;

            if (slot.coilElement.coilStatus is CoilStatus.InBar or CoilStatus.Hide && slot.coilElement.coilColor == blockElementAdd.coilColor)
            {
                if (slot.transform.GetSiblingIndex() < defaultOrder)
                {
                    order = slot.transform.GetSiblingIndex() + 1;
                }
            }
        }
        slotAdd.transform.SetSiblingIndex(order);
    }

    //slot curret add, element add
    public List<(CoilElement, CoilElement)> coilElementAdds = new List<(CoilElement, CoilElement)>();
    public List<SlotElement> AddToSlot(SlotElement slotAdd, CoilElement coilElementAdd)
    {
        var slotListMatch = new List<SlotElement> { slotAdd };

        slotAdd.coilElement.coilColor = coilElementAdd.coilColor;
        slotAdd.coilElement.ChangeColor();
        coilElementAdds.Add((slotAdd.coilElement, coilElementAdd));

        var defaultOrder = slotAdd.transform.GetSiblingIndex();
        var order = defaultOrder;

        m_SlotElements.OrderBy(x => x.transform.GetSiblingIndex());
        foreach (var slot in m_SlotElements)
        {
            if (slot == slotAdd) continue;

            if (slot.coilElement.coilStatus == CoilStatus.InBar && slot.coilElement.coilColor == coilElementAdd.coilColor)
            {
                slotListMatch.Add(slot);

                if (slot.transform.GetSiblingIndex() < defaultOrder)
                    order = slot.transform.GetSiblingIndex() + 1;
            }
        }

        slotAdd.transform.SetSiblingIndex(order);

        Debug.LogFormat("AddToSlot with count = {0}", slotListMatch.Count);
        return slotListMatch;
    }

    public bool CheckCompleteCoil(CoilElement coilElementAdd)
    {
        return m_SlotElements.Count(x => (x.coilElement.coilStatus == CoilStatus.InBar && x.coilElement.coilColor == coilElementAdd.coilColor)) == 2;
    }

    public List<SlotElement> GetSlotEmpty(int countSlot = 1)
    {
        var list = new List<SlotElement>();
        for (var i = 1; i <= countSlot; i++)
        {
            if (CheckFullSlot()) return list;
            var slot = LeanPool.Spawn(m_SlotElementPrefab, m_ParentSlot);
            if (slot == null) return list;
            slot.transform.localPosition = Vector3.zero;
            list.Add(slot);
            m_SlotElements.Add(slot);
        }
        Debug.Log("GetSlotEmpty: " + list.Count);
        return list;
    }

    public List<SlotElement> GetSlotExtraEmpty()
    {
        var slotEmptyList = new List<SlotElement>();
        foreach (var slot in m_SlotElementsExtra)
        {
            if (!slot.isLock && slot.coilElementInactive) slotEmptyList.Add(slot);
        }

        return slotEmptyList;
    }

    public List<CoilElement> GetListCoilInBar(bool ignoneComplete = true)
    {
        var tempList = new List<CoilElement>();
        foreach (var slot in m_SlotElements)
        {
            if (slot.isLock) continue;
            if (ignoneComplete && slot.coilElement.coilStatus == CoilStatus.Completed) continue;

            tempList.Add(slot.coilElement);
        }

        return tempList;
    }

    public List<CoilElement> GetListCoilActiveInSlotExtra()
    {
        var tempList = new List<CoilElement>();
        foreach (var slot in m_SlotElementsExtra)
        {
            if (slot.isLock) continue;
            if (slot.coilElement.coilStatus == CoilStatus.Enable)
                tempList.Add(slot.coilElement);
        }

        return tempList;
    }

    public List<CoilElement> GetListCoilHideInSlotExtra()
    {
        var tempList = new List<CoilElement>();

        foreach (var slot in m_SlotElementsExtra)
        {
            if (!slot.isLock && slot.coilElementInactive)
                tempList.Add(slot.coilElement);
        }

        return tempList;
    }

    public bool AddSlotExtraBooster()
    {
        return false;
    }

    public bool CheckAddSlotExtraBooster(int minSlot)
    {
        var otherCoilList = GetListCoilHideInSlotExtra();
        return otherCoilList.Count >= minSlot;
    }

    public List<CoilElement> GetCoilElementListMatch()
    {
        var coilElementsInBar = GetListCoilInBar();
        var coilElementActive = coilElementsInBar.Where(x => x.coilStatus == CoilStatus.InBar).ToList();

        //get list element match
        List<CoilElement> matchList = new List<CoilElement>();
        foreach (var element in coilElementActive)
        {
            if (coilElementActive.Count(x => x.coilColor == element.coilColor) > matchList.Count)
            {
                matchList = coilElementActive.Where(x => x.coilColor == element.coilColor).ToList();
            }
        }

        return matchList;
    }
    public void ReOrderSlot()
    {
        var list = new List<SlotElement>(m_SlotElements);
        foreach (var slot in list.Where(slot => slot.coilElementInactive))
        {
            ReleaseSlot(slot);
        }
    }
    
    private void ReleaseSlot(SlotElement slotElement)
    {
        if(slotElement == null) return;
        slotElement.transform.SetParent(null);
        m_SlotElements.Remove(slotElement);
        LeanPool.Despawn(slotElement.gameObject);
    }

    public bool CheckFullSlot()
    {
        var list = new List<SlotElement>(m_SlotElements);
        return list.Count(x => x?.coilElement?.coilStatus == CoilStatus.InBar) >= currentMaxSlots;
    }

    public bool CheckOnlyOneSlot()
    {
        return m_SlotElements.Count == currentMaxSlots - 1;
    }
    

    public (CoilElement, CoilElement) GetLastCoilElementAdd()
    {
        for (int i = coilElementAdds.Count - 1; i >= 0; i--)
        {
            if (coilElementAdds[i].Item1.coilStatus == CoilStatus.InBar && coilElementAdds[i].Item2.coilStatus != CoilStatus.Transfer)
            {
                return coilElementAdds[i];
            }
        }

        return (null, null);
    }

    //public (Transform, BoxElement) CheckAndGetSlot(CoffeeElement coffeeElement)
    //{
    //    (Transform, BoxElement) result = (null, null);
    //    foreach (var slot in m_SlotElements)
    //    {
    //        if (slot.isLock || slot.boxElement == null || (int)slot.boxElement.boxColor != (int)coffeeElement.coffeeColor) continue;

    //        var slotCup = slot.boxElement.CheckSlotCupEmpty();

    //        if (slotCup != null)
    //        {
    //            if (slot.boxElement.holder.childCount > 0)
    //                return (slotCup, slot.boxElement);
    //            else
    //                result = (slotCup, slot.boxElement);
    //        }
    //    }

    //    return result;
    //}



    //public List<CoffeeElement> GetListCupInBar()
    //{
    //    var tempList = new List<CoffeeElement>();

    //    var boxList = GetListBoxInBar();
    //    foreach (var box in boxList)
    //    {
    //        tempList.AddRange(box.holder.GetComponentsInChildren<CoffeeElement>());
    //    }
    //    return tempList;
    //}

    //public void ActiveSlotLock()
    //{
    //    foreach (var slot in m_SlotElements)
    //    {
    //        if (slot.isLock)
    //        {
    //            slot.UnLockSlot();
    //            return;
    //        }
    //    }
    //}
}
