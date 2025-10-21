using UnityEngine;
using TMPro;
using Percas.IAR;

public class PopupLuckyWheel_RewardItem : MonoBehaviour
{
    [SerializeField] TMP_Text textAmount;

    public void Init(Reward reward)
    {
        if (reward.RewardType == RewardType.InfiniteLive)
        {
            textAmount.text = $"{(int)reward.RewardAmount / 60}m";
        }
        else
        {
            textAmount.text = $"X{reward.RewardAmount}";
        }
    }
}
