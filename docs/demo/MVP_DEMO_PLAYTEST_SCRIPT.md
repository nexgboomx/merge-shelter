# MVP Demo Playtest Script

## Purpose
Use this script to demo or playtest the current Merge Shelter MVP prototype in a consistent way. The path focuses on the first session, core board/merge/wave loop, retention surfaces, unlocked-level navigation, save/load, and reset behavior.

This is a prototype demo script. It is not a production store-review script.

## Setup Checklist
- [ ] Use Unity `6000.3.16f1` or a freshly built Android debug APK from that Unity version.
- [ ] Recommended APK path: `Builds/Android/merge-shelter-mvp-rc-debug.apk`.
- [ ] Recommended physical device baseline: Samsung SM-S918B, Android 16 / API 36, portrait.
- [ ] Confirm package id: `com.DefaultCompany.mergeshelter`.
- [ ] If using a phone, keep testing limited to Merge Shelter and do not access personal apps or personal data.
- [ ] Start from a fresh install or tap `Reset Save` in-game before the main demo path.

## Recommended Device And Build
- Android physical device in portrait orientation.
- Debug/development APK built from the current demo branch or latest approved `main`.
- Use the runbook at `docs/production/ANDROID_DEBUG_BUILD_RUNBOOK.md` for install, launch, and foreground verification.

## Demo Path

### 1. Fresh Start Or Reset Save
- Action: Launch Merge Shelter, then tap `Reset Save` if the app is not already in a fresh state.
- Expected observation: Level 1 appears, coins and parts are reset to 0, Shelter Lv 1 is shown, daily reward is available, quests are at 0 progress, and the board is empty.

### 2. Level 1 Tutorial
- Action: Read the first tutorial prompt.
- Expected observation: The prompt tells the player to tap an empty board cell. Level objective and HUD text are readable.

### 3. Place Tiles
- Action: Tap an empty board cell.
- Expected observation: A tile appears, tile placement feedback is shown, quest progress can update, and tutorial guidance advances.

### 4. Merge 3 Wood
- Action: Place three Wood tiles close enough to trigger a merge.
- Expected observation: The three matching Wood tiles merge into a stronger Wood tile. Feedback indicates merge success and the tutorial explains the next step.

### 5. Start Wave
- Action: Tap `Start Wave`.
- Expected observation: The wave resolves without a crash, freeze, black screen, or input lock. Victory should show victory/objective/reward language only.

### 6. Reward Double If Available
- Action: If `Reward Double` appears after victory, tap it once.
- Expected observation: Pending reward doubles once, feedback says the reward was doubled, and the double action is no longer available for the same result.
- Note: This is a mock rewarded-ad flow, not a production ad integration.

### 7. Claim Reward
- Action: Tap `Claim Reward`.
- Expected observation: Coins and parts increase, Level 2 unlocks, quest progress updates, and the prompt suggests Next Level or useful optional actions.

### 8. Daily Reward
- Action: Tap `Daily Reward`.
- Expected observation: Coins and parts increase by the displayed daily reward amount, the claimed state becomes readable, and a second claim is blocked or unavailable.
- Note: Daily Reward is local/session prototype behavior, not a real calendar reset.

### 9. Daily Quest
- Action: Review quest rows, then tap `Claim Quest` when a quest shows READY.
- Expected observation: Quest rows show title, progress, READY/claimed state, and reward amounts. Claiming grants coins/parts once and claimed quests no longer appear claimable.

### 10. Next Level
- Action: Tap `Next Level`.
- Expected observation: Level 2 starts cleanly, the board clears, objective and wave roster update, and Start Wave becomes the primary action after planning.

### 11. Levels 1-3 Progression
- Action: Continue through Levels 1-3 by placing tiles, merging where possible, starting waves, and claiming rewards.
- Expected observation: Level progression, reward claim, objectives, wave roster, decision prompts, and HUD readability remain stable.

### 12. Shelter Upgrade
- Action: Tap `Upgrade Shelter` when enough coins are available.
- Expected observation: If insufficient coins, feedback explains the missing amount. If affordable, coins are spent, Shelter Lv increases, and future waves start with higher max HP.

### 13. Level Navigation
- Action: Use `Previous Level`, `Replay`, and `Next Unlocked` when they are visible.
- Expected observation: Replay restarts the selected unlocked level, Previous returns to an earlier unlocked level, Next Unlocked starts the next unlocked level, and locked future levels are blocked in normal flow.
- Expected constraint: Pending reward should block navigation until the reward is claimed.

### 14. Save/Load Or Force-Stop/Relaunch
- Action: Make visible progress, then close or force-stop only Merge Shelter and relaunch it.
- Expected observation: Selected level, highest unlocked level, coins, parts, shelter upgrade level, daily reward state, quest state, and tutorial state persist.

### 15. Reset Save
- Action: Tap `Reset Save`.
- Expected observation: The app returns to new-player Level 1 with empty board, coins 0, parts 0, Shelter Lv 1, daily reward available, quests reset, first tutorial prompt, and locked navigation state.

## Optional Level 10 Retry/Revive Demo
- Action: Progress to Level 10 or use an approved QA/debug path if available. Start a wave with a deliberately weak or empty board.
- Expected observation: Defeat shows actionable player-facing hint text, not raw fail-reason constants.
- Action: Tap `Retry`.
- Expected observation: Level 10 restarts in a playable state.
- Action: Reach defeat again and tap `Revive`.
- Expected observation: Revive restores a playable state, hides immediately after use, cannot be used twice for the same defeat result, and no black screen occurs.
- Note: Physical reachability can vary depending on upgrades and route. Automated tests cover the Level 10 Retry/Revive regression.

## What Not To Demo As Production Features
- `Reward Double` and `Revive` are mock rewarded-ad flows.
- Daily Reward and Daily Quests are local/session prototype systems, not real calendar-backed live systems.
- Art and effects are placeholder/prototype polish, not final art.
- Android APK is a debug/development build, not production-signed.
- There is no backend, cloud save, real IAP, production ad SDK, Play Store submission, or store-ready AAB.

## Quick Troubleshooting
- Unity license/login issue: open Unity Hub, sign in, activate the license, open the project once, then rerun tests or build.
- Missing Android build tools: install Android Build Support, Android SDK and NDK Tools, and OpenJDK for Unity `6000.3.16f1`.
- Device unauthorized: unlock the phone, accept the USB debugging RSA prompt, then rerun `adb devices -l`.
- Install signature conflict: uninstall only `com.DefaultCompany.mergeshelter`, then reinstall the Merge Shelter APK.
- Black screen, crash, freeze, or launch failure: capture logcat only for Merge Shelter failure diagnosis and record the APK path, branch, commit, device model, Android version/API, and blocker severity.
- UI overlap or unreadable text: record the screen state, device model, Android version/API, level, action just taken, and whether the issue blocks core play.
