using MergeShelter.Board;
using MergeShelter.Core;
using UnityEngine;

namespace MergeShelter.UI
{
    public enum PrototypeButtonVisualRole
    {
        Primary = 0,
        Secondary = 1,
        Reward = 2,
        Danger = 3,
        Upgrade = 4,
        Quest = 5,
        Reset = 6
    }

    public readonly struct PrototypeTileVisualStyle
    {
        public string DisplayName { get; }
        public string ShortLabel { get; }
        public string Icon { get; }
        public Color FillColor { get; }
        public Color TextColor { get; }

        public PrototypeTileVisualStyle(string displayName, string shortLabel, string icon, Color fillColor, Color textColor)
        {
            DisplayName = displayName;
            ShortLabel = shortLabel;
            Icon = icon;
            FillColor = fillColor;
            TextColor = textColor;
        }
    }

    public static class PrototypeVisualKit
    {
        public static readonly Color CanvasBackground = new(0.08f, 0.1f, 0.1f, 1f);
        public static readonly Color PanelBackground = new(0.13f, 0.17f, 0.18f, 1f);
        public static readonly Color PanelBorder = new(0.31f, 0.36f, 0.35f, 1f);
        public static readonly Color HudPanelBackground = new(0.105f, 0.135f, 0.14f, 1f);
        public static readonly Color HudPanelSecondaryBackground = new(0.09f, 0.115f, 0.12f, 1f);
        public static readonly Color HudPanelAccent = new(0.22f, 0.31f, 0.29f, 1f);
        public static readonly Color BoardPanelBackground = new(0.1f, 0.13f, 0.14f, 1f);
        public static readonly Color CellBorder = new(0.34f, 0.41f, 0.39f, 1f);
        public static readonly Color ActionPanelBackground = new(0.09f, 0.12f, 0.13f, 1f);
        public static readonly Color ResultPanelBackground = new(0.11f, 0.14f, 0.15f, 1f);
        public static readonly Color ButtonBorder = new(0.49f, 0.54f, 0.5f, 1f);
        public static readonly Color PrimaryText = new(0.94f, 0.94f, 0.88f, 1f);
        public static readonly Color SecondaryText = new(0.74f, 0.76f, 0.72f, 1f);
        public static readonly Color SectionText = new(0.66f, 0.7f, 0.66f, 1f);
        public static readonly Color EmptyCell = new(0.15f, 0.19f, 0.2f, 1f);
        public static readonly Color EmptyCellText = new(0.78f, 0.82f, 0.8f, 1f);
        public static readonly Color ShelterHealthy = new(0.78f, 0.9f, 0.78f, 1f);
        public static readonly Color ShelterWarning = new(0.94f, 0.75f, 0.42f, 1f);
        public static readonly Color ShelterDefeated = new(0.96f, 0.47f, 0.38f, 1f);
        public static readonly Color WaveReady = new(0.64f, 0.8f, 0.9f, 1f);
        public static readonly Color WaveVictory = new(0.66f, 0.93f, 0.64f, 1f);
        public static readonly Color WaveDefeat = new(0.98f, 0.65f, 0.5f, 1f);
        public static readonly Color ResourceText = new(0.92f, 0.87f, 0.72f, 1f);
        public static readonly Color ResultNeutral = new(0.86f, 0.88f, 0.85f, 1f);
        public static readonly Color ButtonText = new(0.98f, 0.98f, 0.94f, 1f);

        private static readonly Color WoodFill = new(0.56f, 0.35f, 0.16f, 1f);
        private static readonly Color MetalFill = new(0.32f, 0.4f, 0.46f, 1f);
        private static readonly Color FoodFill = new(0.22f, 0.47f, 0.28f, 1f);
        private static readonly Color EnergyFill = new(0.2f, 0.43f, 0.7f, 1f);
        private static readonly Color TileText = new(0.98f, 0.96f, 0.9f, 1f);
        private static readonly Color PrimaryButton = new(0.18f, 0.53f, 0.41f, 1f);
        private static readonly Color SecondaryButton = new(0.28f, 0.31f, 0.32f, 1f);
        private static readonly Color RewardButton = new(0.32f, 0.5f, 0.72f, 1f);
        private static readonly Color DangerButton = new(0.64f, 0.34f, 0.28f, 1f);
        private static readonly Color UpgradeButton = new(0.52f, 0.41f, 0.16f, 1f);
        private static readonly Color QuestButton = new(0.38f, 0.5f, 0.24f, 1f);
        private static readonly Color ResetButton = new(0.27f, 0.28f, 0.28f, 1f);

        public static PrototypeTileVisualStyle GetTileStyle(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Wood:
                    return new PrototypeTileVisualStyle("Wood", "Wood", "[W]", WoodFill, TileText);
                case TileType.Metal:
                    return new PrototypeTileVisualStyle("Metal", "Metal", "[M]", MetalFill, TileText);
                case TileType.Food:
                    return new PrototypeTileVisualStyle("Food", "Food", "[F]", FoodFill, TileText);
                case TileType.Energy:
                    return new PrototypeTileVisualStyle("Energy", "Energy", "[E]", EnergyFill, TileText);
                default:
                    return new PrototypeTileVisualStyle("Unknown", "?", "[?]", EmptyCell, EmptyCellText);
            }
        }

        public static Color GetTileFillColor(TileData tile)
        {
            return tile.IsEmpty ? EmptyCell : GetTileStyle(tile.Type).FillColor;
        }

        public static Color GetTileTextColor(TileData tile)
        {
            return tile.IsEmpty ? EmptyCellText : GetTileStyle(tile.Type).TextColor;
        }

        public static string FormatTileLabel(TileData tile)
        {
            if (tile.IsEmpty)
                return "+";

            var style = GetTileStyle(tile.Type);
            return $"{style.Icon} {style.ShortLabel}\nT{tile.Tier}";
        }

        public static Color GetActionButtonColor(string buttonName, bool isPrimary)
        {
            return GetActionButtonColor(GetActionButtonRole(buttonName), isPrimary);
        }

        public static Color GetActionButtonColor(PrototypeButtonVisualRole role, bool isPrimary)
        {
            var color = GetBaseButtonColor(role);
            return isPrimary ? Color.Lerp(color, PrimaryText, 0.18f) : color;
        }

        public static PrototypeButtonVisualRole GetActionButtonRole(string buttonName)
        {
            switch (buttonName)
            {
                case "StartWaveButton":
                case "NextLevelButton":
                    return PrototypeButtonVisualRole.Primary;
                case "ClaimRewardButton":
                case "DailyRewardButton":
                case "DoubleRewardButton":
                    return PrototypeButtonVisualRole.Reward;
                case "RetryButton":
                case "ReviveButton":
                    return PrototypeButtonVisualRole.Danger;
                case "UpgradeShelterButton":
                    return PrototypeButtonVisualRole.Upgrade;
                case "ClaimQuestButton":
                    return PrototypeButtonVisualRole.Quest;
                case "ResetSaveButton":
                    return PrototypeButtonVisualRole.Reset;
                default:
                    return PrototypeButtonVisualRole.Secondary;
            }
        }

        public static Color GetFeedbackColor(PrototypeFeedbackKind kind)
        {
            switch (kind)
            {
                case PrototypeFeedbackKind.TilePlaced:
                case PrototypeFeedbackKind.WaveStart:
                case PrototypeFeedbackKind.NextLevel:
                case PrototypeFeedbackKind.Retry:
                    return WaveReady;
                case PrototypeFeedbackKind.MergeSuccess:
                case PrototypeFeedbackKind.WaveVictory:
                case PrototypeFeedbackKind.RewardClaim:
                case PrototypeFeedbackKind.DailyRewardClaim:
                case PrototypeFeedbackKind.QuestClaim:
                case PrototypeFeedbackKind.ShelterUpgrade:
                case PrototypeFeedbackKind.RewardDouble:
                case PrototypeFeedbackKind.Revive:
                    return WaveVictory;
                case PrototypeFeedbackKind.InvalidPlacement:
                case PrototypeFeedbackKind.WaveDefeat:
                case PrototypeFeedbackKind.Blocked:
                    return WaveDefeat;
                case PrototypeFeedbackKind.ResetSave:
                    return ResultNeutral;
                default:
                    return ResultNeutral;
            }
        }

        public static Color GetCellFeedbackColor(PrototypeFeedbackKind kind)
        {
            switch (kind)
            {
                case PrototypeFeedbackKind.MergeSuccess:
                    return WaveVictory;
                case PrototypeFeedbackKind.InvalidPlacement:
                case PrototypeFeedbackKind.Blocked:
                    return ShelterDefeated;
                case PrototypeFeedbackKind.TilePlaced:
                    return ShelterWarning;
                default:
                    return PrimaryText;
            }
        }

        public static float GetCellFeedbackScale(PrototypeFeedbackKind kind)
        {
            switch (kind)
            {
                case PrototypeFeedbackKind.MergeSuccess:
                    return 1.1f;
                case PrototypeFeedbackKind.TilePlaced:
                    return 1.05f;
                case PrototypeFeedbackKind.InvalidPlacement:
                case PrototypeFeedbackKind.Blocked:
                    return 0.96f;
                default:
                    return 1.04f;
            }
        }

        public static Color GetResultPanelColor(PrototypeFeedbackKind kind)
        {
            return kind == PrototypeFeedbackKind.None
                ? ResultPanelBackground
                : Color.Lerp(ResultPanelBackground, GetFeedbackColor(kind), 0.16f);
        }

        public static Color GetResultPanelFlashColor(PrototypeFeedbackKind kind)
        {
            return kind == PrototypeFeedbackKind.None
                ? ResultPanelBackground
                : Color.Lerp(ResultPanelBackground, GetFeedbackColor(kind), 0.42f);
        }

        private static Color GetBaseButtonColor(PrototypeButtonVisualRole role)
        {
            switch (role)
            {
                case PrototypeButtonVisualRole.Primary:
                    return PrimaryButton;
                case PrototypeButtonVisualRole.Reward:
                    return RewardButton;
                case PrototypeButtonVisualRole.Danger:
                    return DangerButton;
                case PrototypeButtonVisualRole.Upgrade:
                    return UpgradeButton;
                case PrototypeButtonVisualRole.Quest:
                    return QuestButton;
                case PrototypeButtonVisualRole.Reset:
                    return ResetButton;
                default:
                    return SecondaryButton;
            }
        }
    }
}
