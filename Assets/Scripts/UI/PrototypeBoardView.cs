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
        [SerializeField] private Button resetSaveButton;
        [SerializeField] private int cellSize = 72;
        [SerializeField] private int cellSpacing = 6;

        private readonly List<CellView> _cells = new();
        private PrototypeGameController _subscribedController;
        private Font _defaultFont;

        private void Awake()
        {
            ResolveController();
            BuildBoard();
            BuildStartWaveButton();
            BuildProgressionButtons();
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
            boardRoot.sizeDelta = new Vector2(totalWidth, totalHeight);
            return boardRoot;
        }

        private CellView CreateCell(RectTransform parent, int x, int y)
        {
            var cellObject = new GameObject($"Cell_{x}_{y}", typeof(RectTransform));
            cellObject.transform.SetParent(parent, false);

            var background = cellObject.AddComponent<Image>();
            background.color = new Color(0.18f, 0.2f, 0.22f);

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
            label.color = Color.white;
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
                claimRewardButton = CreateActionButton("ClaimRewardButton", "Claim Reward", new Vector2(0f, -280f), new Color(0.2f, 0.45f, 0.72f));

            if (nextLevelButton == null)
                nextLevelButton = CreateActionButton("NextLevelButton", "Next Level", new Vector2(0f, -330f), new Color(0.2f, 0.48f, 0.3f));

            if (retryButton == null)
                retryButton = CreateActionButton("RetryButton", "Retry", new Vector2(0f, -280f), new Color(0.52f, 0.28f, 0.18f));

            if (upgradeShelterButton == null)
                upgradeShelterButton = CreateActionButton("UpgradeShelterButton", "Upgrade Shelter", new Vector2(210f, -280f), new Color(0.45f, 0.35f, 0.14f));

            if (dailyRewardButton == null)
                dailyRewardButton = CreateActionButton("DailyRewardButton", "Daily Reward", new Vector2(-210f, -280f), new Color(0.3f, 0.33f, 0.65f));

            if (claimQuestButton == null)
                claimQuestButton = CreateActionButton("ClaimQuestButton", "Claim Quest", new Vector2(210f, -330f), new Color(0.33f, 0.42f, 0.2f));

            if (doubleRewardButton == null)
                doubleRewardButton = CreateActionButton("DoubleRewardButton", "Double Reward", new Vector2(-210f, -330f), new Color(0.42f, 0.24f, 0.55f));

            if (reviveButton == null)
                reviveButton = CreateActionButton("ReviveButton", "Revive", new Vector2(-210f, -330f), new Color(0.48f, 0.22f, 0.28f));

            if (resetSaveButton == null)
                resetSaveButton = CreateActionButton("ResetSaveButton", "Reset Save", new Vector2(0f, -380f), new Color(0.24f, 0.24f, 0.24f));

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
                new Color(0.13f, 0.42f, 0.32f));
        }

        private Button CreateActionButton(string name, string labelText, Vector2 anchoredPosition, Color backgroundColor)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform));
            buttonObject.transform.SetParent(transform, false);

            var rectTransform = buttonObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(180f, 44f);
            rectTransform.anchoredPosition = anchoredPosition;

            var background = buttonObject.AddComponent<Image>();
            background.color = backgroundColor;

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = background;

            var label = CreateCellLabel(buttonObject.transform);
            label.text = labelText;
            label.fontSize = 20;
            label.resizeTextMaxSize = 20;

            return button;
        }

        private void OnCellClicked(int x, int y)
        {
            if (gameController == null)
                return;

            gameController.TryPlaceNextTile(x, y);
            RefreshCells();
        }

        private void OnStartWaveClicked()
        {
            gameController?.StartWave();
            RefreshActionButtons();
        }

        private void OnClaimRewardClicked()
        {
            gameController?.ClaimReward();
            RefreshActionButtons();
        }

        private void OnNextLevelClicked()
        {
            gameController?.StartNextLevel();
            RefreshCells();
            RefreshActionButtons();
        }

        private void OnRetryClicked()
        {
            gameController?.RetryLevel();
            RefreshCells();
            RefreshActionButtons();
        }

        private void OnUpgradeShelterClicked()
        {
            gameController?.UpgradeShelter();
            RefreshActionButtons();
        }

        private void OnDailyRewardClicked()
        {
            gameController?.ClaimDailyReward();
            RefreshActionButtons();
        }

        private void OnClaimQuestClicked()
        {
            gameController?.ClaimQuest();
            RefreshActionButtons();
        }

        private void OnDoubleRewardClicked()
        {
            gameController?.DoubleReward();
            RefreshActionButtons();
        }

        private void OnReviveClicked()
        {
            gameController?.Revive();
            RefreshCells();
            RefreshActionButtons();
        }

        private void OnResetSaveClicked()
        {
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
                cell.Background.color = GetTileColor(tile);
            }
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
                reviveButton.gameObject.SetActive(gameController.CanRevive);

            if (resetSaveButton != null)
                resetSaveButton.gameObject.SetActive(true);
        }

        private Font GetDefaultFont()
        {
            if (_defaultFont == null)
                _defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return _defaultFont;
        }

        private static string FormatTileLabel(TileData tile)
        {
            return tile.IsEmpty ? "+" : $"{tile.Type}\nT{tile.Tier}";
        }

        private static Color GetTileColor(TileData tile)
        {
            if (tile.IsEmpty)
                return new Color(0.18f, 0.2f, 0.22f);

            switch (tile.Type)
            {
                case TileType.Wood:
                    return new Color(0.47f, 0.28f, 0.13f);
                case TileType.Metal:
                    return new Color(0.36f, 0.43f, 0.48f);
                case TileType.Food:
                    return new Color(0.23f, 0.5f, 0.27f);
                case TileType.Energy:
                    return new Color(0.18f, 0.38f, 0.62f);
                default:
                    return new Color(0.18f, 0.2f, 0.22f);
            }
        }

        private sealed class CellView
        {
            public int X;
            public int Y;
            public Image Background;
            public Text Label;
        }
    }
}
