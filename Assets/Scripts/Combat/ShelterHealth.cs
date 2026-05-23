using System;

namespace MergeShelter.Combat
{
    public sealed class ShelterHealth
    {
        public int MaxHealth { get; }
        public int CurrentHealth { get; private set; }
        public bool IsDestroyed => CurrentHealth <= 0;

        public event Action<int, int> Changed;
        public event Action Destroyed;

        public ShelterHealth(int maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public void Damage(int amount)
        {
            if (amount <= 0 || IsDestroyed)
                return;

            CurrentHealth = Math.Max(0, CurrentHealth - amount);
            Changed?.Invoke(CurrentHealth, MaxHealth);

            if (IsDestroyed)
                Destroyed?.Invoke();
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDestroyed)
                return;

            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
            Changed?.Invoke(CurrentHealth, MaxHealth);
        }
    }
}
