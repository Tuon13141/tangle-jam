using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class CanvasTutorial : MonoBehaviour
{
    [SerializeField] CanvasGroup m_Hand;

    [ReadOnly] public GridController gridController;
    [ReadOnly] public List<CoilElement> coilElements;

    Camera mainCamera;
    public void Setup(GridController gridController)
    {
        this.gridController = gridController;
        mainCamera = CameraSizeHandler.instance.GetComponent<Camera>();
        coilElements = gridController.coilElements.OrderByDescending(x => x.transform.position.z).OrderBy(x => x.cellData.Value).ToList();
        m_Hand.DOFade(1, 0.5f).From(0);
    }

    int currentCoil = 0;
    Tween tweenFade;
    private void Update()
    {
        if (tweenFade?.active == true) return;

        if (currentCoil >= coilElements.Count)
        {
            gameObject.SetActive(false);
            return;
        }

        if (coilElements[currentCoil].coilStatus == CoilStatus.Enable)
        {
            m_Hand.GetComponent<RectTransform>().SetPos(coilElements[currentCoil].transform.position, mainCamera);
        }
        else
        {
            tweenFade = m_Hand.DOFade(0, 0.3f).From(1).OnComplete(() => m_Hand.DOFade(1, 0.3f).From(0));
            currentCoil++;
        }
    }
}
