# Merge Shelter

Merge Shelter is a portrait Unity mobile MVP prototype for a hybrid-casual survival puzzle loop. Players place resource tiles on a compact board, merge 3 matching tiles into stronger defenses, start a wave, survive with shelter HP intact, claim rewards, upgrade the shelter, and progress through a 30-level prototype catalog.

The current project is a debug/development MVP handoff build, not a production store release.

## Current MVP Features
- Board tile placement on a compact mobile-friendly grid.
- Merge 3 matching tiles into a stronger tile.
- Wave resolution against readable enemy rosters.
- Shelter HP, damage, victory, defeat, retry, and revive states.
- Level rewards with coins and parts.
- 30-level catalog with early tutorial levels, midgame upgrade pressure, and late prototype pressure.
- First-run tutorial that guides tile placement, merge intent, Start Wave, Claim Reward, and continuation.
- Player-facing level objectives.
- Pre-wave roster with readable enemy display names.
- Decision prompts for build planning, rewards, upgrades, defeat recovery, and navigation.
- Daily Reward local/session loop.
- Daily Quests with titles, progress, ready, claimed, and one-time reward states.
- Mock Reward Double flow.
- Mock Revive flow.
- Previous, Replay, and Next Unlocked level navigation for unlocked levels.
- Local JSON save/load/reset.
- Android debug APK build support.

## Requirements
- Unity Editor: `6000.3.16f1`
- Unity modules:
  - Android Build Support
  - Android SDK and NDK Tools
  - OpenJDK
- Android package id: `com.DefaultCompany.mergeshelter`
- Primary physical QA baseline: Samsung SM-S918B, Android 16 / API 36

## Open The Project
1. Clone the repository.
2. Open the repository folder in Unity Hub with Unity `6000.3.16f1`.
3. Wait for Unity import and C# compilation to finish.
4. Open `Assets/Scenes/PrototypeSprint1.unity`.

## Run Tests
The project uses Unity EditMode and PlayMode tests. Example command shape:

```bash
UNITY_EDITOR="/home/phung-truong/Unity/Hub/Editor/6000.3.16f1/Editor/Unity"

"$UNITY_EDITOR" \
  -batchmode \
  -quit \
  -projectPath "$(pwd)" \
  -runTests \
  -testPlatform EditMode \
  -testResults "TestResults/editmode-results.xml" \
  -logFile "Logs/editmode.log"

"$UNITY_EDITOR" \
  -batchmode \
  -quit \
  -projectPath "$(pwd)" \
  -runTests \
  -testPlatform PlayMode \
  -testResults "TestResults/playmode-results.xml" \
  -logFile "Logs/playmode.log"
```

Current post-victory-copy-fix automated baseline: EditMode passed `67/67`, PlayMode passed `19/19` on the fix branch before the fix was merged to `main`.

## Build Android Debug APK
Use the Unity menu:

`Merge Shelter > Build Android Prototype APK`

For reproducible release-candidate builds, prefer the CLI build with an explicit output path:

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

Final Sprint 10 MVP RC APK path:

`Builds/Android/merge-shelter-mvp-rc-debug.apk`

## QA And Handoff Docs
- Sprint 11 demo handoff plan: `docs/production/SPRINT_11_DEMO_HANDOFF_PLAN.md`
- Android debug build runbook: `docs/production/ANDROID_DEBUG_BUILD_RUNBOOK.md`
- MVP release candidate checklist: `docs/qa/MVP_RELEASE_CANDIDATE_CHECKLIST.md`
- Sprint smoke history: `docs/qa/SPRINT_1_SMOKE_CHECKLIST.md`
- Demo/playtest script: `docs/demo/MVP_DEMO_PLAYTEST_SCRIPT.md`

## Android QA Security Boundary
When testing on a physical Android phone, test only the Merge Shelter app. Do not open, inspect, capture, pull, or modify personal apps or personal data. Use screenshots and input only after foreground verification confirms `com.DefaultCompany.mergeshelter` is the foreground app.

## Out Of Scope For Current MVP
- Final art.
- Real ads SDK.
- Real IAP.
- Backend.
- Cloud save.
- Real calendar-based daily reset.
- Production signing.
- Store submission.
- Play Store or AAB release packaging.
