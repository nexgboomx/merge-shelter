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
                _subscribedController.BoardChanged -= RefreshCells;

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
                _subscribedController.BoardChanged -= RefreshCells;

            _subscribedController = gameController;

            if (_subscribedController != null)
                _subscribedController.BoardChanged += RefreshCells;
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

        private Button CreateStartWaveButton()
        {
            var buttonObject = new GameObject("StartWaveButton", typeof(RectTransform));
            buttonObject.transform.SetParent(transform, false);

            var rectTransform = buttonObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(180f, 44f);

            var boardHeight = BoardModel.DefaultHeight * cellSize + (BoardModel.DefaultHeight - 1) * cellSpacing;
            rectTransform.anchoredPosition = new Vector2(0f, -(boardHeight * 0.5f + 40f));

            var background = buttonObject.AddComponent<Image>();
            background.color = new Color(0.13f, 0.42f, 0.32f);

            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = background;

            var label = CreateCellLabel(buttonObject.transform);
            label.text = "Start Wave";
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

        private Font GetDefaultFont()
        {
            if (_defaultFont == null)
                _defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

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
