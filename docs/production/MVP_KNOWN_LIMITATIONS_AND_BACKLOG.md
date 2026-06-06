# MVP Known Limitations And Backlog

This document separates accepted MVP prototype limitations from future backlog items and demo caveats. The goal is to prevent prototype constraints from being misread as release blockers during handoff and playtest review.

## A. Known Limitations Accepted For MVP
- Placeholder art: board, HUD, tiles, effects, and buttons use prototype visual polish rather than final art.
- Debug/development APK: Android builds are debug APKs for QA/demo, not production-signed releases.
- Local-only save: progress is stored locally as JSON and has no account, encryption, backend, or cloud sync.
- Local/session-only Daily Reward and Daily Quests: these systems do not use a real calendar reset, server time, or live event schedule.
- Mock ads: Reward Double and Revive use synchronous mock rewarded-ad behavior with no real ad SDK.
- No backend: there are no server services, accounts, economy authority, or remote config.
- No cloud save: progress does not sync across devices.
- No IAP: there is no purchase flow, product catalog, receipt validation, or entitlement system.
- No production signing: release signing, keystore management, and Play Store signing are out of scope.
- No store submission: Play Store/AAB packaging, store review, and store listing assets are out of scope.
- No production analytics: debug analytics events exist, but there is no production dashboard, data pipeline, or KPI reporting.
- Prototype balance/content: the 30-level catalog and economy are tuned for MVP validation and demo readability, not final live operations.
- Limited device coverage: Android physical smoke has focused on Samsung SM-S918B, Android 16 / API 36.

## B. Future Backlog
- Final art pass for board cells, tile icons, shelter status, enemies, rewards, quest UI, buttons, and result panels.
- Stronger content tuning for Levels 1-30 after broader playtest feedback.
- Real rewarded ad SDK integration for Reward Double and Revive if monetization remains in scope.
- IAP design, product catalog, purchase flow, and receipt validation if paid progression is needed.
- Backend/cloud save if account persistence, cross-device progress, or server authority is needed.
- Production Android signing, AAB generation, keystore process, and release build pipeline.
- Store metadata, screenshots, trailer captures, icon, feature graphic, and store copy.
- Accessibility/readability pass for font scale, color contrast, touch targets, and small-screen devices.
- Analytics/event review for funnel, tutorial, economy, retention, ad, revive, and level progression events.
- Expanded playtest feedback loop with multiple devices, new-player observation, bug intake, and prioritized UX follow-up.
- Broader Android device matrix covering lower-resolution phones, older API versions, and performance-sensitive hardware.

## C. Not Bugs During Demo
- Mock rewarded ad behavior: Reward Double and Revive complete immediately because no real ad SDK is integrated.
- Local-only daily resets: Daily Reward and Daily Quests reset based on local prototype save/session behavior, not real calendar time.
- Placeholder visuals: art and effects are intentionally simple and should be evaluated for readability, not final polish.
- Debug build labels/logs: development APK behavior and local logs are expected for the MVP debug RC.
- Limited physical device coverage: passing smoke on Samsung SM-S918B does not imply full device certification.
- Local save reset: `Reset Save` intentionally clears progression and returns the game to new-player Level 1.
- Level navigation controls: Previous, Replay, and Next Unlocked are included to support unlocked-level testing and demo flow; locked future levels should remain blocked in normal flow.

## Handoff Notes
- Treat P0/P1 issues as release blockers until fixed or explicitly deferred by owner approval.
- Treat documented P2/P3 issues as follow-up candidates if the MVP loop remains playable and understandable.
- Use `docs/qa/MVP_RELEASE_CANDIDATE_CHECKLIST.md` for final RC validation.
- Use `docs/demo/MVP_DEMO_PLAYTEST_SCRIPT.md` for a consistent demo path.
- Use `docs/production/ANDROID_DEBUG_BUILD_RUNBOOK.md` for reproducible Android debug builds.
