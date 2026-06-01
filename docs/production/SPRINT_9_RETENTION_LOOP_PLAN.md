# Sprint 9 Retention Loop Plan

## Sprint Goal
Polish the return-session and retention loop so a player can come back to Merge Shelter, understand exactly where they are, see which daily actions matter, and continue testing the 30-level prototype without confusion. Sprint 9 should make the existing daily reward, daily quest, save/load, reward claim, and level progression surfaces clearer without adding backend systems or real calendar logic.

## Current Status After Sprint 8
- Sprint 1 delivered the playable board and wave prototype.
- Sprint 2 added progression, reward claim, level unlock, retry, and shelter upgrades.
- Sprint 3 added daily reward, daily quests, rewarded mock double/revive, and debug analytics.
- Sprint 4 added local JSON save/load/reset and Android debug APK packaging.
- Sprint 5 added mobile phone HUD hierarchy, first-run tutorial, gameplay feedback prefixes, and Android HUD safety fixes.
- Sprint 6 expanded the catalog to 30 levels, added balance simulation coverage, and tuned level rewards and upgrade costs.
- Sprint 7 added the placeholder visual identity, board/HUD/action polish, and lightweight visual feedback effects.
- Sprint 8 added level objectives, readable enemy labels, grouped wave rosters, player-facing defeat hints, board planning prompts, and reward/upgrade decision prompts.
- Current validation baseline after Sprint 8: EditMode tests pass 67/67 and PlayMode tests pass 17/17.
- Current package id: `com.DefaultCompany.mergeshelter`.
- The prototype is readable and playable on Samsung SM-S918B portrait, with no Sprint 8 P0/P1 blockers observed.

## Android Physical Device Validation Baseline
- Primary physical target: Samsung SM-S918B, Android 16 / API 36, portrait.
- Sprint 8 physical smoke passed install, launch, Level 1 tutorial, objective readability, enemy/wave roster readability, decision prompts, Levels 1-3 progression, Daily Reward, Daily Quest, Reward Double, Shelter Upgrade, Level 10 Retry/Revive regression, save/load, Reset Save, and HUD readability.
- Sprint 9 must preserve the Sprint 8 Android baseline and run a fresh physical smoke before sign-off.
- Security boundary remains unchanged: Android QA must test only the Merge Shelter app and must not access personal apps or personal device data.

## Target Player Experience

### Player Returns and Immediately Understands Current Progress
When the app resumes from a saved session, the player should quickly understand their current level, highest unlocked level, shelter upgrade level, HP, coins, parts, daily reward state, quest state, and next useful action. The resume state should feel intentional rather than like a debug reload.

### Daily Reward State Is Clear
The daily reward should clearly communicate whether it is available or already claimed. The reward amount should remain visible enough to explain why claiming it matters, and the claimed state should not look like a broken or missing button.

### Daily Quests Feel Readable and Useful
Daily quests should be easier to scan. Players should understand each quest objective, current progress, reward, ready-to-claim state, and claimed state without parsing a dense status string.

### Unlocked Level Navigation and Testing Is Easier
The player and QA should have a simple way to move among unlocked levels or validate later unlocked content after progression. This is not a full map screen; it is a prototype-safe path for choosing available levels, confirming unlocked state, and reducing repeated replay friction.

### Session Resume Does Not Feel Confusing
After force-stop/relaunch or normal app return, the UI should show a concise resume message and a clear primary action. The player should not need to remember what they were doing before closing the app.

## Retention/Session Loop Focus

### Daily Reward Polish
- Show available, claimed, and reward amount states clearly.
- Keep daily reward copy short and mobile-readable.
- Preserve the existing local/session reward behavior; do not add real calendar reset logic.
- Ensure daily reward feedback reinforces how the reward helps shelter upgrades.

### Daily Quest Readability
- Replace dense quest status text with clearer per-quest labels or compact rows if possible.
- Make progress, ready-to-claim, reward, and claimed states obvious.
- Keep quest actions tappable on Samsung SM-S918B portrait.
- Preserve existing quest definitions and reward behavior unless tests expose a real clarity issue.

### Quest Progress Feedback
- Surface quest progress changes after relevant actions, especially tile placement, level completion, and reward claim.
- When a quest becomes claimable, make the Claim Quest action and reward clear.
- Avoid blocking gameplay with modal instructions.

### Reward Claim Clarity
- After victory, make pending reward, claim action, next-level unlock, and upgrade opportunity easy to understand.
- After claiming a reward, highlight whether the best next action is Next Level, Upgrade Shelter, Daily Reward, or Quest Claim.
- Preserve Reward Double one-use behavior and existing rewarded mock scope.

### Level Navigation for Unlocked Levels / QA
- Add a simple prototype-safe navigation path for unlocked levels or QA validation if it fits the existing UI.
- Clearly communicate selected level and highest unlocked level.
- Prevent locked levels from being selected unless a deliberate QA/debug affordance is added and covered by tests.
- Keep the path lightweight; no final campaign map or art pass.

### Save/Load Return-Session Clarity
- On app launch with existing progress, show the current saved level/progress state clearly.
- Confirm currency, shelter upgrade, daily reward, quest, tutorial, and selected level state persist.
- Reset Save must remain obvious and must still return the app to new-player Level 1.

## Workstreams

### Project Manager
- Convert this plan into focused GitHub issues with acceptance criteria.
- Keep Sprint 9 scoped to retention/session clarity and prototype-safe level navigation.
- Prevent backend, cloud save, real daily timer, monetization SDK, and store-submission scope from entering the sprint.
- Track dependencies between UI clarity, save/load behavior, tests, Android build, and physical smoke.

### Game Design
- Define the desired return-session state and primary-action priority.
- Review daily reward and quest copy for clarity and reward motivation.
- Define simple unlocked-level navigation rules.
- Specify when reward claim should point players toward upgrade, quest, daily reward, or next level.
- Keep copy short enough for portrait phone readability.

### Code
- Improve daily reward and daily quest display using existing systems.
- Add a lightweight unlocked-level navigation or QA-safe level selection path if needed.
- Improve save/load resume messaging without changing save file ownership or adding cloud behavior.
- Preserve tutorial, objectives, wave roster, reward double, revive, retry, economy, and 30-level catalog behavior.
- Add or update EditMode and PlayMode tests for retention/session clarity.
- Keep implementation simple and data-driven.

### UI/UX
- Make daily reward, quest progress, claimable states, and claimed states easier to scan.
- Keep the primary action visually obvious after resume, reward claim, quest claim, upgrade, defeat, and reset.
- Maintain the Sprint 7/8 mobile layout hierarchy and Android HUD safety constraints.
- Verify no text overlap, ghosting, or blocked buttons on Samsung SM-S918B portrait.

### QA
- Extend smoke coverage for daily reward state, quest readability, reward claim clarity, unlocked-level navigation, and resume state.
- Run EditMode and PlayMode tests after changes.
- Build a fresh Android debug APK from Sprint 9.
- Run physical Android smoke on Samsung SM-S918B portrait.
- Add Sprint 9 QA notes to `docs/qa/SPRINT_1_SMOKE_CHECKLIST.md`.

## Definition of Done
- Daily reward state is readable in available and claimed states.
- Daily quests are readable and claimable state is obvious.
- Quest progress feedback is visible after relevant actions.
- Reward claim messaging clearly explains reward, unlock, and next useful action.
- Session resume clearly shows level, progress, currency, shelter upgrade, daily reward, quest state, and primary next action.
- Level navigation for unlocked levels or a QA-safe level path exists.
- Existing first-run tutorial still works.
- 30-level catalog still works.
- Save/load, Reset Save, Reward Double, Revive, Retry, Shelter Upgrade, Daily Reward, and Daily Quest remain stable.
- EditMode tests pass.
- PlayMode tests pass.
- Android physical smoke passes with no P0/P1 blockers.

## Out of Scope
- Backend.
- Cloud save.
- Real calendar-based daily reset.
- Real ad SDK.
- Real IAP.
- Final art.
- Store submission.
- Push notifications.
- LiveOps event calendar.
- Localization infrastructure.
- Full campaign map or final level-select art.

## Exit Criteria
- Sprint 9 QA notes are added to `docs/qa/SPRINT_1_SMOKE_CHECKLIST.md`.
- No P0/P1 blockers remain.
- EditMode tests pass.
- PlayMode tests pass.
- Android physical smoke passes on Samsung SM-S918B portrait.
- PR remains unmerged until QA passes.
