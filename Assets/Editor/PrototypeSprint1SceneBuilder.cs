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
            CreateOpaqueBackground(canvas.transform);

            var hud = new GameObject("PrototypeHud", typeof(RectTransform));
            hud.transform.SetParent(canvas.transform, false);
            var hudView = hud.AddComponent<PrototypeHudView>();

            var levelText = CreateHudText(canvas.transform, "LevelText", new Vector2(24f, -16f), new Vector2(672f, 24f), 18);
            var objectiveText = CreateHudText(canvas.transform, PrototypeHudView.ObjectiveTextName, new Vector2(24f, -42f), new Vector2(672f, 18f), 12, "Goal:");
            var tutorialText = CreateHudText(canvas.transform, "TutorialText", new Vector2(24f, -62f), new Vector2(672f, 28f), 13);
            var shelterLabelText = CreateHudText(canvas.transform, PrototypeHudView.ShelterSectionLabelName, new Vector2(24f, -94f), new Vector2(672f, 14f), 11, "SHELTER");
            var shelterHpText = CreateHudText(canvas.transform, "ShelterHpText", new Vector2(24f, -112f), new Vector2(328f, 24f), 14);
            var shelterUpgradeText = CreateHudText(canvas.transform, PrototypeHudView.ShelterUpgradeTextName, new Vector2(368f, -112f), new Vector2(328f, 24f), 12);
            shelterUpgradeText.alignment = TextAnchor.MiddleRight;
            var rewardsLabelText = CreateHudText(canvas.transform, PrototypeHudView.RewardsSectionLabelName, new Vector2(24f, -144f), new Vector2(672f, 14f), 11, "REWARDS");
            var walletText = CreateHudText(canvas.transform, "WalletText", new Vector2(24f, -162f), new Vector2(328f, 22f), 12);
            var rewardText = CreateHudText(canvas.transform, PrototypeHudView.RewardTextName, new Vector2(368f, -162f), new Vector2(328f, 22f), 12);
            rewardText.alignment = TextAnchor.MiddleRight;
            var questsLabelText = CreateHudText(canvas.transform, PrototypeHudView.QuestsSectionLabelName, new Vector2(24f, -216f), new Vector2(672f, 14f), 11, "QUESTS");
            var questText = CreateHudText(canvas.transform, PrototypeHudView.QuestTextName, new Vector2(24f, -234f), new Vector2(672f, 40f), 12);
            var boardLabelText = CreateHudText(canvas.transform, PrototypeHudView.BoardSectionLabelName, new Vector2(24f, -286f), new Vector2(328f, 14f), 11, "BOARD");
            var nextTileText = CreateHudText(canvas.transform, "NextTileText", new Vector2(368f, -286f), new Vector2(328f, 14f), 12);
            nextTileText.alignment = TextAnchor.MiddleRight;
            var waveRosterText = CreateHudText(canvas.transform, PrototypeHudView.WaveRosterTextName, new Vector2(24f, -302f), new Vector2(672f, 28f), 11, "Wave:");
            var actionsLabelText = CreateBottomHudText(canvas.transform, PrototypeHudView.ActionsSectionLabelName, 208f, new Vector2(672f, 14f), 11);
            actionsLabelText.text = "ACTIONS";
            var resultText = CreateBottomHudText(canvas.transform, "ResultText", 250f, new Vector2(672f, 72f), 14);

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

            WireHud(
                hudView,
                levelText,
                objectiveText,
                tutorialText,
                waveRosterText,
                shelterLabelText,
                shelterHpText,
                shelterUpgradeText,
                boardLabelText,
                nextTileText,
                actionsLabelText,
                rewardsLabelText,
                rewardText,
                questsLabelText,
                questText,
                resultText,
                walletText);
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

        private static void CreateOpaqueBackground(Transform parent)
        {
            var rectTransform = CreateRectTransformObject(PrototypeHudView.OpaqueBackgroundName, parent);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            var image = rectTransform.gameObject.AddComponent<Image>();
            image.color = PrototypeVisualKit.CanvasBackground;
            image.raycastTarget = false;
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

        private static Text CreateHudText(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, int fontSize, string initialText = null)
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
            text.color = PrototypeVisualKit.PrimaryText;
            text.alignment = TextAnchor.UpperLeft;
            text.resizeTextForBestFit = false;
            text.resizeTextMinSize = fontSize;
            text.resizeTextMaxSize = fontSize;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.lineSpacing = 1.05f;
            text.raycastTarget = false;
            text.text = initialText ?? name;
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
            text.color = PrototypeVisualKit.PrimaryText;
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
            image.color = PrototypeVisualKit.GetActionButtonColor(name, false);

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
            text.color = PrototypeVisualKit.ButtonText;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
            text.text = labelText;
            return button;
        }

        private static void WireHud(
            PrototypeHudView hudView,
            Text levelText,
            Text objectiveText,
            Text tutorialText,
            Text waveRosterText,
            Text shelterLabelText,
            Text shelterHpText,
            Text shelterUpgradeText,
            Text boardLabelText,
            Text nextTileText,
            Text actionsLabelText,
            Text rewardsLabelText,
            Text rewardText,
            Text questsLabelText,
            Text questText,
            Text resultText,
            Text walletText)
        {
            var serializedObject = new SerializedObject(hudView);
            serializedObject.FindProperty("levelText").objectReferenceValue = levelText;
            serializedObject.FindProperty("objectiveText").objectReferenceValue = objectiveText;
            serializedObject.FindProperty("tutorialText").objectReferenceValue = tutorialText;
            serializedObject.FindProperty("waveRosterText").objectReferenceValue = waveRosterText;
            serializedObject.FindProperty("shelterLabelText").objectReferenceValue = shelterLabelText;
            serializedObject.FindProperty("shelterHpText").objectReferenceValue = shelterHpText;
            serializedObject.FindProperty("shelterUpgradeText").objectReferenceValue = shelterUpgradeText;
            serializedObject.FindProperty("boardLabelText").objectReferenceValue = boardLabelText;
            serializedObject.FindProperty("nextTileText").objectReferenceValue = nextTileText;
            serializedObject.FindProperty("actionsLabelText").objectReferenceValue = actionsLabelText;
            serializedObject.FindProperty("rewardsLabelText").objectReferenceValue = rewardsLabelText;
            serializedObject.FindProperty("rewardText").objectReferenceValue = rewardText;
            serializedObject.FindProperty("questsLabelText").objectReferenceValue = questsLabelText;
            serializedObject.FindProperty("questText").objectReferenceValue = questText;
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
