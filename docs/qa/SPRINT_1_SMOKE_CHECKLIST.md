# Sprint 1 Smoke Checklist

## Purpose
This checklist is the minimum quality gate for the first playable prototype.

## Build Check

| ID | Test | Expected Result | Status |
|---|---|---|---|
| QA-001 | Open project in Unity | Project opens without compile errors | Pass - Unity 6000.3.16f1 batch open compiles cleanly; asmdef UI references verified |
| QA-002 | Open prototype scene | Scene opens without missing script errors | Pass - `Assets/Scenes/PrototypeSprint1.unity` loads in PlayMode smoke |
| QA-003 | Enter play mode | No immediate exception | Pass - PlayMode smoke tests completed |

## Level Flow Check

| ID | Test | Expected Result | Status |
|---|---|---|---|
| QA-010 | Start Level 1 | Board and HUD appear | Pass - scene smoke finds controller, HUD, board, canvas, and event system |
| QA-011 | Place a tile on empty cell | Tile appears in selected cell | Pass - `Cell_0_0` button places a tile |
| QA-012 | Place a tile on occupied cell | Placement is rejected with feedback | Pass - covered by EditMode board model test |
| QA-013 | Merge 3 same tiles | New higher-tier tile appears | Pass - covered by EditMode merge resolver test |
| QA-014 | Try non-matching tiles | No merge occurs | Pass - covered by EditMode merge resolver test |
| QA-015 | Start enemy wave | Wave begins and shelter can take damage | Pass - Start Wave button and weak-board damage validated |
| QA-016 | Win level | Win screen appears and reward is shown | Pass - strong Level 10 board produces victory |
| QA-017 | Fail level | Fail screen appears and retry is available | Pass - weak Level 10 board produces defeat |

## Progression Check

| ID | Test | Expected Result | Status |
|---|---|---|---|
| QA-020 | Complete Level 1 | Level 2 unlocks or can be selected | Pass - Sprint 2 PlayMode smoke claims Level 1 reward and unlocks Level 2 |
| QA-021 | Play Level 1-10 sequentially | No blocker prevents progress | Not Run |
| QA-022 | Claim reward | Prototype coins increase | Pass - Sprint 2 PlayMode smoke grants coins on Claim Reward |

## Analytics Check

| ID | Test | Expected Result | Status |
|---|---|---|---|
| QA-030 | Start level | level_start event is logged | Pass - debug analytics log observed |
| QA-031 | Place tile | tile_place event is logged | Pass - debug analytics log observed |
| QA-032 | Complete merge | merge_success event is logged | Not Run |
| QA-033 | Win level | level_complete event is logged | Pass - debug analytics log observed |
| QA-034 | Lose level | level_fail event is logged | Pass - debug analytics log observed |

## Sprint 1 Unity Smoke Notes - 2026-05-23

- Unity version: 6000.3.16f1.
- Scene created: `Assets/Scenes/PrototypeSprint1.unity`.
- EditMode tests: 14 passed, 0 failed.
- PlayMode smoke tests: 2 passed, 0 failed.
- Additional editor smoke validator passed: board cell click placed a tile, Start Wave button updated result text, weak Level 10 board failed, strong Level 10 board won.
- Progression checks QA-020 through QA-022 remain not run because level unlock/claim flow is not yet implemented as a player-facing sequence.

## Sprint 1 Asmdef Verification Notes - 2026-05-23

- `Assets/Scripts/MergeShelter.asmdef` compiles with `UnityEngine.UI` scripts using an explicit `UnityEngine.UI` reference.
- `Assets/Tests/EditMode/MergeShelter.EditModeTests.asmdef` compiles and EditMode tests pass: 14 passed, 0 failed.
- `Assets/Tests/PlayMode/MergeShelter.PlayModeTests.asmdef` compiles with an explicit `UnityEngine.UI` reference and PlayMode smoke tests pass: 2 passed, 0 failed.

## Sprint 2 Progression State Notes - 2026-05-23

- Session progression state model added for issue #17.
- EditMode tests pass: 23 passed, 0 failed.
- Existing PlayMode smoke tests pass: 2 passed, 0 failed.
- Model coverage includes level 1 new-player state, locked level rejection, unlocked selection, coin/part spend safety, shelter upgrade level tracking, and pending reward store/clear/claim.

## Sprint 2 Reward Claim Notes - 2026-05-23

- Reward claim and level unlock flow added for issue #18.
- EditMode tests pass: 24 passed, 0 failed.
- PlayMode tests pass: 4 passed, 0 failed.
- PlayMode coverage includes locked Level 2 selection rejection, Claim Reward button visibility, double-claim rejection, Level 1 reward unlocking Level 2, Next Level starting Level 2, and Retry restarting the selected failed level.

## Sprint 2 Shelter Upgrade Notes - 2026-05-23

- Shelter upgrade flow added for issue #19.
- EditMode tests pass: 27 passed, 0 failed.
- PlayMode tests pass: 5 passed, 0 failed.
- Coverage includes insufficient-coin blocking, successful coin spend, increasing upgrade cost, HUD upgrade status, `shelter_upgraded` analytics, and upgraded shelter max HP on the next level.

## Sprint 2 Progression Smoke Notes - 2026-05-23

- Unity version: 6000.3.16f1.
- EditMode tests: 27 passed, 0 failed.
- PlayMode tests: 5 passed, 0 failed.
- Validated through PlayMode smoke: Level 1 win, Claim Reward, Level 2 unlock, Next Level starts Level 2, Retry after defeat, Upgrade Shelter button appears, upgrade is blocked without enough coins, upgrade succeeds with enough coins, and upgraded shelter max HP is visible in the HUD.
- Remaining known issues: no P0/P1 blockers found in automated smoke; full Level 1-10 manual sequence and 10-minute prototype loop remain not run.

## Sprint 3 Daily Reward Notes - 2026-05-23

- Unity version: 6000.3.16f1.
- Compile check: batch project open completed without C# compile errors.
- EditMode tests: 30 passed, 0 failed.
- PlayMode tests: 6 passed, 0 failed.
- Coverage includes session-start daily reward availability, coin/part grant, double-claim block, Daily Reward button visibility, button hiding after claim, HUD claimed state, and `daily_reward_claimed` analytics.
- Remaining known issues: no real date persistence by design; daily reward resets with each prototype session.

## Sprint 3 Daily Quest Notes - 2026-05-23

- Unity version: 6000.3.16f1.
- Compile check: batch project open completed without C# compile errors.
- EditMode tests: 35 passed, 0 failed.
- PlayMode tests: 7 passed, 0 failed.
- Coverage includes Place 10 Tiles, Complete 1 Level, Claim 1 Reward quest completion, single-claim reward grants, uncompleted claim blocking, HUD quest status, Claim Quest button visibility, and `quest_progress` / `quest_completed` / `quest_claimed` analytics hooks.
- Remaining known issues: quests are session-only by design and reset when the prototype session restarts.

## Sprint 3 Rewarded Mock Notes - 2026-05-23

- Unity version: 6000.3.16f1.
- Compile check: batch project open completed without C# compile errors.
- EditMode tests: 37 passed, 0 failed.
- PlayMode tests: 9 passed, 0 failed.
- Coverage includes Double Reward button visibility, one-use double reward, pending coin/part doubling before claim, Revive button visibility after defeat, one-use revive result handling, playable restart after revive, and `ad_offer_preview` / `ad_mock_started` / `ad_mock_completed` / `reward_doubled` / `revive_used` analytics hooks.
- Remaining known issues: rewarded ads are synchronous session-only mocks by design; no real ad SDK or network ad readiness is integrated.

## Sprint 3 Retention Monetization Final Smoke Notes - 2026-05-24

- Unity version: 6000.3.16f1.
- Compile check: batch project open completed without C# compile errors.
- EditMode tests: 37 passed, 0 failed.
- PlayMode tests: 9 passed, 0 failed.
- Automated validation passed for Daily Reward claim, Daily Reward double-claim block, Daily Quest progress, Daily Quest claim, Reward Double after victory, Reward Double one-use limit, Revive after defeat, and Revive one-use limit.
- Debug analytics logs observed for `daily_reward_claimed`, `quest_progress`, `quest_completed`, `quest_claimed`, `ad_offer_preview`, `ad_mock_started`, `ad_mock_completed`, `reward_doubled`, and `revive_used`.
- Remaining known issues: no P0/P1 blockers found in automated smoke. Sprint 3 retention and monetization systems are session-only mocks by design; no real date persistence, real ad SDK, IAP, cloud save, backend, remote config, device matrix pass, or 10-minute manual loop was run.

## Sprint 4 Save Wiring Notes - 2026-05-24

- Unity version: 6000.3.16f1.
- Compile check: batch project open completed without C# compile errors.
- EditMode tests: 41 passed, 0 failed.
- PlayMode tests: 11 passed, 0 failed.
- Automated validation covers save load on startup, coins/highest unlocked/shelter upgrade/daily reward/daily quest persistence across scene reload, save-on-change hooks, Reset Save button behavior, and reset returning to new-player Level 1 state.
- Remaining known issues: save is local JSON only by design; no cloud save, account login, encryption, backend, real IAP, or real ad SDK is integrated.

## Sprint 4 Android Build Docs Notes - 2026-05-24

- Unity version: 6000.3.16f1.
- Documentation added: `docs/release/ANDROID_PROTOTYPE_BUILD.md` and `docs/release/SPRINT_4_RELEASE_CHECKLIST.md`.
- Local toolchain check: Android Build Support, SDK, NDK, and OpenJDK folders are present under the Unity 6000.3.16f1 Android playback engine.
- Build settings check: `Assets/Scenes/PrototypeSprint1.unity` is enabled in `ProjectSettings/EditorBuildSettings.asset`.
- Android APK build and physical device install were not run for this documentation task.

## Sprint 4 Final Save And Android Packaging Smoke Notes - 2026-05-24

- Unity version: 6000.3.16f1.
- Compile check: batch project open completed without C# compile errors.
- EditMode tests: 41 passed, 0 failed.
- PlayMode tests: 11 passed, 0 failed.
- Save/load/reset validation: PlayMode coverage passed for coins, parts, highest unlocked level, selected level, shelter upgrade level, daily reward claimed state, daily quest progress/claimed state, and Reset Save returning to new-player Level 1 state.
- Android build status: passed after configuring the batch build to use `C:\Android\Sdk` plus Unity's embedded NDK r27c and OpenJDK.
- APK filename: `merge-shelter-sprint4-prototype-20260524-b005-debug.apk`.
- APK output path: `Builds/Android/merge-shelter-sprint4-prototype-20260524-b005-debug.apk`.
- Build type: development/debug APK.
- Device install status: not run. `adb devices -l` is available through `C:\Android\Sdk\platform-tools\adb.exe`, but no physical Android device is connected.
- Remaining known issues: no gameplay P0/P1 blockers found in automated tests or APK packaging. Physical Android install and manual device smoke remain pending until a device is connected.

## Sprint 4 Android Emulator/Device Smoke Notes - 2026-05-24

- Machine OS: Ubuntu 24.04.3 LTS, Linux 6.17.0-1017-oem x86_64.
- Unity version: 6000.3.16f1 installed locally with Android Build Support, Android SDK/NDK Tools, and OpenJDK 17.0.18.
- Test target: not run. `adb devices -l` completed through Unity's embedded Android SDK, but no emulator or physical Android device was connected.
- Emulator profile/device model: not run.
- Android API/version: not run on device. Unity Android SDK platforms 34, 35, and 36 are installed.
- APK filename: `merge-shelter-sprint4-prototype-20260524-b005-debug.apk` expected, but no APK is present in `Builds/Android` on this machine.
- APK output path: `Builds/Android/merge-shelter-sprint4-prototype-20260524-b005-debug.apk` expected; file not found.
- Install status: not run because no APK was available and no Android target was connected.
- Launch status: not run.
- Manual smoke result: not run.
- Save/load result: not run on Android.
- Reset Save result: not run on Android.
- EditMode tests: not run. Unity batch mode exited before project import with code 198 because no valid Unity Editor license was activated on this machine.
- PlayMode tests: not run for the same Unity license blocker.
- Remaining known issues: Unity account/license activation is required before compile validation, automated tests, or a fresh Android APK build can run on this machine. The Sprint 4 APK artifact from the previous machine is not committed to the repo or published as a GitHub release.
- P0/P1 blockers: environment blocker only; Android gameplay smoke could not be assessed.
- Physical Android device smoke was not run.

## Sprint 4 Linux Physical Android Device Smoke Notes - 2026-05-24

- Machine OS: Ubuntu 24.04.3 LTS.
- Unity version: 6000.3.16f1 (a56f230f6470).
- Android SDK/ADB path: `/home/phung-truong/Unity/Hub/Editor/6000.3.16f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb`.
- Device manufacturer: samsung.
- Device model: SM-S918B.
- Android version: 16.
- Android API level: 36.
- APK filename: `merge-shelter-sprint4-prototype-linux-device-debug.apk`.
- APK output path: `Builds/Android/merge-shelter-sprint4-prototype-linux-device-debug.apk`.
- Install result: passed with `adb install -r`. Installed package was `com.DefaultCompany.mergeshelter`.
- Launch result: passed with `adb shell monkey -p com.DefaultCompany.mergeshelter 1`; foreground was verified as `com.DefaultCompany.mergeshelter/com.unity3d.player.UnityPlayerGameActivity`.
- Manual smoke result: partial pass with blockers. Launch, board visibility, tile placement, Start Wave, Claim Reward, Next Level, Daily Reward claim, Daily Reward claimed state, Daily Quest progress, Daily Quest claim rewards, insufficient-coin upgrade block, enough-coin shelter upgrade, Reward Double visibility after pending victory reward, Reward Double one-use doubling, Level 10 weak-board defeat, and first Revive-to-playable-state were validated on device.
- Save/load result: passed. After force-stopping and relaunching only Merge Shelter, coins/parts, selected/unlocked level, shelter upgrade level, daily reward claimed state, and daily quest claimed state persisted.
- Reset Save result: passed. Reset Save returned the app to new-player Level 1 with coins/parts reset, Shelter Lv 1, daily reward available, quests at 0 progress, and an empty board.
- Remaining known issues: severe HUD text overlap/stacking appears on the physical phone after repeated progression updates; the Revive button remained visible after a successful revive; tapping that stale Revive button a second time left the app focused but rendering a persistent black screen. Local failure log captured at `android-device-smoke-logcat.txt`; Merge Shelter/Unity log inspection showed no obvious Java fatal exception, but the app remained visually unusable after relaunch/refocus.
- P0/P1 blockers: P1 blocker for Android physical-device smoke completion: stale Revive button plus second Revive tap caused a persistent black-screen app state and blocked Retry and strong-board Level 10 follow-up validation. P1 UI blocker: HUD/status text overlaps enough to make top-left progression/result text hard to read on SM-S918B.
- Security note: Testing used only the Merge Shelter package, game-only screenshots/input after foreground verification, approved system-overlay dismissal/refocus actions to return to the game, and did not access personal apps or device data.

## Post-Fix Android Physical Device Smoke Notes - 2026-05-25

- Device: Samsung SM-S918B.
- Android version/API: Android 16 / API 36.
- APK filename: `merge-shelter-post-fix-device-debug.apk`.
- APK output path: `Builds/Android/merge-shelter-post-fix-device-debug.apk`.
- Install result: latest `main` APK installed successfully with `adb install -r`. A follow-up APK from fix branch `fix/android-hud-text-rendering` was built, but physical reinstall was blocked because the phone returned to `unauthorized` after Unity restarted ADB; user RSA prompt acceptance is required before continuing.
- Launch result: latest `main` APK launched successfully with `adb shell monkey -p com.DefaultCompany.mergeshelter 1`; foreground was verified as `com.DefaultCompany.mergeshelter/com.unity3d.player.UnityPlayerGameActivity`.
- Revive fix result: not fully rerun on device. Smoke was stopped before the revive path because HUD text rendering remained a P1 blocker on latest `main`.
- HUD layout fix result: partial fail on latest `main`. Initial launch layout was improved: top status text, centered board, bounded result area, and bottom buttons were visible and tappable. After repeated progression updates (upgrade blocked, daily reward, ten board taps, quest claim, shelter upgrade), wallet/result text still drew over itself on SM-S918B. A code fix was prepared on `fix/android-hud-text-rendering` to disable Unity UI Best Fit for HUD text and expand bounded wallet/result text areas; automated tests passed, physical verification remains pending device authorization.
- Full smoke result: partial fail/blocker. Launch, initial board visibility, board taps, upgrade blocked, daily reward one-use behavior by hidden button, daily quest progress, quest claim attempt, and button tapability were exercised before stopping on the HUD text rendering blocker.
- Save/load result: not rerun after the HUD blocker.
- Reset Save result: not rerun after the HUD blocker.
- EditMode tests: latest `main` passed 41/41 before APK build; `fix/android-hud-text-rendering` also passed 41/41.
- PlayMode tests: latest `main` passed 13/13 before APK build; `fix/android-hud-text-rendering` also passed 13/13.
- P0/P1 blockers: P1 remains on latest `main` for Android HUD text rendering after repeated progression updates. Branch retest blocker: device authorization is required before reinstalling and physically verifying `fix/android-hud-text-rendering`. No crash, freeze, or black screen was observed in this partial run.
- Security note: Testing used only the Merge Shelter package, game-only screenshots/input after foreground verification, and safe device metadata commands. No personal apps or device data were accessed.

## Severity Rules

| Severity | Definition |
|---|---|
| P0 | Crash, cannot launch, cannot play Level 1, data loss |
| P1 | Major gameplay bug, broken win/fail flow, serious UI blocker |
| P2 | Minor bug, unclear feedback, visual issue |
| P3 | Polish improvement |

## Sprint 1 Exit Gate
Sprint 1 cannot close until:
- All P0 issues are fixed.
- All P1 issues are fixed or explicitly accepted.
- QA-001 through QA-017 pass.
- No crash occurs during a 10-minute prototype loop.
