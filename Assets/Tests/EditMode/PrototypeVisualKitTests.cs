using System.Collections.Generic;
using MergeShelter.Board;
using MergeShelter.Core;
using MergeShelter.UI;
using NUnit.Framework;
using UnityEngine;

namespace MergeShelter.Tests.EditMode
{
    public sealed class PrototypeVisualKitTests
    {
        [Test]
        public void PlaceholderVisualKit_ExposesThemeColors()
        {
            Assert.GreaterOrEqual(PrototypeVisualKit.CanvasBackground.a, 0.99f);
            Assert.GreaterOrEqual(PrototypeVisualKit.PanelBackground.a, 0.99f);
            Assert.GreaterOrEqual(PrototypeVisualKit.PanelBorder.a, 0.99f);
            Assert.GreaterOrEqual(PrototypeVisualKit.BoardPanelBackground.a, 0.99f);
            Assert.GreaterOrEqual(PrototypeVisualKit.CellBorder.a, 0.99f);
            Assert.GreaterOrEqual(PrototypeVisualKit.ActionPanelBackground.a, 0.99f);
            Assert.GreaterOrEqual(PrototypeVisualKit.ResultPanelBackground.a, 0.99f);
            Assert.Greater(ColorDistance(PrototypeVisualKit.CanvasBackground, PrototypeVisualKit.PanelBackground), 0.04f);
            Assert.Greater(ColorDistance(PrototypeVisualKit.EmptyCell, PrototypeVisualKit.CellBorder), 0.08f);
            Assert.Greater(ColorDistance(PrototypeVisualKit.ActionPanelBackground, PrototypeVisualKit.PanelBorder), 0.18f);
            Assert.Greater(ColorDistance(PrototypeVisualKit.ShelterHealthy, PrototypeVisualKit.ShelterDefeated), 0.35f);
            Assert.Greater(ColorDistance(PrototypeVisualKit.WaveVictory, PrototypeVisualKit.WaveDefeat), 0.25f);
        }

        [Test]
        public void PlaceholderVisualKit_TileTypesMapToDistinctReadableStyles()
        {
            var tileTypes = new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy };
            var colors = new List<Color>();
            var icons = new HashSet<string>();

            foreach (var tileType in tileTypes)
            {
                var style = PrototypeVisualKit.GetTileStyle(tileType);
                Assert.IsFalse(string.IsNullOrWhiteSpace(style.DisplayName));
                Assert.IsFalse(string.IsNullOrWhiteSpace(style.ShortLabel));
                Assert.IsTrue(icons.Add(style.Icon), $"{tileType} should have a unique icon.");
                Assert.Greater(Contrast(style.FillColor, style.TextColor), 2.0f, $"{tileType} should have readable tile text contrast.");
                colors.Add(style.FillColor);
            }

            for (var i = 0; i < colors.Count; i++)
            {
                for (var j = i + 1; j < colors.Count; j++)
                    Assert.Greater(ColorDistance(colors[i], colors[j]), 0.16f, "Tile fill colors should be visually distinct.");
            }
        }

        [Test]
        public void PlaceholderVisualKit_FormatsBoardAndButtonStates()
        {
            var woodTile = new TileData(TileType.Wood, 2);
            Assert.That(PrototypeVisualKit.FormatTileLabel(TileData.Empty), Is.EqualTo("+"));
            Assert.That(PrototypeVisualKit.FormatTileLabel(woodTile), Does.Contain("[W]"));
            Assert.That(PrototypeVisualKit.FormatTileLabel(woodTile), Does.Contain("T2"));
            Assert.AreEqual(PrototypeVisualKit.EmptyCell, PrototypeVisualKit.GetTileFillColor(TileData.Empty));
            Assert.AreNotEqual(PrototypeVisualKit.EmptyCell, PrototypeVisualKit.GetTileFillColor(woodTile));

            var normalStart = PrototypeVisualKit.GetActionButtonColor("StartWaveButton", false);
            var primaryStart = PrototypeVisualKit.GetActionButtonColor("StartWaveButton", true);
            var revive = PrototypeVisualKit.GetActionButtonColor("ReviveButton", false);
            Assert.Greater(ColorDistance(normalStart, primaryStart), 0.04f);
            Assert.Greater(ColorDistance(normalStart, revive), 0.15f);
            Assert.AreNotEqual(
                PrototypeVisualKit.GetFeedbackColor(PrototypeFeedbackKind.WaveVictory),
                PrototypeVisualKit.GetFeedbackColor(PrototypeFeedbackKind.WaveDefeat));
            Assert.AreNotEqual(
                PrototypeVisualKit.GetResultPanelColor(PrototypeFeedbackKind.WaveVictory),
                PrototypeVisualKit.GetResultPanelColor(PrototypeFeedbackKind.WaveDefeat));
            Assert.AreNotEqual(
                PrototypeVisualKit.GetResultPanelColor(PrototypeFeedbackKind.WaveVictory),
                PrototypeVisualKit.GetResultPanelFlashColor(PrototypeFeedbackKind.WaveVictory));
            Assert.Greater(
                PrototypeVisualKit.GetCellFeedbackScale(PrototypeFeedbackKind.MergeSuccess),
                PrototypeVisualKit.GetCellFeedbackScale(PrototypeFeedbackKind.TilePlaced));
            Assert.Less(
                PrototypeVisualKit.GetCellFeedbackScale(PrototypeFeedbackKind.InvalidPlacement),
                1f);
        }

        private static float ColorDistance(Color first, Color second)
        {
            var red = first.r - second.r;
            var green = first.g - second.g;
            var blue = first.b - second.b;
            return Mathf.Sqrt(red * red + green * green + blue * blue);
        }

        private static float Contrast(Color first, Color second)
        {
            var firstLuminance = Luminance(first) + 0.05f;
            var secondLuminance = Luminance(second) + 0.05f;
            return firstLuminance > secondLuminance
                ? firstLuminance / secondLuminance
                : secondLuminance / firstLuminance;
        }

        private static float Luminance(Color color)
        {
            return 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
        }
    }
}
