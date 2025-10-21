using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;

public class PopupWarning : MonoBehaviour
{
    [SerializeField] CanvasGroup m_CanvasGroup;
    [SerializeField] TMP_Text m_WarningText;

    Tween tweenHide;
    bool isShow;

    [Button]
    public void Show()
    {
        Show(m_WarningText.text);
    }

    public void Show(string textwarning)
    {
        if (isShow) return;
        isShow = true;

        gameObject.SetActive(true);
        m_WarningText.text = textwarning;

        m_CanvasGroup.DOFade(1, 0.5f).From(0);

        if (tweenHide?.active == true) tweenHide.Kill();
        tweenHide = DOVirtual.DelayedCall(2, Hide);
    }

    public void Hide()
    {
        m_CanvasGroup.DOFade(0, 0.3f).From(1).OnComplete(() =>
        {
            gameObject.SetActive(false);
            isShow = false;
        });
    }
}
