using UnityEngine;
using Percas;
using Sonat;
using Percas.Data;

public class FirebaseAnalyticsManager : MonoBehaviour
{
    private void Awake()
    {
        TrackingManager.OnLevelStart += SonatLevelStart;
        TrackingManager.OnLevelEnd += SonatLevelEnd;
        TrackingManager.OnContinuePlaying += SonatContinuePlaying;
        TrackingManager.OnPhaseStart += SonatLevelStart;
        TrackingManager.OnPhaseEnd += SonatPhaseEnd;
        TrackingManager.OnTrackScreenView += SonatTrackScreenView;
        TrackingManager.OnTutorialBegin += TutorialBegin;
        TrackingManager.OnTutorialComplete += TutorialComplete;

        TrackingManager.OnSpendBooster += SonatSpendBooster;
        TrackingManager.OnEarnBooster += SonatEarnBooster;
    }

    private void OnDestroy()
    {
        TrackingManager.OnLevelStart -= SonatLevelStart;
        TrackingManager.OnLevelEnd -= SonatLevelEnd;
        TrackingManager.OnContinuePlaying -= SonatContinuePlaying;
        TrackingManager.OnPhaseStart -= SonatLevelStart;
        TrackingManager.OnPhaseEnd -= SonatPhaseEnd;
        TrackingManager.OnTrackScreenView -= SonatTrackScreenView;
        TrackingManager.OnTutorialBegin -= TutorialBegin;
        TrackingManager.OnTutorialComplete -= TutorialComplete;

        TrackingManager.OnSpendBooster -= SonatSpendBooster;
        TrackingManager.OnEarnBooster -= SonatEarnBooster;
    }

    private void SonatTrackScreenView(string screenName)
    {
        var log = new SonatLogScreenView()
        {
            screen_name = screenName,
            screen_class = screenName
        };
        log.Post(logAf: false);
    }

    #region Gameplay
    private void SonatLevelStart()
    {
        //Debug.LogError($"level_start = {GameLogic.CurrentLevel}_{GameLogic.CurrentLevelPhase} | continue_with = {GameLogic.ContinueWith}");
        var log = new SonatLogLevelStart()
        {
            mode = Percas.PlayMode.classic.ToString(),
            level = GameLogic.CurrentLevel,
            phase = GameLogic.CurrentLevelPhase,
            continue_with = string.IsNullOrEmpty(GameLogic.ContinueWith) ? "none" : GameLogic.ContinueWith,
            continue_times = GameLogic.ContinueTimes,
            is_first_play = GameLogic.LevelAttempts <= 1
        };
        log.SetExtraParameter(new Sonat.LogParameter[]
        {
            new("level_start_count", GameLogic.LevelAttempts),
        });
        log.Post(logAf: false);
    }

    private void SonatLevelEnd(bool isWin, string failureReason)
    {
        //Debug.LogError($"level_end = {GameLogic.CurrentLevel}_{GameLogic.CurrentLevelPhase} | continue_with = {GameLogic.ContinueWith}");
        var log = new SonatLogLevelEnd()
        {
            mode = Percas.PlayMode.classic.ToString(),
            level = GameLogic.CurrentLevel,
            phase = GameLogic.CurrentLevelPhase,
            use_booster_count = GameLogic.LevelUseBoosterCount,
            play_time = (int)GameLogic.LevelPlayingTime,
            move_count = GameLogic.LevelMoveCount,
            continue_with = string.IsNullOrEmpty(GameLogic.ContinueWith) ? "none" : GameLogic.ContinueWith,
            continue_times = GameLogic.ContinueTimes,
            is_first_play = GameLogic.LevelAttempts <= 1,
            success = isWin,
            lose_cause = failureReason
        };
        log.Post(logAf: false);
    }

    private void SonatContinuePlaying(string continueWith)
    {
        //Debug.LogError($"level_start = {GameLogic.CurrentLevel}_{GameLogic.CurrentLevelPhase} | continue_with = {continueWith}");
        var log = new SonatLogLevelStart()
        {
            mode = Percas.PlayMode.classic.ToString(),
            level = GameLogic.CurrentLevel,
            phase = GameLogic.CurrentLevelPhase,
            continue_with = continueWith,
            continue_times = GameLogic.ContinueTimes,
            is_first_play = GameLogic.LevelAttempts <= 1
        };
        log.SetExtraParameter(new Sonat.LogParameter[]
        {
            new("level_start_count", GameLogic.LevelAttempts),
        });
        log.Post(logAf: false);
    }

    private void SonatPhaseEnd(bool isWin, string failureReason)
    {
        //Debug.LogError($"phase_end = {GameLogic.CurrentLevel}_{GameLogic.CurrentLevelPhase} | continue_with = {GameLogic.ContinueWith}");
        var log = new SonatLogLevelEnd()
        {
            mode = Percas.PlayMode.classic.ToString(),
            level = GameLogic.CurrentLevel,
            phase = GameLogic.CurrentLevelPhase,
            use_booster_count = GameLogic.LevelUseBoosterCount,
            play_time = (int)GameLogic.PhasePlayingTime,
            move_count = GameLogic.LevelMoveCount,
            continue_with = string.IsNullOrEmpty(GameLogic.ContinueWith) ? "none" : GameLogic.ContinueWith,
            continue_times = GameLogic.ContinueTimes,
            is_first_play = GameLogic.LevelAttempts <= 1,
            success = isWin,
            lose_cause = failureReason
        };
        log.Post(logAf: false);
    }
    #endregion

    private void TutorialBegin(string placement, int step)
    {
        var log = new SonatLogTutorialBegin(placement, step);
        log.Post(false);
    }

    private void TutorialComplete(string placement, int step)
    {
        var log = new SonatLogTutorialComplete(placement, step);
        log.Post(false);
    }

    private void SonatSpendBooster(LogCurrency logCurrency)
    {
        var log = new SonatLogSpendVirtualCurrency()
        {
            virtual_currency_name = logCurrency.name,
            virtual_currency_type = logCurrency.type,
            value = 1,
            level = GameLogic.CurrentLevel,
            location = GameLogic.LogLocation,
            screen = logCurrency.screen,
            earn_item_type = logCurrency.item_id,
            earn_item_id = logCurrency.item_type,
        };
        log.Post();
    }

    private void SonatEarnBooster(LogCurrency logCurrency, int value)
    {
        var log = new SonatLogEarnVirtualCurrency()
        {
            virtual_currency_name = logCurrency.name,
            virtual_currency_type = logCurrency.type,
            value = value,
            level = GameLogic.CurrentLevel,
            location = GameLogic.LogLocation,
            screen = logCurrency.screen,
            source = logCurrency.source,
            spend_item_id = logCurrency.item_id,
            spend_item_type = logCurrency.item_type,
        };
        log.Post();
    }
}
