using System;
using UnityEngine;
using TMPro;
using Percas;
using Percas.UI;
using DG.Tweening;
using Percas.Data;

public class ButtonPlay : ButtonBase
{
    [SerializeField] TMP_Text textButton;

    public static Action OnScaleLoop;

    private Tween scaleTween;

    protected override void Awake()
    {
        OnScaleLoop += ScaleLoop;
        base.Awake();
        SetPointerClickEvent(Play);
    }

    private void OnDestroy()
    {
        OnScaleLoop -= ScaleLoop;
        scaleTween?.Kill();
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void OnDisable()
    {
        scaleTween?.Kill();
    }

    private void UpdateUI()
    {
        textButton.text = string.Format(Const.LANG_KEY_BTN_PLAY, GameLogic.CurrentLevel);
    }

    private void Play()
    {
        if (!GameLogic.InternetReachability && GameLogic.CurrentLevel >= GameLogic.LevelNeedsInternet)
        {
            Percas.ServiceLocator.PopupScene.ShowPopup(PopupName.InternetRequired);
            return;
        }

        if (!GameLogic.IsInfiniteLive && GameLogic.CurrentLive <= 0)
        {
            Percas.ServiceLocator.PopupScene.ShowPopup(PopupName.RefillLives);
        }
        else
        {
            // if (GameLogic.UnlockStarRush && ((StarRushManager.Data.CanStart() && !StarRushManager.Data.CallToPlay) || StarRushManager.Data.IsCompleted()))
            // {
            //     Percas.ServiceLocator.PopupScene.ShowPopup(PopupName.StarRush, new PopupStarRushArgs(true));
            // }
            // else
            // {
            //     OnPlay();
            // }
            OnPlay();
        }
    }

    private void OnPlay()
    {
        GlobalSetting.SetPlayMode(Percas.PlayMode.classic);
        PlayerDataManager.SetContinueWith(null);
        PlayerDataManager.OnResetContinueTimes?.Invoke();
        GlobalSetting.OnHomeToGame?.Invoke(null);
    }

    private void ScaleLoop()
    {
        Vector3 targetScale = new(1.1f, 1.1f, 1.1f);
        transform.localScale = Vector3.one;
        scaleTween = transform.DOScale(targetScale, 0.2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            transform.localScale = Vector3.one;
        });
    }
}
