using System.IO;
using MergeShelter.Core;
using MergeShelter.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MergeShelter.EditorTools
{
    public static class PrototypeSprint1SceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/PrototypeSprint1.unity";
        private static readonly Vector2 ReferenceResolution = new(720f, 1280f);

        [MenuItem("Merge Shelter/Build Prototype Sprint 1 Scene")]
        public static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var prototypeGame = new GameObject("PrototypeGame");
            var gameController = prototypeGame.AddComponent<PrototypeGameController>();

            var canvas = CreateCanvas();
            CreateEventSystem();

            var hud = new GameObject("PrototypeHud", typeof(RectTransform));
            hud.transform.SetParent(canvas.transform, false);
            var hudView = hud.AddComponent<PrototypeHudView>();

            var levelText = CreateHudText(canvas.transform, "LevelText", new Vector2(24f, -16f), new Vector2(672f, 28f), 20);
            var tutorialText = CreateHudText(canvas.transform, "TutorialText", new Vector2(24f, -48f), new Vector2(672f, 42f), 14);
            var shelterHpText = CreateHudText(canvas.transform, "ShelterHpText", new Vector2(24f, -98f), new Vector2(328f, 24f), 14);
            var nextTileText = CreateHudText(canvas.transform, "NextTileText", new Vector2(368f, -98f), new Vector2(328f, 24f), 14);
            nextTileText.alignment = TextAnchor.MiddleRight;
            var walletText = CreateHudText(canvas.transform, "WalletText", new Vector2(24f, -130f), new Vector2(672f, 108f), 12);
            var resultText = CreateBottomHudText(canvas.transform, "ResultText", 236f, new Vector2(672f, 92f), 14);

            var boardRoot = CreateRectTransformObject("BoardRoot", canvas.transform);
            boardRoot.anchorMin = Vector2.zero;
            boardRoot.anchorMax = Vector2.one;
            boardRoot.pivot = new Vector2(0.5f, 0.5f);
            boardRoot.offsetMin = Vector2.zero;
            boardRoot.offsetMax = Vector2.zero;
            var boardView = boardRoot.gameObject.AddComponent<PrototypeBoardView>();

            var boardGrid = CreateRectTransformObject("BoardGrid", boardRoot);
            boardGrid.anchorMin = new Vector2(0.5f, 0.5f);
            boardGrid.anchorMax = new Vector2(0.5f, 0.5f);
            boardGrid.pivot = new Vector2(0.5f, 0.5f);
            boardGrid.anchoredPosition = new Vector2(0f, 64f);
            boardGrid.sizeDelta = new Vector2(462f, 462f);

            var startWaveButton = CreateButton(boardRoot, "StartWaveButton", "Start Wave", new Vector2(0f, 84f), new Vector2(200f, 44f));

            WireHud(hudView, levelText, tutorialText, shelterHpText, nextTileText, resultText, walletText);
            WireGameController(gameController, hudView);
            WireBoardView(boardView, gameController, boardGrid, startWaveButton);

            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new IOException($"Failed to save prototype scene at {ScenePath}.");

            AssetDatabase.ImportAsset(ScenePath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
            var sceneGuid = AssetDatabase.AssetPathToGUID(ScenePath);
            if (string.IsNullOrEmpty(sceneGuid))
                throw new IOException($"Unity did not import prototype scene at {ScenePath}.");

            var buildScene = new EditorBuildSettingsScene(ScenePath, true)
            {
                guid = new GUID(sceneGuid)
            };

            EditorBuildSettings.scenes = new[]
            {
                buildScene
            };

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static Canvas CreateCanvas()
        {
            var canvasObject = new GameObject("Canvas");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0f;
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static void CreateEventSystem()
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private static RectTransform CreateRectTransformObject(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            return gameObject.GetComponent<RectTransform>();
        }

        private static Text CreateHudText(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, int fontSize)
        {
            var rectTransform = CreateRectTransformObject(name, parent);
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;

            var text = rectTransform.gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperLeft;
            text.resizeTextForBestFit = false;
            text.resizeTextMinSize = fontSize;
            text.resizeTextMaxSize = fontSize;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.lineSpacing = 1.05f;
            text.raycastTarget = false;
            text.text = name;
            return text;
        }

        private static Text CreateBottomHudText(Transform parent, string name, float bottom, Vector2 size, int fontSize)
        {
            var rectTransform = CreateRectTransformObject(name, parent);
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(0f, 0f);
            rectTransform.pivot = new Vector2(0f, 0f);
            rectTransform.anchoredPosition = new Vector2(24f, bottom);
            rectTransform.sizeDelta = size;

            var text = rectTransform.gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperLeft;
            text.resizeTextForBestFit = false;
            text.resizeTextMinSize = fontSize;
            text.resizeTextMaxSize = fontSize;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.lineSpacing = 1.05f;
            text.raycastTarget = false;
            text.text = name;
            return text;
        }

        private static Button CreateButton(Transform parent, string name, string labelText, Vector2 anchoredPosition, Vector2 size)
        {
            var rectTransform = CreateRectTransformObject(name, parent);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;

            var image = rectTransform.gameObject.AddComponent<Image>();
            image.color = new Color(0.13f, 0.42f, 0.32f);

            var button = rectTransform.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var label = CreateRectTransformObject("Label", rectTransform);
            label.anchorMin = Vector2.zero;
            label.anchorMax = Vector2.one;
            label.offsetMin = Vector2.zero;
            label.offsetMax = Vector2.zero;

            var text = label.gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 20;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
            text.text = labelText;
            return button;
        }

        private static void WireHud(
            PrototypeHudView hudView,
            Text levelText,
            Text tutorialText,
            Text shelterHpText,
            Text nextTileText,
            Text resultText,
            Text walletText)
        {
            var serializedObject = new SerializedObject(hudView);
            serializedObject.FindProperty("levelText").objectReferenceValue = levelText;
            serializedObject.FindProperty("tutorialText").objectReferenceValue = tutorialText;
            serializedObject.FindProperty("shelterHpText").objectReferenceValue = shelterHpText;
            serializedObject.FindProperty("nextTileText").objectReferenceValue = nextTileText;
            serializedObject.FindProperty("resultText").objectReferenceValue = resultText;
            serializedObject.FindProperty("walletText").objectReferenceValue = walletText;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WireGameController(PrototypeGameController gameController, PrototypeHudView hudView)
        {
            var serializedObject = new SerializedObject(gameController);
            serializedObject.FindProperty("hudView").objectReferenceValue = hudView;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void WireBoardView(
            PrototypeBoardView boardView,
            PrototypeGameController gameController,
            RectTransform boardGrid,
            Button startWaveButton)
        {
            var serializedObject = new SerializedObject(boardView);
            serializedObject.FindProperty("gameController").objectReferenceValue = gameController;
            serializedObject.FindProperty("boardRoot").objectReferenceValue = boardGrid;
            serializedObject.FindProperty("startWaveButton").objectReferenceValue = startWaveButton;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
