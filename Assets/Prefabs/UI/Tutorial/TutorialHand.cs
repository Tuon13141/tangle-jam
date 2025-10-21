using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;

public class TutorialHand : MonoBehaviour
{
    [SerializeField] CanvasGroup m_HandCanvasGroup;
    [SerializeField] RectTransform m_HandRect;
    [SerializeField] private SkeletonGraphic m_HandAnim;
    [SerializeField] private  string m_HandAnimName;
    [SerializeField] private float m_AnimationDuration;
    [SerializeField] private float m_DelayAnim;
    
    [ReadOnly] public GridController gridController;
    [ReadOnly] public List<CoilElement> coilElements;

    
    private Camera _mainCamera;
    private int _currentCoil = 0;
    private Sequence  _loopingSequenceHand;
    private int _stateHand = 0;
    private bool _isSetPos = false;
    private bool _showOneTime = false;
    private bool _isShown;
    
    public void Setup(GridController gridController, bool isOneTime = false)
    {
        this.gridController = gridController;
        _mainCamera = CameraSizeHandler.instance.GetComponent<Camera>();
        if (gridController.stageData.tutorialType == LevelAsset.TutorialType.CoilPair ||
            gridController.stageData.tutorialType == LevelAsset.TutorialType.Stack)
        {
            coilElements = gridController.coilTutorial.OrderByDescending(x => x.transform.position.z).ToList();
        }
        else
        {
            coilElements = gridController.coilElements.OrderByDescending(x => x.transform.position.z).ToList();
        }
        SetupDotweenAnimation();
        SetPosZ();
        _isSetPos = true;
        _showOneTime = isOneTime;
    }

    private void SetupDotweenAnimation()
    {
        // --- Bắt đầu tạo Sequence ---
        _loopingSequenceHand = DOTween.Sequence();
        _loopingSequenceHand.AppendCallback(() => 
        {
            _stateHand = 0;
            m_HandAnim.AnimationState.SetAnimation(0, m_HandAnimName, false);
        });
        _loopingSequenceHand.AppendInterval(m_AnimationDuration);
        _loopingSequenceHand.AppendCallback(() =>
        {
            _stateHand = 1;
        });
        _loopingSequenceHand.AppendInterval(m_DelayAnim);
        _loopingSequenceHand.SetLoops(-1);
    }

    public void SetPosZ()
    {
        m_HandRect.anchoredPosition3D = new Vector3(coilElements[_currentCoil].transform.position.x, coilElements[_currentCoil].transform.position.y, 0f);
    }

    private void Update()
    {
        if (_showOneTime && _isShown) return;
        if (_currentCoil >= coilElements.Count)
        {
            gameObject.SetActive(false);
            return;
        }

        if (_stateHand == 1 && !_isSetPos)
        {
            _isSetPos = true;
        }
        if (coilElements[_currentCoil].coilStatus == CoilStatus.Enable)
        {
            if(!_isSetPos) return;
            m_HandRect.SetPos(coilElements[_currentCoil].transform.position, _mainCamera);
        }
        else
        {
            if (_showOneTime)
            {
                _isShown = true;
                gameObject.SetActive(false);
                return;
            }
            _isSetPos = false;
            while (coilElements[_currentCoil].coilStatus != CoilStatus.Enable)
            {
                _currentCoil++;
                if (_currentCoil >= coilElements.Count) break;
            }
        }
    }
}
