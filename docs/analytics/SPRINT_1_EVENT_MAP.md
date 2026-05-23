# Sprint 1 Analytics and Monetization Event Map

## Purpose
Sprint 1 analytics should answer one question: does the first playable loop work?

The analytics system can be a local/debug implementation first. It does not need Firebase integration until MVP+.

## Core KPI Questions

| Question | Signals |
|---|---|
| Do players understand the tutorial? | tutorial_start, tutorial_complete, tutorial_step_complete |
| Do players start and finish levels? | level_start, level_complete, level_fail |
| Is the round length correct? | level_duration_seconds |
| Are players using merge correctly? | tile_place, merge_success, merge_failed |
| Why do players fail? | fail_reason |
| Are future ad placements natural? | ad_offer_preview, reward_claimed |

## Required Event Table

| Event | When It Fires | Required Parameters |
|---|---|---|
| game_start | App/session starts | app_version, platform |
| tutorial_start | First tutorial begins | tutorial_id |
| tutorial_step_complete | Player completes a tutorial step | tutorial_id, step_id |
| tutorial_complete | Tutorial ends | tutorial_id, duration_seconds |
| level_start | Level begins | level_id, attempt_number |
| tile_place | Player places a tile | level_id, tile_type, tile_tier, cell_x, cell_y |
| merge_success | Merge resolves | level_id, tile_type, from_tier, to_tier, merge_size |
| merge_failed | Player attempts invalid merge or no merge occurs after placement | level_id, reason |
| wave_start | Enemy wave starts | level_id, wave_id, enemy_count |
| shelter_damage | Shelter receives damage | level_id, amount, current_hp |
| level_complete | Level is won | level_id, duration_seconds, remaining_hp, coins_earned |
| level_fail | Level is lost | level_id, duration_seconds, fail_reason |
| reward_claimed | Player claims level reward | level_id, coins, parts |
| ad_offer_preview | Future ad placement is shown in prototype UI | placement_id, reward_type |

## Fail Reasons

Use these stable strings:

- weak_wall
- low_attack
- no_heal
- no_energy
- board_blocked
- overwhelmed
- unknown

## Future Monetization Placements

These are not production ads in Sprint 1. They are placement placeholders for UX validation.

| Placement | Format | Trigger | Sprint 1 Behavior |
|---|---|---|---|
| reward_double | Rewarded | Win screen | Button mock only |
| revive | Rewarded | Fail screen | Button mock only |
| daily_chest | Rewarded | Home screen | Not implemented |
| speed_up_upgrade | Rewarded | Upgrade timer | Not implemented |
| after_level_interstitial | Interstitial | After natural level break | Disabled in Sprint 1 |

## Monetization Rules

- No interstitial during active board play.
- No monetization prompt before tutorial completion.
- Rewarded placement must be optional.
- Reward must be visible before the player chooses the ad.
- Remove Ads entitlement should suppress future interstitial placements.

## Sprint 1 Implementation Guidance

Create an `IAnalyticsService` interface and a debug implementation that logs events to console. The implementation can later be swapped for Firebase or another analytics SDK.

## MVP+ Analytics Additions

- session_start/session_end
- shop_open
- purchase_offer_viewed
- purchase_started
- purchase_success
- ad_start
- ad_complete
- ad_fail
- remote_config_applied
- ab_test_assigned
