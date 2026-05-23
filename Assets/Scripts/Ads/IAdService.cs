using System;

namespace MergeShelter.Ads
{
    public enum AdPlacement
    {
        RewardDouble,
        Revive,
        DailyChest,
        SpeedUp,
        InterstitialAfterLevel
    }

    public interface IAdService
    {
        bool IsRewardedReady(AdPlacement placement);
        void ShowRewarded(AdPlacement placement, Action<bool> onCompleted);
        bool IsInterstitialReady(AdPlacement placement);
        void ShowInterstitial(AdPlacement placement);
    }
}
