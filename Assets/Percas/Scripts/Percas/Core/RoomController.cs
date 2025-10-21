using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Percas.UI;
using Percas;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class RoomController : MonoBehaviour
{
    [SerializeField] Camera roomCamera;
    [SerializeField] ButtonBase buttonBuildInRoom, buttonNextHome, buttonNextRoom, buttonCloseRoom;
    [SerializeField] ButtonBase buttonGoToPreviousRoom, buttonGoToNextRoom;
    [SerializeField] RectTransform rtButtonFill, rtButtonNextHome, rtButtonNextRoom;
    [SerializeField] GameObject roomInfo, m_canBuildNoti;
    [Header("Progress Bar")]
    [SerializeField] Slider m_sliderProgress;
    [SerializeField] TMP_Text m_textProgress;
    [SerializeField] GameObject m_giftReceived;

    [Header("VFX")]
    [SerializeField] ParticleSystem fireworks;
    [SerializeField] ParticleSystem topEffect;

    public static Action<Action, bool> OnUpdateUI;
    public static Action<bool, bool> OnDisplayButtonNavigations;
    public static Action OnUpdateNoti;
    public static Action<bool> OnDisplayButtonClose;
    public static Action<float> OnUpdateTextProgress;

    private float initOrthSize;
    private Vector3 initCamPos;

    private readonly float timeTween = 1f;

    private Tween tweenScale;

    private void Awake()
    {
        OnUpdateUI += UpdateUI;
        OnDisplayButtonNavigations += DisplayButtonNavigations;
        OnUpdateNoti += UpdateNoti;
        OnDisplayButtonClose += DisplayButtonClose;
        OnUpdateTextProgress += UpdateTextProgress;

        //initOrthSize = roomCamera.orthographicSize;
        //initCamPos = roomCamera.transform.position;
        buttonBuildInRoom.SetPointerClickEvent(() =>
        {
            if (!GameLogic.IsShowingRoom && GameLogic.TotalCoil > 0)
            {
                UIHomeController.OnDisplay?.Invoke(false, true);
                return;
            }

            if (!GameLogic.IsShowingRoom && GameLogic.TotalCoil <= 0)
            {
                ActionEvent.OnShowToast?.Invoke(Const.LANG_KEY_LACK_COIL);
                ButtonPlay.OnScaleLoop?.Invoke();
            }
        });
        buttonCloseRoom.SetPointerClickEvent(() =>
        {
            if (GameLogic.IsShowingRoom)
            {
                CollectionController.instance.BackBuild();
                m_giftReceived.SetActive(false);
                rtButtonFill.gameObject.SetActive(false);
                rtButtonNextHome.gameObject.SetActive(false);
                rtButtonNextRoom.gameObject.SetActive(false);
                roomCamera.DOOrthoSize(initOrthSize, timeTween).SetEase(Ease.InOutQuart);
                roomCamera.transform.DOMove(initCamPos, timeTween).SetEase(Ease.InOutQuart).OnComplete(() =>
                {
                    UIHomeController.OnDisplay?.Invoke(true, false);
                });
            }
        });
        buttonNextHome.SetPointerClickEvent(NextHome);
        buttonNextRoom.SetPointerClickEvent(NextRoom);
        buttonGoToPreviousRoom.SetPointerClickEvent(GoToPreviousRoom);
        buttonGoToNextRoom.SetPointerClickEvent(GoToNextRoom);
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        initOrthSize = roomCamera.orthographicSize;
        initCamPos = roomCamera.transform.position;
    }

    private void OnDestroy()
    {
        OnUpdateUI -= UpdateUI;
        OnDisplayButtonNavigations -= DisplayButtonNavigations;
        OnUpdateNoti -= UpdateNoti;
        OnDisplayButtonClose -= DisplayButtonClose;
        OnUpdateTextProgress -= UpdateTextProgress;
        tweenScale?.Kill();
    }

    private void OnEnable()
    {
        DisplayButtonNavigations(false, false);
    }

    private void PlayVFX()
    {
        fireworks.Play();
        topEffect.Play();
    }

    private void UpdateUI(Action callback, bool isRoomCompleted)
    {
        m_sliderProgress.gameObject.SetActive(GameLogic.IsShowingRoom);
        UpdateUIAsync(callback, isRoomCompleted);
    }

    private void UpdateNoti()
    {
        tweenScale?.Kill();
        buttonBuildInRoom.transform.localScale = Vector2.one;
        if (!GameLogic.IsShowingRoom && GameLogic.TotalCoil > 0)
        {
            m_canBuildNoti.SetActive(true);
            tweenScale = buttonBuildInRoom.transform.DOScale(Vector2.one * 1.1f, 0.3f).SetLoops(12, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
        else
        {
            m_canBuildNoti.SetActive(false);
        }
    }

    private void UpdateTextProgress(float value)
    {
        m_sliderProgress.value = Mathf.Max(0.1f, value * 0.01f);
        m_textProgress.text = string.Format("{0}%", value);
    }

    private void DisplayButtonClose(bool value)
    {
        buttonCloseRoom.gameObject.SetActive(value);
    }

    private async void UpdateUIAsync(Action callback, bool isRoomCompleted)
    {
        try
        {
            if (isRoomCompleted)
            {
                m_giftReceived.SetActive(true);
                await ZoomOut(isRoomCompleted);
            }
            else
            {
                m_giftReceived.SetActive(false);
                if (GameLogic.OutOfRoom || GameLogic.TotalCoil <= 0)
                {
                    await ZoomOut(false);
                }
                else
                {
                    await ZoomIn(callback);
                }
            }
        }
        catch (Exception) { }
    }

    private async UniTask ZoomIn(Action callback)
    {
        try
        {
            DisplayButtonNavigations(CollectionController.instance.currentCollectionIndexView > 0, CollectionController.instance.currentCollectionIndexView < CollectionController.instance.currentCollectionIndex);

            // roomInfo.SetActive(false);

            rtButtonNextHome.gameObject.SetActive(false);
            rtButtonNextRoom.gameObject.SetActive(false);
            rtButtonFill.gameObject.SetActive(true);

            if (!CollectionController.instance.inRoomBuild)
            {
                CollectionController.instance.BackBuild();
                await UniTask.Delay(CollectionController.instance.timeMoveRoom);
            }

            roomCamera.DOOrthoSize(initOrthSize - 8f, timeTween).SetEase(Ease.InOutQuart);

            //calculate position move camera
            var objectBuild = CollectionController.instance.currentBuild?.objectFill;
            CollectionController.instance.CameraFocus(objectBuild).Forget();

            await UniTask.Delay(250);

            callback?.Invoke();
        }
        catch (Exception) { }
    }

    private async UniTask ZoomOut(bool isRoomCompleted)
    {
        try
        {
            if (isRoomCompleted)
            {
                DisplayButtonNavigations(false, false);
            }
            else
            {
                DisplayButtonNavigations(CollectionController.instance.currentCollectionIndexView > 0, CollectionController.instance.currentCollectionIndexView < CollectionController.instance.currentCollectionIndex);
            }

            rtButtonNextHome.gameObject.SetActive(!isRoomCompleted);
            rtButtonNextRoom.gameObject.SetActive(isRoomCompleted);
            rtButtonFill.gameObject.SetActive(false);
            roomCamera.DOOrthoSize(initOrthSize, timeTween).SetEase(Ease.InOutQuart);
            roomCamera.transform.DOMove(initCamPos, timeTween).SetEase(Ease.InOutQuart);

            if (isRoomCompleted)
            {
                PlayVFX();
            }

            await UniTask.Delay(750);

            // roomInfo.SetActive(true);
        }
        catch (Exception) { }
    }

    private void NextHome()
    {
        CollectionController.instance.BackBuild();
        UIHomeController.OnDisplay?.Invoke(true, true);
    }

    private void NextRoom()
    {
        CollectionController.instance.Next();
    }

    private void DisplayButtonNavigations(bool showPrevious, bool showNext)
    {
        if (GameLogic.TotalCoil > 0)
        {
            if (buttonGoToPreviousRoom.gameObject.activeSelf) buttonGoToPreviousRoom.gameObject.SetActive(false);
            if (buttonGoToNextRoom.gameObject.activeSelf) buttonGoToNextRoom.gameObject.SetActive(false);
        }
        else
        {
            if (buttonGoToPreviousRoom.gameObject.activeSelf != showPrevious) buttonGoToPreviousRoom.gameObject.SetActive(showPrevious);
            if (buttonGoToNextRoom.gameObject.activeSelf != showNext) buttonGoToNextRoom.gameObject.SetActive(showNext);
        }
    }

    private void GoToPreviousRoom()
    {
        CollectionController.instance.Previous();
    }

    private void GoToNextRoom()
    {
        CollectionController.instance.Next();
    }
}
