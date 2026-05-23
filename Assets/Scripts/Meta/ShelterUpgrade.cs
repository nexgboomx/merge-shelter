using MergeShelter.Economy;

namespace MergeShelter.Meta
{
    public sealed class ShelterUpgrade
    {
        private const int MaxHealthBonusPerUpgradeLevel = 25;

        public int Level { get; private set; } = 1;

        public int GetUpgradeCost()
        {
            return 100 + (Level - 1) * 75;
        }

        public int GetMaxHealthBonus()
        {
            return (Level - 1) * MaxHealthBonusPerUpgradeLevel;
        }

        public int GetMaxHealth(int baseMaxHealth)
        {
            return baseMaxHealth + GetMaxHealthBonus();
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
