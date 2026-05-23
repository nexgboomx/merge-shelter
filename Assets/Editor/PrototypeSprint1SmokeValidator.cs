using System;
using System.Reflection;
using MergeShelter.Board;
using MergeShelter.Core;
using MergeShelter.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace MergeShelter.EditorTools
{
    public static class PrototypeSprint1SmokeValidator
    {
        private const string ScenePath = "Assets/Scenes/PrototypeSprint1.unity";

        public static void Run()
        {
            EditorSceneManager.OpenScene(ScenePath);

            var controller = FindRequired<PrototypeGameController>();
            var hudView = FindRequired<PrototypeHudView>();
            var boardView = FindRequired<PrototypeBoardView>();
            var canvas = FindRequired<Canvas>();

            Require(canvas.GetComponent<GraphicRaycaster>() != null, "Canvas is missing GraphicRaycaster.");
            Require(GameObject.Find("EventSystem") != null, "Scene is missing EventSystem.");
            RequireSerializedReference(controller, "hudView");
            RequireSerializedReference(boardView, "gameController");
            RequireSerializedReference(boardView, "boardRoot");
            RequireSerializedReference(boardView, "startWaveButton");
            RequireSerializedReference(hudView, "levelText");
            RequireSerializedReference(hudView, "tutorialText");
            RequireSerializedReference(hudView, "shelterHpText");
            RequireSerializedReference(hudView, "nextTileText");
            RequireSerializedReference(hudView, "resultText");
            RequireSerializedReference(hudView, "walletText");

            InvokePrivate(controller, "Awake");
            InvokePrivate(boardView, "Awake");
            InvokePrivate(boardView, "OnEnable");
            InvokePrivate(boardView, "Start");

            var cellButton = FindRequiredButton("Cell_0_0");
            cellButton.onClick.Invoke();
            Require(!controller.GetTileAt(0, 0).IsEmpty, "Clicking Cell_0_0 did not place a tile.");

            var startWaveButton = FindRequiredButton("StartWaveButton");
            startWaveButton.onClick.Invoke();
            Require(!string.IsNullOrWhiteSpace(GetResultText().text), "Start Wave did not update result text.");

            ValidateWeakAndStrongOutcomes(controller);
            Debug.Log("Prototype Sprint 1 smoke validation passed.");
        }

        private static void ValidateWeakAndStrongOutcomes(PrototypeGameController controller)
        {
            controller.StartLevel(9);
            controller.StartWave();
            var weakResult = GetResultText().text;
            Require(weakResult.Contains("Defeat"), $"Expected weak board defeat, got: {weakResult}");

            controller.StartLevel(9);
            var board = GetBoard(controller);
            board.SetTile(new BoardPosition(0, 0), new TileData(TileType.Wood, 3));
            board.SetTile(new BoardPosition(1, 0), new TileData(TileType.Metal, 3));
            board.SetTile(new BoardPosition(2, 0), new TileData(TileType.Food, 3));
            board.SetTile(new BoardPosition(3, 0), new TileData(TileType.Energy, 3));

            controller.StartWave();
            var strongResult = GetResultText().text;
            Require(strongResult.Contains("Victory"), $"Expected strong board victory, got: {strongResult}");
            Require(weakResult != strongResult, "Weak and strong board outcomes matched unexpectedly.");
        }

        private static T FindRequired<T>() where T : UnityEngine.Object
        {
            var found = UnityEngine.Object.FindObjectOfType<T>();
            Require(found != null, $"Missing required scene object: {typeof(T).Name}");
            return found;
        }

        private static Button FindRequiredButton(string name)
        {
            var button = GameObject.Find(name)?.GetComponent<Button>();
            Require(button != null, $"Missing required button: {name}");
            return button;
        }

        private static Text GetResultText()
        {
            var resultText = GameObject.Find("ResultText")?.GetComponent<Text>();
            Require(resultText != null, "Missing ResultText.");
            return resultText;
        }

        private static BoardModel GetBoard(PrototypeGameController controller)
        {
            var field = typeof(PrototypeGameController).GetField("_board", BindingFlags.Instance | BindingFlags.NonPublic);
            Require(field != null, "PrototypeGameController._board was not found.");
            return (BoardModel)field.GetValue(controller);
        }

        private static void RequireSerializedReference(UnityEngine.Object target, string propertyName)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(propertyName);
            Require(property != null, $"{target.name}.{propertyName} serialized property was not found.");
            Require(property.objectReferenceValue != null, $"{target.name}.{propertyName} is not wired.");
        }

        private static void InvokePrivate(object target, string methodName)
        {
            var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Require(method != null, $"{target.GetType().Name}.{methodName} was not found.");
            method.Invoke(target, null);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
    }
}
