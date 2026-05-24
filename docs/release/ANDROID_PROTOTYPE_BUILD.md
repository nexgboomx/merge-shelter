# Android Prototype Build

This guide is for Sprint 4 internal Android prototype packaging only. It is meant for QA device testing, gameplay capture, and early creative capture. It does not cover store release signing, IAP, real ads, backend services, or cloud save.

## Required Unity Version

- Unity Editor: `6000.3.16f1`
- Project version file: `ProjectSettings/ProjectVersion.txt`
- Prototype scene: `Assets/Scenes/PrototypeSprint1.unity`

Use this exact editor version when possible. If another Unity 6 editor is used, run EditMode and PlayMode tests before sharing an APK.

## Required Unity Modules

Install these modules through Unity Hub for `6000.3.16f1`:

- Android Build Support
- Android SDK and NDK Tools
- OpenJDK

Local validation on 2026-05-24 confirmed the Unity install contains:

- `Editor/Data/PlaybackEngines/AndroidPlayer`
- `Editor/Data/PlaybackEngines/AndroidPlayer/SDK`
- `Editor/Data/PlaybackEngines/AndroidPlayer/NDK`
- `Editor/Data/PlaybackEngines/AndroidPlayer/OpenJDK`

## Current Android Project Settings

Current settings observed in `ProjectSettings/ProjectSettings.asset`:

- Product name: `merge-shelter`
- Company name: `DefaultCompany`
- App version: `1.0`
- Android version code: `1`
- Minimum API level: Android API 25
- Target API level: automatic/highest installed
- Target architecture: ARM64
- Android game category: enabled
- Custom keystore: disabled

The project currently has no store-ready Android application identifier configured. Keep this acceptable for prototype builds; set a production identifier only when store release work starts.

## Switch Platform To Android

1. Open the project in Unity `6000.3.16f1`.
2. Open `File > Build Profiles`.
3. Select `Android`.
4. Click `Switch Platform`.
5. Wait for script compilation and asset import to finish.
6. Confirm `Assets/Scenes/PrototypeSprint1.unity` is enabled in the scene list.

In older Unity UI layouts, use `File > Build Settings` and follow the same Android platform switch flow.

## APK Build Steps

1. Open `File > Build Profiles`.
2. Select `Android`.
3. Use `Build Type: APK` or leave `Build App Bundle` disabled.
4. Enable `Development Build` for QA prototype builds.
5. Leave `Script Debugging` off unless debugging a device-only issue.
6. Confirm the scene list includes only the playable prototype scene unless another test scene is intentionally required.
7. Click `Build`.
8. Save the artifact under `Builds/Android/`.

Recommended output path:

```text
Builds/Android/merge-shelter-sprint4-prototype-YYYYMMDD-b001-debug.apk
```

Replace `YYYYMMDD` with the build date and increment `b001` when multiple APKs are produced on the same date.

## Batch APK Build

The repo includes an Editor-only build entry point for repeatable prototype APK builds:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.3.16f1\Editor\Unity.exe' `
  -batchmode `
  -nographics `
  -quit `
  -projectPath . `
  -buildTarget Android `
  -executeMethod MergeShelter.EditorTools.PrototypeAndroidBuild.BuildDebugApk `
  -buildOutputPath Builds\Android\merge-shelter-sprint4-prototype-YYYYMMDD-b001-debug.apk `
  -logFile Logs\AndroidPrototypeBuild.log
```

The build method uses enabled scenes from `ProjectSettings/EditorBuildSettings.asset`, creates an APK, and enables Unity `Development Build` output for internal QA.

## Debug Signing Notes

- Debug signing is acceptable for Sprint 4.
- `AndroidUseCustomKeystore` is currently disabled.
- Do not add a production keystore in this sprint.
- Do not commit keystore files or signing passwords.
- Store signing and Play Console upload are out of scope.

## Manual Device Install

Use a test Android device with USB debugging enabled.

1. Connect the device by USB.
2. Confirm the device is visible:

```powershell
adb devices
```

3. Install or replace the prototype APK:

```powershell
adb install -r Builds/Android/merge-shelter-sprint4-prototype-YYYYMMDD-b001-debug.apk
```

4. Launch the app from the device launcher.

If install fails because of an existing incompatible signature, uninstall the existing prototype first:

```powershell
adb uninstall com.DefaultCompany.merge-shelter
```

The exact package id may change if Android application identifier settings are updated later.

## Manual Device Smoke Checklist

Run this checklist on a physical Android device after installing the APK.

- App launches without crash.
- Prototype board appears.
- A board cell can be tapped and places the next tile.
- Start Wave button starts wave resolution.
- Weak board can lose a harder level.
- Strong merged board can win the same level.
- Claim Reward grants pending reward.
- Next Level starts an unlocked next level.
- Retry works after defeat.
- Daily Reward can be claimed once.
- Daily Quest progress updates after tile placement, level completion, and reward claim.
- Reward Double mock button appears only after victory with pending reward and doubles once.
- Revive mock button appears only after defeat and can be used once.
- Upgrade Shelter button appears.
- Upgrade is blocked when coins are insufficient.
- Upgrade succeeds when enough coins are available.
- Close and relaunch keeps coins, parts, highest unlocked level, selected level, shelter upgrade level, daily reward state, and daily quest state.
- Reset Save button returns the prototype to new-player Level 1 state.
- No real ads, IAP, account login, or network backend prompts appear.

## Known Prototype Limits

- Local JSON save only.
- No cloud save.
- No account login.
- No encryption.
- No store signing.
- No real ad SDK.
- No real IAP.
- No backend or remote config.
- No final art, final app icon, or store metadata.

## Troubleshooting

If Unity reports `Android SDK not found` and references `cmdline-tools/latest/bin/sdkmanager.bat`, reinstall Android SDK and NDK Tools from Unity Hub for the required editor version or configure an SDK that contains:

- Android SDK Command-line Tools latest
- Android SDK Platform-Tools
- Android SDK Platform, API 35 or newer
- Android SDK Build-Tools 35.0.0 or newer

After repairing the SDK, rerun the batch APK build command and confirm the APK appears under `Builds/Android/`.
