using System.Collections.Generic;
using MergeShelter.Board;
using MergeShelter.Core;
using UnityEngine;
using UnityEngine.UI;

namespace MergeShelter.UI
{
    /// <summary>
    /// Runtime-generated Sprint 1 board UI. This intentionally uses simple Unity UI
    /// controls so the prototype scene can be wired without final art assets.
    /// </summary>
    public sealed class PrototypeBoardView : MonoBehaviour
    {
        public const string ActionPanelName = "ActionButtonPanel";

        private const float ReferenceWidth = 720f;
        private const float ReferenceHeight = 1280f;
        private const float BoardVerticalOffset = 64f;
        private const float ButtonWidth = 200f;
        private const float ButtonHeight = 44f;
        private const float PrimaryButtonWidth = 220f;
        private const float PrimaryButtonHeight = 52f;
        private const float ButtonColumnSpacing = 24f;
        private const float ButtonBottomRow = 32f;
        private const float ButtonRowSpacing = 56f;
        private const float CellFeedbackDuration = 0.28f;
        private const float ButtonFeedbackDuration = 0.1f;
        private const float ButtonFeedbackScale = 1.04f;

        [SerializeField] private PrototypeGameController gameController;
        [SerializeField] private RectTransform boardRoot;
        [SerializeField] private Button startWaveButton;
        [SerializeField] private Button claimRewardButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button upgradeShelterButton;
        [SerializeField] private Button dailyRewardButton;
        [SerializeField] private Button claimQuestButton;
        [SerializeField] private Button doubleRewardButton;
        [SerializeField] private Button reviveButton;
        [SerializeField] private Button previousLevelButton;
        [SerializeField] private Button replayLevelButton;
        [SerializeField] private Button nextUnlockedLevelButton;
        [SerializeField] private Button resetSaveButton;
        [SerializeField] private Image actionPanelImage;
        [SerializeField] private int cellSize = 72;
        [SerializeField] private int cellSpacing = 6;

        private readonly List<CellView> _cells = new();
        private PrototypeGameController _subscribedController;
        private Font _defaultFont;
        private bool _isApplyingLayout;
        private int _feedbackCellX = -1;
        private int _feedbackCellY = -1;
        private float _feedbackCellClearTime;
        private Color _feedbackCellColor;
        private RectTransform _buttonPulseTarget;
        private float _buttonPulseClearTime;

        public PrototypeFeedbackKind LastCellFeedbackKind { get; private set; } = PrototypeFeedbackKind.None;
        public bool HasActiveCellFeedback => _feedbackCellX >= 0 && Time.unscaledTime < _feedbackCellClearTime;
        public float CellEffectDurationSeconds => CellFeedbackDuration;

        private void Awake()
        {
            ResolveController();
            ConfigureCanvasScaler();
            ConfigureViewRoot();
            EnsureActionPanel();
            BuildBoard();
            BuildStartWaveButton();
            BuildProgressionButtons();
            ApplyPhoneSafeLayout();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!_isApplyingLayout)
                ApplyPhoneSafeLayout();
        }

        private void Update()
        {
            if (_feedbackCellX >= 0 && Time.unscaledTime >= _feedbackCellClearTime)
            {
                _feedbackCellX = -1;
                _feedbackCellY = -1;
                ResetCellScales();
                RefreshCells();
            }

            if (_buttonPulseTarget != null && Time.unscaledTime >= _buttonPulseClearTime)
            {
                _buttonPulseTarget.localScale = Vector3.one;
                _buttonPulseTarget = null;
            }
        }

        private void OnEnable()
        {
            SubscribeToController();
        }

        private void Start()
        {
            SubscribeToController();
            RefreshCells();
        }

        private void OnDisable()
        {
            if (_subscribedController != null)
            {
                _subscribedController.BoardChanged -= RefreshCells;
                _subscribedController.ProgressionChanged -= RefreshActionButtons;
            }

            _subscribedController = null;
        }

        private void ResolveController()
        {
            if (gameController == null)
                gameController = FindObjectOfType<PrototypeGameController>();
        }

        private void SubscribeToController()
        {
            ResolveController();

            if (_subscribedController == gameController)
                return;

            if (_subscribedController != null)
            {
                _subscribedController.BoardChanged -= RefreshCells;
                _subscribedController.ProgressionChanged -= RefreshActionButtons;
            }

            _subscribedController = gameController;

            if (_subscribedController != null)
            {
                _subscribedController.BoardChanged += RefreshCells;
                _subscribedController.ProgressionChanged += RefreshActionButtons;
            }
        }

        private void BuildBoard()
        {
            if (_cells.Count > 0)
                return;

            var width = gameController != null ? gameController.BoardWidth : BoardModel.DefaultWidth;
            var height = gameController != null ? gameController.BoardHeight : BoardModel.DefaultHeight;
            var root = ResolveBoardRoot(width, height);
            var grid = root.GetComponent<GridLayoutGroup>() ?? root.gameObject.AddComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = width;
            grid.cellSize = new Vector2(cellSize, cellSize);
            grid.spacing = new Vector2(cellSpacing, cellSpacing);
            grid.childAlignment = TextAnchor.MiddleCenter;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var cell = CreateCell(root, x, y);
                    _cells.Add(cell);
                }
            }
        }

        private RectTransform ResolveBoardRoot(int width, int height)
        {
            if (boardRoot == null || boardRoot.transform == transform)
            {
                var gridObject = new GameObject("BoardGrid", typeof(RectTransform));
                gridObject.transform.SetParent(transform, false);
                boardRoot = gridObject.GetComponent<RectTransform>();
                boardRoot.anchorMin = new Vector2(0.5f, 0.5f);
                boardRoot.anchorMax = new Vector2(0.5f, 0.5f);
                boardRoot.pivot = new Vector2(0.5f, 0.5f);
                boardRoot.anchoredPosition = Vector2.zero;
            }

            var totalWidth = width * cellSize + (width - 1) * cellSpacing;
            var totalHeight = height * cellSize + (height - 1) * cellSpacing;
            boardRoot.anchorMin = new Vector2(0.5f, 0.5f);
            boardRoot.anchorMax = new Vector2(0.5f, 0.5f);
            boardRoot.pivot = new Vector2(0.5f, 0.5f);
            boardRoot.anchoredPosition = new Vector2(0f, BoardVerticalOffset);
            boardRoot.sizeDelta = new Vector2(totalWidth, totalHeight);
            ApplyBoardPanelStyle();
            return boardRoot;
        }

        private void ApplyBoardPanelStyle()
        {
            if (boardRoot == null)
                return;

            var image = boardRoot.GetComponent<Image>() ?? boardRoot.gameObject.AddComponent<Image>();
            image.color = PrototypeVisualKit.BoardPanelBackground;
            image.raycastTarget = false;

            var outline = boardRoot.GetComponent<Outline>() ?? boardRoot.gameObject.AddComponent<Outline>();
            outline.effectColor = PrototypeVisualKit.PanelBorder;
            outline.effectDistance = new Vector2(3f, -3f);
            outline.useGraphicAlpha = false;
        }

        private CellView CreateCell(RectTransform parent, int x, int y)
        {
            var cellObject = new GameObject($"Cell_{x}_{y}", typeof(RectTransform));
            cellObject.transform.SetParent(parent, false);

            var background = cellObject.AddComponent<Image>();
            background.color = PrototypeVisualKit.EmptyCell;

            var outline = cellObject.AddComponent<Outline>();
            outline.effectColor = PrototypeVisualKit.CellBorder;
            outline.effectDistance = new Vector2(2f, -2f);
            outline.useGraphicAlpha = false;

            var button = cellObject.AddComponent<Button>();
            button.targetGraphic = background;

            var label = CreateCellLabel(cellObject.transform);
            var cellX = x;
            var cellY = y;
            button.onClick.AddListener(() => OnCellClicked(cellX, cellY));

            return new CellView
            {
                X = x,
                Y = y,
                Transform = (RectTransform)cellObject.transform,
                Background = background,
                Label = label
            };
        }

        private Text CreateCellLabel(Transform parent)
        {
            var labelObject = new GameObject("Label", typeof(RectTransform));
            labelObject.transform.SetParent(parent, false);

            var rectTransform = labelObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            var label = labelObject.AddComponent<Text>();
            label.alignment = TextAnchor.MiddleCenter;
            label.font = GetDefaultFont();
            label.fontSize = 18;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 10;
            label.resizeTextMaxSize = 18;
            label.fontStyle = FontStyle.Bold;
            label.color = PrototypeVisualKit.PrimaryText;
            label.raycastTarget = false;
            return label;
        }

        private void BuildStartWaveButton()
        {
            if (startWaveButton == null)
                startWaveButton = CreateStartWaveButton();

            startWaveButton.onClick.RemoveListener(OnStartWaveClicked);
            startWaveButton.onClick.AddListener(OnStartWaveClicked);
        }

        private void BuildProgressionButtons()
        {
            if (claimRewardButton == null)
                claimRewardButton = CreateActionButton("ClaimRewardButton", "Claim Reward", new Vector2(0f, -280f), PrototypeVisualKit.GetActionButtonColor("ClaimRewardButton", false));

            if (nextLevelButton == null)
                nextLevelButton = CreateActionButton("NextLevelButton", "Next Level", new Vector2(0f, -330f), PrototypeVisualKit.GetActionButtonColor("NextLevelButton", false));

            if (retryButton == null)
                retryButton = CreateActionButton("RetryButton", "Retry", new Vector2(0f, -280f), PrototypeVisualKit.GetActionButtonColor("RetryButton", false));

            if (upgradeShelterButton == null)
                upgradeShelterButton = CreateActionButton("UpgradeShelterButton", "Upgrade Shelter", new Vector2(210f, -280f), PrototypeVisualKit.GetActionButtonColor("UpgradeShelterButton", false));

            if (dailyRewardButton == null)
                dailyRewardButton = CreateActionButton("DailyRewardButton", "Daily Reward", new Vector2(-210f, -280f), PrototypeVisualKit.GetActionButtonColor("DailyRewardButton", false));

            if (claimQuestButton == null)
                claimQuestButton = CreateActionButton("ClaimQuestButton", "Claim Quest", new Vector2(210f, -330f), PrototypeVisualKit.GetActionButtonColor("ClaimQuestButton", false));

            if (doubleRewardButton == null)
                doubleRewardButton = CreateActionButton("DoubleRewardButton", "Double Reward", new Vector2(-210f, -330f), PrototypeVisualKit.GetActionButtonColor("DoubleRewardButton", false));

            if (reviveButton == null)
                reviveButton = CreateActionButton("ReviveButton", "Revive", new Vector2(-210f, -330f), PrototypeVisualKit.GetActionButtonColor("ReviveButton", false));

            if (previousLevelButton == null)
                previousLevelButton = CreateActionButton("PreviousLevelButton", "Prev Level", new Vector2(-210f, -380f), PrototypeVisualKit.GetActionButtonColor("PreviousLevelButton", false));

            if (replayLevelButton == null)
                replayLevelButton = CreateActionButton("ReplayLevelButton", "Replay", new Vector2(0f, -380f), PrototypeVisualKit.GetActionButtonColor("ReplayLevelButton", false));

            if (nextUnlockedLevelButton == null)
                nextUnlockedLevelButton = CreateActionButton("NextUnlockedLevelButton", "Next Unlocked", new Vector2(210f, -380f), PrototypeVisualKit.GetActionButtonColor("NextUnlockedLevelButton", false));

            if (resetSaveButton == null)
                resetSaveButton = CreateActionButton("ResetSaveButton", "Reset Save", new Vector2(0f, -380f), PrototypeVisualKit.GetActionButtonColor("ResetSaveButton", false));

            claimRewardButton.onClick.RemoveListener(OnClaimRewardClicked);
            claimRewardButton.onClick.AddListener(OnClaimRewardClicked);
            nextLevelButton.onClick.RemoveListener(OnNextLevelClicked);
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            retryButton.onClick.RemoveListener(OnRetryClicked);
            retryButton.onClick.AddListener(OnRetryClicked);
            upgradeShelterButton.onClick.RemoveListener(OnUpgradeShelterClicked);
            upgradeShelterButton.onClick.AddListener(OnUpgradeShelterClicked);
            dailyRewardButton.onClick.RemoveListener(OnDailyRewardClicked);
            dailyRewardButton.onClick.AddListener(OnDailyRewardClicked);
            claimQuestButton.onClick.RemoveListener(OnClaimQuestClicked);
            claimQuestButton.onClick.AddListener(OnClaimQuestClicked);
            doubleRewardButton.onClick.RemoveListener(OnDoubleRewardClicked);
            doubleRewardButton.onClick.AddListener(OnDoubleRewardClicked);
            reviveButton.onClick.RemoveListener(OnReviveClicked);
            reviveButton.onClick.AddListener(OnReviveClicked);
            previousLevelButton.onClick.RemoveListener(OnPreviousLevelClicked);
            previousLevelButton.onClick.AddListener(OnPreviousLevelClicked);
            replayLevelButton.onClick.RemoveListener(OnReplayLevelClicked);
            replayLevelButton.onClick.AddListener(OnReplayLevelClicked);
            nextUnlockedLevelButton.onClick.RemoveListener(OnNextUnlockedLevelClicked);
            nextUnlockedLevelButton.onClick.AddListener(OnNextUnlockedLevelClicked);
            resetSaveButton.onClick.RemoveListener(OnResetSaveClicked);
            resetSaveButton.onClick.AddListener(OnResetSaveClicked);
            RefreshActionButtons();
        }

        private Button CreateStartWaveButton()
        {
            var boardHeight = BoardModel.DefaultHeight * cellSize + (BoardModel.DefaultHeight - 1) * cellSpacing;
            return CreateActionButton(
                "StartWaveButton",
                "Start Wave",
                new Vector2(0f, -(boardHeight * 0.5f + 40f)),
                PrototypeVisualKit.GetActionButtonColor("StartWaveButton", false));
        }

        private Button CreateActionButton(string name, string labelText, Vector2 anchoredPosition, Color backgroundColor)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform));
            buttonObject.transform.SetParent(transform, false);

            var rectTransform = buttonObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(ButtonWidth, ButtonHeight);
            rectTransform.anchoredPosition = anchoredPosition;

            var background = buttonObject.AddComponent<Image>();
            background.color = backgroundColor;

            var outline = buttonObject.AddComponent<Outline>();
            outline.effectColor = PrototypeVisualKit.ButtonBorder;
            outline.effectDistance = new Vector2(2f, -2f);
            outline.useGraphicAlpha = false;

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = background;

            var label = CreateCellLabel(buttonObject.transform);
            label.text = labelText;
            label.fontSize = 20;
            label.resizeTextMaxSize = 20;
            label.color = PrototypeVisualKit.ButtonText;

            return button;
        }

        private void OnCellClicked(int x, int y)
        {
            if (gameController == null)
                return;

            var placed = gameController.TryPlaceNextTile(x, y);
            var feedbackKind = placed
                ? gameController.LastFeedbackKind
                : gameController.LastFeedbackKind == PrototypeFeedbackKind.Blocked
                    ? PrototypeFeedbackKind.Blocked
                    : PrototypeFeedbackKind.InvalidPlacement;
            ShowCellFeedback(x, y, feedbackKind);
            RefreshCells();
        }

        private void OnStartWaveClicked()
        {
            PulseButton(startWaveButton);
            gameController?.StartWave();
            RefreshActionButtons();
        }

        private void OnClaimRewardClicked()
        {
            PulseButton(claimRewardButton);
            gameController?.ClaimReward();
            RefreshActionButtons();
        }

        private void OnNextLevelClicked()
        {
            PulseButton(nextLevelButton);
            gameController?.StartNextLevel();
            RefreshCells();
            RefreshActionButtons();
        }

        private void OnRetryClicked()
        {
            PulseButton(retryButton);
            gameController?.RetryLevel();
            RefreshCells();
            RefreshActionButtons();
        }

        private void OnUpgradeShelterClicked()
        {
            PulseButton(upgradeShelterButton);
            gameController?.UpgradeShelter();
            RefreshActionButtons();
        }

        private void OnDailyRewardClicked()
        {
            PulseButton(dailyRewardButton);
            gameController?.ClaimDailyReward();
            RefreshActionButtons();
        }

        private void OnClaimQuestClicked()
        {
            PulseButton(claimQuestButton);
            gameController?.ClaimQuest();
            RefreshActionButtons();
        }

        private void OnDoubleRewardClicked()
        {
            PulseButton(doubleRewardButton);
            gameController?.DoubleReward();
            RefreshActionButtons();
        }

        private void OnReviveClicked()
        {
            if (gameController == null)
                return;

            if (!gameController.CanRevive)
            {
                RefreshActionButtons();
                return;
            }

            PulseButton(reviveButton);
            if (reviveButton != null)
            {
                reviveButton.interactable = false;
                reviveButton.gameObject.SetActive(false);
            }

            if (gameController.Revive())
                RefreshCells();

            RefreshActionButtons();
        }

        private void OnPreviousLevelClicked()
        {
            PulseButton(previousLevelButton);
            if (gameController != null && gameController.SelectPreviousUnlockedLevel())
                RefreshCells();

            RefreshActionButtons();
        }

        private void OnReplayLevelClicked()
        {
            PulseButton(replayLevelButton);
            if (gameController != null && gameController.ReplaySelectedLevel())
                RefreshCells();

            RefreshActionButtons();
        }

        private void OnNextUnlockedLevelClicked()
        {
            PulseButton(nextUnlockedLevelButton);
            if (gameController != null && gameController.SelectNextUnlockedLevel())
                RefreshCells();

            RefreshActionButtons();
        }

        private void OnResetSaveClicked()
        {
            PulseButton(resetSaveButton);
            gameController?.ResetSave();
            RefreshCells();
            RefreshActionButtons();
        }

        private void RefreshCells()
        {
            if (gameController == null)
                return;

            foreach (var cell in _cells)
            {
                var tile = gameController.GetTileAt(cell.X, cell.Y);
                cell.Label.text = FormatTileLabel(tile);
                cell.Label.color = PrototypeVisualKit.GetTileTextColor(tile);
                cell.Background.color = GetCellDisplayColor(cell, tile);
            }
        }

        private void ShowCellFeedback(int x, int y, PrototypeFeedbackKind kind)
        {
            _feedbackCellX = x;
            _feedbackCellY = y;
            _feedbackCellClearTime = Time.unscaledTime + CellFeedbackDuration;
            _feedbackCellColor = GetFeedbackCellColor(kind);
            LastCellFeedbackKind = kind;
            ApplyCellEffectScale(x, y, kind);
        }

        private Color GetCellDisplayColor(CellView cell, TileData tile)
        {
            var baseColor = GetTileColor(tile);
            if (_feedbackCellX == cell.X && _feedbackCellY == cell.Y && Time.unscaledTime < _feedbackCellClearTime)
                return Color.Lerp(baseColor, _feedbackCellColor, 0.68f);

            return baseColor;
        }

        private void PulseButton(Button button)
        {
            if (button == null || !button.gameObject.activeInHierarchy || !button.interactable)
                return;

            if (_buttonPulseTarget != null)
                _buttonPulseTarget.localScale = Vector3.one;

            _buttonPulseTarget = (RectTransform)button.transform;
            _buttonPulseTarget.localScale = Vector3.one * ButtonFeedbackScale;
            _buttonPulseClearTime = Time.unscaledTime + ButtonFeedbackDuration;
        }

        private void RefreshActionButtons()
        {
            if (gameController == null)
                return;

            if (startWaveButton != null)
                startWaveButton.gameObject.SetActive(!gameController.IsLevelEnded);

            if (claimRewardButton != null)
                claimRewardButton.gameObject.SetActive(gameController.CanClaimReward);

            if (nextLevelButton != null)
                nextLevelButton.gameObject.SetActive(gameController.CanStartNextLevel);

            if (retryButton != null)
                retryButton.gameObject.SetActive(gameController.CanRetryLevel);

            if (upgradeShelterButton != null)
                upgradeShelterButton.gameObject.SetActive(true);

            if (dailyRewardButton != null)
                dailyRewardButton.gameObject.SetActive(gameController.CanClaimDailyReward);

            if (claimQuestButton != null)
                claimQuestButton.gameObject.SetActive(gameController.CanClaimQuest);

            if (doubleRewardButton != null)
                doubleRewardButton.gameObject.SetActive(gameController.CanDoubleReward);

            if (reviveButton != null)
            {
                var canRevive = gameController.CanRevive;
                reviveButton.gameObject.SetActive(canRevive);
                reviveButton.interactable = canRevive;
            }

            if (previousLevelButton != null)
                previousLevelButton.gameObject.SetActive(gameController.CanSelectPreviousUnlockedLevel);

            if (replayLevelButton != null)
                replayLevelButton.gameObject.SetActive(gameController.CanReplaySelectedLevel);

            if (nextUnlockedLevelButton != null)
                nextUnlockedLevelButton.gameObject.SetActive(gameController.CanSelectNextUnlockedLevel);

            if (resetSaveButton != null)
                resetSaveButton.gameObject.SetActive(true);

            ApplyPhoneSafeLayout();
        }

        private void ApplyPhoneSafeLayout()
        {
            if (_isApplyingLayout)
                return;

            _isApplyingLayout = true;
            try
            {
                ConfigureCanvasScaler();
                ConfigureViewRoot();
                EnsureActionPanel();

                if (boardRoot != null)
                {
                    boardRoot.anchorMin = new Vector2(0.5f, 0.5f);
                    boardRoot.anchorMax = new Vector2(0.5f, 0.5f);
                    boardRoot.pivot = new Vector2(0.5f, 0.5f);
                    boardRoot.anchoredPosition = new Vector2(0f, BoardVerticalOffset);
                }

                PlaceActionButton(dailyRewardButton, -1, 0);
                PlaceActionButton(upgradeShelterButton, 0, 0);
                PlaceActionButton(resetSaveButton, 1, 0);
                PlaceActionButton(claimQuestButton, -1, 1);
                PlaceActionButton(startWaveButton, 0, 1);
                PlaceActionButton(doubleRewardButton, -1, 2);
                PlaceActionButton(reviveButton, -1, 2);
                PlaceActionButton(claimRewardButton, 0, 2);
                PlaceActionButton(nextLevelButton, 0, 2);
                PlaceActionButton(retryButton, 0, 2);
                PlaceActionButton(previousLevelButton, -1, 2);
                PlaceActionButton(replayLevelButton, 0, 2);
                PlaceActionButton(nextUnlockedLevelButton, 1, 2);
                ConfigureActionPanel();
            }
            finally
            {
                _isApplyingLayout = false;
            }
        }

        private void ConfigureCanvasScaler()
        {
            var canvas = GetComponentInParent<Canvas>();
            var scaler = canvas != null ? canvas.GetComponent<CanvasScaler>() : null;
            if (scaler == null)
                return;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = Screen.width > Screen.height ? 1f : 0f;
        }

        private void ConfigureViewRoot()
        {
            var rectTransform = transform as RectTransform;
            if (rectTransform == null)
                return;

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
        }

        private void PlaceActionButton(Button button, int column, int row)
        {
            if (button == null)
                return;

            var isPrimary = IsPrimaryAction(button);
            var width = isPrimary ? PrimaryButtonWidth : ButtonWidth;
            var height = isPrimary ? PrimaryButtonHeight : ButtonHeight;
            var rectTransform = (RectTransform)button.transform;
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.anchoredPosition = new Vector2(
                column * (ButtonWidth + ButtonColumnSpacing),
                ButtonBottomRow + row * ButtonRowSpacing);

            if (button.targetGraphic is Image image)
                image.color = PrototypeVisualKit.GetActionButtonColor(button.name, isPrimary);

            var outline = button.GetComponent<Outline>() ?? button.gameObject.AddComponent<Outline>();
            outline.effectColor = isPrimary
                ? PrototypeVisualKit.PrimaryText
                : PrototypeVisualKit.ButtonBorder;
            outline.effectDistance = isPrimary
                ? new Vector2(3f, -3f)
                : new Vector2(2f, -2f);
            outline.useGraphicAlpha = false;

            var label = button.GetComponentInChildren<Text>(true);
            if (label != null)
            {
                label.fontSize = isPrimary ? 22 : 20;
                label.resizeTextMaxSize = label.fontSize;
                label.color = PrototypeVisualKit.ButtonText;
                label.fontStyle = FontStyle.Bold;
            }
        }

        private void EnsureActionPanel()
        {
            if (actionPanelImage != null)
            {
                actionPanelImage.color = PrototypeVisualKit.ActionPanelBackground;
                actionPanelImage.raycastTarget = false;
                actionPanelImage.transform.SetAsFirstSibling();
                return;
            }

            var existing = transform.Find(ActionPanelName);
            if (existing != null && existing.TryGetComponent<Image>(out var existingImage))
            {
                actionPanelImage = existingImage;
                actionPanelImage.raycastTarget = false;
                actionPanelImage.transform.SetAsFirstSibling();
                return;
            }

            var panelObject = new GameObject(ActionPanelName, typeof(RectTransform), typeof(Image));
            panelObject.transform.SetParent(transform, false);
            panelObject.transform.SetAsFirstSibling();
            actionPanelImage = panelObject.GetComponent<Image>();
            actionPanelImage.color = PrototypeVisualKit.ActionPanelBackground;
            actionPanelImage.raycastTarget = false;
        }

        private void ConfigureActionPanel()
        {
            if (actionPanelImage == null)
                return;

            var rectTransform = (RectTransform)actionPanelImage.transform;
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(0f, ButtonBottomRow + ButtonRowSpacing);
            rectTransform.sizeDelta = new Vector2(
                ButtonWidth * 3f + ButtonColumnSpacing * 2f + 32f,
                ButtonRowSpacing * 2f + PrimaryButtonHeight + 28f);
            actionPanelImage.color = PrototypeVisualKit.ActionPanelBackground;
            actionPanelImage.raycastTarget = false;
            actionPanelImage.transform.SetAsFirstSibling();
        }

        private bool IsPrimaryAction(Button button)
        {
            if (gameController == null || button == null)
                return false;

            if (button == startWaveButton)
                return !gameController.IsLevelEnded;

            if (button == claimRewardButton)
                return gameController.CanClaimReward;

            if (button == nextLevelButton)
                return gameController.CanStartNextLevel;

            if (button == retryButton)
                return gameController.CanRetryLevel;

            if (button == reviveButton)
                return gameController.CanRevive;

            return false;
        }

        private Font GetDefaultFont()
        {
            if (_defaultFont == null)
                _defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return _defaultFont;
        }

        private static string FormatTileLabel(TileData tile)
        {
            return PrototypeVisualKit.FormatTileLabel(tile);
        }

        private static Color GetTileColor(TileData tile)
        {
            return PrototypeVisualKit.GetTileFillColor(tile);
        }

        private static Color GetFeedbackCellColor(PrototypeFeedbackKind kind)
        {
            return PrototypeVisualKit.GetCellFeedbackColor(kind);
        }

        private void ApplyCellEffectScale(int x, int y, PrototypeFeedbackKind kind)
        {
            ResetCellScales();

            foreach (var cell in _cells)
            {
                if (cell.X != x || cell.Y != y)
                    continue;

                cell.Transform.localScale = Vector3.one * PrototypeVisualKit.GetCellFeedbackScale(kind);
                return;
            }
        }

        private void ResetCellScales()
        {
            foreach (var cell in _cells)
                cell.Transform.localScale = Vector3.one;
        }

        private sealed class CellView
        {
            public int X;
            public int Y;
            public RectTransform Transform;
            public Image Background;
            public Text Label;
        }
    }
}
