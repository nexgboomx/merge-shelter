# Sprint 3 Retention and Monetization Mock Plan

## Sprint Goal
Add the first retention and monetization loops without integrating real ads, IAP, cloud save, or a backend.

Sprint 1 proved the core board/wave loop. Sprint 2 added progression, reward claim, level unlock, retry, and shelter upgrades. Sprint 3 focuses on giving the player reasons to return and creating safe monetization hooks that can later be replaced with real SDKs.

## Target Player Experience
1. Player opens the prototype.
2. Player sees daily reward availability.
3. Player claims a daily reward.
4. Player sees simple daily quests.
5. Player plays a level and completes quest progress.
6. Player wins or fails a level.
7. On the result screen, player can optionally use a rewarded mock action:
   - Double reward after victory.
   - Revive after defeat.
8. Player receives transparent reward feedback.
9. Analytics logs retention and monetization-intent events.

## Sprint 3 Definition of Done
- Daily reward is claimable once per session/day mock.
- Daily quest model exists.
- At least three quests are supported.
- Quest progress updates from prototype gameplay events.
- Rewarded-ad mock placements exist for reward double and revive.
- No real ad SDK is used.
- Rewarded mock can be toggled or simulated instantly.
- Analytics events are emitted for daily reward, quest, ad offer, ad complete, revive, and reward double.
- EditMode and PlayMode tests pass.
- QA smoke notes are updated.

## Scope

### Must Have
- Daily reward model.
- Daily quest model.
- Quest progress and claim flow.
- Rewarded mock service.
- Double reward mock on victory.
- Revive mock on defeat.
- Analytics hooks.
- UI buttons for Daily Reward, Double Reward, Revive, Claim Quest.
- Tests.

### Should Have
- Simple session reset method for QA.
- Clear result text after rewarded mock actions.
- Quest list text in HUD or result area.

### Out of Scope
- Real AdMob, Unity Ads, AppLovin, ironSource, LevelPlay, MAX.
- Real IAP.
- Remote Config.
- Cloud save.
- Push notifications.
- Battle pass.
- Final art.

## Core Systems

### Daily Reward
Session-only mock, later replace with real date/time persistence.

Acceptance:
- Player can claim once.
- Claim grants coins and parts.
- Button hides or disables after claim.
- daily_reward_claimed analytics fires.

### Daily Quests
Initial quest examples:
- Place 10 tiles.
- Complete 1 level.
- Claim 1 reward.
- Upgrade shelter 1 time.

Acceptance:
- Quest progress updates from gameplay actions.
- Quest completion can be claimed once.
- Claim grants coins or parts.
- quest_progress and quest_claimed analytics fire.

### Rewarded Mock Ads
Placements:
- reward_double after victory before claim.
- revive after defeat.

Acceptance:
- ad_offer_preview fires when placement appears.
- ad_mock_started fires when clicked.
- ad_mock_completed fires immediately for Sprint 3.
- reward_double doubles pending reward once.
- revive restores shelter and allows another attempt or converts defeat into retryable state.

## Agent Workstreams

### Project Manager Agent
- Keep Sprint 3 scope focused on mock retention/monetization.
- Prevent real SDK integration until product metrics justify it.

### Game Design Agent
- Define daily reward amount.
- Define first quest reward values.
- Define revive/double reward constraints.

### Code Agent
- Implement models and UI hooks.
- Add tests.
- Keep systems session-only and simple.

### Data Agent
- Define analytics events and parameters.
- Ensure mock ad events mirror future production ad events.

### QA Agent
- Verify daily reward, quest claim, reward double, and revive flows.

### Strategy Agent
- Identify recordable ad moments from Sprint 3.

## Analytics Events
- daily_reward_viewed
- daily_reward_claimed
- quest_progress
- quest_completed
- quest_claimed
- ad_offer_preview
- ad_mock_started
- ad_mock_completed
- reward_doubled
- revive_used

## Sprint 3 Exit Criteria
- Daily reward flow works.
- At least three quests work.
- Victory reward double mock works once.
- Defeat revive mock works once.
- EditMode tests pass.
- PlayMode tests pass.
- No P0/P1 QA blockers.
