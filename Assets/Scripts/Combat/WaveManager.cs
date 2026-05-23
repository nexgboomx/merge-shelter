using System;
using System.Collections.Generic;

namespace MergeShelter.Combat
{
    public sealed class WaveManager
    {
        private readonly ShelterHealth _shelter;

        public int CurrentWave { get; private set; }
        public bool IsRunning { get; private set; }

        public event Action<int> WaveStarted;
        public event Action<int> WaveCompleted;
        public event Action<int> WaveFailed;

        public WaveManager(ShelterHealth shelter)
        {
            _shelter = shelter;
        }

        public void StartWave(IReadOnlyList<EnemyData> enemies)
        {
            if (IsRunning)
                return;

            IsRunning = true;
            CurrentWave++;
            WaveStarted?.Invoke(CurrentWave);

            // Prototype simulation: total damage = enemy damage sum.
            var totalDamage = 0;
            foreach (var enemy in enemies)
                totalDamage += enemy.Damage;

            _shelter.Damage(totalDamage);

            IsRunning = false;

            if (_shelter.IsDestroyed)
                WaveFailed?.Invoke(CurrentWave);
            else
                WaveCompleted?.Invoke(CurrentWave);
        }
    }
}
