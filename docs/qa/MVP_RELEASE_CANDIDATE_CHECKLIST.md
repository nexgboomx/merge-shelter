# MVP Release Candidate Checklist

## Purpose And Scope
- [ ] Validate that the Merge Shelter MVP release candidate is installable, launchable, playable, readable, and stable on Android physical hardware.
  Expected result: the build has no P0/P1 blockers and is ready for MVP release-candidate review.
- [ ] Confirm this checklist covers only the local Unity prototype, Android debug APK, local save, mock rewarded flows, and current 30-level catalog.
  Expected result: no backend, cloud save, real calendar reset, real ad SDK, real IAP, final art, store submission, or production signing is tested as part of this RC.
- [ ] Confirm PR remains unmerged until QA sign-off is complete.
  Expected result: the release-candidate branch is reviewed and tested before merge.

## Test Environment
- Tester:
- Date:
- Branch:
- Commit:
- Machine OS:
- Unity version:
- Android SDK/ADB path:
- Device manufacturer/model:
- Android version/API:
- Orientation:
- Network state:
- Notes:

## Build And APK
- APK filename:
- APK output path:
- Package id: `com.DefaultCompany.mergeshelter`
- Build command or Unity menu path:
- Build log path:
- Build result: Pass / Fail / Blocked
- Build blocker:
- [ ] Fresh Android debug APK was built from the target branch.
  Expected result: APK exists at the documented output path.
- [ ] APK is a debug/development build, not a production-signed store build.
  Expected result: production signing and store submission remain out of scope.
- [ ] Final MVP RC APK path is recorded in Sprint 10 QA notes.
  Expected result: another tester can locate the exact APK that was smoked.

## Automated Tests
- EditMode result:
- EditMode result file:
- PlayMode result:
- PlayMode result file:
- Automated test blocker:
- [ ] EditMode tests pass.
  Expected result: all catalog, economy, board, save, visual kit, and support tests pass.
- [ ] PlayMode tests pass.
  Expected result: all scene, tutorial, retention, reward, revive, navigation, layout, save/load, and reset tests pass.
- [ ] 30-level catalog tests pass.
  Expected result: exactly 30 sequential levels exist, Level 1 remains tutorial-ready, Level 10 remains useful for Retry/Revive regression, and Level 30 remains beatable by the strong-board assumption.
- [ ] Visual kit tests pass.
  Expected result: opaque background, readable colors, tile styles, button states, and feedback colors remain valid.

## Android Physical Device Security Boundary
- [ ] Testing is limited to the Merge Shelter app.
  Expected result: no personal apps or personal data are opened, inspected, captured, pulled, or modified.
- [ ] Before screenshots or input, foreground is verified as `com.DefaultCompany.mergeshelter`.
  Expected result: screenshots/input are used only while Merge Shelter is foreground.
- [ ] Do not open banking, wallet, crypto, email, SMS, contacts, gallery, files, browser, settings, notification content, or personal apps.
  Expected result: privacy boundary is preserved.
- [ ] Do not use `adb pull`, browse storage, read photos/downloads/documents/contacts/SMS/call logs/clipboard/accounts/app data, use backup, root, fastboot, recovery, bootloader, or privileged operations.
  Expected result: only approved Merge Shelter QA commands are used.
- [ ] Use `adb logcat -d` only if Merge Shelter crashes, freezes, or fails to launch.
  Expected result: logs are captured only for app failure diagnosis.

## Install And Launch
- Install result: Pass / Fail / Blocked
- Launch result: Pass / Fail / Blocked
- Foreground verification result:
- Install/launch blocker:
- [ ] `adb devices -l` shows an authorized Android target.
  Expected result: target device is visible before install.
- [ ] APK installs with `adb install -r <APK>`.
  Expected result: install succeeds without uninstalling any package except `com.DefaultCompany.mergeshelter` if needed for signature/package conflict.
- [ ] Merge Shelter launches with `adb shell monkey -p com.DefaultCompany.mergeshelter 1`.
  Expected result: app opens without crash, freeze, launcher fallback, or black screen.
- [ ] Foreground verification shows `com.DefaultCompany.mergeshelter/com.unity3d.player.UnityPlayerGameActivity`.
  Expected result: app is confirmed foreground before screenshots/input.
- [ ] Opaque background clears screen on launch.
  Expected result: no ghosting, stale text, or transparent background artifacts.

## First-Session Tutorial Flow
- Result: Pass / Fail / Blocked
- Blocker:
- [ ] Reset Save or fresh install starts at Level 1.
  Expected result: Level 1, zero coins/parts, Shelter Lv 1, daily reward available, quests at 0 progress, empty board.
- [ ] First tutorial prompt tells player to tap an empty board cell.
  Expected result: tutorial copy is readable and does not overlap other HUD text.
- [ ] First tile placement advances tutorial.
  Expected result: tile appears, feedback is visible, tutorial moves to placing more tiles.
- [ ] Additional tile placements advance merge intent.
  Expected result: tutorial explains merging 3 matching tiles.
- [ ] Start Wave guidance appears when enough tutorial progress is made.
  Expected result: Start Wave is visible, primary, and tappable.
- [ ] Victory and reward guidance appear after the first wave.
  Expected result: Claim Reward is visible, primary, and reward copy is readable.
- [ ] Claim Reward advances to Next Level or optional daily/upgrade guidance.
  Expected result: first-session flow remains understandable.

## Core Board/Merge/Wave Loop
- Result: Pass / Fail / Blocked
- Blocker:
- [ ] Tapping an empty board cell places the next tile.
  Expected result: tile label/color appears in the tapped cell.
- [ ] Tapping an occupied cell is blocked with actionable guidance.
  Expected result: player sees clear invalid-placement feedback.
- [ ] Three matching adjacent tiles merge.
  Expected result: merged tile appears at the expected tier and feedback indicates merge success.
- [ ] Start Wave resolves the current board.
  Expected result: wave result appears without crash, freeze, black screen, or input lock.
- [ ] Victory creates a pending reward.
  Expected result: Claim Reward and any available Reward Double action appear.
- [ ] Defeat creates Retry/Revive options when applicable.
  Expected result: Retry and Revive appear only in valid defeat states.

## Levels 1-3 Progression
- Result: Pass / Fail / Blocked
- Blocker:
- [ ] Level 1 can be completed and reward claimed.
  Expected result: Level 2 unlocks.
- [ ] Next Level starts Level 2 cleanly.
  Expected result: board clears, Level 2 objective/wave roster display, and Start Wave is primary.
- [ ] Level 2 can be completed and reward claimed.
  Expected result: Level 3 unlocks.
- [ ] Next Level starts Level 3 cleanly.
  Expected result: Level 3 displays readable objective, prompt, roster, and actions.
- [ ] Currency and shelter upgrade state remain coherent through Levels 1-3.
  Expected result: coins/parts increase from rewards and upgrade availability is readable.

## 30-Level Catalog Validation
- Result: Pass / Fail / Blocked
- Blocker:
- [ ] Automated catalog tests pass.
  Expected result: catalog has exactly 30 levels with sequential LevelIds 1-30.
- [ ] Level bands remain valid.
  Expected result: Levels 1-5 are forgiving, 6-10 require basic merge planning, 11-20 stress upgrade/resource planning, and 21-30 apply late prototype pressure.
- [ ] Level 30 exists in automated validation.
  Expected result: Level 30 is present and beatable by the late strong-board assumption.

## Level 10 Retry/Revive Regression
- Result: Pass / Fail / Blocked
- Blocker:
- [ ] Automated Level 10 Retry/Revive tests pass.
  Expected result: Level 10 remains useful for defeat, Retry, Revive, stale click, and no-black-screen regression.
- [ ] Physical device reaches a defeat path on Level 10 or another reliable current-level defeat path if available.
  Expected result: defeat message is readable and actionable.
- [ ] Retry returns to a playable state.
  Expected result: same selected level restarts with fresh board and Start Wave available.
- [ ] Revive appears only after defeat.
  Expected result: Revive is unavailable before defeat and after already being used for the same result.
- [ ] Revive hides after successful use.
  Expected result: Revive button is hidden/disabled immediately and Start Wave returns.
- [ ] A stale Revive tap does not corrupt state.
  Expected result: no black screen, no scene loss, no duplicate revive, no input lock.

## Reward Double Regression
- Result: Pass / Fail / Blocked
- Blocker:
- [ ] Reward Double appears only after victory with pending reward.
  Expected result: no Double Reward button before pending victory reward.
- [ ] Reward Double doubles pending reward once.
  Expected result: pending coins/parts update once.
- [ ] Reward Double cannot be used twice for the same result.
  Expected result: second attempt is blocked or button is unavailable.
- [ ] Claim Reward after Double Reward grants the doubled amount.
  Expected result: wallet updates correctly and pending reward clears.

## Daily Reward
- Result: Pass / Fail / Blocked
- Blocker:
- [ ] Daily Reward available state is readable.
  Expected result: daily reward amount and available state are visible.
- [ ] Daily Reward claim works once.
  Expected result: coins/parts increase by the displayed daily reward amount.
- [ ] Daily Reward claimed state is readable.
  Expected result: claimed state remains visible and does not look broken.
- [ ] Daily Reward cannot be double-claimed in the same local/session flow.
  Expected result: second claim is unavailable or blocked with clear feedback.

## Daily Quests
- Result: Pass / Fail / Blocked
- Blocker:
- [ ] Quest text shows each quest title and progress.
  Expected result: Place 10 Tiles, Complete 1 Level, and Claim 1 Reward are readable.
- [ ] Quest progress updates after relevant actions.
  Expected result: tile placement, level completion, and reward claim update matching quest rows.
- [ ] READY state is obvious.
  Expected result: completed unclaimed quests show READY.
- [ ] Claim Quest grants coins/parts once.
  Expected result: wallet increases by the displayed quest reward and claimed state appears.
- [ ] Claimed quests are not shown as claimable.
  Expected result: claimed rows show claimed and cannot be claimed again.
- [ ] Blocked claim gives clear feedback.
  Expected result: when no quest is ready, the app explains why claim is blocked.

## Level Navigation
- Result: Pass / Fail / Blocked
- Blocker:
- [ ] Level 1 has no Previous Level button in new-player state.
  Expected result: player cannot navigate below Level 1.
- [ ] Level 1 has no Next Unlocked button until Level 2 is unlocked.
  Expected result: locked future levels are not exposed in normal flow.
- [ ] Replay current level works.
  Expected result: current selected level restarts cleanly and board state is cleared.
- [ ] Previous Level works after Level 2 is unlocked and selected.
  Expected result: selecting Previous starts Level 1 cleanly.
- [ ] Next Unlocked works from an earlier selected unlocked level.
  Expected result: selecting Next Unlocked starts Level 2 or the next available unlocked level cleanly.
- [ ] Locked future levels remain blocked.
  Expected result: no normal player action starts a level above highest unlocked.
- [ ] Pending reward blocks level navigation.
  Expected result: navigation controls hide or block until reward is claimed.
- [ ] Selected level persists through save/load and force-stop/relaunch.
  Expected result: app resumes on the selected level after relaunch.

## Save/Load And Force-Stop/Relaunch
- Result: Pass / Fail / Blocked
- Blocker:
- [ ] Make progress before relaunch.
  Expected result: selected level, highest unlocked level, coins/parts, shelter upgrade, daily reward, daily quest, and tutorial state change from fresh values.
- [ ] Force-stop only Merge Shelter with `adb shell am force-stop com.DefaultCompany.mergeshelter`.
  Expected result: no other apps or device settings are touched.
- [ ] Relaunch only Merge Shelter.
  Expected result: app opens normally and foreground is verified.
- [ ] Progress persists after relaunch.
  Expected result: selected/current level, highest unlocked level, coins/parts, shelter upgrade level, daily reward state, quest progress/claimed state, and tutorial state match pre-relaunch state.
- [ ] Return-session UI is understandable.
  Expected result: player can tell current level, resources, quest/daily state, objective, wave roster, and next useful action.

## Reset Save
- Result: Pass / Fail / Blocked
- Blocker:
- [ ] Reset Save button is visible and tappable.
  Expected result: button remains inside safe Android portrait layout.
- [ ] Reset Save returns to new-player Level 1.
  Expected result: Level 1, highest unlocked Level 1, selected Level 1, empty board, and first tutorial prompt.
- [ ] Currency and shelter upgrade reset.
  Expected result: coins 0, parts 0, Shelter Lv 1, base HP.
- [ ] Daily reward and quests reset.
  Expected result: daily reward available, all quest progress 0, no claimed quest state.
- [ ] Navigation resets.
  Expected result: Replay is available, Previous Level and Next Unlocked are hidden.

## UI/HUD Readability
- Result: Pass / Fail / Blocked
- Blocker:
- [ ] Objective text is readable.
  Expected result: Goal line is visible and not overlapped.
- [ ] Wave roster is readable.
  Expected result: enemies use readable display names, duplicates are grouped, and raw IDs are not shown.
- [ ] Decision prompts are readable.
  Expected result: build, reward, upgrade, defeat, and navigation prompts fit inside result/status area.
- [ ] Quest text is readable.
  Expected result: quest titles, progress, READY, claimed, and rewards are scannable.
- [ ] Action buttons are tappable.
  Expected result: primary and secondary buttons do not overlap and remain inside the action panel.
- [ ] Board remains visible and centered.
  Expected result: board is not covered by HUD or action buttons.
- [ ] No critical text overlap.
  Expected result: level, tutorial, HP, wallet, rewards, quests, roster, result, and actions stay separated.
- [ ] No ghosting/redraw regression.
  Expected result: text does not stack, smear, or leave stale copies after repeated state changes.
- [ ] No black screen.
  Expected result: app remains visually usable through launch, progression, revive/retry, relaunch, and reset.

## Severity Definitions
| Severity | Definition | Release Candidate Action |
|---|---|---|
| P0 | Crash, cannot launch, black screen, cannot play Level 1, save corruption, reset broken, or privacy boundary violated. | Must fix before RC sign-off. |
| P1 | Main progression blocked, Start Wave/Claim Reward/Next Level broken, tutorial broken, severe HUD overlap, major reward/revive/upgrade regression. | Must fix or explicitly defer with owner approval before RC sign-off. |
| P2 | Playable but unclear copy, minor layout/readability issue, prompt missing in a non-critical state, minor economy clarity issue. | Document and triage; may ship if accepted. |
| P3 | Cosmetic issue, polish request, documentation improvement, non-blocking enhancement. | Document as backlog or known limitation. |

## Known Limitations
- [ ] Daily reward is local/session-only and does not use real calendar reset.
- [ ] Daily quests are local/session-only and do not use backend or live service reset.
- [ ] Reward Double and Revive are mock rewarded-ad flows with no real ad SDK.
- [ ] IAP, backend, cloud save, push notifications, and store submission are out of scope.
- [ ] Visuals are placeholder/prototype art, not final art.
- [ ] Android build is debug/development and not production-signed.
- [ ] Physical Revive validation may require a reliable defeat path if early unlocked levels resolve as victories.
- Additional known limitations:

## Final Sign-Off
- Overall RC result: Pass / Fail / Blocked
- P0 blockers:
- P1 blockers:
- P2 issues accepted:
- P3 issues/backlog:
- Final MVP RC APK path:
- Final test result files:
- Final Android device:
- Final Android version/API:
- Security boundary confirmed: Yes / No
- Tester sign-off:
- Date:
- Release owner sign-off:
- Date:
- Notes:
