# Sprint 7 Visual Polish Plan

## Sprint Goal
Establish a lightweight visual identity for Merge Shelter and replace the most debug-looking UI surfaces with readable placeholder art, simple style rules, and prototype-safe effects.

Sprint 7 should make the board, HUD, actions, rewards, quests, and feedback feel more like a coherent mobile survival game while keeping the project independent of final art, outsourced assets, heavy packages, real monetization SDKs, backend features, cloud save, or store submission work.

## Current Status After Sprint 6
- Sprint 1 delivered the playable board and wave prototype foundation.
- Sprint 2 added progression, reward claim, level unlock, retry, and shelter upgrades.
- Sprint 3 added daily reward, daily quests, rewarded mock double reward, rewarded mock revive, and debug analytics hooks.
- Sprint 4 added local JSON save/load/reset and Android debug APK packaging.
- Sprint 5 added mobile phone HUD hierarchy, first-run tutorial flow, simple gameplay feedback, Android HUD safety fixes, and physical device smoke validation.
- Sprint 6 expanded the prototype to 30 levels, added balance simulation coverage, tuned progression rewards and upgrade costs, and ran physical Android smoke on the Sprint 6 APK.
- Current validation baseline after Sprint 6: EditMode tests pass 59/59 and PlayMode tests pass 17/17.
- Current package id for Android smoke is `com.DefaultCompany.mergeshelter`.
- Current UI is functional and phone-readable, but still reads as programmer/debug UI rather than a styled mobile prototype.

## Android Physical Device Validation Baseline
- Primary physical target: Samsung SM-S918B, Android 16 / API 36, portrait.
- Sprint 6 physical smoke passed for install, launch, Level 1 tutorial, board interaction, Start Wave, rewards, progression through early and mid levels, Daily Reward, Daily Quest, shelter upgrade, Reward Double, Retry, Revive, save/load, Reset Save, HUD readability, and Android black-screen regression coverage.
- Level catalog and economy tests validate Levels 1-30, representative board assumptions, Level 10 Retry/Revive regression coverage, and Level 30 strong-board beatability.
- Sprint 7 must preserve the Sprint 6 Android baseline and run a fresh physical smoke before sign-off.

## Visual Direction

### Post-Apocalyptic Shelter
- The game should read as a small shelter surviving night waves with scavenged materials, emergency systems, and resource pressure.
- Visual language should use sturdy panels, muted survival colors, simple hazard accents, and clear shelter/resource cues.
- The prototype should avoid final illustration dependencies while still suggesting walls, metal scraps, food supplies, power, danger, and recovery.

### Clean Mobile Readability
- Phone readability is higher priority than decorative detail.
- Text, buttons, tiles, and result states must remain legible on Samsung SM-S918B portrait.
- Contrast should be strong enough for quick scanning, with no text ghosting, no critical overlap, and no tiny action targets.
- Visual polish must keep the existing opaque Android-safe background behavior.

### Simple Survival/Resource Fantasy
- Wood should feel defensive.
- Metal should feel offensive or turret-like.
- Food should feel restorative.
- Energy should feel shield/power-like.
- Rewards, quests, and upgrades should feel like shelter progress rather than raw debug counters.

## Placeholder Art Rules
- Use generated sprites, simple shapes, Unity UI primitives, or tiny repo-owned placeholder assets only.
- Do not require final art before Sprint 7 can close.
- Do not import copyrighted assets, scraped artwork, store screenshots, icon packs with unclear licenses, or third-party art bundles.
- Do not add heavy packages or large binary dependencies for placeholder visuals.
- Keep assets easy to replace later through clear names, small sizes, and limited coupling to gameplay code.
- Prefer data-light, deterministic visuals that are safe for Git and CI.
- Placeholder art should improve readability and feel without pretending to be final production art.

## UI Style Targets

### Board Cells
- Replace plain debug squares with a consistent shelter-grid style.
- Keep cell boundaries clear and tap areas stable.
- Show empty cells, occupied cells, selected/feedback cells, and invalid taps with distinct visual states.
- Preserve centered board placement and phone-safe spacing.

### Tile Types
- Give Wood, Metal, Food, and Energy distinct colors, labels, and simple icon-like shapes.
- Keep tier information readable on every tile.
- Higher tiers should feel stronger through scale, border, glow, stripe count, or another simple visual marker.
- Tile visuals must remain understandable without final art.

### Shelter Status
- Make HP and shelter upgrade level feel like a shelter panel instead of loose text.
- Use clear health/status styling for normal, damaged, defeated, and upgraded states.
- Keep upgrade affordability readable without dense wallet strings.

### Enemy/Wave Status
- Add a small wave/status treatment that communicates whether the player is building, resolving, victorious, or defeated.
- Keep enemy details lightweight; Sprint 7 does not need animated enemies or a full combat screen.
- Result explanations should stay concise and bounded.

### Rewards/Quests
- Separate coins, parts, daily reward, quest progress, and pending rewards into readable visual groups.
- Use small placeholder icons or badges where they improve scanning.
- Claimed, available, ready, and blocked states should be visually distinct.

### Action Buttons
- Primary action should remain visually obvious in each state: Start Wave, Claim Reward, Retry/Revive, and Next Level.
- Secondary actions should remain tappable but less dominant.
- Buttons should share a consistent style, with color/state differences for positive, blocked, reward, danger, and reset actions.
- Button style changes must not shrink tap targets or reintroduce overlap.

### Result Panel
- Result and tutorial messages should sit in a bounded panel or message area.
- Victory, defeat, reward, blocked, revive, upgrade, and reset messages should have recognizable color/status styling.
- Long messages should wrap or clamp safely.

## Effects Scope
- Tile placement: short cell pulse or tile pop that confirms a valid tap.
- Merge: stronger pulse, brief scale, border flash, or color burst on the merged cell.
- Invalid tap: quick blocked flash and concise blocked result message.
- Wave victory: result panel highlight and subtle success pulse.
- Wave defeat: result panel danger styling and clear retry/revive affordance.
- Reward claim: coin/parts feedback pulse or reward button highlight.
- Upgrade: shelter status pulse and upgraded HP/status feedback.
- Revive: revive feedback that returns immediately to playable board state and hides stale revive affordances.
- Reset: reset feedback that clearly returns to new-player Level 1 state.
- Effects must be lightweight, non-blocking, deterministic enough for tests, and safe on Android.

## Workstreams

### Project Manager
- Convert this plan into GitHub issues with acceptance criteria.
- Keep Sprint 7 scoped to visual identity, placeholder art, UI style, and simple effects.
- Track dependencies between UI style, placeholder assets, feedback behavior, tests, and Android smoke.
- Protect the sprint from final art, store, monetization, or backend scope creep.

### UI/UX
- Define the visual hierarchy for board, shelter status, resources, quests, rewards, actions, and result panel.
- Specify button state language for primary, secondary, reward, danger, blocked, and reset actions.
- Keep all UI readable and tappable on Samsung SM-S918B portrait.
- Preserve the Sprint 5/6 mobile layout safety constraints.

### Placeholder Art
- Create a small visual kit for tile types, board cells, resource icons, status badges, and basic panel treatments.
- Use repo-owned generated or simple-shape placeholders only.
- Name assets clearly so final art can replace them later.
- Keep binary asset size low.

### Game Feel
- Tune simple feedback timing for placement, merge, invalid tap, wave result, reward, upgrade, revive, and reset.
- Avoid long-running animations that block input or make Android screenshots unstable.
- Keep effects visible enough for player comprehension but simple enough for prototype maintenance.

### Code
- Integrate placeholder visuals through existing Unity UI and board view systems.
- Keep gameplay, progression, save/load, tutorial, level tuning, and economy behavior stable.
- Add tests for required visual elements, action state visibility, effect state hooks, and Android-safe UI assumptions.
- Avoid broad refactors or new rendering packages.

### QA
- Update smoke coverage for visual readability and effect regressions.
- Run EditMode and PlayMode tests after visual changes.
- Build a fresh Android debug APK from Sprint 7.
- Run physical Android smoke on Samsung SM-S918B portrait.
- Record P0/P1 blockers and security-safe device metadata only.

## Definition Of Done
- A Sprint 7 visual kit exists for board cells, tile types, shelter/resource panels, action buttons, and result/status messages.
- Board, HUD, rewards, quests, actions, and result area look less debug while remaining clearly placeholder.
- Core actions have visible lightweight feedback for placement, merge, invalid tap, wave victory/defeat, reward claim, upgrade, revive, and reset.
- Existing first-run tutorial, save/load, Reset Save, Retry, Revive, Reward Double, Daily Reward, Daily Quest, level progression, and Sprint 6 level/economy behavior remain stable.
- UI remains readable and tappable on Samsung SM-S918B portrait.
- Automated tests cover visual UI presence, action state visibility, effect state hooks, and Android-safe layout assumptions where practical.
- EditMode tests pass.
- PlayMode tests pass.
- Fresh Android physical smoke passes with no P0/P1 blockers.
- QA notes document the Sprint 7 Android smoke result.

## Out Of Scope
- Final art.
- Outsourced art.
- Real ads SDK.
- Real IAP.
- Backend.
- Cloud save.
- Store submission.
- Account login.
- Push notifications.
- Full enemy animation system.
- Full audio production pass.
- Store screenshots or marketing creatives.
- New gameplay systems beyond what is needed to present existing mechanics more clearly.

## Exit Criteria
- Visual kit exists.
- Board/HUD/actions look less debug.
- Core actions have visual effects.
- EditMode tests pass.
- PlayMode tests pass.
- Android physical smoke passes on Samsung SM-S918B portrait.
- No P0/P1 blockers remain.
