namespace Percas.IAA
{
    public class IAAData
    {
        #region Configs
        public bool IsAdRemoved;
        public int BannerInitialLevel = 3;
        public int AppOpenFrequency = 30;
        public string LastTimeAdShown;
        public string LastTimeAdBreakShown;
        public string LastTimeAppOpenShown;
        #endregion

        #region Performance
        public int InterCount = 0;
        public int VideoCount = 0;
        public double UserAdRevenue = 0;
        #endregion
    }
}