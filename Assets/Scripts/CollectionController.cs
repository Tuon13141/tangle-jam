using NaughtyAttributes;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Percas;
using System.Linq;
using Percas.UI;

public enum CollectionStatus
{
    View = 0,
    Build = 1,
}

public class CollectionController : Kit.Common.Singleton<CollectionController>
{
    [ReadOnly] public CollectionStatus status;
    [ReadOnly] public int currentCollectionIndex;
    [ReadOnly] public int currentCollectionIndexView;
    [ReadOnly] public BuildElement currentBuild;

    public bool inRoomBuild => currentCollectionIndexView == currentCollectionIndex;

    [ShowNativeProperty]
    public bool collectionFillDoneAll => m_CollectionElementDatas.Last().Value1.process >= 100;

    [SerializeField] Camera m_MainCamera;
    [SerializeField] ParticleSystem m_FillCorectParticle;
    [SerializeField] Gradient m_RopeColors;
    [SerializeField] List<TupleSerialize<CollectionElement, Color, Color>> m_CollectionElementDatas;
    [SerializeField] List<TupleSerialize<CollectionElement, Color, Color>> m_CollectionElementDatas_Clone;

    [HorizontalLine(2, EColor.Blue)]
    [SerializeField] TMP_Text m_RoomTitle;
    [SerializeField] TMP_Text m_RoomProcess;
    [SerializeField] TMP_Text m_RoomInfo;
    [SerializeField] Renderer m_Background;
    [SerializeField] RopeElement m_RopeElement;
    [SerializeField] ButtonFill m_ButtonFill;
    [SerializeField] PointInfoElement m_PointInfoElement;

    public Camera maincamera => m_MainCamera;
    public ParticleSystem fillCorrectParticle => m_FillCorectParticle;

    public readonly float pointInfoPosOffsetY = 0;

    public Gradient colors => m_RopeColors;
    public RopeElement ropeElement => m_RopeElement;
    public ButtonFill buttonFill => m_ButtonFill;
    public PointInfoElement pointInfo => m_PointInfoElement;

    protected override void Awake()
    {
        base.Awake();

        //setup data, status, set currentCollectionIndex
        currentCollectionIndex = currentCollectionIndexView = -1;
        for (int i = 0; i < m_CollectionElementDatas.Count; i++)
        {
            var collection = m_CollectionElementDatas[i].Value1;
            collection.Setup(this, i);
        }

        Percas.Data.PlayerDataManager.OnEarnCoil += buttonFill.ResetButtonText;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Percas.Data.PlayerDataManager.OnEarnCoil -= buttonFill.ResetButtonText;
    }

    private void OnEnable()
    {
        //setup view
        if (currentCollectionIndex == -1) currentCollectionIndex = currentCollectionIndexView = m_CollectionElementDatas.Count - 1;
        SetupView(false);

        buttonFill.SetIndexFill(Percas.Data.PlayerDataManager.PlayerData.Coil);
    }

    public void SetupBuild()
    {
        RoomController.OnUpdateTextProgress?.Invoke(m_CollectionElementDatas[currentCollectionIndexView].Value1.process);
    }

    //[Button]
    public readonly int timeMoveRoom = 500;
    public void SetupView(bool usedTween = true)
    {
        if (currentCollectionIndexView >= m_CollectionElementDatas.Count) return;

        //setup common UI

        m_RoomProcess.text = string.Format("{0}%", m_CollectionElementDatas[currentCollectionIndexView].Value1.process);
        m_RoomTitle.text = string.Format("{0}", m_CollectionElementDatas[currentCollectionIndexView].Value1.gameObject.name);
        // m_RoomInfo.text = $"Completed {string.Format("{0}%", m_CollectionElementDatas[currentCollectionIndexView].Value1.process)}";
        RoomController.OnUpdateTextProgress?.Invoke(m_CollectionElementDatas[currentCollectionIndexView].Value1.process);
        ChangeColorBackground(currentCollectionIndexView, usedTween);

        //disable ui build
        if (GameLogic.OutOfRoom || (pointInfo.gameObject.activeSelf && currentCollectionIndexView != currentCollectionIndex))
        {
            pointInfo.gameObject.SetActive(false);
        }

        //active ui build
        if (!GameLogic.OutOfRoom)
        {
            UniTask.Delay(timeMoveRoom).ContinueWith(() =>
            {
                if (this?.pointInfo == null) return;

                if (!pointInfo.gameObject.activeSelf && currentCollectionIndexView == currentCollectionIndex)
                {
                    pointInfo.gameObject.SetActive(true);
                    var current = m_CollectionElementDatas[currentCollectionIndexView].Value1.GetBuildElement();
                    pointInfo.SetPosition(current.objectFill.objectRenderer.bounds.center.SetVector3(y: current.objectFill.objectRenderer.bounds.max.y + pointInfoPosOffsetY));
                    pointInfo.SetIndex(Mathf.RoundToInt(current.objectFill.progress * current.countFill));
                }
            }).Forget();
        }

        //setup position collection
        for (int i = 0; i < m_CollectionElementDatas.Count; i++)
        {
            var collection = m_CollectionElementDatas[i].Value1;

            if (usedTween)
            {
                collection.transform.DOLocalMove((i - currentCollectionIndexView) * new Vector3(-25, 0, -25), timeMoveRoom / 1000f).SetEase(Ease.InOutQuart);
                collection.transform.DOScale(collection.rootScale * ((i == currentCollectionIndexView) ? 1 : 0.5f), timeMoveRoom / 1000f);
            }
            else
            {
                collection.SetupPosition((i - currentCollectionIndexView) * new Vector3(-25, 0, -25));
                collection.transform.localScale = collection.rootScale * ((i == currentCollectionIndexView) ? 1 : 0.5f);
            }
        }
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void Fill()
    {
        if (Percas.Data.PlayerDataManager.PlayerData.Coil > 0)
        {
            Fill(Percas.Data.PlayerDataManager.PlayerData.Coil);
        }
    }

    private void OnRoomCompleted()
    {
        Percas.IAR.RewardGainController.OnAddRewardGain?.Invoke(new Percas.IAR.RewardGainCoin(1000, Vector3.zero, new Percas.Data.LogCurrency("currency", "coin", "room_complete", "non_iap", "ads", "rwd_ads")));
        Percas.IAR.RewardGainController.OnStartGaining?.Invoke();
        if (currentCollectionIndexView >= m_CollectionElementDatas.Count - 1)
        {
            RoomController.OnUpdateUI?.Invoke(null, false);
            return;
        }
        RoomController.OnUpdateUI?.Invoke(null, true);
    }

    // Next Room
    [Button(">", enabledMode: EButtonEnableMode.Playmode)]
    public void Next()
    {
        if (currentCollectionIndexView >= m_CollectionElementDatas.Count - 1) return;

        currentCollectionIndexView += 1;

        SetupView();

        RoomController.OnUpdateUI?.Invoke(null, false);
        RoomController.OnDisplayButtonNavigations?.Invoke(currentCollectionIndexView > 0, currentCollectionIndexView < currentCollectionIndex);
    }

    // Previous Room
    [Button("<", enabledMode: EButtonEnableMode.Playmode)]
    public void Previous()
    {
        if (currentCollectionIndexView <= 0) return;

        currentCollectionIndexView -= 1;

        SetupView();

        RoomController.OnUpdateUI?.Invoke(null, false);
        RoomController.OnDisplayButtonNavigations?.Invoke(currentCollectionIndexView > 0, currentCollectionIndexView < currentCollectionIndex);
    }

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void BackBuild()
    {
        if (currentCollectionIndex == -1) return;

        currentCollectionIndexView = currentCollectionIndex;

        SetupView();
    }

    MaterialPropertyBlock mpb;
    public void ChangeColorBackground(int indexColor, bool usedTween = true)
    {
        if (mpb == null) mpb = new MaterialPropertyBlock();
        var color1 = m_CollectionElementDatas[indexColor % m_CollectionElementDatas.Count].Value2;
        var color2 = m_CollectionElementDatas[indexColor % m_CollectionElementDatas.Count].Value3;

        if (usedTween)
        {
            DOVirtual.Color(mpb.GetColor("_ColorFrom"), color1, 0.3f, (value) =>
            {
                mpb.SetColor("_ColorFrom", value);
                m_Background.SetPropertyBlock(mpb, 0);
            });

            DOVirtual.Color(mpb.GetColor("_ColorTo"), color2, 0.3f, (value) =>
            {
                mpb.SetColor("_ColorTo", value);
                m_Background.SetPropertyBlock(mpb, 0);
            });
        }
        else
        {
            mpb.SetColor("_ColorFrom", color1);
            mpb.SetColor("_ColorTo", color2);
            m_Background.SetPropertyBlock(mpb, 0);
        }
    }
    [Button]
    public async void Fill(int value)
    {
        if (Percas.Data.PlayerDataManager.PlayerData.Coil <= 0) return;

        if (currentCollectionIndex < m_CollectionElementDatas.Count)
        {
            //fill
            //var currentCollection = m_CollectionElementDatas[currentCollectionIndex].Value1;
            //var fillResult = await currentCollection.Fill(value);
            //if (!fillResult) return;
            var currentCollection = m_CollectionElementDatas[currentCollectionIndex].Value1;

            if (currentCollection.isHolder) return;
            currentCollection.isHolder = true;

            //fill
            bool fillResult;
            do
            {
                fillResult = await currentCollection.Fill(Percas.Data.PlayerDataManager.PlayerData.Coil);

                await UniTask.DelayFrame(1);

                if (!buttonFill.isHoldToBuild) currentCollection.isHolder = false;
                if (Percas.Data.PlayerDataManager.PlayerData.Coil == 0) break; //block fill = 0
                if (currentCollection.currentProcess >= currentCollection.totalPorocess) break; //done room
            }
            while (fillResult || currentCollection.isHolder);

            //save
            Percas.Data.PlayerDataManager.SetCollectionProcess(currentCollectionIndex, currentCollection.currentProcess);
            //User.data.SetCollectionProcess(currentCollectionIndex, currentCollection.currentProcess);
            //User.Save();

            if (GameLogic.TotalCoil <= 0) RoomController.OnUpdateUI?.Invoke(null, false);

            //check build done collection
            if (currentCollection.currentProcess >= currentCollection.totalPorocess)
            {
                Debug.Log("fill done");

                //await UniTask.Delay(500);
                currentCollectionIndex += 1;

                await UniTask.DelayFrame(1);

                //Next();

                OnRoomCompleted();

                if (currentCollectionIndex < m_CollectionElementDatas.Count)
                {
                    currentBuild = m_CollectionElementDatas[currentCollectionIndex].Value1.GetObject(0);
                }
            }

            // m_RoomInfo.text = $"Completed {string.Format("{0}%", currentCollection.process)}";
        }
        else
        {
            UIHomeController.OnDisplay?.Invoke(false, false);
        }
    }

    public void StopFill()
    {
        var currentCollection = m_CollectionElementDatas[currentCollectionIndex].Value1;

        currentCollection.isHolder = false;
        currentCollection.StopFill();
    }

    public async UniTask CameraFocus(RevealByProgress target)
    {
        if (target != null)
        {
            Ray ray = maincamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            Vector3 normal = ray.direction.normalized;

            Plane dragPlane = new Plane(normal, target.objectRenderer.bounds.center);

            dragPlane.Raycast(ray, out float distance);
            Vector3 center = ray.GetPoint(distance);

            if (Vector3.Distance(center, target.objectRenderer.bounds.center) > 0.001f)
            {
                var rootPos = maincamera.transform.position;
                await DOVirtual.Float(0, 1, 1f, x =>
                {
                    maincamera.transform.position = Vector3.Lerp(rootPos, rootPos - (center - target.objectRenderer.bounds.center), x);

                }).SetEase(Ease.InOutQuart).AsyncWaitForCompletion();

                //await maincamera.transform.DOMove(maincamera.transform.position - (center - target.objectRenderer.bounds.center), 1f)
                //                                 .SetEase(Ease.InOutQuart)
                //                                 .AsyncWaitForCompletion();
            }
        }
    }
}
