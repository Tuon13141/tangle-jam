using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RibbonEffect : MonoBehaviour
{
    [SerializeField] AnimationSupport m_Animation;

    [ReadOnly] public bool isShow;

    public async UniTask PlayAnimation()
    {
        if (!isShow) return;
        isShow = false;

		m_Animation.Play("day_dayAction",0.3f);
        await UniTask.Delay(300);
    }
}
