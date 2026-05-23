# Sprint 1 Level Pack

## Purpose
This file defines the first 10 playable prototype levels for Merge Shelter. The goal is not final balancing. The goal is to validate whether the board, merge, shelter HP, and wave loop are understandable and replayable.

## Level Rules
- One new concept per level.
- Round target length: 60–90 seconds.
- Failure should be understandable.
- Every level should create at least one clear ad-recordable moment.

## Tile Behavior for Sprint 1

| Tile | Prototype Function | Merge Result |
|---|---|---|
| Wood | Adds temporary shelter defense | Higher tier gives more defense |
| Metal | Adds turret attack power | Higher tier gives more attack |
| Food | Restores shelter HP | Higher tier restores more HP |
| Energy | Charges emergency skill | Higher tier gives more charge |

Sprint 1 wave resolution uses a single deterministic board check before combat. Each occupied tile contributes `tier * tier` value to its role, so merged tier 2 and tier 3 tiles matter more than several unmerged tier 1 tiles. Enemy pressure is compared against total Wood defense, Metal attack, Food healing, and Energy shield value to produce the wave's shelter damage and failure explanation.

## Enemy Types for Sprint 1

| Enemy | Role | HP | Damage | Speed | First Appears |
|---|---|---:|---:|---:|---:|
| Walker | Basic enemy | 10 | 8 | 1.0 | Level 1 |
| Runner | Fast pressure | 8 | 6 | 1.6 | Level 4 |
| Tank | Slow high HP enemy | 25 | 14 | 0.6 | Level 7 |
| Bomber | Wall pressure enemy | 15 | 22 | 0.8 | Level 9 |

## Level Table

| Level | Name | New Concept | Starting Tiles | Enemy Wave | Win Condition | Tutorial Message |
|---:|---|---|---|---|---|---|
| 1 | First Night | Place and merge Wood | Wood only | 2 Walkers | Shelter survives | Drag 3 Wood tiles together to build a stronger wall. |
| 2 | Scrap Defense | Introduce Metal | Wood, Metal | 3 Walkers | Shelter survives | Merge Metal to boost your turret power. |
| 3 | Hold the Gate | Balance Wood and Metal | Wood, Metal | 4 Walkers | Shelter survives | Walls buy time. Turrets end the wave. Use both. |
| 4 | Fast Shadows | Introduce Runner | Wood, Metal | 2 Walkers, 2 Runners | Shelter survives | Runners hit fast. Merge early before the wave arrives. |
| 5 | Emergency Meal | Introduce Food | Wood, Metal, Food | 3 Walkers, 2 Runners | Shelter HP above 0 | Merge Food to recover damaged shelter HP. |
| 6 | Power Surge | Introduce Energy | Wood, Metal, Food, Energy | 4 Walkers, 2 Runners | Shelter survives | Merge Energy to charge an emergency skill. |
| 7 | Heavy Footsteps | Introduce Tank | Wood, Metal, Energy | 1 Tank, 3 Walkers | Shelter survives | Tanks are slow but tough. Prepare stronger Metal merges. |
| 8 | Broken Line | Board pressure | Wood, Metal, Food | 2 Tanks, 2 Runners | Shelter survives | Bad placement can block future merges. Keep space open. |
| 9 | Fuse Warning | Introduce Bomber | Wood, Metal, Energy | 1 Bomber, 2 Runners, 2 Walkers | Shelter survives | Bombers punish weak walls. Upgrade Wood before impact. |
| 10 | Night Boss | Prototype boss check | All tiles | 1 Tank, 1 Bomber, 4 Walkers | Shelter survives with visible reward | Survive the final night by combining defense, attack, heal, and energy. |

## Reward Targets

| Level Range | Coins | Parts | Notes |
|---|---:|---:|---|
| 1–3 | 50–80 | 0 | Fast early upgrades |
| 4–6 | 80–120 | 1 | Introduce material reward |
| 7–9 | 120–180 | 1–2 | Encourage replay |
| 10 | 250 | 5 | Boss reward moment |

## Failure Communication

When a player fails, the result screen should show one of these reasons:

| Failure Cause | Message |
|---|---|
| Low Wood defense | Your walls were too weak. Merge more Wood before the wave. |
| Low Metal attack | Enemies survived too long. Build stronger Metal turrets. |
| Ignored Food | You had no recovery. Merge Food when shelter HP is low. |
| Ignored Energy | Emergency skill was not charged. Merge Energy earlier. |
| Bad spacing | Your board was blocked. Leave room for future merges. |

## Recordable Ad Moments

1. Level 4: runner wave nearly destroys shelter.
2. Level 6: emergency skill saves the shelter.
3. Level 9: bomber approaches while player races to merge Wood.
4. Level 10: boss wave with all systems active.
