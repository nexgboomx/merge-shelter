using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MergeShelter.EditorTools
{
    public static class PrototypeAndroidBuild
    {
        private const string DefaultOutputPath = "Builds/Android/merge-shelter-sprint4-prototype-debug.apk";

        [MenuItem("Merge Shelter/Build Android Prototype APK")]
        public static void BuildDebugApk()
        {
            var outputPath = GetCommandLineArgument("-buildOutputPath") ?? DefaultOutputPath;
            outputPath = Path.GetFullPath(outputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            ConfigureAndroidExternalTools();

            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            if (scenes.Length == 0)
                throw new InvalidOperationException("No enabled scenes found in EditorBuildSettings.");

            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
                throw new InvalidOperationException("Failed to switch active build target to Android.");

            EditorUserBuildSettings.buildAppBundle = false;

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.Development
            };

            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;
            if (summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException($"Android prototype build failed: {summary.result}");

            Debug.Log($"Android prototype APK built: {outputPath}");
        }

        private static void ConfigureAndroidExternalTools()
        {
            var sdkPath = GetCommandLineArgument("-androidSdkPath") ?? Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT") ?? Environment.GetEnvironmentVariable("ANDROID_HOME");
            if (!string.IsNullOrWhiteSpace(sdkPath))
            {
                AndroidExternalToolsSettings.sdkRootPath = Path.GetFullPath(sdkPath);
                Debug.Log($"Android SDK path set to: {AndroidExternalToolsSettings.sdkRootPath}");
            }

            var ndkPath = GetCommandLineArgument("-androidNdkPath") ?? Environment.GetEnvironmentVariable("ANDROID_NDK_ROOT") ?? Environment.GetEnvironmentVariable("ANDROID_NDK_HOME");
            if (!string.IsNullOrWhiteSpace(ndkPath))
            {
                AndroidExternalToolsSettings.ndkRootPath = Path.GetFullPath(ndkPath);
                Debug.Log($"Android NDK path set to: {AndroidExternalToolsSettings.ndkRootPath}");
            }

            var jdkPath = GetCommandLineArgument("-androidJdkPath") ?? Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrWhiteSpace(jdkPath))
            {
                AndroidExternalToolsSettings.jdkRootPath = Path.GetFullPath(jdkPath);
                Debug.Log($"Android JDK path set to: {AndroidExternalToolsSettings.jdkRootPath}");
            }
        }

        private static string GetCommandLineArgument(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                    return args[i + 1];
            }

            return null;
        }
    }
}
