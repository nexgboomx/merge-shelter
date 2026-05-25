# Sprint 5 Mobile Vertical Slice Polish Plan

## Sprint Goal
Make the Android prototype understandable and satisfying in the first minute on a portrait phone.

Sprint 5 is a polish sprint for the existing vertical slice. It should improve mobile UI hierarchy, first-run tutorial clarity, and simple game feel feedback without expanding the game into final art, real monetization, backend services, or store submission work.

## Current Status After Sprints 1-4
- Sprint 1 delivered the playable board and wave prototype foundation.
- Sprint 2 added progression, reward claim, level unlock, retry, and shelter upgrades.
- Sprint 3 added daily reward, daily quests, rewarded mock double reward, rewarded mock revive, and debug analytics hooks.
- Sprint 4 added local JSON save/load/reset and Android debug APK packaging.
- Latest validation baseline: EditMode tests pass 41/41 and PlayMode tests pass 13/13.
- Current package id for Android smoke is `com.DefaultCompany.mergeshelter`.

## Android Physical Device Validation Status
- Primary physical target tested: Samsung SM-S918B, Android 16 / API 36, portrait.
- Post-fix Android smoke passed for launch, board interaction, Start Wave, rewards, progression, Daily Reward, Daily Quest, shelter upgrade, Reward Double, Retry, Revive, save/load, Reset Save, and HUD readability.
- Previous Sprint 4 P1 blockers were stale Revive causing black-screen state and HUD text ghosting/overlap after repeated progression updates.
- Those blockers were fixed and verified on the physical device before Sprint 5 planning.
- Sprint 5 must preserve the SM-S918B portrait readability and run a fresh Android device smoke before sign-off.

## Target Player Experience
1. New player launches the Android prototype and immediately understands the current level, shelter HP, next tile, wallet, quest status, and primary action.
2. First-run tutorial points the player to the board, tile placement, merging, Start Wave, reward claim, and next level flow.
3. Board taps, blocked actions, successful actions, victory, defeat, rewards, upgrades, revive, and reset provide visible feedback.
4. Bottom actions remain tappable without overlap on portrait phone screens.
5. The board stays centered and readable while top status and bottom actions remain stable.
6. The first minute feels like a coherent mobile game prototype rather than a debug panel.

## Sprint 5 Definition of Done
- Mobile UI hierarchy is simplified into clear top status, centered board, bounded result/tutorial messaging, and bottom actions.
- First-run tutorial exists for Level 1 and guides the player through the first complete loop.
- Core actions have simple visible feedback.
- Android portrait layout remains readable and tappable on Samsung SM-S918B.
- PlayMode tests cover the expected UI elements, action availability, tutorial flow basics, and layout stability.
- Android physical device smoke passes with no P0/P1 blockers.
- QA notes document test target, APK, results, blockers, and known limitations.

## Workstreams

### Project Manager
- Convert this plan into GitHub issues with clear acceptance criteria.
- Keep Sprint 5 scoped to mobile vertical-slice polish.
- Track exit criteria and Android smoke readiness.
- Decide whether remaining P2/P3 polish issues can move to Sprint 6.

### UI/UX
- Define the portrait phone hierarchy for level/status, board, tutorial/result text, and action buttons.
- Improve labels and button grouping so the primary next action is obvious.
- Keep tap targets large enough for phone use.
- Ensure text wraps or clamps safely without overlapping critical UI.

### Game Design
- Write the first-run tutorial steps and expected player sequence for Level 1.
- Clarify tutorial copy for tile placement, merging intent, Start Wave, reward claim, and Next Level.
- Identify feedback moments that teach the loop without adding new systems.
- Confirm Level 1 completion remains achievable during the tutorial.

### Code
- Implement only the UI, tutorial, and simple feedback hooks needed for this sprint.
- Keep the existing board, progression, save, rewarded mock, and build systems intact.
- Add or update PlayMode coverage for the tutorial and mobile layout.
- Avoid unrelated refactors.

### Game Feel
- Add simple prototype feedback for key actions: tile placed, invalid action, merge success, wave start, damage, victory, defeat, reward claim, upgrade, reward double, revive, and reset.
- Prefer lightweight UI color, text, scale, or timing feedback over final animation/VFX work.
- Keep effects readable and non-blocking on Android portrait.

### QA
- Update the smoke checklist for Sprint 5.
- Run EditMode and PlayMode tests before APK build.
- Build and install a fresh Android debug APK.
- Run physical device smoke on Samsung SM-S918B portrait.
- Record P0/P1 blockers and security-safe device metadata only.

## Scope

### Mobile UI Hierarchy
- Top panel for level name, tutorial/status, shelter HP, next tile, wallet, and quest summary.
- Middle board that stays centered and visually dominant.
- Bounded result/tutorial message area.
- Bottom action button grid for primary and secondary actions.

### First-Run Tutorial
- Level 1 tutorial sequence for placing a tile, understanding merge intent, starting a wave, claiming reward, and moving to the next level.
- Tutorial should use clear, short text and visible state changes.
- Tutorial should not require final art or a new narrative system.

### Simple Game Feel Feedback
- Immediate visible response for successful taps.
- Clear blocked-action feedback.
- Visible state feedback for win, loss, reward, revive, and upgrade.
- Lightweight prototype implementation only.

### Readable/Tappable Android Layout
- Samsung SM-S918B portrait remains the main device target.
- Buttons do not overlap each other.
- HUD text does not overlap the board or action buttons.
- Result text wraps or clamps safely.

### PlayMode Tests
- Required UI elements exist.
- Expected buttons are active and tappable when their state is valid.
- Tutorial flow reaches Level 1 completion.
- Layout survives reward claim, quest claim, upgrade, reward double, revive, save/load, and reset.

### Android Device Smoke
- Fresh debug APK is built from the Sprint 5 branch.
- APK installs and launches on the physical device.
- Manual smoke validates first-minute comprehension, core flow, feedback, save/load, reset, and no P0/P1 blockers.

## Out of Scope
- Final art.
- Real ads SDK.
- Real IAP.
- Backend.
- Cloud save.
- Store submission.
- Account login.
- Push notifications.
- Full economy rebalance.
- New content beyond tutorial support for the existing vertical slice.

## Exit Criteria
- New player can understand what to tap in the first minute.
- Level 1 can be completed through the tutorial.
- Core actions have visible feedback.
- Samsung SM-S918B portrait remains readable.
- Action buttons remain tappable and do not block play.
- EditMode tests pass.
- PlayMode tests pass.
- Android device smoke has no P0/P1 blockers.
- QA notes are updated with Sprint 5 Android smoke results.
