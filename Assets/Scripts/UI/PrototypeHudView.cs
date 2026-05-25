using System.Collections.Generic;
using System.Text;
using MergeShelter.Board;
using MergeShelter.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace MergeShelter.UI
{
    public sealed class PrototypeHudView : MonoBehaviour
    {
        public const string OpaqueBackgroundName = "PrototypeOpaqueBackground";

        private const float HorizontalPadding = 24f;
        private const float TopPadding = 16f;
        private const float LevelHeight = 28f;
        private const float TutorialTop = 48f;
        private const float TutorialHeight = 42f;
        private const float StatusTop = 98f;
        private const float StatusHeight = 24f;
        private const float WalletTop = 130f;
        private const float WalletHeight = 108f;
        private const float ResultBottom = 236f;
        private const float ResultHeight = 92f;

        [SerializeField] private Text levelText;
        [SerializeField] private Text tutorialText;
        [SerializeField] private Text shelterHpText;
        [SerializeField] private Text nextTileText;
        [SerializeField] private Text resultText;
        [SerializeField] private Text walletText;

        private bool _isApplyingLayout;

        private void Awake()
        {
            EnsureOpaqueCanvasBackground();
            ApplyPhoneSafeLayout();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!_isApplyingLayout)
                ApplyPhoneSafeLayout();
        }

        public void ApplyPhoneSafeLayout()
        {
            if (_isApplyingLayout)
                return;

            _isApplyingLayout = true;
            try
            {
                ConfigureTopText(levelText, TopPadding, LevelHeight, 20, TextAnchor.UpperLeft);
                ConfigureTopText(tutorialText, TutorialTop, TutorialHeight, 14, TextAnchor.UpperLeft);
                ConfigureTopText(shelterHpText, StatusTop, StatusHeight, 14, TextAnchor.MiddleLeft, 0f, 0.5f, HorizontalPadding, 8f);
                ConfigureTopText(nextTileText, StatusTop, StatusHeight, 14, TextAnchor.MiddleRight, 0.5f, 1f, 8f, HorizontalPadding);
                ConfigureTopText(walletText, WalletTop, WalletHeight, 12, TextAnchor.UpperLeft);
                ConfigureBottomText(resultText, ResultBottom, ResultHeight, 14, TextAnchor.UpperLeft);
            }
            finally
            {
                _isApplyingLayout = false;
            }
        }

        public void SetLevel(int levelId, string displayName)
        {
            if (levelText != null)
                levelText.text = $"Level {levelId}: {displayName}";
        }

        public void SetTutorial(string message)
        {
            if (tutorialText != null)
                tutorialText.text = message;
        }

        public void SetShelterHp(int current, int max)
        {
            if (shelterHpText != null)
                shelterHpText.text = $"Shelter HP: {current}/{max}";
        }

        public void SetNextTile(TileData tile)
        {
            if (nextTileText != null)
                nextTileText.text = tile.IsEmpty ? "Next: None" : $"Next: {tile.Type} T{tile.Tier}";
        }

        public void SetResult(string message)
        {
            if (resultText != null)
            {
                ConfigureBottomText(resultText, ResultBottom, ResultHeight, 14, TextAnchor.UpperLeft);
                resultText.text = message;
            }
        }

        public void SetWallet(int coins, int parts)
        {
            if (walletText != null)
            {
                ConfigureTopText(walletText, WalletTop, WalletHeight, 12, TextAnchor.UpperLeft);
                walletText.text = $"Coins: {coins} | Parts: {parts}";
            }
        }

        public void SetProgression(
            int coins,
            int parts,
            int shelterUpgradeLevel,
            int upgradeCost,
            bool canAffordUpgrade,
            bool canClaimDailyReward = false,
            bool hasClaimedDailyReward = false,
            int dailyRewardCoins = 0,
            int dailyRewardParts = 0,
            IReadOnlyList<DailyQuestState> dailyQuests = null)
        {
            if (walletText == null)
                return;

            ConfigureTopText(walletText, WalletTop, WalletHeight, 12, TextAnchor.UpperLeft);
            var affordText = canAffordUpgrade ? "can afford" : $"need {upgradeCost - coins} more";
            var dailyRewardStatus = hasClaimedDailyReward ? "claimed" : canClaimDailyReward ? "available" : "unavailable";
            walletText.text =
                $"Coins: {coins} | Parts: {parts} | Shelter Lv {shelterUpgradeLevel}\nUpgrade: {upgradeCost} coins ({affordText}) | Daily Reward: +{dailyRewardCoins} coins, +{dailyRewardParts} parts ({dailyRewardStatus})\n{FormatQuestStatus(dailyQuests)}";
        }

        private static string FormatQuestStatus(IReadOnlyList<DailyQuestState> dailyQuests)
        {
            if (dailyQuests == null || dailyQuests.Count == 0)
                return "Daily Quests: none";

            var builder = new StringBuilder("Daily Quests: ");
            for (var i = 0; i < dailyQuests.Count; i++)
            {
                var quest = dailyQuests[i];
                if (i > 0)
                    builder.Append(" | ");

                builder.Append(quest.Title);
                builder.Append(' ');
                builder.Append(quest.Progress);
                builder.Append('/');
                builder.Append(quest.Target);

                if (quest.Claimed)
                    builder.Append(" claimed");
                else if (quest.Completed)
                    builder.Append(" ready");
            }

            return builder.ToString();
        }

        private static void ConfigureTopText(
            Text text,
            float top,
            float height,
            int fontSize,
            TextAnchor alignment,
            float anchorMinX = 0f,
            float anchorMaxX = 1f,
            float leftInset = HorizontalPadding,
            float rightInset = HorizontalPadding)
        {
            if (text == null)
                return;

            var rectTransform = (RectTransform)text.transform;
            rectTransform.anchorMin = new Vector2(anchorMinX, 1f);
            rectTransform.anchorMax = new Vector2(anchorMaxX, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.offsetMin = new Vector2(leftInset, -top - height);
            rectTransform.offsetMax = new Vector2(-rightInset, -top);

            ConfigureText(text, fontSize, alignment);
        }

        private static void ConfigureBottomText(Text text, float bottom, float height, int fontSize, TextAnchor alignment)
        {
            if (text == null)
                return;

            var rectTransform = (RectTransform)text.transform;
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.offsetMin = new Vector2(HorizontalPadding, bottom);
            rectTransform.offsetMax = new Vector2(-HorizontalPadding, bottom + height);

            ConfigureText(text, fontSize, alignment);
        }

        private static void ConfigureText(Text text, int fontSize, TextAnchor alignment)
        {
            text.fontSize = fontSize;
            text.resizeTextForBestFit = false;
            text.resizeTextMinSize = fontSize;
            text.resizeTextMaxSize = fontSize;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.alignment = alignment;
            text.lineSpacing = 1.05f;
            text.raycastTarget = false;
        }

        private void EnsureOpaqueCanvasBackground()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
                return;

            var existing = canvas.transform.Find(OpaqueBackgroundName);
            if (existing != null)
            {
                existing.SetAsFirstSibling();
                return;
            }

            var backgroundObject = new GameObject(OpaqueBackgroundName, typeof(RectTransform), typeof(Image));
            backgroundObject.transform.SetParent(canvas.transform, false);
            backgroundObject.transform.SetAsFirstSibling();

            var rectTransform = backgroundObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            var image = backgroundObject.GetComponent<Image>();
            image.color = new Color(0.12f, 0.12f, 0.12f, 1f);
            image.raycastTarget = false;
        }
    }
}
