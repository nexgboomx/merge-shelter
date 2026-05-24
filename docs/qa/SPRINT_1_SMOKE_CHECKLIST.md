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
