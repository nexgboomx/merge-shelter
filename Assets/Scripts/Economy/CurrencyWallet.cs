using System;
using System.Collections.Generic;

namespace MergeShelter.Economy
{
    public enum CurrencyType
    {
        Coins,
        Gems,
        Parts,
        EventTokens
    }

    public sealed class CurrencyWallet
    {
        private readonly Dictionary<CurrencyType, int> _balances = new();

        public event Action<CurrencyType, int> BalanceChanged;

        public int Get(CurrencyType type)
        {
            return _balances.TryGetValue(type, out var value) ? value : 0;
        }

        public void Add(CurrencyType type, int amount)
        {
            if (amount <= 0) return;
            _balances[type] = Get(type) + amount;
            BalanceChanged?.Invoke(type, _balances[type]);
        }

        public bool TrySpend(CurrencyType type, int amount)
        {
            if (amount <= 0 || Get(type) < amount)
                return false;

            _balances[type] = Get(type) - amount;
            BalanceChanged?.Invoke(type, _balances[type]);
            return true;
        }
    }
}
