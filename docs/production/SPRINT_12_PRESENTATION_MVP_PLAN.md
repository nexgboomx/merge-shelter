# Sprint 12 Presentation MVP Plan

## Current Problem
Merge Shelter is technically stable enough for MVP debug RC handoff, but the Android portrait gameplay screen still reads like an internal debug/test harness. The core loop is playable and validated, yet the first-session screen needs stronger visual hierarchy, clearer grouping, and less debug-like presentation before it is comfortable for demo/playtest review.

The Sprint 11 post-fix RC baseline passed automated tests and targeted Android physical smoke with no P0/P1 blockers. Sprint 12 should preserve that stability while improving how the existing game looks and reads on a phone.

## Sprint Goal
Make the Android portrait game screen demo-ready by improving layout, readability, hierarchy, and presentation without adding new gameplay systems.

The player should be able to understand the first-session screen quickly: current goal, shelter state, board, next tile, wave/enemy context, quest/reward state, result prompt, and primary action should be visually organized and clearly tappable.

## Focus Areas

### Screen Layout
- Move from debug-list presentation toward a composed mobile game screen.
- Keep top information, board, result/status, and action areas visually distinct.
- Preserve a centered board and stable portrait layout on Samsung SM-S918B.
- Avoid large empty or ambiguous areas that make the screen feel unfinished.

### Visual Hierarchy
- Make the primary next action obvious in each state: Start Wave, Claim Reward, Next Level, Retry, or Revive.
- Deemphasize secondary actions without hiding necessary functionality.
- Use consistent section grouping so players can scan Shelter, Board, Rewards, Quests, and Actions.
- Reduce visual competition between tutorial copy, objective copy, result text, and resource state.

### Board Presentation
- Make the board look like the central game object, not a test grid.
- Improve board framing, cell contrast, and relationship to the HUD.
- Keep the board centered, readable, and tappable.
- Preserve all existing board size, placement, merge, and wave behavior.

### Tile Readability
- Keep Wood, Metal, Food, and Energy visually distinct.
- Improve tile labels/icons within the current placeholder-art constraints.
- Ensure tier labels remain readable on portrait phone screens.
- Preserve the existing tile generation, merge, and evaluator logic.

### Action Button Styling
- Make primary buttons visually stronger than secondary buttons.
- Keep reward, quest, upgrade, navigation, reset, retry, and revive buttons distinct by role.
- Avoid overlapping buttons on portrait phone screens.
- Keep touch targets comfortably tappable.

### Objective, Wave, And Quest Readability
- Keep level objective and wave roster player-facing and concise.
- Make quest progress, READY, and claimed states scannable.
- Avoid raw IDs, raw fail reasons, or test-like labels in normal player view.
- Keep enough information for QA without making the main screen feel like a debug console.

### Result Panel Presentation
- Make result/status text feel like a bounded game feedback panel.
- Keep victory, defeat, reward, quest, upgrade, navigation, and reset feedback readable.
- Preserve the PR #65 victory-copy fix: victory text must never include defeat wording or defeat hints.
- Avoid text overlap, ghosting, or long text spilling into buttons or board space.

### Separate Player-Facing UI From Debug/QA Information
- Hide or deemphasize debug-like data in normal view unless it directly helps the player.
- Keep package/build labels and QA-oriented details out of the primary gameplay hierarchy where possible.
- Retain QA usefulness through docs, tests, and existing validated flows rather than exposing internal state as primary UI.

## Must Preserve
- All gameplay logic.
- 30-level catalog.
- First-run tutorial.
- Level objectives.
- Wave roster.
- Daily Reward.
- Daily Quests.
- Reward Double mock.
- Revive mock.
- Retry behavior.
- Level navigation.
- Local save/load/reset.
- Android opaque background and no-ghosting safety.
- Existing EditMode and PlayMode tests.
- Sprint 11 final post-fix QA baseline and no-P0/P1 blocker status.

## Out Of Scope
- New gameplay systems.
- New level mechanics.
- Economy redesign.
- Final production art.
- Outsourced art.
- Real ads SDK.
- Real IAP.
- Backend.
- Cloud save.
- Production analytics.
- Production signing.
- Store submission.
- Play Store/AAB release packaging.

## Workstreams

### Project Manager
- Keep scope focused on presentation MVP only.
- Prevent new gameplay, monetization, backend, or content scope from entering Sprint 12.
- Track visual acceptance criteria and Android smoke readiness.

### UI/UX
- Define a cleaner portrait hierarchy for first-session gameplay.
- Prioritize player-facing clarity over debug visibility.
- Review text length, button placement, touch targets, and section grouping.

### Code
- Implement presentation improvements inside existing Unity UI systems.
- Reuse current `PrototypeVisualKit`, HUD, board view, and scene builder patterns.
- Avoid changes to combat math, progression, save format, level data, or economy unless required to fix a regression.

### QA
- Re-run EditMode and PlayMode tests after UI changes.
- Run Android physical smoke on Samsung SM-S918B portrait.
- Verify tutorial, core loop, reward/daily/quest, navigation, save/load/reset, reward double, revive, and victory-copy behavior remain stable.
- Record any P2/P3 presentation issues separately from P0/P1 blockers.

## Definition Of Done
- Android portrait screen is visually cleaner and less debug-like.
- First-session flow looks demo-ready enough for playtest review.
- Board remains centered, readable, and tappable.
- Objective, wave roster, quest, reward, and result text are readable.
- Action buttons are visually grouped, role-styled, and tappable.
- Debug-like data is hidden or deemphasized in normal view.
- No major text overlap, ghosting, black screen, input lock, or button overlap occurs.
- All existing gameplay behavior is preserved.
- EditMode tests pass.
- PlayMode tests pass.
- Android physical smoke passes.
- No P0/P1 blockers remain.

## Exit Criteria
- Sprint 12 presentation work lands on `sprint-12-presentation-mvp`.
- QA notes document automated test counts, Android build path, device, Android version/API, presentation result, regression result, and blockers.
- Remaining visual limitations are documented as P2/P3 follow-up items if the MVP remains playable and understandable.
- PR remains unmerged until QA passes.
