using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Linq;
using TMPro;
using Percas.Data;
using System;

public class CollectionElement : MonoBehaviour
{
    [SerializeField] List<BuildElement> m_ObjectFill;

    [ReadOnly] public int totalPorocess;
    [ReadOnly] public int currentProcess;

    public float process => Mathf.RoundToInt(currentProcess * 100f / totalPorocess);
    public BuildElement GetBuildElement() => currentFill;

    public BuildElement GetObject(int objectIndex)
    {
        return m_ObjectFill[objectIndex];
    }

    CollectionController controller;
    BuildElement currentFill
    {
        get { return controller.currentBuild; }
        set { controller.currentBuild = value; }
    }

    public Vector3 rootScale { get; private set; }

    public async void Setup(CollectionController controller, int index)
    {
        this.controller = controller;
        rootScale = transform.localScale;

        totalPorocess = m_ObjectFill.Select(x => x.countFill).Sum();
        currentProcess = PlayerDataManager.GetCollectionProcess(index);//User.data.GetCollectionProcess(index);

        if (currentProcess > 0 && currentProcess < totalPorocess) controller.currentCollectionIndex = controller.currentCollectionIndexView = index;
        if (currentProcess == 0 && controller.currentCollectionIndexView == -1) controller.currentCollectionIndex = controller.currentCollectionIndexView = index;

        var countFill = currentProcess;
        foreach (var objectFill in m_ObjectFill)
        {
            if (countFill > 0)
            {
                if (countFill < objectFill.countFill)
                {
                    objectFill.objectFill.progress = 1 - (float)countFill / objectFill.countFill;
                    countFill = 0;

                    currentFill = objectFill;
                }
                else
                {
                    objectFill.objectFill.progress = 0;
                    countFill -= objectFill.countFill;
                }
            }
            else
            {
                objectFill.objectFill.progress = 1;
                if (currentFill?.objectFill == null) currentFill = objectFill;
            }
        }

        if (currentFill?.objectFill != null && controller.currentCollectionIndex == index)
        {
            await UniTask.DelayFrame(1);
            controller.pointInfo.SetPosition(currentFill.objectFill.objectRenderer.bounds.center.SetVector3(y: currentFill.objectFill.objectRenderer.bounds.max.y + controller.pointInfoPosOffsetY));
            controller.pointInfo.SetIndex(Mathf.RoundToInt(currentFill.objectFill.progress * currentFill.countFill));
            RoomController.OnUpdateTextProgress?.Invoke(this.process);

        }
    }

    public void SetupPosition(Vector3 position)
    {
        gameObject.SetActive(true);
        transform.localPosition = position;
    }

    readonly float timeMove = 0.5f;
    readonly float speedFill = 150;
    [ReadOnly] public bool isFill = false;
    [ReadOnly] public bool fillDone = false;
    [ReadOnly] public bool isHolder;

    Tween tweenFill;
    RevealByProgress targetObjectFollow;

    public async UniTask<bool> Fill(int value)
    {
        var countFill = value;

        if (isFill || value == 0) return false;

        isFill = true;
        foreach (var data in m_ObjectFill)
        {
            var objectFill = data.objectFill;
            if (objectFill.progress <= 0) continue;

            currentFill = data;

            //pointer setup
            controller.pointInfo.SetPosition(objectFill.objectRenderer.bounds.center.SetVector3(y: objectFill.objectRenderer.bounds.max.y + controller.pointInfoPosOffsetY));
            controller.pointInfo.SetIndex(Mathf.RoundToInt(objectFill.progress * data.countFill));

            if (countFill == 0) break;

            if (targetObjectFollow != objectFill)
            {
                //calculate position move camera
                targetObjectFollow = CollectionController.instance.currentBuild?.objectFill;
                await controller.CameraFocus(targetObjectFollow);

                // if (GameStatic.FillStepByStep) break;
            }
            if (!isHolder)
            {
                break;
            }

            //rope setup
            controller.ropeElement.gameObject.SetActive(true);
            controller.ropeElement.startPoint.position = controller.buttonFill.GetPosition();
            controller.ropeElement.endPoint.position = controller.buttonFill.GetPosition() + Vector3.up;
            controller.ropeElement.ChangeColor(controller.colors.Evaluate(UnityEngine.Random.value));
            controller.ropeElement.ChangeMesh();
            controller.ropeElement.PlayAnimation(true);

            //init var
            var currentCountFill = Mathf.RoundToInt(objectFill.progress * data.countFill);
            var fillIndex = Mathf.Min(countFill, currentCountFill);
            var countFillTemp = countFill;
            var fillIndexTemp = 0;

            Static.MoveCurved(controller.ropeElement.endPoint, objectFill.objectRenderer.bounds.center, controller.ropeElement.startPoint.position, timeMove, new Vector3(0, 5, 0), ease: Ease.OutQuad).Forget();

            tweenFill?.Kill();
            tweenFill = DOVirtual.Int(1, fillIndex, timeMove, async x =>
            {
                // StartCoroutine(PlayVibrationSequenceCoroutine());

                if (x > fillIndexTemp)
                {
                    var fillindex = x - fillIndexTemp;
                    fillIndexTemp = x;

                    controller.buttonFill.SetIndexFill(countFillTemp - x);

                    await UniTask.Delay(Mathf.CeilToInt(timeMove * 1000));

                    //Debug.LogFormat("{0} : {1} - {2}", Time.frameCount, x, indexSpawnParticle);
                    var process = (float)(data.countFill - currentCountFill + x) / data.countFill;
                    controller.pointInfo.SetIndex(currentCountFill - x);
                    AudioController.Instance.PlayVibration(Percas.HapticType.Light, 5);
                    objectFill.progress = 1 - process;

                    // set progress bar
                    currentProcess += fillindex;
                    RoomController.OnUpdateTextProgress?.Invoke(this.process);
                }

            }).SetEase(Ease.Linear);

            await tweenFill.AsyncWaitForCompletion();
            await UniTask.DelayFrame(1);

            //await UniTask.Delay(100);
            await Static.MoveCurved(controller.ropeElement.startPoint, objectFill.objectRenderer.bounds.center, controller.ropeElement.startPoint.position, timeMove, new Vector3(0, 5, 0), ease: Ease.OutQuad);
            controller.ropeElement.PlayAnimation(false);
            controller.ropeElement.gameObject.SetActive(false);

            countFill -= fillIndexTemp;

            if (Static.Approximately(objectFill.progress, 0))
            {
                controller.fillCorrectParticle.transform.position = objectFill.objectRenderer.bounds.center;
                controller.fillCorrectParticle.Play();

                _ = objectFill.transform.DOPunchScale(Vector3.up * objectFill.transform.localScale.y * 0.25f, 0.5f, 1, 1);
            }
            else
            {
                break;
            }
        }

        if (value - countFill > 0)
            PlayerDataManager.OnSpendCoil.Invoke(value - countFill, x => Debug.Log(x), "room_fill", new LogCurrency("currency", "coil", "room", null, "feature", "room_fill"));

        isFill = false;
        return countFill != value;
    }

    public void StopFill()
    {
        if (tweenFill.IsActive())
        {
            Debug.Log("Kill tween");
            tweenFill.Kill();
        }
    }

}
