using MergeShelter.Economy;

namespace MergeShelter.Meta
{
    public sealed class ShelterUpgrade
    {
        public int Level { get; private set; } = 1;

        public int GetUpgradeCost()
        {
            return 100 + (Level - 1) * 75;
        }

        public bool TryUpgrade(CurrencyWallet wallet)
        {
            var cost = GetUpgradeCost();
            if (!wallet.TrySpend(CurrencyType.Coins, cost))
                return false;

            Level++;
            return true;
        }
    }
}
