# Sprint 1 Implementation Breakdown

## Sprint Goal
Create a playable prototype of the 60-second survival merge loop.

The Sprint 1 prototype does not need final art, final economy, production ads, or store-ready polish. It must prove that the core interaction is understandable, replayable, and suitable for recording early market-test creatives.

## Target Player Experience
1. Player starts Level 1.
2. Player sees a 6x6 board and shelter HP.
3. Player places Wood, Metal, Food, and Energy tiles.
4. Player merges three matching tiles to create a stronger tile.
5. A simple enemy wave attacks.
6. The shelter survives or fails.
7. Player receives a clear win/fail result.
8. Player understands what to improve next round.

## Sprint 1 Definition of Done
- Level 1 to Level 10 can be played in sequence.
- Board placement works on mobile-sized layout.
- Merge-3 logic works with same type and same tier tiles.
- Shelter HP changes after enemy attacks.
- Win/fail state is visible.
- Reward screen grants prototype coins.
- Basic analytics interface is callable.
- QA smoke checklist passes.

## Agent Workstreams

### Project Manager Agent
Tasks:
- Keep Sprint 1 scope frozen.
- Maintain issue priority.
- Track blockers across Design, Code, Art/UX, QA, and Data.
- Confirm Sprint 1 exit criteria before moving to MVP+.

Deliverables:
- Sprint status report.
- Risk log update.
- Go/no-go recommendation.

### Game Design Agent
Tasks:
- Finalize first 10 levels.
- Define enemy stats for first wave types.
- Define tile effects for Wood, Metal, Food, Energy.
- Define tutorial sequence.

Deliverables:
- Level 1–10 table.
- Enemy stat table.
- Tile behavior table.
- Tutorial text spec.

### Code Agent
Tasks:
- Convert board model into Unity scene behavior.
- Implement board input controller.
- Implement tile view/controller binding.
- Implement wave simulation.
- Implement result flow.
- Add edit mode tests and play mode smoke test.

Deliverables:
- Playable prototype scene.
- Board input scripts.
- Wave manager integration.
- Tests.

### Art and UX Agent
Tasks:
- Define placeholder tile visuals.
- Define cell states.
- Define HUD layout.
- Define result screen layout.
- Define basic merge feedback timing.

Deliverables:
- Prototype UI spec.
- Placeholder asset list.
- Screen flow sketch.

### Content and LiveOps Agent
Tasks:
- Write tutorial microcopy.
- Name first 10 levels.
- Draft first daily mission examples.
- Draft first event theme concept.

Deliverables:
- Tutorial copy.
- Level name list.
- Mission text list.

### QA Agent
Tasks:
- Execute smoke tests.
- Test merge edge cases.
- Test win/fail flow.
- Test 10-level progression.
- Log P0/P1/P2 issues.

Deliverables:
- Sprint 1 QA report.
- Bug list.
- Release risk notes.

### Data and Monetization Agent
Tasks:
- Define analytics event map.
- Define ad placements for future implementation.
- Confirm no interstitial during active play.
- Define prototype KPI dashboard requirements.

Deliverables:
- Analytics event table.
- Monetization placement rules.
- KPI measurement plan.

### Strategy Agent
Tasks:
- Define first 5 creative ad concepts.
- Map each creative to actual prototype moments.
- Define market test assumptions.
- Define kill/continue metric thresholds.

Deliverables:
- Creative hypothesis list.
- Soft-test market plan.
- KPI thresholds.

## Code Task Breakdown

### Task C1: Unity Board Scene
Acceptance:
- A 6x6 grid is visible.
- Empty cells can receive tiles.
- Occupied cells reject placement.

### Task C2: Tile Spawning and Placement
Acceptance:
- Player can select or receive a tile.
- Player can place the tile on a valid cell.
- Invalid placement gives feedback.

### Task C3: Merge Resolution
Acceptance:
- Three connected same type and same tier tiles merge.
- Result tile increases tier by one.
- Non-matching tiles do not merge.

### Task C4: Enemy Wave Prototype
Acceptance:
- Wave starts after board phase.
- Enemy damage reduces shelter HP.
- Shelter destroyed triggers fail.
- Shelter surviving triggers win.

### Task C5: Reward Flow
Acceptance:
- Win grants prototype coins.
- Fail shows retry option.
- Result screen is readable.

### Task C6: Analytics Hooks
Acceptance:
- level_start is called.
- level_complete is called.
- level_fail is called.
- merge_success is called.

## Design Task Breakdown

### Task D1: Level 1–10 Table
Required columns:
- Level ID
- New mechanic
- Starting tile set
- Enemy count
- Enemy damage
- Win condition
- Tutorial message

### Task D2: Tile Behavior Rules
Required tiles:
- Wood: wall or defense HP support
- Metal: turret or attack support
- Food: heal support
- Energy: skill charge support

### Task D3: Failure Communication
The prototype must show why the player failed:
- Not enough defense
- Shelter HP depleted
- Bad placement or missed merge opportunity

## QA Smoke Checklist
- App opens.
- Prototype scene loads.
- Level 1 starts.
- Tile can be placed.
- Merge can happen.
- Wave can start.
- Shelter HP changes.
- Win screen appears.
- Fail screen appears.
- No crash during 10-minute loop.

## Sprint 1 Exit Criteria
Move to MVP+ only if:
- Core loop is playable.
- First-time player understands the goal.
- Round length is near 60–90 seconds.
- QA smoke checklist passes.
- There are at least 3 recordable ad moments.

## Out of Scope for Sprint 1
- Final art.
- Real ads SDK.
- Real IAP.
- Clan.
- PvP.
- Live server.
- Full store release.
- Final balancing.
