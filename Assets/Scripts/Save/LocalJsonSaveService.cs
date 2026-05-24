using System;
using System.IO;
using UnityEngine;

namespace MergeShelter.Save
{
    public sealed class LocalJsonSaveService : ISaveService
    {
        public const string DefaultFileName = "merge_shelter_save.json";

        private readonly string _saveDirectory;
        private readonly string _saveFilePath;

        public LocalJsonSaveService(string saveDirectory = null, string fileName = DefaultFileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Save file name is required.", nameof(fileName));

            _saveDirectory = string.IsNullOrWhiteSpace(saveDirectory)
                ? Application.persistentDataPath
                : saveDirectory;
            _saveFilePath = Path.Combine(_saveDirectory, fileName);
        }

        public string SaveFilePath => _saveFilePath;

        public void Save(GameSaveData saveData)
        {
            if (saveData == null)
                throw new ArgumentNullException(nameof(saveData));

            saveData.EnsureDefaults();
            Directory.CreateDirectory(_saveDirectory);
            var json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(_saveFilePath, json);
        }

        public bool TryLoad(out GameSaveData saveData)
        {
            saveData = null;
            if (!HasSave())
                return false;

            try
            {
                var json = File.ReadAllText(_saveFilePath);
                if (string.IsNullOrWhiteSpace(json))
                    return false;

                var trimmedJson = json.Trim();
                if (!trimmedJson.StartsWith("{", StringComparison.Ordinal) ||
                    !trimmedJson.EndsWith("}", StringComparison.Ordinal))
                {
                    return false;
                }

                var loaded = JsonUtility.FromJson<GameSaveData>(trimmedJson);
                if (loaded == null || !loaded.IsValid())
                    return false;

                loaded.EnsureDefaults();
                saveData = loaded;
                return true;
            }
            catch (Exception)
            {
                saveData = null;
                return false;
            }
        }

        public void Delete()
        {
            if (File.Exists(_saveFilePath))
                File.Delete(_saveFilePath);
        }

        public void Reset()
        {
            Delete();
        }

        public bool HasSave()
        {
            return File.Exists(_saveFilePath);
        }
    }
}
