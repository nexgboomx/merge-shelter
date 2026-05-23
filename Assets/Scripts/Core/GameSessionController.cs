using System.Collections.Generic;
using MergeShelter.Analytics;
using MergeShelter.Board;
using MergeShelter.Combat;
using MergeShelter.Economy;

namespace MergeShelter.Core
{
    public sealed class GameSessionController
    {
        private readonly BoardModel _board;
        private readonly MergeResolver _mergeResolver;
        private readonly ShelterHealth _shelter;
        private readonly WaveManager _waveManager;
        private readonly CurrencyWallet _wallet;
        private readonly IAnalyticsService _analytics;

        public GameSessionController(IAnalyticsService analytics = null)
        {
            _analytics = analytics;
            _board = new BoardModel();
            _mergeResolver = new MergeResolver();
            _shelter = new ShelterHealth(100);
            _waveManager = new WaveManager(_shelter);
            _wallet = new CurrencyWallet();

            _waveManager.WaveCompleted += OnWaveCompleted;
            _waveManager.WaveFailed += OnWaveFailed;
        }

        public bool PlaceTile(BoardPosition position, TileData tile)
        {
            var placed = _board.TryPlace(position, tile);
            if (!placed) return false;

            _mergeResolver.TryResolveMerge(_board, position, out _);
            return true;
        }

        public void StartPrototypeWave()
        {
            _analytics?.Track("level_start", new Dictionary<string, object>
            {
                ["level_id"] = 1
            });

            var enemies = new List<EnemyData>
            {
                new EnemyData { EnemyId = "walker_01", MaxHealth = 10, Damage = 10, Speed = 1f },
                new EnemyData { EnemyId = "walker_02", MaxHealth = 10, Damage = 10, Speed = 1f }
            };

            _waveManager.StartWave(enemies);
        }

        private void OnWaveCompleted(int wave)
        {
            _wallet.Add(CurrencyType.Coins, 50);
            _analytics?.Track("level_complete", new Dictionary<string, object>
            {
                ["level_id"] = wave,
                ["reward_coins"] = 50
            });
        }

        private void OnWaveFailed(int wave)
        {
            _analytics?.Track("level_fail", new Dictionary<string, object>
            {
                ["level_id"] = wave
            });
        }
    }
}
