using System;
using System.Collections.Generic;
using System.Text;
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
    public enum PrototypeTutorialStep
    {
        PlaceFirstTile = 0,
        PlaceMoreTiles = 1,
        MergeIntent = 2,
        StartWave = 3,
        ClaimReward = 4,
        Continue = 5,
        Complete = 6
    }

    public enum PrototypeFeedbackKind
    {
        None = 0,
        TilePlaced = 1,
        MergeSuccess = 2,
        InvalidPlacement = 3,
        WaveStart = 4,
        WaveVictory = 5,
        WaveDefeat = 6,
        RewardClaim = 7,
        DailyRewardClaim = 8,
        QuestClaim = 9,
        ShelterUpgrade = 10,
        RewardDouble = 11,
        Revive = 12,
        ResetSave = 13,
        NextLevel = 14,
        Retry = 15,
        Blocked = 16
    }

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
        private PrototypeTutorialStep _tutorialStep;
        private int _tutorialTilesPlaced;
        private readonly List<PrototypeFeedbackKind> _feedbackHistory = new();

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
        public PrototypeTutorialStep TutorialStep => _tutorialStep;
        public int TutorialTilesPlaced => _tutorialTilesPlaced;
        public bool IsTutorialComplete => _tutorialStep == PrototypeTutorialStep.Complete;
        public PrototypeFeedbackKind LastFeedbackKind { get; private set; } = PrototypeFeedbackKind.None;
        public string LastFeedbackMessage { get; private set; } = string.Empty;

        public event Action BoardChanged;
        public event Action ProgressionChanged;

        private void Awake()
        {
            _analytics = new DebugAnalyticsService();
            _levels = SprintOneLevelCatalog.CreateLevels();
            _saveService = CreateSaveService();
            var saveData = LoadSaveData();
            _progression = saveData != null
                ? SessionProgressionState.FromSaveData(saveData)
                : new SessionProgressionState();
            LoadTutorialState(saveData);
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
                ShowFeedback(PrototypeFeedbackKind.Blocked, $"Level {levelId} is locked. Claim rewards to unlock it.");
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
                ShowFeedback(PrototypeFeedbackKind.Blocked, "No reward is waiting to claim.");
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
            var tutorialAdvanced = AdvanceTutorialAfterRewardClaim(reward.LevelId);
            SaveProgression();
            RefreshHud();
            var nextLevelMessage = CanStartNextLevel ? $" Level {_progression.SelectedLevel + 1} unlocked." : string.Empty;
            var upgradePrompt = FormatUpgradePrompt();
            ShowFeedback(PrototypeFeedbackKind.RewardClaim, $"Reward claimed: +{reward.Coins} coins, +{reward.Parts} parts.{nextLevelMessage}{upgradePrompt}{FormatQuestProgressSuffix(questProgress)}{FormatTutorialSuffix(tutorialAdvanced)}");
            ProgressionChanged?.Invoke();
            return true;
        }

        public bool ClaimDailyReward()
        {
            if (!_progression.TryClaimDailyReward(out var reward))
            {
                RefreshHud();
                ShowFeedback(PrototypeFeedbackKind.Blocked, "Daily reward already claimed this session.");
                ProgressionChanged?.Invoke();
                return false;
            }

            _analytics.Track("daily_reward_claimed", new Dictionary<string, object>
            {
                ["coins"] = reward.Coins,
                ["parts"] = reward.Parts
            });

            var tutorialCompleted = AdvanceTutorialAfterOptionalContinueAction();
            RefreshHud();
            ShowFeedback(PrototypeFeedbackKind.DailyRewardClaim, $"Daily reward claimed: +{reward.Coins} coins, +{reward.Parts} parts.{FormatTutorialSuffix(tutorialCompleted)}");
            SaveProgression();
            ProgressionChanged?.Invoke();
            return true;
        }

        public bool ClaimQuest()
        {
            if (!_progression.TryClaimDailyQuest(out var reward))
            {
                RefreshHud();
                ShowFeedback(PrototypeFeedbackKind.Blocked, "No quest ready. Finish progress first; claimed quests pay once.");
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
            ShowFeedback(PrototypeFeedbackKind.QuestClaim, $"Quest claimed: {reward.Title}. +{reward.Coins} coins, +{reward.Parts} parts.");
            SaveProgression();
            ProgressionChanged?.Invoke();
            return true;
        }

        public bool DoubleReward()
        {
            if (!CanDoubleReward)
            {
                RefreshHud();
                ShowFeedback(PrototypeFeedbackKind.Blocked, "Double Reward is not available for this result.");
                ProgressionChanged?.Invoke();
                return false;
            }

            if (!ShowMockRewardedAd(AdPlacement.RewardDouble))
            {
                RefreshHud();
                ShowFeedback(PrototypeFeedbackKind.Blocked, "Double Reward mock ad was not completed.");
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
            ShowFeedback(PrototypeFeedbackKind.RewardDouble, $"Reward doubled. Pending reward: +{reward.Coins} coins, +{reward.Parts} parts.");
            SaveProgression();
            ProgressionChanged?.Invoke();
            return true;
        }

        public bool Revive()
        {
            if (!CanRevive)
            {
                RefreshHud();
                ShowFeedback(PrototypeFeedbackKind.Blocked, "Revive is not available for this result.");
                ProgressionChanged?.Invoke();
                return false;
            }

            if (!ShowMockRewardedAd(AdPlacement.Revive))
            {
                RefreshHud();
                ShowFeedback(PrototypeFeedbackKind.Blocked, "Revive mock ad was not completed.");
                ProgressionChanged?.Invoke();
                return false;
            }

            _reviveUsedThisResult = true;
            _analytics.Track("revive_used", new Dictionary<string, object>
            {
                ["level_id"] = _currentLevel.LevelId
            });

            StartSelectedLevel(preserveReviveUsage: true);
            var reviveObjective = !string.IsNullOrWhiteSpace(_currentLevel.Objective)
                ? $" Goal: {_currentLevel.Objective}."
                : string.Empty;
            ShowFeedback(PrototypeFeedbackKind.Revive, $"Revive used. Level restarted with a fresh shelter.{reviveObjective}");
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
                ShowFeedback(PrototypeFeedbackKind.Blocked, $"Upgrade blocked. Need {missingCoins} more coins for Shelter Lv {previousLevel + 1}.");
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

            var tutorialCompleted = AdvanceTutorialAfterOptionalContinueAction();
            RefreshHud();
            ShowFeedback(PrototypeFeedbackKind.ShelterUpgrade, $"Shelter upgraded to Lv {newLevel}. Future waves start with {GetShelterMaxHp()} HP.{FormatTutorialSuffix(tutorialCompleted)}");
            SaveProgression();
            ProgressionChanged?.Invoke();
            return true;
        }

        public void ResetSave()
        {
            _saveService?.Reset();
            _progression = new SessionProgressionState();
            ResetTutorialState();
            StartSelectedLevel();
            ShowFeedback(PrototypeFeedbackKind.ResetSave, "Save reset. Progress returned to Level 1.");
            BoardChanged?.Invoke();
            ProgressionChanged?.Invoke();
        }

        public bool StartNextLevel()
        {
            if (!CanStartNextLevel)
            {
                ShowFeedback(PrototypeFeedbackKind.Blocked, "Next level is locked. Claim the pending reward first.");
                ProgressionChanged?.Invoke();
                return false;
            }

            var tutorialShouldComplete = _tutorialStep == PrototypeTutorialStep.Continue;
            var started = TryStartLevel(_progression.SelectedLevel + 1);
            if (started && tutorialShouldComplete)
            {
                SetTutorialStep(PrototypeTutorialStep.Complete, saveAfterChange: false);
                RefreshHud();
                ShowFeedback(PrototypeFeedbackKind.NextLevel, "Tutorial complete. Level 2 is ready.");
                SaveProgression();
                ProgressionChanged?.Invoke();
            }
            else if (started)
            {
                ShowFeedback(PrototypeFeedbackKind.NextLevel, $"Level {_progression.SelectedLevel} started. Build before the wave.");
            }

            return started;
        }

        public bool RetryLevel()
        {
            if (!CanRetryLevel)
            {
                ShowFeedback(PrototypeFeedbackKind.Blocked, "Retry is available after defeat.");
                ProgressionChanged?.Invoke();
                return false;
            }

            StartSelectedLevel();
            var retryObjective = !string.IsNullOrWhiteSpace(_currentLevel.Objective)
                ? $" Goal: {_currentLevel.Objective}."
                : string.Empty;
            ShowFeedback(PrototypeFeedbackKind.Retry, $"Retry started. Rebuild stronger before the wave.{retryObjective}");
            return true;
        }

        private void StartSelectedLevel(bool preserveReviveUsage = false)
        {
            var levelIndex = Mathf.Clamp(_progression.SelectedLevel - 1, 0, _levels.Count - 1);
            StartLevel(_levels[levelIndex], preserveReviveUsage);
        }

        private void StartLevel(LevelDefinition level, bool preserveReviveUsage = false)
        {
            UnsubscribeWaveEvents();
            var reviveWasUsed = preserveReviveUsage && _reviveUsedThisResult;
            _currentLevel = level;
            _board.Clear();
            _shelter = new ShelterHealth(GetShelterMaxHp());
            _waveManager = new WaveManager(_shelter);
            _tileGenerator.Configure(_currentLevel);
            _levelEnded = false;
            _lastLevelWon = false;
            _lastLevelFailed = false;
            _rewardDoubleUsedThisResult = false;
            _reviveUsedThisResult = reviveWasUsed;
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
            hudView?.SetResult(IsTutorialComplete
                ? GetBuildPhasePrompt()
                : GetTutorialResultHint());
            BoardChanged?.Invoke();
            ProgressionChanged?.Invoke();
        }

        public bool TryPlaceNextTile(int x, int y)
        {
            if (_levelEnded)
            {
                ShowFeedback(PrototypeFeedbackKind.Blocked, "Board is locked after the wave. Use the result action.");
                return false;
            }

            var position = new BoardPosition(x, y);
            var placed = _board.TryPlace(position, _nextTile);
            if (!placed)
            {
                _analytics.Track("merge_failed", new Dictionary<string, object>
                {
                    ["level_id"] = _currentLevel.LevelId,
                    ["reason"] = "invalid_placement"
                });
                ShowFeedback(PrototypeFeedbackKind.InvalidPlacement, "Cell occupied. Place near matching tiles to set up merges.");
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
            var feedbackKind = PrototypeFeedbackKind.TilePlaced;
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
                feedbackKind = PrototypeFeedbackKind.MergeSuccess;
            }
            else
            {
                resultMessage = "Tile placed. Build toward a merge of 3.";
            }

            AdvanceTutorialAfterTilePlaced();

            var questProgress = RecordQuestProgress(DailyQuestModel.PlaceTilesQuestId, 1);
            _nextTile = _tileGenerator.GenerateNextTile();
            RefreshHud();
            ShowFeedback(feedbackKind, $"{resultMessage}{FormatQuestProgressSuffix(questProgress)}");
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
            {
                ShowFeedback(PrototypeFeedbackKind.Blocked, "Wave is already resolved. Choose the next result action.");
                return;
            }

            _lastBoardEvaluation = _boardEvaluator.Evaluate(_board, _currentLevel.Enemies);
            ShowFeedback(PrototypeFeedbackKind.WaveStart, "Wave started. Shelter is resolving the attack.");

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
            AdvanceTutorialAfterWaveCompleted();
            RefreshHud();
            var explanation = _lastBoardEvaluation?.ResultExplanation ?? "Victory!";
            var objectiveSuffix = !string.IsNullOrWhiteSpace(_currentLevel.Objective)
                ? $" Objective complete: {_currentLevel.Objective}."
                : string.Empty;
            var rewardMessage = rewardStored
                ? $" Reward pending: +{_currentLevel.CoinReward} coins, +{_currentLevel.PartsReward} parts."
                : " Reward is already pending.";
            ShowFeedback(PrototypeFeedbackKind.WaveVictory, $"{explanation}{objectiveSuffix}{rewardMessage}{FormatQuestProgressSuffix(questProgress)}");
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
            var defeatExplanation = _lastBoardEvaluation?.ResultExplanation ??
                                    "Defeat. Your shelter was overwhelmed.";
            var defeatHint = PrototypeBoardEvaluator.GetDefeatHint(failReason);
            ShowFeedback(PrototypeFeedbackKind.WaveDefeat, $"{defeatExplanation} {defeatHint}");
            PreviewAvailableAdOffers();
            ProgressionChanged?.Invoke();
        }

        private void RefreshHud()
        {
            hudView?.SetLevel(_currentLevel.LevelId, _currentLevel.DisplayName);
            hudView?.SetObjective(_currentLevel.Objective);
            hudView?.SetWaveRoster(_levelEnded ? string.Empty : EnemyData.FormatWaveRoster(_currentLevel.Enemies));
            hudView?.SetTutorial(IsTutorialComplete ? _currentLevel.TutorialMessage : GetTutorialMessage());
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

        private GameSaveData LoadSaveData()
        {
            if (_saveService != null && _saveService.TryLoad(out var saveData))
                return saveData;

            return null;
        }

        private void LoadTutorialState(GameSaveData saveData)
        {
            if (saveData == null)
            {
                ResetTutorialState();
                return;
            }

            if (!saveData.TutorialStateSaved)
            {
                _tutorialStep = saveData.HighestUnlockedLevel > SessionProgressionState.FirstLevel ||
                                saveData.SelectedLevel > SessionProgressionState.FirstLevel
                    ? PrototypeTutorialStep.Complete
                    : PrototypeTutorialStep.PlaceFirstTile;
                _tutorialTilesPlaced = 0;
                return;
            }

            _tutorialStep = ClampTutorialStep(saveData.TutorialStep);
            _tutorialTilesPlaced = Mathf.Max(0, saveData.TutorialTilesPlaced);
        }

        private void SaveProgression()
        {
            if (_saveService == null || _progression == null)
                return;

            try
            {
                var saveData = _progression.ToSaveData();
                saveData.TutorialStateSaved = true;
                saveData.TutorialStep = (int)_tutorialStep;
                saveData.TutorialTilesPlaced = _tutorialTilesPlaced;
                _saveService.Save(saveData);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Save failed: {exception.Message}");
            }
        }

        public bool HasShownFeedback(PrototypeFeedbackKind kind)
        {
            return _feedbackHistory.Contains(kind);
        }

        private void ShowFeedback(PrototypeFeedbackKind kind, string message)
        {
            LastFeedbackKind = kind;
            LastFeedbackMessage = message ?? string.Empty;

            if (kind != PrototypeFeedbackKind.None)
            {
                _feedbackHistory.Add(kind);
                if (_feedbackHistory.Count > 32)
                    _feedbackHistory.RemoveAt(0);
            }

            hudView?.SetFeedback(kind, LastFeedbackMessage);
        }

        private void ResetTutorialState()
        {
            _tutorialStep = PrototypeTutorialStep.PlaceFirstTile;
            _tutorialTilesPlaced = 0;
        }

        private static PrototypeTutorialStep ClampTutorialStep(int step)
        {
            if (step < (int)PrototypeTutorialStep.PlaceFirstTile)
                return PrototypeTutorialStep.PlaceFirstTile;

            if (step > (int)PrototypeTutorialStep.Complete)
                return PrototypeTutorialStep.Complete;

            return (PrototypeTutorialStep)step;
        }

        private bool SetTutorialStep(PrototypeTutorialStep step, bool saveAfterChange = true)
        {
            if (_tutorialStep == step)
                return false;

            _tutorialStep = step;
            if (saveAfterChange)
                SaveProgression();

            return true;
        }

        private bool AdvanceTutorialAfterTilePlaced()
        {
            if (IsTutorialComplete)
                return false;

            _tutorialTilesPlaced++;

            if (_tutorialStep == PrototypeTutorialStep.PlaceFirstTile)
                return SetTutorialStep(PrototypeTutorialStep.PlaceMoreTiles, saveAfterChange: false);

            if (_tutorialStep == PrototypeTutorialStep.PlaceMoreTiles && _tutorialTilesPlaced >= 2)
                return SetTutorialStep(PrototypeTutorialStep.MergeIntent, saveAfterChange: false);

            if (_tutorialStep == PrototypeTutorialStep.MergeIntent && _tutorialTilesPlaced >= 3)
                return SetTutorialStep(PrototypeTutorialStep.StartWave, saveAfterChange: false);

            return false;
        }

        private bool AdvanceTutorialAfterWaveCompleted()
        {
            if (_tutorialStep == PrototypeTutorialStep.PlaceFirstTile ||
                _tutorialStep == PrototypeTutorialStep.PlaceMoreTiles ||
                _tutorialStep == PrototypeTutorialStep.MergeIntent ||
                _tutorialStep == PrototypeTutorialStep.StartWave)
            {
                return SetTutorialStep(PrototypeTutorialStep.ClaimReward, saveAfterChange: false);
            }

            return false;
        }

        private bool AdvanceTutorialAfterRewardClaim(int levelId)
        {
            if (levelId == SessionProgressionState.FirstLevel &&
                _tutorialStep == PrototypeTutorialStep.ClaimReward)
            {
                return SetTutorialStep(PrototypeTutorialStep.Continue, saveAfterChange: false);
            }

            return false;
        }

        private bool AdvanceTutorialAfterOptionalContinueAction()
        {
            if (_tutorialStep != PrototypeTutorialStep.Continue)
                return false;

            return SetTutorialStep(PrototypeTutorialStep.Complete, saveAfterChange: false);
        }

        private string GetTutorialMessage()
        {
            switch (_tutorialStep)
            {
                case PrototypeTutorialStep.PlaceFirstTile:
                    return "Tutorial: tap an empty board cell to place your first tile.";
                case PrototypeTutorialStep.PlaceMoreTiles:
                    return "Tutorial: place two more tiles. Matching 3 creates a stronger tile.";
                case PrototypeTutorialStep.MergeIntent:
                    return "Tutorial: one more nearby tile can make a merge. Build before the wave.";
                case PrototypeTutorialStep.StartWave:
                    return "Tutorial: good. Tap Start Wave to test your shelter.";
                case PrototypeTutorialStep.ClaimReward:
                    return "Tutorial: victory creates a reward. Tap Claim Reward.";
                case PrototypeTutorialStep.Continue:
                    return "Tutorial: tap Next Level. Daily Reward can help upgrades.";
                default:
                    return _currentLevel != null ? _currentLevel.TutorialMessage : string.Empty;
            }
        }

        private string GetBuildPhasePrompt()
        {
            var tiles = _currentLevel?.AvailableTiles;
            if (tiles == null || tiles.Count == 0)
                return "Place tiles, merge, then start the wave.";

            var builder = new StringBuilder();
            foreach (var tile in tiles)
            {
                if (builder.Length > 0)
                    builder.Append(", ");

                builder.Append(GetTileRoleHint(tile));
            }

            builder.Append(". Merge 3 matching tiles, then start the wave.");
            return builder.ToString();
        }

        private static string GetTileRoleHint(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Wood: return "Wood (walls)";
                case TileType.Metal: return "Metal (turrets)";
                case TileType.Food: return "Food (healing)";
                case TileType.Energy: return "Energy (shields)";
                default: return tileType.ToString();
            }
        }

        private string FormatUpgradePrompt()
        {
            if (!_progression.CanAffordShelterUpgrade)
                return string.Empty;

            return $" Upgrade available: Shelter Lv {_progression.ShelterUpgradeLevel + 1} for {_progression.ShelterUpgradeCost} coins.";
        }

        private string GetTutorialResultHint()
        {
            switch (_tutorialStep)
            {
                case PrototypeTutorialStep.PlaceFirstTile:
                    return "Tap a board cell to place a tile.";
                case PrototypeTutorialStep.PlaceMoreTiles:
                    return "Place more tiles to set up your first merge.";
                case PrototypeTutorialStep.MergeIntent:
                    return "Matching 3 tiles merge into a stronger tile.";
                case PrototypeTutorialStep.StartWave:
                    return "Start Wave is the next step.";
                case PrototypeTutorialStep.ClaimReward:
                    return "Claim the pending reward.";
                case PrototypeTutorialStep.Continue:
                    return "Continue to Level 2 or use a reward action.";
                default:
                    return "Place tiles, merge, then start the wave.";
            }
        }

        private static string FormatTutorialSuffix(bool advanced)
        {
            return advanced ? " Tutorial updated." : string.Empty;
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

        private static string FormatQuestProgressSuffix(DailyQuestProgressResult progress)
        {
            if (progress.IsEmpty)
                return string.Empty;

            if (progress.NewlyCompleted)
                return $" Quest ready: {progress.Title} ({FormatQuestReward(progress.RewardCoins, progress.RewardParts)}).";

            return $" Quest: {progress.Title} {progress.Progress}/{progress.Target}.";
        }

        private static string FormatQuestReward(int coins, int parts)
        {
            if (parts > 0)
                return $"+{coins}c, +{parts}p";

            return $"+{coins}c";
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
