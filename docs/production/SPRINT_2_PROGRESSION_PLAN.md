# Sprint 2 Progression Plan

## Sprint Goal
Turn the Sprint 1 playable prototype into a repeatable MVP+ progression loop.

Sprint 1 proved that the player can place tiles, merge, start a wave, and see weak versus strong board outcomes. Sprint 2 focuses on making the player want to continue after one round.

## Target Player Experience
1. Player opens the prototype home screen.
2. Player sees current level, wallet, and shelter upgrade status.
3. Player enters Level 1.
4. Player completes the level and claims rewards.
5. Level 2 unlocks.
6. Player spends coins to upgrade shelter.
7. Upgrade makes future waves easier.
8. Player can continue through Level 1-10 with a basic progression loop.

## Sprint 2 Definition of Done
- Level progression from 1 to 10 is player-facing.
- Win screen has a Claim Reward action.
- Coins and parts persist during session.
- Shelter upgrade can be purchased with coins.
- Shelter upgrade affects max HP or wave survivability.
- Level unlock state works during session.
- QA smoke checklist covers progression, reward, and upgrade flow.
- No real ads, IAP, or server dependencies yet.

## Scope

### Must Have
- Runtime progression state model.
- Level unlock and current level selection.
- Reward claim flow.
- Shelter upgrade model wired into prototype gameplay.
- Basic home or progression panel.
- Updated analytics events for reward and upgrade.
- Tests for progression and upgrade rules.

### Should Have
- Simple next-level button on victory.
- Retry button on defeat.
- Level 1-10 sequential smoke path.
- Clear upgrade feedback text.

### Out of Scope
- Real rewarded ads.
- Real IAP.
- Persistent cloud save.
- Clan.
- Battle pass.
- Final art.
- LiveOps calendar.

## Agent Workstreams

### Project Manager Agent
- Maintain Sprint 2 scope.
- Ensure Sprint 1 prototype remains stable.
- Track progression blockers.

### Game Design Agent
- Define shelter upgrade effect and cost curve.
- Define level unlock pacing.
- Define reward claim copy.

### Code Agent
- Implement progression state.
- Implement reward claim.
- Implement upgrade flow.
- Wire progression UI.
- Add tests.

### Art and UX Agent
- Define basic home/progression panel layout.
- Define buttons: Play, Claim, Next Level, Retry, Upgrade.

### QA Agent
- Validate Level 1-10 sequential progression.
- Validate reward claim and upgrade spend.
- Validate weak and strong board behavior after upgrade.

### Data Agent
- Add reward_claimed and shelter_upgraded analytics hooks.
- Track level_unlock and level_selected.

### Strategy Agent
- Identify which Sprint 2 moments are recordable for UA creatives.

## Code Task Breakdown

### C1: Progression State
Create a simple session-only progression state.

Acceptance:
- Tracks highest unlocked level.
- Tracks selected/current level.
- Tracks coins and parts.
- Tracks shelter upgrade level.

### C2: Reward Claim Flow
Acceptance:
- Level completion stores pending reward.
- Claim Reward button grants coins and parts.
- Reward cannot be claimed twice.
- reward_claimed analytics fires.

### C3: Level Unlock Flow
Acceptance:
- Winning and claiming reward unlocks next level.
- Player can advance to next unlocked level.
- Locked levels cannot be started.

### C4: Shelter Upgrade Flow
Acceptance:
- Upgrade button spends coins.
- Upgrade increases shelter max HP or protection modifier.
- Cost increases per level.
- shelter_upgraded analytics fires.

### C5: Progression UI
Acceptance:
- Player can see current level.
- Player can see wallet.
- Player can claim reward after victory.
- Player can retry after defeat.
- Player can start next level after reward claim.
- Player can upgrade shelter from UI.

### C6: Tests
Acceptance:
- Progression state tests pass.
- Reward claim tests pass.
- Upgrade cost tests pass.
- PlayMode smoke path includes at least Level 1 -> reward claim -> Level 2.

## Sprint 2 Exit Criteria
- Level 1 -> 2 progression works in PlayMode.
- Level 1-10 can be manually played in sequence.
- Shelter upgrade is useful and visible.
- QA has no P0/P1 blockers.
- Prototype has at least 5 recordable gameplay/progression moments.
