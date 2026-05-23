# QA Test Plan

## P0 Checks
- App launches.
- Level 1 starts.
- Player can place tiles.
- Merge rule works.
- Enemy wave starts.
- Win and fail states work.
- No crash during 10-minute play.

## Gameplay Tests
- Merge only same type and same tier.
- Merged tile increments tier.
- Board full state handled.
- Frozen/blocked cell prevents placement.
- Enemy damage reduces shelter HP.
- Shelter HP reaches zero and fails.

## Monetization Tests
- Rewarded ad button is optional.
- Reward is granted only after ad complete event.
- Interstitial never appears during active board play.
- Remove-ads state suppresses interstitial.

## Analytics Tests
- tutorial_start fires once.
- tutorial_complete fires on completion.
- level_start includes level_id.
- level_complete includes duration and remaining_hp.
- ad_complete includes placement_id and reward_type.

## Release Candidate Gates
- Crash-free users >99%.
- No P0/P1 bugs.
- Store assets ready.
- Privacy policy linked.
- Ads/IAP use production IDs.
