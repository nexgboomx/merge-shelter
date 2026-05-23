# Sprint 1 Smoke Checklist

## Purpose
This checklist is the minimum quality gate for the first playable prototype.

## Build Check

| ID | Test | Expected Result | Status |
|---|---|---|---|
| QA-001 | Open project in Unity | Project opens without compile errors | Not Run |
| QA-002 | Open prototype scene | Scene opens without missing script errors | Not Run |
| QA-003 | Enter play mode | No immediate exception | Not Run |

## Level Flow Check

| ID | Test | Expected Result | Status |
|---|---|---|---|
| QA-010 | Start Level 1 | Board and HUD appear | Not Run |
| QA-011 | Place a tile on empty cell | Tile appears in selected cell | Not Run |
| QA-012 | Place a tile on occupied cell | Placement is rejected with feedback | Not Run |
| QA-013 | Merge 3 same tiles | New higher-tier tile appears | Not Run |
| QA-014 | Try non-matching tiles | No merge occurs | Not Run |
| QA-015 | Start enemy wave | Wave begins and shelter can take damage | Not Run |
| QA-016 | Win level | Win screen appears and reward is shown | Not Run |
| QA-017 | Fail level | Fail screen appears and retry is available | Not Run |

## Progression Check

| ID | Test | Expected Result | Status |
|---|---|---|---|
| QA-020 | Complete Level 1 | Level 2 unlocks or can be selected | Not Run |
| QA-021 | Play Level 1–10 sequentially | No blocker prevents progress | Not Run |
| QA-022 | Claim reward | Prototype coins increase | Not Run |

## Analytics Check

| ID | Test | Expected Result | Status |
|---|---|---|---|
| QA-030 | Start level | level_start event is logged | Not Run |
| QA-031 | Place tile | tile_place event is logged | Not Run |
| QA-032 | Complete merge | merge_success event is logged | Not Run |
| QA-033 | Win level | level_complete event is logged | Not Run |
| QA-034 | Lose level | level_fail event is logged | Not Run |

## Severity Rules

| Severity | Definition |
|---|---|
| P0 | Crash, cannot launch, cannot play Level 1, data loss |
| P1 | Major gameplay bug, broken win/fail flow, serious UI blocker |
| P2 | Minor bug, unclear feedback, visual issue |
| P3 | Polish improvement |

## Sprint 1 Exit Gate
Sprint 1 cannot close until:
- All P0 issues are fixed.
- All P1 issues are fixed or explicitly accepted.
- QA-001 through QA-017 pass.
- No crash occurs during a 10-minute prototype loop.
