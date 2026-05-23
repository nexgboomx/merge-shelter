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
            var totalDamage = 0;
            foreach (var enemy in enemies)
                totalDamage += enemy.Damage;

            StartWave(enemies, totalDamage);
        }

        public void StartWave(IReadOnlyList<EnemyData> enemies, int incomingDamage)
        {
            if (IsRunning)
                return;

            IsRunning = true;
            CurrentWave++;
            WaveStarted?.Invoke(CurrentWave);

            _shelter.Damage(incomingDamage);

            IsRunning = false;

            if (_shelter.IsDestroyed)
                WaveFailed?.Invoke(CurrentWave);
            else
                WaveCompleted?.Invoke(CurrentWave);
        }
    }
}
