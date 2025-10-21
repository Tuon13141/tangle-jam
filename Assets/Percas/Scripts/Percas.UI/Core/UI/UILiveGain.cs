using UnityEngine;

namespace Percas.UI
{
    public class UILiveGain : UIRewardGainBase
    {
        [SerializeField] bool isInfinite;
        [SerializeField] GameObject iconLive, iconLiveInfinite;

        public override void Init()
        {
            iconLive.SetActive(!isInfinite);
            iconLiveInfinite.SetActive(isInfinite);
        }

        public void SetInfinite()
        {
            isInfinite = true;
        }
    }
}
