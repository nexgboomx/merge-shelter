using MergeShelter.Economy;

namespace MergeShelter.Meta
{
    public sealed class ShelterUpgrade
    {
        private const int MaxHealthBonusPerUpgradeLevel = 25;

        public int Level { get; private set; }

        public ShelterUpgrade(int level = 1)
        {
            if (level < 1)
                level = 1;

            Level = level;
        }

        public int GetUpgradeCost()
        {
            if (Level <= 1)
                return 100;

            return 450 + (Level - 2) * 600;
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
