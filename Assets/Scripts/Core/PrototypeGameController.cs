using System;
using System.Collections.Generic;
using MergeShelter.Ads;
using MergeShelter.Analytics;
using MergeShelter.Board;
using MergeShelter.Combat;
using MergeShelter.Levels;
using MergeShelter.Meta;
using MergeShelter.Save;
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
        private readonly PrototypeBoardEvaluator _boardEvaluator = new();
        private readonly IAdService _adService = new MockRewardedAdService();

        private IAnalyticsService _analytics;
        private ISaveService _saveService;
        private SessionProgressionState _progression;
        private IReadOnlyList<LevelDefinition> _levels;
        private LevelDefinition _currentLevel;
        private ShelterHealth _shelter;
        private WaveManager _waveManager;
        private PrototypeBoardEvaluationResult _lastBoardEvaluation;
        private TileData _nextTile;
        private bool _levelEnded;
        private bool _lastLevelWon;
        private bool _lastLevelFailed;
        private bool _rewardDoubleUsedThisResult;
        private bool _reviveUsedThisResult;
        private bool _rewardDoubleOfferPreviewed;
        private bool _reviveOfferPreviewed;

        public static string SaveDirectoryOverrideForTests { get; set; }

        public int BoardWidth => _board.Width;
        public int BoardHeight => _board.Height;
        public int CurrentLevelId => _currentLevel?.LevelId ?? _progression.CurrentLevel;
        public int SelectedLevel => _progression.SelectedLevel;
        public int HighestUnlockedLevel => _progression.HighestUnlockedLevel;
        public int Coins => _progression.Coins;
        public int Parts => _progression.Parts;
        public int ShelterUpgradeLevel => _progression.ShelterUpgradeLevel;
        public int ShelterUpgradeCost => _progression.ShelterUpgradeCost;
        public bool CanAffordShelterUpgrade => _progression.CanAffordShelterUpgrade;
        public bool CanClaimDailyReward => _progression.CanClaimDailyReward;
        public bool HasClaimedDailyReward => _progression.HasClaimedDailyReward;
        public int DailyRewardCoins => _progression.DailyRewardCoins;
        public int DailyRewardParts => _progression.DailyRewardParts;
        public bool CanClaimQuest => _progression.HasClaimableDailyQuest;
        public IReadOnlyList<DailyQuestState> DailyQuests => _progression.DailyQuests;
        public int CurrentShelterMaxHp => _shelter?.MaxHealth ?? GetShelterMaxHp();
        public bool IsLevelEnded => _levelEnded;
        public bool HasPendingReward => _progression.HasPendingReward;
        public int PendingRewardCoins => _progression.PendingReward.Coins;
        public int PendingRewardParts => _progression.PendingReward.Parts;
        public bool CanClaimReward => _levelEnded && _lastLevelWon && _progression.HasPendingReward;
        public bool CanDoubleReward =>
            _levelEnded &&
            _lastLevelWon &&
            _progression.HasPendingReward &&
            !_rewardDoubleUsedThisResult &&
            _adService.IsRewardedReady(AdPlacement.RewardDouble);
        public bool CanRevive =>
            _levelEnded &&
            _lastLevelFailed &&
            !_reviveUsedThisResult &&
            _adService.IsRewardedReady(AdPlacement.Revive);
        public bool CanStartNextLevel =>
            _levels != null &&
            _levelEnded &&
            !_progression.HasPendingReward &&
            _progression.SelectedLevel < _progression.HighestUnlockedLevel &&
            _progression.SelectedLevel < _levels.Count;
        public bool CanRetryLevel => _levelEnded && _lastLevelFailed;

        public event Action BoardChanged;
        public event Action ProgressionChanged;

        private void Awake()
        {
            _analytics = new DebugAnalyticsService();
            _levels = SprintOneLevelCatalog.CreateLevels();
            _saveService = CreateSaveService();
            _progression = LoadProgression();
            StartSelectedLevel();
        }

        public void StartLevel(int levelIndex)
        {
            // Debug/test shortcut. Player-facing level selection should use TryStartLevel or StartNextLevel.
            var clampedIndex = Mathf.Clamp(levelIndex, 0, _levels.Count - 1);
            var level = _levels[clampedIndex];
            if (level.LevelId > _progression.HighestUnlockedLevel)
                _progression.UnlockThroughLevel(level.LevelId);

            _progression.TrySelectLevel(level.LevelId);
            StartLevel(level);
        }

        public bool TryStartLevel(int levelId)
        {
            if (!_progression.TrySelectLevel(levelId))
            {
                hudView?.SetResult($"Level {levelId} is locked. Claim rewards to unlock it.");
                ProgressionChanged?.Invoke();
                return false;
            }

            _analytics.Track("level_selected", new Dictionary<string, object>
            {
                ["level_id"] = levelId,
                ["highest_unlocked_level"] = _progression.HighestUnlockedLevel
            });

            StartSelectedLevel();
            SaveProgression();
            return true;
        }

        public bool ClaimReward()
        {
            if (!CanClaimReward)
            {
                hudView?.SetResult("No reward is waiting to claim.");
                ProgressionChanged?.Invoke();
                return false;
            }

            var previousHighestUnlocked = _progression.HighestUnlockedLevel;
            if (!_progression.TryClaimPendingReward(out var reward))
                return false;

            _analytics.Track("reward_claimed", new Dictionary<string, object>
            {
                ["level_id"] = reward.LevelId,
                ["coins"] = reward.Coins,
                ["parts"] = reward.Parts
            });

            if (_progression.HighestUnlockedLevel > previousHighestUnlocked)
            {
                _analytics.Track("level_unlocked", new Dictionary<string, object>
                {
                    ["level_id"] = _progression.HighestUnlockedLevel
                });
            }

            var questProgress = RecordQuestProgress(DailyQuestModel.ClaimRewardQuestId, 1);
            SaveProgression();
            RefreshHud();
            var nextLevelMessage = CanStartNextLevel ? $" Level {_progression.SelectedLevel + 1} unlocked." : string.Empty;
            hudView?.SetResult($"Reward claimed: +{reward.Coins} coins, +{reward.Parts} parts.{nextLevelMessage}{FormatQuestCompletionSuffix(questProgress)}");
            ProgressionChanged?.Invoke();
            return true;
        }

        public bool ClaimDailyReward()
        {
            if (!_progression.TryClaimDailyReward(out var reward))
            {
                RefreshHud();
                hudView?.SetResult("Daily reward already claimed this session.");
                ProgressionChanged?.Invoke();
                return false;
            }

            _analytics.Track("daily_reward_claimed", new Dictionary<string, object>
            {
                ["coins"] = reward.Coins,
                ["parts"] = reward.Parts
            });

            RefreshHud();
            hudView?.SetResult($"Daily reward claimed: +{reward.Coins} coins, +{reward.Parts} parts.");
            SaveProgression();
            ProgressionChanged?.Invoke();
            return true;
        }

        public bool ClaimQuest()
        {
            if (!_progression.TryClaimDailyQuest(out var reward))
            {
                RefreshHud();
                hudView?.SetResult("No completed daily quest is ready to claim.");
                ProgressionChanged?.Invoke();
                return false;
            }

            _analytics.Track("quest_claimed", new Dictionary<string, object>
            {
                ["quest_id"] = reward.QuestId,
                ["title"] = reward.Title,
                ["coins"] = reward.Coins,
                ["parts"] = reward.Parts
            });

            RefreshHud();
            hudView?.SetResult($"Quest claimed: {reward.Title}. +{reward.Coins} coins, +{reward.Parts} parts.");
            SaveProgression();
            ProgressionChanged?.Invoke();
            return true;
        }

        public bool DoubleReward()
        {
            if (!CanDoubleReward)
            {
                RefreshHud();
                hudView?.SetResult("Double Reward is not available for this result.");
                ProgressionChanged?.Invoke();
                return false;
            }

            if (!ShowMockRewardedAd(AdPlacement.RewardDouble))
            {
                RefreshHud();
                hudView?.SetResult("Double Reward mock ad was not completed.");
                ProgressionChanged?.Invoke();
                return false;
            }

            if (!_progression.TryDoublePendingReward(out var reward))
                return false;

            _rewardDoubleUsedThisResult = true;
            _analytics.Track("reward_doubled", new Dictionary<string, object>
            {
                ["level_id"] = reward.LevelId,
                ["coins_pending"] = reward.Coins,
                ["parts_pending"] = reward.Parts
            });

            RefreshHud();
            hudView?.SetResult($"Reward doubled. Pending reward: +{reward.Coins} coins, +{reward.Parts} parts.");
            SaveProgression();
            ProgressionChanged?.Invoke();
            return true;
        }

        public bool Revive()
        {
            if (!CanRevive)
            {
                RefreshHud();
                hudView?.SetResult("Revive is not available for this result.");
                ProgressionChanged?.Invoke();
                return false;
            }

            if (!ShowMockRewardedAd(AdPlacement.Revive))
            {
                RefreshHud();
                hudView?.SetResult("Revive mock ad was not completed.");
                ProgressionChanged?.Invoke();
                return false;
            }

            _reviveUsedThisResult = true;
            _analytics.Track("revive_used", new Dictionary<string, object>
            {
                ["level_id"] = _currentLevel.LevelId
            });

            StartSelectedLevel();
            hudView?.SetResult("Revive used. Level restarted with a fresh shelter.");
            ProgressionChanged?.Invoke();
            return true;
        }

        public bool UpgradeShelter()
        {
            var cost = _progression.ShelterUpgradeCost;
            var previousLevel = _progression.ShelterUpgradeLevel;
            if (!_progression.TryUpgradeShelter())
            {
                var missingCoins = Mathf.Max(0, cost - _progression.Coins);
                RefreshHud();
                hudView?.SetResult($"Upgrade blocked. Need {missingCoins} more coins for Shelter Lv {previousLevel + 1}.");
                ProgressionChanged?.Invoke();
                return false;
            }

            var newLevel = _progression.ShelterUpgradeLevel;
            _analytics.Track("shelter_upgraded", new Dictionary<string, object>
            {
                ["shelter_level"] = newLevel,
                ["previous_level"] = previousLevel,
                ["coins_spent"] = cost,
                ["coins_remaining"] = _progression.Coins,
                ["max_hp"] = GetShelterMaxHp()
            });

            RefreshHud();
            hudView?.SetResult($"Shelter upgraded to Lv {newLevel}. Future waves start with {GetShelterMaxHp()} HP.");
            SaveProgression();
            ProgressionChanged?.Invoke();
            return true;
        }

        public void ResetSave()
        {
            _saveService?.Reset();
            _progression = new SessionProgressionState();
            StartSelectedLevel();
            hudView?.SetResult("Save reset. Progress returned to Level 1.");
            BoardChanged?.Invoke();
            ProgressionChanged?.Invoke();
        }

        public bool StartNextLevel()
        {
            if (!CanStartNextLevel)
            {
                hudView?.SetResult("Next level is locked. Claim the pending reward first.");
                ProgressionChanged?.Invoke();
                return false;
            }

            return TryStartLevel(_progression.SelectedLevel + 1);
        }

        public bool RetryLevel()
        {
            if (!CanRetryLevel)
            {
                hudView?.SetResult("Retry is available after defeat.");
                ProgressionChanged?.Invoke();
                return false;
            }

            StartSelectedLevel();
            return true;
        }

        private void StartSelectedLevel()
        {
            var levelIndex = Mathf.Clamp(_progression.SelectedLevel - 1, 0, _levels.Count - 1);
            StartLevel(_levels[levelIndex]);
        }

        private void StartLevel(LevelDefinition level)
        {
            UnsubscribeWaveEvents();
            _currentLevel = level;
            _board.Clear();
            _shelter = new ShelterHealth(GetShelterMaxHp());
            _waveManager = new WaveManager(_shelter);
            _tileGenerator.Configure(_currentLevel);
            _levelEnded = false;
            _lastLevelWon = false;
            _lastLevelFailed = false;
            _rewardDoubleUsedThisResult = false;
            _reviveUsedThisResult = false;
            _rewardDoubleOfferPreviewed = false;
            _reviveOfferPreviewed = false;

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
            ProgressionChanged?.Invoke();
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

            string resultMessage;
            if (_mergeResolver.TryResolveMerge(_board, position, out var mergedTile))
            {
                _analytics.Track("merge_success", new Dictionary<string, object>
                {
                    ["level_id"] = _currentLevel.LevelId,
                    ["tile_type"] = mergedTile.Type.ToString(),
                    ["to_tier"] = mergedTile.Tier,
                    ["merge_size"] = 3
                });
                resultMessage = $"Merged {mergedTile.Type} into tier {mergedTile.Tier}!";
            }
            else
            {
                resultMessage = "Tile placed. Build toward a merge of 3.";
            }

            var questProgress = RecordQuestProgress(DailyQuestModel.PlaceTilesQuestId, 1);
            _nextTile = _tileGenerator.GenerateNextTile();
            RefreshHud();
            hudView?.SetResult($"{resultMessage}{FormatQuestCompletionSuffix(questProgress)}");
            BoardChanged?.Invoke();
            ProgressionChanged?.Invoke();
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

            _lastBoardEvaluation = _boardEvaluator.Evaluate(_board, _currentLevel.Enemies);

            _analytics.Track("wave_start", new Dictionary<string, object>
            {
                ["level_id"] = _currentLevel.LevelId,
                ["wave_id"] = 1,
                ["enemy_count"] = _currentLevel.Enemies.Count,
                ["enemy_pressure"] = _lastBoardEvaluation.EnemyPressure,
                ["total_protection"] = _lastBoardEvaluation.TotalProtection,
                ["net_damage"] = _lastBoardEvaluation.NetDamage
            });

            _waveManager.StartWave(_currentLevel.Enemies, _lastBoardEvaluation.NetDamage);
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
            _lastLevelWon = true;
            _lastLevelFailed = false;
            var rewardStored = _progression.TryStorePendingReward(
                _currentLevel.LevelId,
                _currentLevel.CoinReward,
                _currentLevel.PartsReward);

            _analytics.Track("level_complete", new Dictionary<string, object>
            {
                ["level_id"] = _currentLevel.LevelId,
                ["remaining_hp"] = _shelter.CurrentHealth,
                ["coins_pending"] = _currentLevel.CoinReward,
                ["parts_pending"] = _currentLevel.PartsReward,
                ["reward_pending"] = rewardStored
            });

            var questProgress = RecordQuestProgress(DailyQuestModel.CompleteLevelQuestId, 1);
            RefreshHud();
            var explanation = _lastBoardEvaluation?.ResultExplanation ?? "Victory!";
            var rewardMessage = rewardStored
                ? $" Reward pending: +{_currentLevel.CoinReward} coins, +{_currentLevel.PartsReward} parts."
                : " Reward is already pending.";
            hudView?.SetResult($"{explanation}{rewardMessage}{FormatQuestCompletionSuffix(questProgress)}");
            PreviewAvailableAdOffers();
            ProgressionChanged?.Invoke();
        }

        private void OnWaveFailed(int wave)
        {
            _levelEnded = true;
            _lastLevelWon = false;
            _lastLevelFailed = true;
            var failReason = _lastBoardEvaluation?.FailReason;
            if (string.IsNullOrEmpty(failReason))
                failReason = PrototypeBoardEvaluator.Unknown;

            _analytics.Track("level_fail", new Dictionary<string, object>
            {
                ["level_id"] = _currentLevel.LevelId,
                ["fail_reason"] = failReason
            });
            RefreshHud();
            hudView?.SetResult(_lastBoardEvaluation?.ResultExplanation ??
                               "Defeat. Your shelter was overwhelmed. Try stronger merges before the wave.");
            PreviewAvailableAdOffers();
            ProgressionChanged?.Invoke();
        }

        private void RefreshHud()
        {
            hudView?.SetLevel(_currentLevel.LevelId, _currentLevel.DisplayName);
            hudView?.SetTutorial(_currentLevel.TutorialMessage);
            hudView?.SetShelterHp(_shelter.CurrentHealth, _shelter.MaxHealth);
            hudView?.SetNextTile(_nextTile);
            hudView?.SetProgression(
                _progression.Coins,
                _progression.Parts,
                _progression.ShelterUpgradeLevel,
                _progression.ShelterUpgradeCost,
                _progression.CanAffordShelterUpgrade,
                _progression.CanClaimDailyReward,
                _progression.HasClaimedDailyReward,
                _progression.DailyRewardCoins,
                _progression.DailyRewardParts,
                _progression.DailyQuests);
        }

        private int GetShelterMaxHp()
        {
            return _progression.GetShelterMaxHealth(shelterMaxHp);
        }

        private static ISaveService CreateSaveService()
        {
            return string.IsNullOrWhiteSpace(SaveDirectoryOverrideForTests)
                ? new LocalJsonSaveService()
                : new LocalJsonSaveService(SaveDirectoryOverrideForTests);
        }

        private SessionProgressionState LoadProgression()
        {
            if (_saveService != null && _saveService.TryLoad(out var saveData))
                return SessionProgressionState.FromSaveData(saveData);

            return new SessionProgressionState();
        }

        private void SaveProgression()
        {
            if (_saveService == null || _progression == null)
                return;

            try
            {
                _saveService.Save(_progression.ToSaveData());
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Save failed: {exception.Message}");
            }
        }

        private DailyQuestProgressResult RecordQuestProgress(string questId, int amount)
        {
            if (!_progression.TryAddDailyQuestProgress(questId, amount, out var progress))
                return default;

            _analytics.Track("quest_progress", new Dictionary<string, object>
            {
                ["quest_id"] = progress.QuestId,
                ["title"] = progress.Title,
                ["progress"] = progress.Progress,
                ["target"] = progress.Target,
                ["completed"] = progress.Completed
            });

            if (progress.NewlyCompleted)
            {
                _analytics.Track("quest_completed", new Dictionary<string, object>
                {
                    ["quest_id"] = progress.QuestId,
                    ["title"] = progress.Title,
                    ["reward_coins"] = progress.RewardCoins,
                    ["reward_parts"] = progress.RewardParts
                });
            }

            SaveProgression();
            return progress;
        }

        private static string FormatQuestCompletionSuffix(DailyQuestProgressResult progress)
        {
            return progress.NewlyCompleted ? $" Quest complete: {progress.Title}." : string.Empty;
        }

        private bool ShowMockRewardedAd(AdPlacement placement)
        {
            _analytics.Track("ad_mock_started", new Dictionary<string, object>
            {
                ["placement"] = placement.ToString(),
                ["level_id"] = _currentLevel.LevelId
            });

            var completed = false;
            _adService.ShowRewarded(placement, success =>
            {
                completed = success;
                _analytics.Track("ad_mock_completed", new Dictionary<string, object>
                {
                    ["placement"] = placement.ToString(),
                    ["level_id"] = _currentLevel.LevelId,
                    ["success"] = success
                });
            });

            return completed;
        }

        private void PreviewAvailableAdOffers()
        {
            if (CanDoubleReward && !_rewardDoubleOfferPreviewed)
            {
                TrackAdOfferPreview(AdPlacement.RewardDouble);
                _rewardDoubleOfferPreviewed = true;
            }

            if (CanRevive && !_reviveOfferPreviewed)
            {
                TrackAdOfferPreview(AdPlacement.Revive);
                _reviveOfferPreviewed = true;
            }
        }

        private void TrackAdOfferPreview(AdPlacement placement)
        {
            _analytics.Track("ad_offer_preview", new Dictionary<string, object>
            {
                ["placement"] = placement.ToString(),
                ["level_id"] = _currentLevel.LevelId
            });
        }

        private void UnsubscribeWaveEvents()
        {
            if (_shelter != null)
                _shelter.Changed -= OnShelterChanged;

            if (_waveManager != null)
            {
                _waveManager.WaveCompleted -= OnWaveCompleted;
                _waveManager.WaveFailed -= OnWaveFailed;
            }
        }
    }
}
