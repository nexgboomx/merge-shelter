using System;

namespace MergeShelter.Ads
{
    public sealed class MockRewardedAdService : IAdService
    {
        public bool IsRewardedReady(AdPlacement placement)
        {
            return placement == AdPlacement.RewardDouble || placement == AdPlacement.Revive;
        }

        public void ShowRewarded(AdPlacement placement, Action<bool> onCompleted)
        {
            onCompleted?.Invoke(IsRewardedReady(placement));
        }

        public bool IsInterstitialReady(AdPlacement placement)
        {
            return false;
        }

        public void ShowInterstitial(AdPlacement placement)
        {
        }
    }
}
