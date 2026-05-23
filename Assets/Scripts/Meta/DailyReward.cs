using System;
using MergeShelter.Economy;

namespace MergeShelter.Meta
{
    public readonly struct DailyRewardClaim
    {
        public int Coins { get; }
        public int Parts { get; }
        public bool IsEmpty => Coins <= 0 && Parts <= 0;

        public DailyRewardClaim(int coins, int parts)
        {
            if (coins < 0)
                throw new ArgumentOutOfRangeException(nameof(coins));

            if (parts < 0)
                throw new ArgumentOutOfRangeException(nameof(parts));

            Coins = coins;
            Parts = parts;
        }
    }

    public sealed class DailyReward
    {
        public const int DefaultCoinReward = 75;
        public const int DefaultPartsReward = 1;

        public int CoinReward { get; }
        public int PartsReward { get; }
        public bool HasClaimed { get; private set; }
        public bool CanClaim => !HasClaimed;

        public DailyReward(int coinReward = DefaultCoinReward, int partsReward = DefaultPartsReward)
        {
            if (coinReward < 0)
                throw new ArgumentOutOfRangeException(nameof(coinReward));

            if (partsReward < 0)
                throw new ArgumentOutOfRangeException(nameof(partsReward));

            CoinReward = coinReward;
            PartsReward = partsReward;
        }

        public bool TryClaim(CurrencyWallet wallet, out DailyRewardClaim reward)
        {
            if (wallet == null)
                throw new ArgumentNullException(nameof(wallet));

            reward = default;
            if (!CanClaim)
                return false;

            reward = new DailyRewardClaim(CoinReward, PartsReward);
            wallet.Add(CurrencyType.Coins, CoinReward);
            wallet.Add(CurrencyType.Parts, PartsReward);
            HasClaimed = true;
            return true;
        }
    }
}
