using System;
using System.Collections.Generic;
using MergeShelter.Analytics;
using MergeShelter.Board;
using MergeShelter.Combat;
using MergeShelter.Economy;
using MergeShelter.Levels;
using MergeShelter.UI;
using UnityEngine;

namespace MergeShelter.Core
{
    /// <summary>
    /// Sprint 1 prototype controller.
    /// Attach this to a GameObject in the prototype scene and wire PrototypeHudView in the inspector.
    /// This class intentionally keeps scene behavior simple while the team validates the core loop.
    /// </summary>
    public sealed class PrototypeGameController : MonoBehaviour
    {
        [SerializeField] private PrototypeHudView hudView;
        [SerializeField] private int startingLevelIndex;
        [SerializeField] private int shelterMaxHp = 100;

        private readonly BoardModel _board = new();
        private readonly MergeResolver _mergeResolver = new();
        private readonly PrototypeTileGenerator _tileGenerator = new();
        private readonly CurrencyWallet _wallet = new();

        private IAnalyticsService _analytics;
        private IReadOnlyList<LevelDefinition> _levels;
        private LevelDefinition _currentLevel;
        private ShelterHealth _shelter;
        private WaveManager _waveManager;
        private TileData _nextTile;
        private int _coins;
        private int _parts;
        private bool _levelEnded;

        public int BoardWidth => _board.Width;
        public int BoardHeight => _board.Height;

        public event Action BoardChanged;

        private void Awake()
        {
            _analytics = new DebugAnalyticsService();
            _levels = SprintOneLevelCatalog.CreateLevels();
            StartLevel(Mathf.Clamp(startingLevelIndex, 0, _levels.Count - 1));
        }

        public void StartLevel(int levelIndex)
        {
            _currentLevel = _levels[Mathf.Clamp(levelIndex, 0, _levels.Count - 1)];
            _board.Clear();
            _shelter = new ShelterHealth(shelterMaxHp);
            _waveManager = new WaveManager(_shelter);
            _tileGenerator.Configure(_currentLevel);
            _levelEnded = false;

            _shelter.Changed += OnShelterChanged;
            _waveManager.WaveCompleted += OnWaveCompleted;
            _waveManager.WaveFailed += OnWaveFailed;

            _analytics.Track("level_start", new Dictionary<string, object>
            {
                ["level_id"] = _currentLevel.LevelId,
                ["attempt_number"] = 1
            });

            _nextTile = _tileGenerator.GenerateNextTile();
            RefreshHud();
            hudView?.SetResult("Place tiles, merge, then start the wave.");
            BoardChanged?.Invoke();
        }

        public bool TryPlaceNextTile(int x, int y)
        {
            if (_levelEnded)
                return false;

            var position = new BoardPosition(x, y);
            var placed = _board.TryPlace(position, _nextTile);
            if (!placed)
            {
                _analytics.Track("merge_failed", new Dictionary<string, object>
                {
                    ["level_id"] = _currentLevel.LevelId,
                    ["reason"] = "invalid_placement"
                });
                hudView?.SetResult("Invalid cell. Choose an empty board space.");
                return false;
            }

            _analytics.Track("tile_place", new Dictionary<string, object>
            {
                ["level_id"] = _currentLevel.LevelId,
                ["tile_type"] = _nextTile.Type.ToString(),
                ["tile_tier"] = _nextTile.Tier,
                ["cell_x"] = x,
                ["cell_y"] = y
            });

            if (_mergeResolver.TryResolveMerge(_board, position, out var mergedTile))
            {
                _analytics.Track("merge_success", new Dictionary<string, object>
                {
                    ["level_id"] = _currentLevel.LevelId,
                    ["tile_type"] = mergedTile.Type.ToString(),
                    ["to_tier"] = mergedTile.Tier,
                    ["merge_size"] = 3
                });
                hudView?.SetResult($"Merged {mergedTile.Type} into tier {mergedTile.Tier}!");
            }
            else
            {
                hudView?.SetResult("Tile placed. Build toward a merge of 3.");
            }

            _nextTile = _tileGenerator.GenerateNextTile();
            RefreshHud();
            BoardChanged?.Invoke();
            return true;
        }

        public TileData GetTileAt(int x, int y)
        {
            return _board.GetTile(new BoardPosition(x, y));
        }

        public void StartWave()
        {
            if (_levelEnded)
                return;

            _analytics.Track("wave_start", new Dictionary<string, object>
            {
                ["level_id"] = _currentLevel.LevelId,
                ["wave_id"] = 1,
                ["enemy_count"] = _currentLevel.Enemies.Count
            });

            _waveManager.StartWave(_currentLevel.Enemies);
        }

        private void OnShelterChanged(int current, int max)
        {
            _analytics.Track("shelter_damage", new Dictionary<string, object>
            {
                ["level_id"] = _currentLevel.LevelId,
                ["current_hp"] = current
            });
            hudView?.SetShelterHp(current, max);
        }

        private void OnWaveCompleted(int wave)
        {
            _levelEnded = true;
            _coins += _currentLevel.CoinReward;
            _parts += _currentLevel.PartsReward;
            _wallet.Add(CurrencyType.Coins, _currentLevel.CoinReward);
            _wallet.Add(CurrencyType.Parts, _currentLevel.PartsReward);

            _analytics.Track("level_complete", new Dictionary<string, object>
            {
                ["level_id"] = _currentLevel.LevelId,
                ["remaining_hp"] = _shelter.CurrentHealth,
                ["coins_earned"] = _currentLevel.CoinReward
            });

            RefreshHud();
            hudView?.SetResult($"Victory! +{_currentLevel.CoinReward} coins, +{_currentLevel.PartsReward} parts.");
        }

        private void OnWaveFailed(int wave)
        {
            _levelEnded = true;
            _analytics.Track("level_fail", new Dictionary<string, object>
            {
                ["level_id"] = _currentLevel.LevelId,
                ["fail_reason"] = "overwhelmed"
            });
            RefreshHud();
            hudView?.SetResult("Defeat. Your shelter was overwhelmed. Try stronger merges before the wave.");
        }

        private void RefreshHud()
        {
            hudView?.SetLevel(_currentLevel.LevelId, _currentLevel.DisplayName);
            hudView?.SetTutorial(_currentLevel.TutorialMessage);
            hudView?.SetShelterHp(_shelter.CurrentHealth, _shelter.MaxHealth);
            hudView?.SetNextTile(_nextTile);
            hudView?.SetWallet(_coins, _parts);
        }
    }
}
