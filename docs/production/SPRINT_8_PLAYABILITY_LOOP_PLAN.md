# Sprint 8 Playability Loop Plan

## Sprint Goal
Make the 30-level prototype feel like playable content rather than a data-driven test harness. After Sprint 8, a player should understand what each level asks of them, why they won or lost, which enemies they face, how to plan their board, and when to spend resources — without reading code or design docs.

## Current Status After Sprint 7
- Sprint 1 delivered the playable board and wave prototype.
- Sprint 2 added progression, reward claim, level unlock, retry, and shelter upgrades.
- Sprint 3 added daily reward, daily quests, rewarded mock double/revive, and debug analytics.
- Sprint 4 added local JSON save/load/reset and Android debug APK packaging.
- Sprint 5 added mobile phone HUD hierarchy, first-run tutorial, gameplay feedback prefixes, and Android HUD safety fixes.
- Sprint 6 expanded the catalog to 30 levels with 11 enemy types, added balance simulation coverage, tuned progression rewards and upgrade costs.
- Sprint 7 established a lightweight visual identity with themed colors, styled board/cell borders, action button role colors, result panel, section labels, and cell/result feedback effects.
- Current validation baseline: EditMode tests pass 62/62, PlayMode tests pass 17/17.
- Current package id: `com.DefaultCompany.mergeshelter`.
- The prototype is now visually readable and touch-responsive on Samsung SM-S918B portrait, but level content reads as short tutorial strings, enemies are opaque string IDs, and the player has no structured way to understand wave composition, level objectives, or resource decisions.

## Android Physical Device Validation Baseline
- Primary physical target: Samsung SM-S918B, Android 16 / API 36, portrait.
- Sprint 7 physical smoke passed for install, launch, visual identity, board/HUD/action polish, visual effects, tutorial, progression, economy, save/load, Reset Save, and HUD readability.
- Sprint 8 must preserve the Sprint 7 Android baseline and run a fresh physical smoke before sign-off.

## Target Player Experience

### Player Understands Each Level Objective
Each level currently has a `DisplayName` and a `TutorialMessage`, but the tutorial message serves double duty as both a tip and an objective. Sprint 8 should separate these so the player sees a clear objective (what to do) and an optional hint (how to approach it). The objective should be visible before the player starts placing tiles.

### Player Understands Enemy/Wave Pressure
The prototype has 11 enemy factory methods (Walker, Runner, Tank, Bomber, Bruiser, SiegeTank, Demolisher, Bulwark, StormRunner, AlphaBomber, Colossus) but players only see the aggregate wave result. Sprint 8 should give each enemy type a readable display name and a one-line behavior tag so the player can scan the incoming wave before tapping Start Wave.

### Player Has Clearer Reasons to Place/Merge/Upgrade/Claim/Start Wave
The current loop works mechanically but does not surface why a player should prefer one action over another at any moment. Sprint 8 should add concise, non-blocking prompts at decision points:
- Before Start Wave: show the wave threat summary so the player knows if the board is ready.
- After defeat: surface the specific fail reason as an actionable hint (e.g., "Your walls were too weak — merge more Wood before the next attempt").
- After victory with pending reward: remind the player what claiming unlocks.
- When shelter upgrade is affordable: highlight the benefit ("Upgrade to 150 HP for the next wave").
- When a daily quest is close to completion: show the remaining count.

### 30-Level Progression Feels Like Content
The 30 levels already have names and tuned enemy compositions, but the player moves through them seeing only "Level N: Name" and a single tutorial line. Sprint 8 should make the level sequence feel like a campaign by:
- Grouping levels into named acts or zones (e.g., Levels 1–5 "First Nights", 6–10 "Inner District", 11–15 "Outer Wall", 16–20 "Ruined Highway", 21–25 "Collapsed District", 26–30 "Final Perimeter").
- Showing a brief intro line when entering a new zone.
- Displaying the enemy roster for the current level before Start Wave.

## Content Focus

### Level Objectives
- Add a short objective string to each `LevelDefinition` that states the win condition clearly (e.g., "Survive 1 wave with shelter intact").
- Display the objective in the HUD above or near the tutorial/tip area.
- Keep the existing `TutorialMessage` as a strategy hint shown below the objective.
- Objectives should be scannable in under 3 seconds.

### Win-Condition Messaging
- After victory, the result message should reference the objective and strongest tile contribution (already partially done by `PrototypeBoardEvaluator.BuildVictoryExplanation`).
- After defeat, the result message should reference the specific fail reason with an actionable suggestion. The evaluator already provides `WeakWall`, `LowAttack`, `NoHeal`, `NoEnergy`, `BoardBlocked`, and `Overwhelmed` — Sprint 8 should surface these as player-facing tips rather than debug constants.
- After retry or revive, remind the player of the objective and the previous fail reason.

### Enemy/Wave Variety Labels
- Add `DisplayName` and `BehaviorTag` fields to `EnemyData` (e.g., DisplayName: "Bomber", BehaviorTag: "Damages walls").
- Before Start Wave, show a compact wave roster: enemy count by type with behavior tags (e.g., "2× Walker · 1× Bomber (walls) · 1× Tank (tough)").
- Keep the roster concise — one line or two lines maximum.
- The roster replaces or supplements the current "Place tiles, merge, then start the wave." placeholder text during the build phase.

### Board Planning Hints
- During the build phase, show which tile types are available for the current level (already in `LevelDefinition.AvailableTiles`) with a short reminder of what each type contributes.
- After placing several tiles, if no merge group is close, show a gentle hint ("Try placing matching tiles next to each other").
- These hints should be unobtrusive and should not block or slow down experienced players.

### Reward/Upgrade Decision Prompts
- When a reward is pending and the next level is available, show the reward amount and what it unlocks.
- When shelter upgrade is affordable after claiming, show the HP benefit ("Upgrade shelter to Lv 3 → 150 HP").
- When a daily quest is 1–2 actions from completion, mention it in the result text after the relevant action.
- These prompts augment the existing feedback messages — they do not replace core gameplay text.

## Workstreams

### Project Manager
- Convert this plan into GitHub issues with acceptance criteria.
- Keep Sprint 8 scoped to content surfacing and playability prompts.
- Protect the sprint from new game mechanics, combat refactors, or backend scope.
- Track dependencies between level content, enemy labels, HUD prompts, tests, and Android smoke.

### Game Design
- Write objective strings for all 30 levels.
- Define display names and behavior tags for all 11 enemy types.
- Define zone/act groupings for the 30-level sequence.
- Review and refine defeat-reason-to-hint mappings.
- Specify prompt copy for upgrade, reward, and quest decision points.
- Keep all copy concise — mobile-scannable in under 3 seconds per line.

### Code
- Add `Objective` field to `LevelDefinition` and populate it in `SprintOneLevelCatalog`.
- Add `DisplayName` and `BehaviorTag` fields to `EnemyData` and populate them in the catalog factory methods.
- Add zone/act metadata to the level catalog or a parallel lookup.
- Surface the wave roster in the HUD during the build phase.
- Surface the level objective in the HUD.
- Surface actionable defeat hints from the existing fail-reason evaluator.
- Add upgrade/reward/quest decision prompts to the existing feedback system.
- Add tests for new content fields, wave roster formatting, defeat hint mapping, and prompt visibility.
- Keep gameplay, progression, save/load, tutorial, economy, and visual polish stable.

### UI/UX
- Define layout for the level objective, wave roster, and decision prompts within the existing HUD hierarchy.
- Ensure new text elements fit the Sprint 7 visual style (section labels, color palette, font sizes).
- Keep all new text non-blocking — the player must be able to tap through or ignore prompts.
- Verify readability on Samsung SM-S918B portrait.

### QA
- Update smoke coverage for level objectives, wave roster, defeat hints, and decision prompts.
- Run EditMode and PlayMode tests after content changes.
- Build a fresh Android debug APK from Sprint 8.
- Run physical Android smoke on Samsung SM-S918B portrait.
- Record P0/P1 blockers and security-safe device metadata only.

## Definition of Done
- Level objectives are visible and readable in the HUD for all 30 levels.
- Wave/enemy pressure is readable before Start Wave with enemy names and behavior tags.
- Player decision prompts are concise and non-blocking for reward, upgrade, quest, and retry actions.
- Defeat messages surface actionable hints derived from the board evaluator fail reasons.
- Zone/act groupings give the 30-level sequence a sense of campaign progression.
- Existing first-run tutorial still works.
- 30-level catalog still works with no broken level definitions.
- Save/load, Reset Save, Daily Reward, Daily Quest, Reward Double, Revive, and Retry remain stable.
- EditMode tests pass.
- PlayMode tests pass.
- Fresh Android physical smoke passes with no P0/P1 blockers.
- QA notes document the Sprint 8 Android smoke result.

## Out of Scope
- Final art.
- New enemy mechanics requiring heavy combat refactor (no new AI, pathfinding, or real-time behavior).
- Animated enemy sprites or combat visuals.
- Disaster modifiers (blizzard, blackout, earthquake, etc.).
- Hero system implementation.
- Real ads SDK.
- Real IAP.
- Backend.
- Cloud save.
- Store submission.
- Audio or sound effects.
- Localization infrastructure.

## Exit Criteria
- Level objectives exist for all 30 levels.
- Enemy display names and behavior tags exist for all enemy types.
- Wave roster is shown before Start Wave.
- Defeat hints are actionable.
- Decision prompts are present and non-blocking.
- EditMode tests pass.
- PlayMode tests pass.
- Android physical smoke passes on Samsung SM-S918B portrait.
- Sprint 8 QA notes are added to `docs/qa/SPRINT_1_SMOKE_CHECKLIST.md`.
- No P0/P1 blockers remain.
- PR remains unmerged until QA passes.
