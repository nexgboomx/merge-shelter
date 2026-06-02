# Android Debug Build Runbook

## Purpose
This runbook documents how to produce, install, launch, and troubleshoot a reproducible Merge Shelter Android debug APK for MVP release-candidate validation.

The MVP RC APK path is:

`Builds/Android/merge-shelter-mvp-rc-debug.apk`

## Supported Environment
- Unity Editor: `6000.3.16f1` (`6000.3.16f1 (a56f230f6470)` in `ProjectSettings/ProjectVersion.txt`)
- Unity modules:
  - Android Build Support
  - Android SDK and NDK Tools
  - OpenJDK
- Android package id: `com.DefaultCompany.mergeshelter`
- Android minimum SDK from project settings: API 25
- Android target SDK from project settings: automatic/default (`AndroidTargetSdkVersion: 0`)
- Primary physical smoke baseline: Samsung SM-S918B, Android 16 / API 36

Use Unity Hub to install the exact Unity version and Android modules when setting up a new machine.

## Branch Expectations
- Build the final MVP RC from the approved RC branch or from `main` after the RC branch is merged.
- For Sprint 10 hardening work, use `sprint-10-rc-hardening`.
- The working tree should be clean except for intentional build outputs, logs, and test result files.
- Do not commit generated APKs, Unity logs, or local test result output unless a release process explicitly asks for them.

## Clean Checkout
```bash
git clone https://github.com/nexgboomx/merge-shelter.git
cd merge-shelter
git checkout sprint-10-rc-hardening
git pull origin sprint-10-rc-hardening
git lfs pull || true
git status --short --branch
```

Expected result: the branch is current and no unexpected source or documentation changes are present.

## Unity Menu Build
Open the project folder in Unity `6000.3.16f1`, wait for import and compilation to finish, then run:

`Merge Shelter > Build Android Prototype APK`

Note: the current build helper has an older default APK path. For release-candidate builds, prefer the CLI command below so the output path is explicit and reproducible.

## CLI Build
Use the existing build method with the `-buildOutputPath` override:

```bash
UNITY_EDITOR="/home/phung-truong/Unity/Hub/Editor/6000.3.16f1/Editor/Unity"

"$UNITY_EDITOR" \
  -batchmode \
  -quit \
  -projectPath "$(pwd)" \
  -executeMethod MergeShelter.EditorTools.PrototypeAndroidBuild.BuildDebugApk \
  -buildOutputPath "Builds/Android/merge-shelter-mvp-rc-debug.apk" \
  -logFile "Logs/android-mvp-rc-build.log"
```

If Unity does not auto-detect Android tools, pass explicit paths:

```bash
"$UNITY_EDITOR" \
  -batchmode \
  -quit \
  -projectPath "$(pwd)" \
  -executeMethod MergeShelter.EditorTools.PrototypeAndroidBuild.BuildDebugApk \
  -buildOutputPath "Builds/Android/merge-shelter-mvp-rc-debug.apk" \
  -androidSdkPath "<ANDROID_SDK_PATH>" \
  -androidNdkPath "<ANDROID_NDK_PATH>" \
  -androidJdkPath "<JDK_PATH>" \
  -logFile "Logs/android-mvp-rc-build.log"
```

The build method also checks these environment variables when explicit paths are not supplied:

- `ANDROID_SDK_ROOT`
- `ANDROID_HOME`
- `ANDROID_NDK_ROOT`
- `ANDROID_NDK_HOME`
- `JAVA_HOME`

## Sprint-Specific APK Names
Use sprint-specific names only for pre-RC QA builds. Examples:

- `Builds/Android/merge-shelter-sprint10-rc-hardening-device-debug.apk`
- `Builds/Android/merge-shelter-sprint9-retention-loop-device-debug.apk`
- `Builds/Android/merge-shelter-sprint8-playability-device-debug.apk`

The final MVP release-candidate debug APK should use:

`Builds/Android/merge-shelter-mvp-rc-debug.apk`

## Verify APK Output
```bash
ls -lah Builds/Android/merge-shelter-mvp-rc-debug.apk
```

Expected result: the APK exists, has a non-zero file size, and has a recent timestamp from the current branch build.

## Automated Test Output
Store automated test logs and result XML under `Logs/` or `TestResults/`.

Suggested paths:

- EditMode result XML: `TestResults/mvp-rc-editmode-results.xml`
- PlayMode result XML: `TestResults/mvp-rc-playmode-results.xml`
- Unity build log: `Logs/android-mvp-rc-build.log`
- Android failure log, only if needed: `Logs/android-mvp-rc-logcat.txt`

Example Unity test command shape:

```bash
"$UNITY_EDITOR" \
  -batchmode \
  -quit \
  -projectPath "$(pwd)" \
  -runTests \
  -testPlatform EditMode \
  -testResults "TestResults/mvp-rc-editmode-results.xml" \
  -logFile "Logs/mvp-rc-editmode.log"
```

Run PlayMode the same way with `-testPlatform PlayMode` and PlayMode-specific output names.

## Verify Android Device
Use the Unity-installed ADB or another trusted Android SDK ADB:

```bash
ADB="/home/phung-truong/Unity/Hub/Editor/6000.3.16f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb"

"$ADB" devices -l
"$ADB" shell getprop ro.product.manufacturer
"$ADB" shell getprop ro.product.model
"$ADB" shell getprop ro.build.version.release
"$ADB" shell getprop ro.build.version.sdk
```

Expected result: one authorized physical Android device is listed and safe metadata is recorded in QA notes.

## Install APK
```bash
"$ADB" install -r Builds/Android/merge-shelter-mvp-rc-debug.apk
```

If install fails because of a package or signature conflict, uninstall only Merge Shelter and retry:

```bash
"$ADB" uninstall com.DefaultCompany.mergeshelter
"$ADB" install -r Builds/Android/merge-shelter-mvp-rc-debug.apk
```

Do not uninstall any other package.

## Launch APK
```bash
"$ADB" shell monkey -p com.DefaultCompany.mergeshelter 1
```

Verify Merge Shelter is foreground before screenshots or input:

```bash
"$ADB" shell dumpsys window | grep -E "mCurrentFocus|mFocusedApp"
```

Expected result: foreground focus references `com.DefaultCompany.mergeshelter` and Unity player activity.

## Security Boundary For Device QA
- Test only the Merge Shelter app.
- Do not open, inspect, capture, pull, or modify personal apps or personal data.
- Do not tap notifications or inspect notification content.
- Do not use `adb pull`.
- Do not browse device storage.
- Do not read photos, downloads, documents, contacts, SMS, call logs, clipboard, accounts, app data, or files.
- Use screenshots and input only after foreground verification confirms Merge Shelter is foreground.
- Use `adb logcat -d` only if Merge Shelter crashes, freezes, or fails to launch.

## Common Troubleshooting

### Unity License Or Login
- Symptom: batchmode build exits before compilation or reports license/login failure.
- Action: open Unity Hub, sign in, activate the license, open the project once, and confirm compilation succeeds.

### Missing Android Module
- Symptom: Unity cannot switch to Android or reports missing Android build support.
- Action: install Android Build Support, Android SDK and NDK Tools, and OpenJDK for Unity `6000.3.16f1` through Unity Hub.

### SDK License Issue
- Symptom: Gradle or Android build fails with SDK license errors.
- Action: open Unity Android preferences or Android SDK tools and accept SDK licenses for the installed SDK. Re-run the same CLI build after licenses are accepted.

### Device Unauthorized
- Symptom: `adb devices -l` shows `unauthorized`.
- Action: unlock the phone, accept the USB debugging RSA prompt, then rerun `adb devices -l`.

### Device Not Listed
- Symptom: `adb devices -l` shows no physical device.
- Action: check USB cable, USB mode, Developer Options, and USB debugging. Do not run unrelated device commands.

### Install Signature Conflict
- Symptom: `adb install -r` fails with a signature/package conflict.
- Action: uninstall only `com.DefaultCompany.mergeshelter`, then reinstall the same APK.

### Black Screen, Crash, Freeze, Or Launch Failure
- Symptom: app launches to black screen, freezes, crashes, or fails to foreground.
- Action: capture logcat only for failure diagnosis:

```bash
"$ADB" logcat -d > Logs/android-mvp-rc-logcat.txt
```

Record the failure in QA notes with the APK path, branch, commit, device model, Android version/API, and whether the issue is P0/P1.

## Out Of Scope
- Production signing
- Play Store or AAB packaging
- Store submission
- Real ad SDK
- Real IAP
- Backend services
- Cloud save
- Real calendar-based daily reset
