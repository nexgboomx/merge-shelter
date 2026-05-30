using System.Collections.Generic;
using System.Text;
using MergeShelter.Board;
using MergeShelter.Core;
using MergeShelter.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace MergeShelter.UI
{
    public sealed class PrototypeHudView : MonoBehaviour
    {
        public const string OpaqueBackgroundName = "PrototypeOpaqueBackground";
        public const string ShelterSectionLabelName = "ShelterSectionLabel";
        public const string BoardSectionLabelName = "BoardSectionLabel";
        public const string ActionsSectionLabelName = "ActionsSectionLabel";
        public const string RewardsSectionLabelName = "RewardsSectionLabel";
        public const string QuestsSectionLabelName = "QuestsSectionLabel";
        public const string ShelterUpgradeTextName = "ShelterUpgradeText";
        public const string RewardTextName = "RewardText";
        public const string QuestTextName = "QuestText";
        public const string ResultPanelName = "ResultStatusPanel";

        private const float HorizontalPadding = 24f;
        private const float TopPadding = 16f;
        private const float LevelHeight = 30f;
        private const float TutorialTop = 50f;
        private const float TutorialHeight = 36f;
        private const float ShelterLabelTop = 94f;
        private const float SectionLabelHeight = 14f;
        private const float StatusTop = 112f;
        private const float StatusHeight = 24f;
        private const float RewardsLabelTop = 144f;
        private const float WalletTop = 162f;
        private const float WalletHeight = 22f;
        private const float QuestsLabelTop = 216f;
        private const float QuestTop = 234f;
        private const float QuestHeight = 40f;
        private const float BoardLabelTop = 286f;
        private const float ResultBottom = 250f;
        private const float ResultHeight = 72f;
        private const float ActionsLabelBottom = 208f;
        private const float ResultPulseScale = 1.04f;
        private const float ResultEffectSeconds = 0.18f;

        [SerializeField] private Text levelText;
        [SerializeField] private Text tutorialText;
        [SerializeField] private Text shelterLabelText;
        [SerializeField] private Text shelterHpText;
        [SerializeField] private Text shelterUpgradeText;
        [SerializeField] private Text boardLabelText;
        [SerializeField] private Text nextTileText;
        [SerializeField] private Text actionsLabelText;
        [SerializeField] private Text rewardsLabelText;
        [SerializeField] private Text rewardText;
        [SerializeField] private Text questsLabelText;
        [SerializeField] private Text questText;
        [SerializeField] private Text resultText;
        [SerializeField] private Text walletText;
        [SerializeField] private Image resultPanelImage;

        private bool _isApplyingLayout;
        private float _resultEffectClearTime;
        public PrototypeFeedbackKind CurrentFeedbackKind { get; private set; } = PrototypeFeedbackKind.None;
        public string CurrentFeedbackMessage { get; private set; } = string.Empty;
        public PrototypeFeedbackKind LastResultEffectKind { get; private set; } = PrototypeFeedbackKind.None;
        public bool HasActiveResultEffect => Time.unscaledTime < _resultEffectClearTime;
        public float ResultEffectDurationSeconds => ResultEffectSeconds;

        private void Awake()
        {
            EnsureOpaqueCanvasBackground();
            EnsureSectionLabelsAndText();
            EnsureResultPanel();
            ApplyPhoneSafeLayout();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!_isApplyingLayout)
                ApplyPhoneSafeLayout();
        }

        private void Update()
        {
            if (_resultEffectClearTime > 0f && Time.unscaledTime >= _resultEffectClearTime)
                ClearResultPulse();
        }

        public void ApplyPhoneSafeLayout()
        {
            if (_isApplyingLayout)
                return;

            _isApplyingLayout = true;
            try
            {
                EnsureSectionLabelsAndText();
                EnsureResultPanel();
                ConfigureTopText(levelText, TopPadding, LevelHeight, 20, TextAnchor.UpperLeft);
                ConfigureTopText(tutorialText, TutorialTop, TutorialHeight, 14, TextAnchor.UpperLeft);
                ConfigureTopText(shelterLabelText, ShelterLabelTop, SectionLabelHeight, 11, TextAnchor.UpperLeft);
                ConfigureTopText(shelterHpText, StatusTop, StatusHeight, 14, TextAnchor.MiddleLeft, 0f, 0.5f, HorizontalPadding, 8f);
                ConfigureTopText(shelterUpgradeText, StatusTop, StatusHeight, 12, TextAnchor.MiddleRight, 0.5f, 1f, 8f, HorizontalPadding);
                ConfigureTopText(rewardsLabelText, RewardsLabelTop, SectionLabelHeight, 11, TextAnchor.UpperLeft);
                ConfigureTopText(walletText, WalletTop, WalletHeight, 12, TextAnchor.MiddleLeft, 0f, 0.5f, HorizontalPadding, 8f);
                ConfigureTopText(rewardText, WalletTop, WalletHeight, 12, TextAnchor.MiddleRight, 0.5f, 1f, 8f, HorizontalPadding);
                ConfigureTopText(questsLabelText, QuestsLabelTop, SectionLabelHeight, 11, TextAnchor.UpperLeft);
                ConfigureTopText(questText, QuestTop, QuestHeight, 12, TextAnchor.UpperLeft);
                ConfigureTopText(boardLabelText, BoardLabelTop, SectionLabelHeight, 11, TextAnchor.UpperLeft, 0f, 0.5f, HorizontalPadding, 8f);
                ConfigureTopText(nextTileText, BoardLabelTop, SectionLabelHeight, 12, TextAnchor.UpperRight, 0.5f, 1f, 8f, HorizontalPadding);
                ConfigureBottomText(resultText, ResultBottom, ResultHeight, 14, TextAnchor.UpperLeft);
                ConfigureBottomText(actionsLabelText, ActionsLabelBottom, SectionLabelHeight, 11, TextAnchor.UpperLeft);
                ConfigureResultPanel();
                ApplyHudVisualStyles();
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
            {
                shelterHpText.text = $"HP: {current}/{max}";
                shelterHpText.color = GetShelterHpColor(current, max);
            }
        }

        public void SetNextTile(TileData tile)
        {
            if (nextTileText != null)
                nextTileText.text = tile.IsEmpty ? "Next: None" : $"Next: {tile.Type} T{tile.Tier}";
        }

        public void SetResult(string message)
        {
            SetResultInternal(PrototypeFeedbackKind.None, message);
        }

        public void SetFeedback(PrototypeFeedbackKind kind, string message)
        {
            SetResultInternal(kind, message);
        }

        private void SetResultInternal(PrototypeFeedbackKind kind, string message)
        {
            CurrentFeedbackKind = kind;
            CurrentFeedbackMessage = message ?? string.Empty;
            EnsureResultPanel();

            if (resultText != null)
            {
                ConfigureBottomText(resultText, ResultBottom, ResultHeight, 14, TextAnchor.UpperLeft);
                resultText.text = FormatFeedbackMessage(kind, CurrentFeedbackMessage);
                resultText.color = PrototypeVisualKit.GetFeedbackColor(kind);
                resultText.transform.localScale = kind == PrototypeFeedbackKind.None
                    ? Vector3.one
                    : Vector3.one * ResultPulseScale;
            }

            if (kind == PrototypeFeedbackKind.None)
            {
                LastResultEffectKind = PrototypeFeedbackKind.None;
                _resultEffectClearTime = 0f;
            }
            else
            {
                LastResultEffectKind = kind;
                _resultEffectClearTime = Time.unscaledTime + ResultEffectSeconds;
            }

            ConfigureResultPanel();
            ApplyResultPanelColor();
        }

        public void SetWallet(int coins, int parts)
        {
            EnsureSectionLabelsAndText();
            if (walletText != null)
            {
                ConfigureTopText(walletText, WalletTop, WalletHeight, 12, TextAnchor.UpperLeft);
                walletText.text = $"Coins: {coins} | Parts: {parts}";
                walletText.color = PrototypeVisualKit.ResourceText;
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
            EnsureSectionLabelsAndText();
            if (walletText == null)
                return;

            ConfigureTopText(walletText, WalletTop, WalletHeight, 12, TextAnchor.MiddleLeft, 0f, 0.5f, HorizontalPadding, 8f);
            ConfigureTopText(shelterUpgradeText, StatusTop, StatusHeight, 12, TextAnchor.MiddleRight, 0.5f, 1f, 8f, HorizontalPadding);
            ConfigureTopText(rewardText, WalletTop, WalletHeight, 12, TextAnchor.MiddleRight, 0.5f, 1f, 8f, HorizontalPadding);
            ConfigureTopText(questText, QuestTop, QuestHeight, 12, TextAnchor.UpperLeft);
            var affordText = canAffordUpgrade ? "can afford" : $"need {upgradeCost - coins} more";
            var dailyRewardStatus = hasClaimedDailyReward ? "claimed" : canClaimDailyReward ? "available" : "unavailable";

            walletText.text = $"Coins: {coins} | Parts: {parts}";
            walletText.color = PrototypeVisualKit.ResourceText;

            if (shelterUpgradeText != null)
            {
                shelterUpgradeText.text = $"Lv {shelterUpgradeLevel} | Upgrade {upgradeCost} ({affordText})";
                shelterUpgradeText.color = canAffordUpgrade
                    ? PrototypeVisualKit.ShelterHealthy
                    : PrototypeVisualKit.SecondaryText;
            }

            if (rewardText != null)
            {
                rewardText.text = $"Daily: {dailyRewardStatus} (+{dailyRewardCoins}c, +{dailyRewardParts}p)";
                rewardText.color = canClaimDailyReward
                    ? PrototypeVisualKit.WaveReady
                    : PrototypeVisualKit.SecondaryText;
            }

            if (questText != null)
            {
                questText.text = FormatQuestStatus(dailyQuests);
                questText.color = PrototypeVisualKit.PrimaryText;
            }
        }

        private static string FormatQuestStatus(IReadOnlyList<DailyQuestState> dailyQuests)
        {
            if (dailyQuests == null || dailyQuests.Count == 0)
                return "No daily quests.";

            var builder = new StringBuilder();
            for (var i = 0; i < dailyQuests.Count; i++)
            {
                var quest = dailyQuests[i];
                if (i > 0)
                    builder.Append(" | ");

                builder.Append(GetShortQuestTitle(quest));
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

        private static string GetShortQuestTitle(DailyQuestState quest)
        {
            switch (quest.QuestId)
            {
                case DailyQuestModel.PlaceTilesQuestId:
                    return "Tiles";
                case DailyQuestModel.CompleteLevelQuestId:
                    return "Level";
                case DailyQuestModel.ClaimRewardQuestId:
                    return "Reward";
                default:
                    return string.IsNullOrWhiteSpace(quest.Title) ? "Quest" : quest.Title;
            }
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
            text.color = PrototypeVisualKit.PrimaryText;
        }

        private void ApplyHudVisualStyles()
        {
            ApplySectionLabelStyle(shelterLabelText);
            ApplySectionLabelStyle(boardLabelText);
            ApplySectionLabelStyle(actionsLabelText);
            ApplySectionLabelStyle(rewardsLabelText);
            ApplySectionLabelStyle(questsLabelText);

            ApplyTextColor(levelText, PrototypeVisualKit.PrimaryText);
            ApplyTextColor(tutorialText, PrototypeVisualKit.SecondaryText);
            ApplyTextColor(nextTileText, PrototypeVisualKit.SecondaryText);

            if (resultText != null)
                resultText.color = PrototypeVisualKit.GetFeedbackColor(CurrentFeedbackKind);

            ApplyResultPanelColor();
        }

        private static void ApplySectionLabelStyle(Text text)
        {
            if (text == null)
                return;

            text.color = PrototypeVisualKit.SectionText;
            text.fontStyle = FontStyle.Bold;
        }

        private static void ApplyTextColor(Text text, Color color)
        {
            if (text != null)
                text.color = color;
        }

        private void EnsureSectionLabelsAndText()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
                return;

            shelterLabelText = ResolveOrCreateText(canvas.transform, shelterLabelText, ShelterSectionLabelName, "SHELTER");
            boardLabelText = ResolveOrCreateText(canvas.transform, boardLabelText, BoardSectionLabelName, "BOARD");
            actionsLabelText = ResolveOrCreateText(canvas.transform, actionsLabelText, ActionsSectionLabelName, "ACTIONS");
            rewardsLabelText = ResolveOrCreateText(canvas.transform, rewardsLabelText, RewardsSectionLabelName, "REWARDS");
            questsLabelText = ResolveOrCreateText(canvas.transform, questsLabelText, QuestsSectionLabelName, "QUESTS");
            shelterUpgradeText = ResolveOrCreateText(canvas.transform, shelterUpgradeText, ShelterUpgradeTextName, "Lv 1 | Upgrade 100");
            rewardText = ResolveOrCreateText(canvas.transform, rewardText, RewardTextName, "Daily: available");
            questText = ResolveOrCreateText(canvas.transform, questText, QuestTextName, "Tiles 0/10 | Level 0/1 | Reward 0/1");
        }

        private void EnsureResultPanel()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
                return;

            if (resultPanelImage != null)
            {
                resultPanelImage.raycastTarget = false;
                ApplyResultPanelColor();
                return;
            }

            var existing = canvas.transform.Find(ResultPanelName);
            if (existing != null && existing.TryGetComponent<Image>(out var existingImage))
            {
                resultPanelImage = existingImage;
                resultPanelImage.raycastTarget = false;
                return;
            }

            var panelObject = new GameObject(ResultPanelName, typeof(RectTransform), typeof(Image));
            panelObject.transform.SetParent(canvas.transform, false);
            resultPanelImage = panelObject.GetComponent<Image>();
            resultPanelImage.raycastTarget = false;
            ApplyResultPanelColor();
        }

        private void ConfigureResultPanel()
        {
            if (resultPanelImage == null)
                return;

            var rectTransform = (RectTransform)resultPanelImage.transform;
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.offsetMin = new Vector2(-32f, ResultBottom - 8f);
            rectTransform.offsetMax = new Vector2(32f, ResultBottom + ResultHeight + 8f);

            if (resultText != null)
            {
                var panelIndex = resultPanelImage.transform.GetSiblingIndex();
                var resultIndex = resultText.transform.GetSiblingIndex();
                if (panelIndex == 0)
                    resultPanelImage.transform.SetSiblingIndex(1);
                else if (panelIndex > resultIndex)
                    resultPanelImage.transform.SetSiblingIndex(Mathf.Max(1, resultIndex));
            }
        }

        private void ApplyResultPanelColor()
        {
            if (resultPanelImage != null)
            {
                resultPanelImage.color = HasActiveResultEffect
                    ? PrototypeVisualKit.GetResultPanelFlashColor(CurrentFeedbackKind)
                    : PrototypeVisualKit.GetResultPanelColor(CurrentFeedbackKind);
            }
        }

        private static Text ResolveOrCreateText(Transform parent, Text current, string name, string fallbackText)
        {
            if (current != null)
                return current;

            var existing = parent.Find(name);
            if (existing != null && existing.TryGetComponent<Text>(out var existingText))
                return existingText;

            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            var text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.color = PrototypeVisualKit.PrimaryText;
            text.text = fallbackText;
            ConfigureText(text, 12, TextAnchor.UpperLeft);
            return text;
        }

        private void EnsureOpaqueCanvasBackground()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
                return;

            var existing = canvas.transform.Find(OpaqueBackgroundName);
            if (existing != null)
            {
                if (existing.TryGetComponent<Image>(out var existingImage))
                {
                    existingImage.color = PrototypeVisualKit.CanvasBackground;
                    existingImage.raycastTarget = false;
                }

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
            image.color = PrototypeVisualKit.CanvasBackground;
            image.raycastTarget = false;
        }

        private void ClearResultPulse()
        {
            if (resultText != null)
                resultText.transform.localScale = Vector3.one;

            _resultEffectClearTime = 0f;
            ApplyResultPanelColor();
        }

        private static string FormatFeedbackMessage(PrototypeFeedbackKind kind, string message)
        {
            var prefix = GetFeedbackPrefix(kind);
            if (string.IsNullOrEmpty(prefix))
                return message;

            return $"{prefix} {message}";
        }

        private static string GetFeedbackPrefix(PrototypeFeedbackKind kind)
        {
            switch (kind)
            {
                case PrototypeFeedbackKind.TilePlaced:
                    return "TILE:";
                case PrototypeFeedbackKind.MergeSuccess:
                    return "MERGE:";
                case PrototypeFeedbackKind.InvalidPlacement:
                case PrototypeFeedbackKind.Blocked:
                    return "BLOCKED:";
                case PrototypeFeedbackKind.WaveStart:
                    return "WAVE:";
                case PrototypeFeedbackKind.WaveVictory:
                    return "WIN:";
                case PrototypeFeedbackKind.WaveDefeat:
                    return "DEFEAT:";
                case PrototypeFeedbackKind.RewardClaim:
                    return "REWARD:";
                case PrototypeFeedbackKind.DailyRewardClaim:
                    return "DAILY:";
                case PrototypeFeedbackKind.QuestClaim:
                    return "QUEST:";
                case PrototypeFeedbackKind.ShelterUpgrade:
                    return "UPGRADE:";
                case PrototypeFeedbackKind.RewardDouble:
                    return "DOUBLE:";
                case PrototypeFeedbackKind.Revive:
                    return "REVIVE:";
                case PrototypeFeedbackKind.ResetSave:
                    return "RESET:";
                case PrototypeFeedbackKind.NextLevel:
                    return "NEXT:";
                case PrototypeFeedbackKind.Retry:
                    return "RETRY:";
                default:
                    return string.Empty;
            }
        }

        private static Color GetShelterHpColor(int current, int max)
        {
            if (max <= 0 || current <= 0)
                return PrototypeVisualKit.ShelterDefeated;

            return current < max / 2
                ? PrototypeVisualKit.ShelterWarning
                : PrototypeVisualKit.ShelterHealthy;
        }
    }
}
