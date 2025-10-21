using Elements;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabManager", menuName = "ScriptableObjects/PrefabManager", order = 1)]
public class PrefabManager : ScriptableObject
{
    [Header("Element")]
    [SerializeField] CoilElement m_CoilPrefab;
    [SerializeField] RopeElement m_RopePrefab;
    [SerializeField] TubeElement m_TubePrefab;
    [SerializeField] ButtonsElement m_ButtonsElement;
    [SerializeField] PinControlElement m_PinControlElement;
    [SerializeField] PinWallElement m_PinWallElement;
    [SerializeField] private KeyElement m_KeyElement;
    [SerializeField] private LockElement m_LockElement;
    [Header("Map")]
    [SerializeField] GridElement m_GridElementPrefab;

    [Header("Effect")]
    [SerializeField] MergeEffect m_CoilFillCollectParticle;
    [SerializeField] RibbonEffect m_RibbonEffect;
    [SerializeField] CoilRollEffect m_CoilEffectEffect;

    [Header("UI")]
    [SerializeField] CanvasTutorial m_CanvasTutorial;
    [SerializeField] TutorialHand m_TutorialHand;

    public CoilElement GetCoilPrefab() => m_CoilPrefab;

    public RopeElement GetRopePrefab() => m_RopePrefab;
    
    public CoilRollEffect GetCoilEffectPrefab() => m_CoilEffectEffect;

    public TubeElement GetTubePrefab() => m_TubePrefab;

    public ButtonsElement GetButtonsElementPrefab() => m_ButtonsElement;

    public PinControlElement GetPinControlPrefab() => m_PinControlElement;

    public PinWallElement GetPinWallPrefab() => m_PinWallElement;

    public GridElement GetGridElementPrefab() => m_GridElementPrefab;

    public MergeEffect GetCoilFillCollectParticle() => m_CoilFillCollectParticle;

    public RibbonEffect GetRibbonEffect() => m_RibbonEffect;

    public CanvasTutorial GetCanvasTutorial() => m_CanvasTutorial;

    public TutorialHand GetTutorialHand() => m_TutorialHand;
    public KeyElement GetKeyElement() => m_KeyElement;
    public LockElement GetLockElement() => m_LockElement;
}
