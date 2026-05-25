# Sprint 6 Level Tuning And Economy Curve Plan

## Sprint Goal
Tune Merge Shelter from a 10-level vertical slice into a 30-level prototype progression path with a readable difficulty ramp and a simple economy curve.

Sprint 6 should make Levels 1-30 exist, feel intentionally paced, and support repeated smoke testing without expanding into final content production, real monetization, backend features, cloud save, or store submission work.

## Current Status After Sprint 5
- Sprint 1 delivered the playable board and wave prototype foundation.
- Sprint 2 added progression, reward claim, level unlock, retry, and shelter upgrades.
- Sprint 3 added daily reward, daily quests, rewarded mock double reward, rewarded mock revive, and debug analytics hooks.
- Sprint 4 added local JSON save/load/reset and Android debug APK packaging.
- Sprint 5 added mobile phone HUD hierarchy, first-run tutorial flow, simple gameplay feedback, Android HUD safety fixes, and physical device smoke validation.
- Current validation baseline after Sprint 5: EditMode tests pass 41/41 and PlayMode tests pass 17/17.
- Current package id for Android smoke is `com.DefaultCompany.mergeshelter`.
- Current level catalog covers Levels 1-10 and needs expansion to Levels 1-30.

## Android Physical Device Validation Baseline
- Primary physical target: Samsung SM-S918B, Android 16 / API 36, portrait.
- Sprint 5 physical smoke passed for install, launch, first-run tutorial, board interaction, Start Wave, rewards, progression, Daily Reward, Daily Quest, shelter upgrade, Reward Double, Retry, Revive, save/load, Reset Save, HUD readability, and gameplay feedback.
- Previous Android P1 blockers around stale Revive black-screen state, HUD text overlap, and save/load HUD refresh were fixed and verified.
- Sprint 6 must preserve the Sprint 5 Android baseline and run a fresh physical smoke before sign-off.

## Target Player Experience For Levels 1-30
1. Levels 1-5 teach placement, merging, wave start, rewards, daily support, and upgrades without punishing early mistakes.
2. Levels 6-10 require basic merging and make weak boards lose more consistently.
3. Levels 11-20 ask the player to plan upgrades and resources across multiple levels instead of only solving each board in isolation.
4. Levels 21-30 introduce stronger late-prototype pressure where poor board planning, delayed upgrades, or ignored rewards should cause visible defeats.
5. Rewards should feel sufficient for steady progress, while upgrades still require choices.
6. Daily Reward and Daily Quest rewards should help recovery and pacing without trivializing the level curve.
7. The player should understand why a board won or lost through existing result text and feedback.

## Difficulty Curve

### Levels 1-5: Tutorial/Forgiving
- Level 1 remains the guided first-run tutorial loop.
- Levels 2-3 introduce mixed tile types and basic enemy scaling.
- Levels 4-5 introduce early pressure but allow recovery from imperfect boards.
- Weak boards can still win some early levels, but totally empty or careless boards should lose by Level 5.
- Expected player behavior: place tiles, attempt merges, start waves, claim rewards, and see upgrade affordance.

### Levels 6-10: Basic Merge Requirement
- Enemy pressure should expect at least one useful merge or several correctly placed tier 1 tiles.
- Weak boards should lose most levels in this band.
- Medium boards should win most levels with some shelter damage.
- Strong boards should win clearly and teach the value of tier upgrades.
- Level 10 remains a milestone/boss-style check that can defeat empty or weak boards and validate Retry/Revive flows.

### Levels 11-20: Upgrade And Resource Planning
- Enemy pressure should assume players have claimed several level rewards and at least one shelter upgrade.
- Coins should support upgrades at planned intervals, but not after every level.
- Parts should begin to matter as a visible secondary reward track, even if the prototype does not yet add a deep parts sink.
- Medium boards with reasonable upgrades should pass.
- Weak boards or skipped upgrades should fail more often.

### Levels 21-30: Stronger Board Planning And Late Prototype Pressure
- Enemy compositions should require stronger merge planning and better tile mix use.
- Empty boards and low-effort boards should reliably fail.
- Medium boards should be stressed and may need upgrades or rewarded mock support.
- Strong boards should win but not feel completely automatic.
- Level 30 should serve as the Sprint 6 endpoint check for the current prototype economy and combat math.

## Economy Curve

### Rewards
- Coin rewards should increase gradually with level difficulty.
- Early rewards should quickly teach reward claim and unlock flow.
- Midgame rewards should create upgrade decisions rather than constant affordability.
- Late prototype rewards should feel meaningful enough to continue but not erase pressure.

### Parts
- Parts should remain a secondary progression signal in Sprint 6.
- Parts should start low in Levels 1-5, become regular in Levels 6-20, and increase in Levels 21-30.
- If no new parts sink is added, parts should be tuned conservatively and documented as future economy headroom.

### Shelter Upgrade Timing
- Target first upgrade: achievable around Levels 2-3 with normal reward claim behavior.
- Target second upgrade: achievable around Levels 5-8 depending on Daily Reward and quest usage.
- Target later upgrades: spaced so the player cannot upgrade after every level without extra reward support.
- Upgrade timing should be checked against weak, medium, and strong board outcomes.

### Daily Reward Impact
- Daily Reward should help a struggling player recover or reach an upgrade earlier.
- Daily Reward should not make Levels 1-10 impossible to fail.
- Daily Reward should remain useful in Levels 11-30 without becoming the dominant source of progress.

### Quest Reward Impact
- Quest rewards should support active play: placing tiles, completing levels, and claiming rewards.
- Quest reward amounts should accelerate progress modestly without replacing level rewards.
- Quest claim timing should be included in economy test scenarios.

## Balance Assumptions

### Weak Board
- Empty board or scattered tier 1 tiles with little merge intent.
- Should pass only the most forgiving early levels.
- Should lose most Levels 6-30.
- Used to verify defeat, Retry, Revive, and fail explanation clarity.

### Medium Board
- Several placed tiles, at least one basic merge in relevant tile types, and occasional upgrades.
- Should pass Levels 1-10 with some damage.
- Should pass many Levels 11-20 if upgrade timing is reasonable.
- Should become stressed in Levels 21-30.

### Strong Board
- Multiple useful merges, good tile mix, and planned shelter upgrades.
- Should win most levels.
- Should still take pressure in late prototype levels so victory remains meaningful.
- Used to verify that skillful preparation is rewarded without making the curve flat.

## Workstreams

### Project Manager
- Convert this plan into GitHub issues with acceptance criteria.
- Define Sprint 6 gates for level count, balance validation, economy validation, and Android smoke.
- Track dependencies between level tuning, economy values, tests, and QA.
- Keep scope focused on prototype level tuning.

### Game Design
- Define Levels 1-30 with enemy compositions, available tile sets, tutorial notes, and expected board strength.
- Specify target win/loss outcomes for weak, medium, and strong board scenarios.
- Document any intended difficulty spikes and recovery levels.
- Keep level messaging concise and compatible with the Sprint 5 HUD.

### Economy
- Tune coin and parts rewards across Levels 1-30.
- Define expected shelter upgrade timing with and without Daily Reward and quest rewards.
- Check whether level rewards, Daily Reward, and quest rewards combine into a reasonable curve.
- Document known economy limitations and future parts sinks.

### Code
- Extend the level catalog to 30 levels with minimal structural changes.
- Keep the board, wave, save, tutorial, feedback, and Android build systems stable.
- Add or update tests for level existence, reward values, progression through Level 30, and balance scenarios.
- Avoid unrelated gameplay refactors.

### QA
- Update smoke coverage for Levels 1-30.
- Run EditMode and PlayMode tests after level/economy tuning.
- Build a fresh Android debug APK from Sprint 6.
- Run physical Android smoke on Samsung SM-S918B portrait.
- Record P0/P1 blockers and security-safe device metadata only.

## Definition Of Done
- Levels 1-30 exist in the prototype level catalog.
- Each level has clear display name, tutorial/status message, available tile set, enemy composition, coin reward, and parts reward.
- Difficulty curve has explicit weak, medium, and strong board expectations.
- Economy curve documents expected reward pacing, parts pacing, and shelter upgrade timing.
- Automated tests cover level catalog completeness, reward progression sanity, and representative weak/medium/strong board outcomes.
- Sprint 5 mobile HUD, tutorial, feedback, save/load, reset, and Revive behavior remain stable.
- EditMode tests pass.
- PlayMode tests pass.
- Fresh Android physical smoke passes with no P0/P1 blockers.
- QA notes document the Sprint 6 Android smoke result.

## Out Of Scope
- Final art.
- Real ads SDK.
- Real IAP.
- Backend.
- Cloud save.
- Store submission.
- Account login.
- Push notifications.
- Full live economy model.
- New long-term parts sink unless explicitly approved as a separate issue.
- New content systems beyond the data needed for Levels 1-30.

## Exit Criteria
- Levels 1-30 exist.
- Level and economy tuning notes are captured in production docs or issue acceptance criteria.
- Weak, medium, and strong board assumptions are validated through tests or QA notes.
- EditMode tests pass.
- PlayMode tests pass.
- Android physical smoke passes on Samsung SM-S918B portrait.
- No P0/P1 blockers remain.
