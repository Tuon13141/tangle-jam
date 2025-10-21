using System;
using UnityEngine;

//public class EventIconLuckyWheel : EventIconBase
//{
//    [SerializeField] EventRewardController _rewardPrefab;

//    public static Action<int> OnDisplayRewardGained;

//    private void Awake()
//    {
//        OnDisplayRewardGained += Init;
//    }

//    private void OnDestroy()
//    {
//        OnDisplayRewardGained -= Init;
//    }

//    public override void Init(int value)
//    {
//        EventRewardController eventRewardController = Instantiate(_rewardPrefab, OriginalPosTrans);
//        eventRewardController.GetComponent<AnimShow>().Show();
//        eventRewardController.DisplayIcon(true);
//        eventRewardController.Init(OriginalPosTrans.anchoredPosition);
//        eventRewardController.Movement(TargetTrans.position, () =>
//        {
//            this.ShowRewardValue(value);
//            this.SetNewTransScale(this.transform);

//            EventProgressValue.OnUpdateVisibility?.Invoke(0);

//            // RewardGainController.OnPlayRewardComplete?.Invoke();

//            SoundManager.Instance.PlaySfxRewind(GlobalSetting.GetSFX(Const.SFX_EARN_ITEM));
//        });
//    }
//}
