using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using Percas;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tuon
{
    public enum CoilColor
    {
        None = 0,
        Red = 1,
        Blue = 2,
        Green = 3,
        Yellow = 4,
        Brown = 5,
        Purple = 6,
        Pink = 7,
        Deep_Blue = 8,
        White = 9,
    }

    public enum CoilStatus
    {
        Enable = 0,
        Disable = 1,
        Mystery = 2,

        Hide = 6,
        Transfer = 7,
        InBar = 8,
        Completed = 9,
    }

    //[ExecuteAlways]
    public class CoilElement : MonoBehaviour, IPointerClickHandler
    {
        //const string ANIMATION_COIL_DOWN = "Coil_Up";
        //const string ANIMATION_COIL_UP = "Coil_Down";

        const string ANIMATION_COIL_DOWN = "Coil Unwinding v2";
        const string ANIMATION_COIL_UP = "Coil Winding v2";

        [SerializeField] ColorManager m_ColorManager;
        [SerializeField] GameObject m_MysteryCoil;
        [SerializeField] GameObject m_OutLine;
        [SerializeField] GameObject m_FakeShadown;
        [SerializeField] GameObject m_FakeConnect;
        //[SerializeField] List<Renderer> m_Renderers;
        [SerializeField] Renderer m_Renderer;

        [SerializeField] AnimationSupport m_Animation1;
        [SerializeField] AnimationSupport m_Animation2;
        [SerializeField] AnimationSupport m_Animation3;

        public AnimationSupport animation1 => m_Animation1;
        public AnimationSupport animation2 => m_Animation2;
        public AnimationSupport animation3 => m_Animation3;

        public GameObject fakeConnect => m_FakeConnect;

        public void SetFakeConnect(bool value, StageData.Direction? direction = null)
        {
            fakeConnect.SetActive(value);
            if (direction != null)
                switch (direction)
                {
                    case StageData.Direction.Up:
                        fakeConnect.transform.SetLocalEulerAngles(y: -90);
                        break;

                    case StageData.Direction.Right:
                        fakeConnect.transform.SetLocalEulerAngles(y: 0);
                        break;

                    case StageData.Direction.Down:
                        fakeConnect.transform.SetLocalEulerAngles(y: 90);
                        break;

                    case StageData.Direction.Left:
                        fakeConnect.transform.SetLocalEulerAngles(y: 180);
                        break;
                }
        }

        [HorizontalLine(2, EColor.Blue)]
        //[OnValueChanged("ChangeColor")]
        [ReadOnly] public Color coilColor;

        //[OnValueChanged("ChangeStatus")]
        [ReadOnly] public CoilStatus coilStatus;

        [ReadOnly] public GridElement gridElement;

        [ReadOnly] public bool isCoilPair;
        [ReadOnly] public RibbonEffect ribbonEffect;
        [ReadOnly] public CoilElement coilElementConnect;
        public bool CheckElementConnect()
        {
            return true;
        }

        [ReadOnly] public StageData.CellData cellData;

        private float timeMove = 0.2f;
        private float timePlayAnim = 0.3f;

        private void Awake()
        {
            if (gameObject.scene.name != null)
            {
                CoilSetup();
            }
            if (Application.isPlaying)
            {
                timeMove = GameConfig.Instance.TimeCoilMoveToSpool;
                timePlayAnim = GameConfig.Instance.TimeCoilPlayAnim;
            }
        }

        [ContextMenu("Coil Setup")]
        public void CoilSetup()
        {
            ChangeColor();
            ChangeStatus();
        }

        public void SetOutline(bool value)
        {
            //if (mpb == null) mpb = new MaterialPropertyBlock();

            //mpb.SetFloat("_OutlineWidth", value ? 5 : 0);
            //m_Renderer.SetPropertyBlock(mpb, 0);

            m_OutLine.SetActive(value);
        }

        public void SetFakeShadow(bool value)
        {
            m_FakeShadown.SetActive(value);
        }

        MaterialPropertyBlock mpb;
        public void ChangeColor()
        {
            // if (mpb == null) mpb = new MaterialPropertyBlock();
            //
            // //var color = m_ColorManager.GetCoilColor(coilColor);
            // mpb.SetColor("_BaseColor", coilColor);
            //
            // //foreach (var renderer in m_Renderers)
            // //{
            // //    renderer.SetPropertyBlock(mpb, 0);
            // //}

            // //m_Renderer.SetPropertyBlock(mpb, 0);
            var material = m_ColorManager.GetCoilMaterial(coilColor);
            m_Renderer.material = material;
        }

        public void ChangeColor(Color color)
        {
            coilColor = color;
            Debug.LogFormat("ChangeColor: {0} (#{1})", coilColor, ColorUtility.ToHtmlStringRGB(coilColor));

            var material = m_ColorManager.GetCoilMaterial(color);
            Debug.LogFormat("ChangeColor Material: {0}", material);

            m_Renderer.material = material;
        }

        public void ChangeStatus()
        {
            animation1.Stop();
            animation2.Stop();
            animation3.Stop();
            SetFakeShadow(true);

            //SetOutline(coilStatus == CoilStatus.Enable);
            switch (coilStatus)
            {
                case CoilStatus.Enable:
                    animation1.Play("Coil Enable");
                    goto show_coil;

                case CoilStatus.Disable:
                    animation1.SetupFirstFrameAnimation("Coil Enable");
                    goto show_coil;

                case CoilStatus.Mystery:
                    m_MysteryCoil.SetActive(true);
                    goto show_coil;

                case CoilStatus.InBar:
                    SetFakeShadow(false);
                    goto hide_coil;

                case CoilStatus.Hide:
                    SetFakeShadow(false);
                    goto hide_coil;

                default:
                    break;
            }
            return;

        show_coil:
            animation2.SetupLastFrameAnimation(ANIMATION_COIL_UP);
            animation3.SetupLastFrameAnimation("Spool Appear");
            return;

        hide_coil:
            animation2.SetupFirstFrameAnimation(ANIMATION_COIL_UP);
            animation3.SetupFirstFrameAnimation("Spool Appear");
            return;
        }

        public void ChangeStatus(CoilStatus coilStatus)
        {
            this.coilStatus = coilStatus;
            ChangeStatus();
        }

        public void ActiveMysteryCoil()
        {
            if (coilStatus != CoilStatus.Mystery) return;
            m_MysteryCoil.SetActive(false);
            ChangeStatus(CoilStatus.Enable);
            CheckMove();
        }

        public async UniTask MoveOtherSpool(CoilElement coilElementConnectTemp)
        {
            if (coilElementConnectTemp == null) return;

            if (coilStatus == CoilStatus.Mystery) m_MysteryCoil.SetActive(false);

            if (coilStatus == CoilStatus.InBar) coilStatus = CoilStatus.Transfer;
            else coilStatus = CoilStatus.Hide;

            SetFakeShadow(false);

            coilElementConnectTemp.ChangeColor(coilColor);
            SetOutline(false);

            if (gridElement == null) gridElement = GetComponentInParent<GridElement>();
            gridElement?.CheckCoilActive();

            //var rope = RopeController.instance.GetRopeElement(transform);
            var rope = PoolingManager.instance.ropePool.Get();
            rope.coilColor = coilColor;
            rope.rateRopeLengh = 2f;
            rope.Setup();

            rope.startPoint.position = transform.position;
            rope.endPoint.position = transform.position + Vector3.up;
            rope.ChangeMesh();

            animation2.Play(ANIMATION_COIL_DOWN, timePlayAnim);
            UniTask.Delay(Mathf.RoundToInt(timePlayAnim * 1000)).ContinueWith(() => animation2.SetupLastFrameAnimation(ANIMATION_COIL_DOWN)).Forget();

            coilElementConnectTemp.animation3.Play("Spool Appear", 0.1f);
            Static.MoveCurved(rope.endPoint, coilElementConnectTemp.transform, rope.endPoint.position, timeMove, new Vector3(0, 5, 0)).Forget();
            await UniTask.Delay(Mathf.CeilToInt(timeMove * 500f));
            coilElementConnectTemp.animation2.Play(ANIMATION_COIL_UP, timePlayAnim);
            coilElementConnectTemp.SetFakeShadow(true);

            await UniTask.Delay(100);
            await Static.MoveCurved(rope.startPoint, coilElementConnectTemp.transform, rope.startPoint.position, timeMove, new Vector3(0, 5, 0));
            rope.RevivePool();
            await animation3.PlayAsync("Spool Disappear");
            coilStatus = CoilStatus.Hide;
            SetFakeShadow(false);

            SetOutline(false);

            gridElement?.CheckTubeElenentTarget();
        }

        public async UniTask FillToPicture(float timeFill)
        {
            var listPointFill = PictureController.instance.GetLevelPointFill(coilColor);
            //Debug.Log(string.Join(",", listPointFill.Select(x => string.Format("{0}", x.Item1).ToArray())));
            if (listPointFill.Count == 0)
            {
                Debug.LogErrorFormat("FillToPicture Error: not find color list - {0}", coilColor);
                return;
            }

            var listPointFilling = listPointFill.Select(p => 32 * p.Item2.y + p.Item2.x).ToList();

            //var rope = RopeController.instance.GetRopeElement(transform);
            var rope = PoolingManager.instance.ropePool.Get();
            rope.coilColor = coilColor;
            rope.Setup();

            rope.PlayAnimation(true);
            rope.startPoint.position = transform.position;
            rope.endPoint.position = transform.position + Vector3.up;
            rope.ChangeMesh();

            animation2.Play(ANIMATION_COIL_DOWN, timeFill);
            UniTask.Delay(Mathf.RoundToInt(timeFill * 1000)).ContinueWith(() => m_Renderer.gameObject.SetActive(false)).Forget();

            rope.particle.gameObject.SetActive(true);
            var main = rope.particle.main;
            main.startColor = coilColor;

            int indexDelay = Mathf.CeilToInt(listPointFill.Count / 20f);
            for (int i = 0; i < listPointFill.Count; i++)
            {
                var pointer = listPointFill[i];
                rope.endPoint.position = pointer.Item1;
                PictureController.instance.FillColor(pointer.Item2);
                if (i % indexDelay == 0)
                {
                    await UniTask.Delay(Mathf.RoundToInt(timeFill * 1000 / ((float)listPointFill.Count / indexDelay)));
                }
                if (i % 5 == 0) PictureController.instance.ShakePicture();

                rope.particle.Emit(1);
                rope.particle.DestroyAllParticles(rope.endPoint, 30);
            }

            await Static.MoveCurved(rope.startPoint, rope.endPoint.transform, rope.startPoint.position, timeMove, new Vector3(0, 5, 0));
            await animation3.PlayAsync("Spool Disappear");
            PictureController.instance.ResetShakePicture();
            var effect = PoolingManager.instance.mergeParticlePool.Get();
            effect.transform.position = transform.position;

            UniTask.Delay(500).ContinueWith(() => PoolingManager.instance.mergeParticlePool.Release(effect)).Forget();

            //GetComponentInParent<SlotElement>()?.particleCollect.Play();
            rope.particle.gameObject.SetActive(false);
            rope.PlayAnimation(false);
            rope.RevivePool();
            PictureController.instance.CurrenPointFill = Static.RemoveSublistElements(PictureController.instance.CurrenPointFill, listPointFilling);
        }

        #region CollectBooster

        public async UniTask RollCollectToPicture(float timeFill, Transform target)
        {
            var listPointFill = PictureController.instance.GetLevelPointFill(coilColor);
            if (listPointFill.Count == 0)
            {
                Debug.LogErrorFormat("FillToPicture Error: not find color list - {0}", coilColor);
                return;
            }
            var listPointFilling = listPointFill.Select(p => 32 * p.Item2.y + p.Item2.x).ToList();
            if (PictureController.instance.CurrenPointFill == null) PictureController.instance.CurrenPointFill = new List<int>();
            PictureController.instance.CurrenPointFill.AddRange(listPointFilling);
            var rope = PoolingManager.instance.ropePool.Get();
            rope.coilColor = coilColor;
            rope.Setup();
            rope.PlayAnimation(true);
            rope.startPoint.position = target.position;
            rope.endPoint.position = target.position + Vector3.up;
            rope.ChangeMesh();
            rope.particle.gameObject.SetActive(true);
            var main = rope.particle.main;
            main.startColor = coilColor;
            int indexDelay = Mathf.CeilToInt(listPointFill.Count / 20f);
            for (int i = 0; i < listPointFill.Count; i++)
            {
                var pointer = listPointFill[i];
                rope.endPoint.position = pointer.Item1;
                PictureController.instance.FillColor(pointer.Item2);
                if (i % indexDelay == 0)
                {
                    await UniTask.Delay(Mathf.RoundToInt(timeFill * 1000 / ((float)listPointFill.Count / indexDelay)));
                }
                rope.particle.Emit(1);
                rope.particle.DestroyAllParticles(rope.endPoint, 30);
            }
            await Static.MoveCurved(rope.startPoint, rope.endPoint.transform, rope.startPoint.position, timeMove, new Vector3(0, 5, 0));
            var effect = PoolingManager.instance.mergeParticlePool.Get();
            effect.transform.position = target.position;
            UniTask.Delay(500).ContinueWith(() => PoolingManager.instance.mergeParticlePool.Release(effect)).Forget();
            rope.particle.gameObject.SetActive(false);
            rope.PlayAnimation(false);
            rope.RevivePool();
            PictureController.instance.CurrenPointFill = Static.RemoveSublistElements(PictureController.instance.CurrenPointFill, listPointFilling);
            if (tweenDelayCall?.active == true) tweenDelayCall.Kill();
            tweenDelayCall = DOVirtual.DelayedCall(0.5f, () =>
            {
                Debug.LogFormat("Run tween Check: {0}", Time.frameCount);
                LevelController.instance.CheckStage();
            });
        }

        public async UniTask FillToRollCollect(float timeFill, Transform target)
        {
            await FillToRoll(timeFill, target);
        }

        #endregion
        public async UniTask FillToRoll(float timeFill, Transform target, Vector3 offset = default, CoilStatus coilInit = CoilStatus.Transfer)
        {
            var rope = PoolingManager.instance.ropePool.Get();
            ColorUtility.TryParseHtmlString("#606060", out var darkColor);
            rope.coilColor = coilInit == CoilStatus.Mystery ? darkColor : coilColor;
            rope.Setup();
            rope.PlayAnimation(true);
            rope.startPoint.position = transform.position + offset;
            rope.endPoint.position = transform.position + Vector3.up;
            rope.ChangeMesh();
            animation2.Play(ANIMATION_COIL_DOWN, timeFill);
            await Static.MoveCurved(rope.endPoint, target.position + offset, rope.endPoint.position, timeFill / 2, new Vector3(0, 2, 0));
            rope.particle.gameObject.SetActive(true);
            var main = rope.particle.main;
            main.startColor = coilColor;
            await UniTask.Delay(Mathf.RoundToInt(timeFill / 2 * 1000));
            rope.particle.Emit(1);
            rope.particle.DestroyAllParticles(rope.endPoint, 30);
            await Static.MoveCurved(rope.startPoint, rope.endPoint.transform, rope.startPoint.position, timeFill / 2, new Vector3(0, 2, 0));
            rope.particle.gameObject.SetActive(false);
            rope.PlayAnimation(false);
            rope.RevivePool();
        }

        public async UniTask RollToCoil(float timeFill, Transform target, Vector3 offset = default, CoilStatus coilInit = CoilStatus.Transfer)
        {
            var rope = PoolingManager.instance.ropePool.Get();
            ColorUtility.TryParseHtmlString("#606060", out var darkColor);
            rope.coilColor = coilInit == CoilStatus.Mystery ? darkColor : coilColor;
            rope.Setup();
            rope.PlayAnimation(true);
            rope.startPoint.position = target.position + offset;
            rope.endPoint.position = target.position + offset;
            rope.ChangeMesh();
            await Static.MoveCurved(rope.endPoint, transform.position, rope.endPoint.position, timeFill / 2, new Vector3(0, 2, 0));
            animation2.Play(ANIMATION_COIL_UP, timeFill);
            rope.particle.gameObject.SetActive(true);
            var main = rope.particle.main;
            main.startColor = coilColor;
            await UniTask.Delay(Mathf.RoundToInt(timeFill / 2 * 500));
            rope.particle.Emit(1);
            rope.particle.DestroyAllParticles(rope.endPoint, 30);
            await Static.MoveCurved(rope.startPoint, rope.endPoint.transform, rope.startPoint.position, timeFill / 2, new Vector3(0, 2, 0));
            var effect = PoolingManager.instance.mergeParticlePool.Get();
            effect.transform.position = transform.position;
            UniTask.Delay(500).ContinueWith(() =>
            {
                PoolingManager.instance.mergeParticlePool.Release(effect);
            }).Forget();
            rope.particle.gameObject.SetActive(false);
            rope.PlayAnimation(false);
            rope.RevivePool();
        }

        public async UniTask MoveToShuffle(float time, Transform target, Vector3 offset = default)
        {
            var seq = DOTween.Sequence();
            seq.Append(transform.DOMove(target.position + offset, time));
            seq.Join(transform.DOScale(Vector3.zero, time));
            await UniTask.Delay(TimeSpan.FromSeconds(time));
        }

        public async UniTask MoveToBoard(float time)
        {
            var seq = DOTween.Sequence();
            seq.Append(transform.DOLocalMove(Vector3.zero, time));
            seq.Join(transform.DOScale(Vector3.one, time));
            await UniTask.Delay(TimeSpan.FromSeconds(time));
            transform.localScale = Vector3.one;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //Debug.LogFormat("OnPointerClick = {0}", eventData);
            if (coilStatus != CoilStatus.Hide)
            {
                AudioController.Instance.PlayVibration(HapticType.Light, 5);
                ActionEvent.OnCoilTap?.Invoke();
            }

            switch (coilStatus)
            {
                case CoilStatus.Enable:
                    AddCoilToSlot();

                    ActionEvent.OnPlaySFXCoilClickOn?.Invoke();
                    break;

                case CoilStatus.Mystery:
                case CoilStatus.Disable:
                    animation1.Play("Coil Cant Move");

                    //if (GameLogic.CurrentLevel <= 2) TutorialManager.OnOpenTutorialByIndex?.Invoke(1);
                    //else ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_CANNOT_MOVE);
                    break;

                case CoilStatus.Hide:
                    break;
            }
        }

        public async void AddCoilToSlot(bool forceAdd = false)
        {
            if (!forceAdd && coilStatus != CoilStatus.Enable) return;

            var connectSlotCoils = SlotController.instance.GetSlotEmpty();

            if (connectSlotCoils == null || connectSlotCoils.Count == 0)
            {
                if (forceAdd)
                {
                    var otherCoilList = SlotController.instance.GetListCoilHideInSlotExtra();
                    if (otherCoilList == null || otherCoilList.Count == 0) return;

                    var elementExtra = otherCoilList[0];
                    MoveOtherSpool(elementExtra).ContinueWith(() =>
                    {
                        elementExtra.ChangeStatus(CoilStatus.Enable);
                        SlotController.instance.ReOrderSlot();
                    }).Forget();

                    if (!SlotController.instance.parentSlotExtra.gameObject.activeSelf) SlotController.instance.parentSlotExtra.gameObject.SetActive(true);
                    return;
                }
                else
                {
                    return;
                }
            }

            if (!forceAdd && isCoilPair && SlotController.instance.CheckCompleteCoil(coilElementConnect))
            {
                coilElementConnect.AddCoilToSlot(true);
                return;
            }

            var connectSlot = connectSlotCoils[0];

            //get list coil match
            var slotListMatch = SlotController.instance.AddToSlot(connectSlot, this);
            connectSlot.EnableFlexalonAnimator(false);

            //first setup
            if (slotListMatch.Count == 3)
            {
                slotListMatch[0].coilElement.coilStatus = CoilStatus.Completed;
                slotListMatch[1].coilElement.coilStatus = CoilStatus.Completed;
                slotListMatch[2].coilElement.coilStatus = CoilStatus.Completed;
            }

            //move coil to slot
            if (connectSlot.coilElement.coilStatus != CoilStatus.Completed)
            {
                connectSlot.coilElement.ChangeStatus(CoilStatus.InBar);
            }

            //if isCoilPair
            if (isCoilPair && coilElementConnect != null)
            {
                coilElementConnect.isCoilPair = false;
                isCoilPair = false;

                coilStatus = CoilStatus.Transfer;
                if (coilElementConnect.coilStatus != CoilStatus.Hide) coilElementConnect.coilStatus = CoilStatus.Transfer;
                //coilElementConnect.SetFakeConnect(false);
                //SetFakeConnect(false);
                await ribbonEffect.PlayAnimation();

                ribbonEffect.gameObject.SetActive(false);
                if (coilElementConnect.coilStatus != CoilStatus.Hide) coilElementConnect.AddCoilToSlot(true);
            }

            if (gridElement != null)
            {
                gridElement.controller.CoilElementCollect(this);
            }
            await MoveOtherSpool(connectSlot.coilElement);

            await UniTask.DelayFrame(1);
            connectSlot.EnableFlexalonAnimator(true);

            //check merge
            if (slotListMatch.Count == 3)
            {
                SlotController.instance.PlayWarningAnimation(false);
                await MergeCoil(slotListMatch[0], slotListMatch[1], slotListMatch[2]);
                SlotController.instance.PlayWarningAnimation(false);
            }
            else
            {
                SlotController.instance.PlayWarningAnimation(SlotController.instance.CheckOnlyOneSlot());
                LevelController.instance.CheckLost();
            }
        }

        float hightMerge = 3f;
        Tween tweenDelayCall;
        public async UniTask MergeCoil(SlotElement slot1, SlotElement slot2, SlotElement slot3)
        {
            Percas.ActionEvent.OnPlaySFXCoilMerge?.Invoke();

            var coil1 = slot1.coilElement;
            var coil2 = slot2.coilElement;
            var coil3 = slot3.coilElement;

            coil1.SetupMergeCoil();
            coil2.SetupMergeCoil();
            coil3.SetupMergeCoil();

            coil1.transform.DOLocalMoveY(hightMerge, 0.2f);
            coil2.transform.DOLocalMoveY(hightMerge, 0.2f);
            coil3.transform.DOLocalMoveY(hightMerge, 0.2f);

            slot1.SpawnCoil();
            slot2.SpawnCoil();
            slot3.SpawnCoil();
            SlotController.instance.ReOrderSlot();
            LevelController.instance.coilFillList.Add(coil3);
            Percas.ActionEvent.OnItemsMatched?.Invoke();
            await UniTask.Delay(200);

            var centerPoint = coil3.transform.position;
            if (LevelController.instance.coilFillList.Count == 2)
            {
                centerPoint = coil2.transform.position;
            }
            else if (LevelController.instance.coilFillList.Count == 3)
            {
                centerPoint = coil1.transform.position;
            }

            coil1.transform.DOMove(centerPoint, 0.2f);
            coil2.transform.DOMove(centerPoint, 0.2f);
            coil3.transform.DOMove(centerPoint, 0.2f);
            await UniTask.Delay(200);

            coil1.gameObject.SetActive(false);
            coil2.gameObject.SetActive(false);
            await coil3.FillToPicture(GameConfig.Instance.TimeCoilFillPicture);
            coil3.gameObject.SetActive(false);

            await UniTask.Delay(200);
            LevelController.instance.coilFillList.Remove(coil3);
            coil1.ResetSlotStatus();
            coil2.ResetSlotStatus();
            coil3.ResetSlotStatus();

            Percas.ActionEvent.OnPlaySFXCoilHide?.Invoke();
            PoolingManager.instance.coilElementPool.Release(coil1);
            PoolingManager.instance.coilElementPool.Release(coil2);
            PoolingManager.instance.coilElementPool.Release(coil3);
            //GameObject.Destroy(coil1.gameObject);
            //GameObject.Destroy(coil2.gameObject);
            //GameObject.Destroy(coil3.gameObject);

            if (tweenDelayCall?.active == true) tweenDelayCall.Kill();
            tweenDelayCall = DOVirtual.DelayedCall(0.5f, () =>
            {
                Debug.LogFormat("Run tween Check: {0}", Time.frameCount);
                LevelController.instance.CheckStage();
            });
        }

        Transform rootParent;

        public void SetupMergeCoil()
        {
            SetFakeShadow(false);
            rootParent = transform.parent;
            transform.parent = null;
        }

        public void ResetSlotStatus()
        {
            transform.parent = rootParent;

            transform.localPosition = Vector3.zero;
            coilStatus = CoilStatus.Hide;
            SetFakeShadow(false);
            coilColor = Color.clear;
            gameObject.SetActive(true);
            ChangeStatus();
        }

        public void CheckMove()
        {
            if (coilStatus != CoilStatus.Enable && coilStatus != CoilStatus.Disable) return;
            if (gridElement == null) gridElement = GetComponentInParent<GridElement>();

            var map = gridElement.controller.GetMap();
            var stackOut = new Stack<Vector2Int>();
            //Debug.LogFormat("CheckMove {1}: {0}", gridElement.indexGrid, gridElement.transform.parent.parent.gameObject.name);
            var result = Utils.CanMoveToBusStopCells(map, gridElement.indexGrid, out stackOut);
            //Debug.LogFormat("CheckMove {2}: {0} - {1}", gridElement.indexGrid, result, gridElement.transform.parent.parent.gameObject.name);

            if (isCoilPair && coilElementConnect != null)
            {
                var otherResult = Utils.CanMoveToBusStopCells(map, coilElementConnect.gridElement.indexGrid, out stackOut);
                result = result || otherResult;
            }


            if (result && coilStatus == CoilStatus.Disable)
            {
                ChangeStatus(CoilStatus.Enable);
            }
            else if (!result && coilStatus == CoilStatus.Enable)
            {
                ChangeStatus(CoilStatus.Disable);
            }
        }
    }

}
