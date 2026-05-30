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
- Branch/APK tested: `fix/android-hud-text-rendering` after adding an opaque canvas background behind the prototype UI to clear Android text redraws.
- Install result: passed with `adb install -r` after the phone was re-authorized.
- Launch result: passed with `adb shell monkey -p com.DefaultCompany.mergeshelter 1`; foreground was verified as `com.DefaultCompany.mergeshelter/com.unity3d.player.UnityPlayerGameActivity`.
- Revive fix result: passed. Level 10 weak-board defeat showed Revive and Retry; Retry returned to a playable Level 10 state. Revive returned to a playable Level 10 state, hid the Revive button immediately, and a stale second tap at the old Revive position did not change state or cause a black screen. A later defeat showed Retry only, with Revive unavailable.
- HUD layout/rendering fix result: passed on SM-S918B portrait. Launch, repeated progression updates, reward claim, Next Level, save/load relaunch, Reset Save, Level 10 defeat, Retry, Revive, stale Revive tap, and final Reset Save all kept HUD text readable without the previous text ghosting/stacking.
- Full smoke result: passed on the branch APK. Validated app launch, board visibility, board tap placement, Start Wave, Claim Reward, Next Level, Retry, Daily Reward one-use behavior, Daily Quest progress and claim, insufficient-coin upgrade block, successful shelter upgrade, Reward Double one-use behavior, Revive one-use behavior, HUD readability, and tappable action buttons.
- Save/load result: passed. After force-stopping and relaunching only Merge Shelter, Level 2, coins/parts, Shelter Lv 2, daily reward claimed state, and daily quest claimed/ready states persisted.
- Reset Save result: passed. Reset Save returned to new-player Level 1 with coins/parts reset to 0, Shelter Lv 1, daily reward available, quests at 0 progress, empty board, and clean HUD rendering.
- EditMode tests: passed 41/41 (`Logs/EditModeResults-android-hud-background.xml`).
- PlayMode tests: passed 13/13 (`Logs/PlayModeResults-android-hud-background.xml`).
- Remaining known issues: no device smoke blocker observed on the branch APK. The branch still needs review/merge before the post-fix result applies to `main`.
- P0/P1 blockers: none observed on `fix/android-hud-text-rendering` during this physical-device run.
- Security note: Testing used only the Merge Shelter package, game-only screenshots/input after foreground verification, approved force-stop/relaunch of Merge Shelter, and safe device metadata commands. No personal apps or device data were accessed.

## Sprint 5 Android Physical Device Smoke Notes - 2026-05-25

- Machine OS: Ubuntu 24.04.3 LTS, Linux 6.17.0-1017-oem x86_64.
- Unity version: 6000.3.16f1 (a56f230f6470).
- Device manufacturer/model: samsung SM-S918B.
- Android version/API: Android 16 / API 36.
- APK filename: `merge-shelter-sprint5-polish-device-debug.apk`.
- APK output path: `Builds/Android/merge-shelter-sprint5-polish-device-debug.apk`.
- EditMode tests: passed 41/41 before device install and after the HUD save/load refresh fix (`Logs/EditModeResults-sprint5-android-smoke.xml`, `Logs/EditModeResults-sprint5-smoke-hudfix.xml`).
- PlayMode tests: passed 17/17 before device install and after the HUD save/load refresh fix (`Logs/PlayModeResults-sprint5-android-smoke.xml`, `Logs/PlayModeResults-sprint5-smoke-hudfix.xml`).
- Install result: passed with `adb install -r` using Unity's embedded adb at `/home/phung-truong/Unity/Hub/Editor/6000.3.16f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb`. The fixed APK was rebuilt and reinstalled after code fix `c91a5cb`.
- Launch result: passed with `adb shell monkey -p com.DefaultCompany.mergeshelter 1`; foreground was verified as `com.DefaultCompany.mergeshelter/com.unity3d.player.UnityPlayerGameActivity`.
- First-run tutorial result: passed. Reset/new-player state showed first-tile guidance. Board tap advanced to place-more-tiles guidance, a second tile advanced to merge-intent guidance, a third matching tile merged and advanced to Start Wave guidance, victory advanced to Claim Reward guidance, Claim Reward advanced to Next Level/optional reward guidance, and daily reward usage moved the tutorial out of first-run state. Final Reset Save restarted the first tutorial step.
- Gameplay feedback result: passed for tile placement (`TILE:`), merge success (`MERGE:`), invalid placement (`BLOCKED:`), wave victory (`WIN:`), wave defeat (`DEFEAT:`), reward claim (`REWARD:`), daily reward claim (`DAILY:`), quest claim (`QUEST:`), shelter upgrade (`UPGRADE:`), reward double (`DOUBLE:`), revive (`REVIVE:`), and reset save (`RESET:`). Wave start action was tappable and resolved correctly; its text feedback is transient because the prototype resolves waves immediately and is covered by PlayMode feedback state assertions.
- UI/HUD result: passed after code fix `c91a5cb`. Portrait HUD remained readable on SM-S918B with no observed text overlap, ghosting, or blocked buttons across tutorial, reward, quest, upgrade, save/load, defeat, revive, stale revive tap, and reset flows. Initial smoke found a HUD refresh regression after save/load where dynamic Sprint 5 labels showed fallback values; `PrototypeHudView.SetProgression()` now initializes dynamic text fields before applying saved progression state.
- Save/load result: passed after the HUD refresh fix. After making progress, upgrading shelter, claiming daily reward, claiming quests, force-stopping only Merge Shelter, and relaunching only Merge Shelter, Level 2, coins/parts, Shelter Lv 2, 125 HP, daily reward claimed state, and quest progress/claimed states displayed correctly.
- Reset Save result: passed. Reset Save returned to new-player Level 1 with coins/parts reset to 0, Shelter Lv 1, daily reward available, quests at 0 progress, empty board, first-run tutorial guidance, and clean HUD rendering.
- Revive regression result: passed. Level 10 empty-board defeat showed Revive and Retry, Revive returned to a playable Level 10 state, hid Revive, restored HP to 100/100, and a stale tap at the old Revive location did not change state or produce a black screen.
- P0/P1 blockers: none after code fix `c91a5cb`. No crash/freeze occurred, so `adb logcat -d` was not run.
- Security note: Testing used only the Merge Shelter package, safe metadata commands, allowed install/launch/force-stop commands, foreground verification, and game-only screenshots/input after foreground verification. No personal apps, notifications, storage, or personal device data were opened, pulled, browsed, or inspected.

## Sprint 6 Android Physical Device Smoke Notes - 2026-05-27

- Machine OS: Ubuntu 24.04.3 LTS, Linux 6.17.0-1017-oem x86_64.
- Unity version: 6000.3.16f1 (a56f230f6470).
- Device manufacturer/model: samsung SM-S918B.
- Android version/API: Android 16 / API 36.
- APK filename: `merge-shelter-sprint6-tuning-device-debug.apk`.
- APK output path: `Builds/Android/merge-shelter-sprint6-tuning-device-debug.apk`.
- EditMode tests: passed 59/59 (`Logs/sprint6-editmode-results.xml`).
- PlayMode tests: passed 17/17 (`Logs/sprint6-playmode-results.xml`).
- Install result: passed with Unity embedded adb at `/home/phung-truong/Unity/Hub/Editor/6000.3.16f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb`.
- Launch result: passed with `adb shell monkey -p com.DefaultCompany.mergeshelter 1`; foreground was verified as `com.DefaultCompany.mergeshelter/com.unity3d.player.UnityPlayerGameActivity`.
- Level catalog result: passed. Automated tests validate the 30-level catalog, sequential LevelIds 1-30, broad pressure/reward curve, Level 1 tutorial support, Level 10 retry/revive regression coverage, and Level 30 strong-board beatability. Physical smoke progressed normally from Level 1 through Level 16.
- Economy/upgrade result: passed. Daily reward, all three daily quest claims, reward double, level rewards, and shelter upgrade UI worked on device. Shelter upgraded to Lv 2 and subsequent levels started with 125/125 HP; later rewards made the next upgrade affordable without breaking HUD layout.
- Tutorial regression result: passed. Reset/new-player state showed first-tile guidance; tile placement advanced tutorial copy, three Wood tiles merged, Start Wave guidance appeared, victory moved to Claim Reward guidance, Claim Reward unlocked the next-level guidance, and Reset Save restored the first tutorial step.
- Gameplay smoke result: passed. Validated launch, board visibility, board taps, merge feedback, Start Wave, victory, Claim Reward, Next Level, Daily Reward, Daily Quest claim, Reward Double, shelter upgrade, Retry, Revive, and normal progression through early and mid prototype levels.
- Level 10 Retry/Revive regression: passed in PlayMode tests for Level 10 specifically. On physical device, the same stale-revive regression path was validated on Level 16 after normal unlock progression because the upgraded shelter made empty Level 10 survivable; Revive hid immediately, stale Revive tap did not alter state or black-screen, a second defeat showed Retry only, and Retry returned to a playable state.
- Save/load result: passed. After force-stopping and relaunching only Merge Shelter, Level 4, coins/parts, Shelter Lv 2, daily claimed state, quest claimed states, and completed tutorial state persisted correctly.
- Reset Save result: passed. Reset Save returned the app to new-player Level 1 with coins/parts reset to 0, Shelter Lv 1, daily reward available, quests at 0 progress, empty board, first-run tutorial guidance, and readable HUD.
- UI/HUD result: passed on SM-S918B portrait. HUD text stayed readable with no observed ghosting or blocking overlap across tutorial, early progression, reward/quest/daily/upgrade actions, save/load, mid-level progression, defeat, revive, stale tap, retry, and reset.
- P0/P1 blockers: none observed. No crash, freeze, launch failure, install failure, or Android black-screen regression occurred, so `adb logcat -d` was not run.
- Security note: Testing used only the Merge Shelter package, safe device metadata commands, allowed install/launch/force-stop commands, foreground verification, and game-only screenshots/input after foreground verification. No personal apps, notifications, storage, or personal device data were opened, pulled, browsed, or inspected.

## Sprint 7 Android Physical Device Smoke Notes - 2026-05-30

- Machine OS: Ubuntu 24.04.3 LTS, Linux 6.17.0-1017-oem x86_64.
- Unity version: 6000.3.16f1 (a56f230f6470).
- Device manufacturer/model: samsung SM-S918B.
- Android version/API: Android 16 / API 36.
- APK filename: `merge-shelter-sprint7-visual-polish-device-debug.apk`.
- APK output path: `Builds/Android/merge-shelter-sprint7-visual-polish-device-debug.apk`.
- EditMode tests: passed 62/62 (`TestResults/editmode-sprint7.xml`).
- PlayMode tests: passed 17/17 (`TestResults/playmode-sprint7.xml`).
- Install result: passed with `adb install -r` using Unity's embedded adb at `/home/phung-truong/Unity/Hub/Editor/6000.3.16f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb`.
- Launch result: passed with `adb shell monkey -p com.DefaultCompany.mergeshelter 1`; foreground was verified as `com.DefaultCompany.mergeshelter/com.unity3d.player.UnityPlayerGameActivity`.
- Visual identity result: passed. Opaque dark canvas background renders cleanly with no black-screen regression. Board panel has a visible border against the canvas background. Board cells have visible individual borders. Tile type colors are distinct and readable: Wood (brown), Metal (gray-blue), Food (green), Energy (blue) confirmed through placement. Empty cells show "+" placeholder in readable contrast. Tile labels show icon, short name, and tier (e.g., `[W] Wood T1`, `[W] Wood T2` after merge). The visual kit establishes a post-apocalyptic shelter palette with muted survival tones.
- Board/HUD/action polish result: passed. HUD section labels (SHELTER, REWARDS, QUESTS, BOARD, ACTIONS) are styled in bold section text color and readable on SM-S918B portrait. Shelter HP shows healthy (green), upgrade status shows "can afford" (green) vs "need N more" (secondary). Wallet and daily reward text use resource/ready colors. Quest status is compact and readable with progress/ready/claimed states. Action button panel groups buttons clearly with a dark bounded background. Primary actions (Start Wave, Claim Reward, Next Level, Retry, Revive) are visually larger (220x52) with highlighted color and primary border. Secondary actions (Upgrade Shelter, Daily Reward, Reset Save) are smaller (200x44) with role-specific colors: reward (blue), upgrade (gold), quest (green), danger (red), reset (gray). Button colors match `PrototypeVisualKit` role assignments.
- Visual effects result: passed. Tile placement produced a short cell pulse with warm feedback color and 1.05x scale. Merge produced a stronger green pulse with 1.1x scale. Invalid tap on occupied cell produced a red/orange blocked flash with 0.96x scale (subtle shrink). Result panel showed feedback-colored tint on all tested actions: TILE (blue/ready), MERGE (green/victory), BLOCKED (red/defeat), WIN (green/victory), REWARD (green/victory), DAILY (green/victory), QUEST (green/victory), UPGRADE (green/victory), RESET (neutral/gray). Result text prefixes (TILE:, MERGE:, BLOCKED:, WIN:, REWARD:, DAILY:, QUEST:, UPGRADE:, RESET:) were readable. Effects cleared after a short duration without blocking subsequent taps. No input blocking observed during any effect.
- Tutorial regression result: passed. New-player Level 1 showed "tap an empty board cell to place your first tile." First tile advanced to "place two more tiles." Third tile triggered merge and advanced to "Tap Start Wave." Victory advanced to "Claim Reward." Claim Reward advanced to "tap Next Level." Daily Reward usage completed the tutorial. Reset Save returned to the first tutorial step.
- Gameplay/economy regression result: passed. Level 1 victory awarded 50 coins. Daily Reward claimed +75 coins, +1 parts. Shelter upgrade from Lv 1 to Lv 2 spent 100 coins. Level 2 started with 125/125 HP (upgrade applied). Quest claim awarded +60 coins, +1 parts for Complete 1 Level. Reward and quest currency grants accumulated correctly.
- Level 10 Retry/Revive regression: passed in automated PlayMode tests (17/17). PlayMode tests validate Level 10 empty-board defeat, Retry returning to playable Level 10, Revive returning to playable Level 10 with Revive button hidden, stale Revive tap causing no state change or black screen, and second defeat showing Retry only with Revive unavailable.
- Save/load result: passed. After Reset Save, force-stopping, and relaunching only Merge Shelter, Level 1 new-player state persisted correctly: Level 1, coins 0, parts 0, Shelter Lv 1, HP 100/100, daily reward available, quests at 0 progress, first-run tutorial guidance, and clean visual rendering.
- Reset Save result: passed. Reset Save returned the app to new-player Level 1 with `RESET: Save reset. Progress returned to Level 1.` feedback in neutral color. All state reset: coins/parts to 0, Shelter Lv 1, daily reward available, quests at 0, empty board, and first tutorial step.
- UI/HUD result: passed on SM-S918B portrait. HUD text stayed readable with no observed ghosting, overlap, or blocked buttons across tutorial, tile placement, merge, invalid tap, wave victory, reward claim, daily reward claim, quest claim, shelter upgrade, next level, save/load relaunch, and reset flows. Sprint 7 visual polish improved readability over Sprint 6 baseline while maintaining all layout safety constraints.
- P0/P1 blockers: none observed. No crash, freeze, launch failure, install failure, or Android black-screen regression occurred. `adb logcat -d` was not needed.
- Security note: Testing used only the Merge Shelter package, safe device metadata commands (`ro.product.manufacturer`, `ro.product.model`, `ro.build.version.release`, `ro.build.version.sdk`), allowed install/launch/force-stop commands, foreground verification before all screenshots and input, and game-only screenshots/input after foreground verification. No personal apps, notifications, storage, or personal device data were opened, pulled, browsed, or inspected.

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
