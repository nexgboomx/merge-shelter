# Merge Shelter MVP Debug RC Release Notes

## Release
- Release name/version: Merge Shelter MVP Debug RC
- Date: 2026-06-03
- Repository: `nexgboomx/merge-shelter`
- Repo/branch baseline: `main` after PR #65 victory-copy fix.
- Handoff branch: `sprint-11-demo-handoff`
- Package id: `com.DefaultCompany.mergeshelter`
- Unity version: `6000.3.16f1`

## Summary
Merge Shelter is a portrait mobile Unity prototype for a survival puzzle loop. Players place resource tiles on a compact board, merge 3 matching tiles into stronger defenses, start waves, survive with shelter HP intact, claim rewards, unlock levels, upgrade the shelter, and return to readable daily reward and quest goals.

This release is a debug/development MVP release candidate for demo, QA, and handoff. It is not production-signed and is not a store submission build.

## Sprint Highlights
- Sprint 1: Built the playable board, tile placement, merge, wave, and initial PlayMode smoke foundation.
- Sprint 2: Added rewards, progression, next-level unlocks, retry, and shelter upgrades.
- Sprint 3: Added Daily Reward, Daily Quests, mock Reward Double, mock Revive, and debug analytics events.
- Sprint 4: Added local JSON save/load/reset and Android debug APK packaging.
- Sprint 5: Improved mobile HUD hierarchy, first-run tutorial, feedback text, and Android layout safety.
- Sprint 6: Expanded the level catalog to 30 levels and tuned the prototype level/economy curve.
- Sprint 7: Added placeholder visual identity, board/HUD/action polish, and lightweight UI effects.
- Sprint 8: Added clear objectives, readable enemy labels, grouped wave roster, planning prompts, and better win/defeat messaging.
- Sprint 9: Polished daily quest readability, retention/session state, reward clarity, and unlocked-level navigation.
- Sprint 10: Added MVP RC plan, RC checklist, Android debug build runbook, reproducible RC APK build, and full physical-device RC smoke notes.
- Sprint 11: Adds demo/handoff documentation, README updates, playtest script, release notes, and backlog/known-limitations documentation.

## Android Validation Baseline
- Sprint 10 MVP RC automated tests:
  - EditMode: passed 67/67.
  - PlayMode: passed 18/18.
- PR #65 victory-copy fix automated validation:
  - EditMode: passed 67/67.
  - PlayMode: passed 19/19.
- Android physical smoke status: Sprint 10 MVP RC physical smoke passed with no P0/P1 blockers.
- Primary physical device: Samsung SM-S918B.
- Android version/API: Android 16 / API 36.
- Final Sprint 10 MVP RC APK path: `Builds/Android/merge-shelter-mvp-rc-debug.apk`.
- Post-fix Sprint 11 targeted Android smoke should rebuild or reinstall the post-fix APK and add final QA notes before final handoff sign-off.

## Key Implemented Systems
- Board placement on a compact mobile portrait board.
- Merge 3 matching tiles into stronger tiers.
- Wave resolution against player-facing enemy rosters.
- Shelter HP, damage, victory, defeat, retry, and revive states.
- Reward claim, coin/parts progression, next-level unlocks, and shelter upgrade timing.
- 30-level catalog with sequential levels and broad difficulty/economy bands.
- First-run tutorial, level objectives, wave roster, and decision prompts.
- Daily Reward local/session loop.
- Daily Quests with title, progress, READY, claimed, and one-time reward states.
- Mock Reward Double flow.
- Mock Revive flow with stale-click/black-screen regression coverage.
- Previous, Replay, and Next Unlocked level navigation for unlocked levels.
- Local JSON save/load/reset.
- Reproducible Android debug APK build path and runbook.

## PR #65 Victory-Copy Fix
Sprint 10 RC QA found one P2 copy issue where a survivable-damage victory could display `WIN:` while also including defeat explanation text. PR #65 fixed the result-copy path so victory messages use victory/objective/reward language only and do not include defeat wording or raw defeat fail-reason hints.

Gameplay math, economy, rewards, progression, and save behavior were not changed by the fix.

## Known Limitations
- Visuals are placeholder/prototype art.
- APK is a debug/development build, not a production-signed release.
- Save is local-only JSON.
- Daily Reward and Daily Quests are local/session prototype systems, not real calendar-backed live systems.
- Reward Double and Revive are mock rewarded-ad flows with no real ad SDK.
- No backend, cloud save, IAP, production analytics, production signing, store metadata, AAB packaging, or store submission is included.
- Balance and content are prototype-tuned for MVP validation, not final live tuning.
- Physical-device coverage is focused on Samsung SM-S918B and does not represent a full device matrix.

See `docs/production/MVP_KNOWN_LIMITATIONS_AND_BACKLOG.md` for accepted limitations, future backlog, and demo caveats.

## Recommended Demo Path
Use `docs/demo/MVP_DEMO_PLAYTEST_SCRIPT.md`.

Recommended short path:
1. Start from fresh install or tap `Reset Save`.
2. Complete Level 1 tutorial: place tiles, merge 3 Wood, Start Wave.
3. Use Reward Double if available, then Claim Reward.
4. Claim Daily Reward and any READY Daily Quest.
5. Start Level 2, continue through Levels 1-3 progression, and show Shelter Upgrade.
6. Show Previous, Replay, and Next Unlocked navigation after levels are unlocked.
7. Force-stop/relaunch or close/reopen to show save/load persistence.
8. Tap Reset Save to return to new-player Level 1.
9. Optionally demo Level 10 Retry/Revive if reachable or validate it through automated tests.

## Sign-Off
- Release owner:
- QA owner:
- Build owner:
- Date:
- Branch/commit:
- APK path:
- EditMode result:
- PlayMode result:
- Android physical smoke result:
- P0/P1 blockers:
- Known P2/P3 issues accepted:
- Final decision: Approved / Blocked / Needs follow-up
