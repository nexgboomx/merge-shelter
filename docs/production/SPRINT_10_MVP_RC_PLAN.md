# Sprint 10 MVP RC Hardening Plan

## Sprint Goal
Prepare Merge Shelter for an MVP release-candidate pass by hardening the current prototype, cleaning up QA and release documentation, confirming reproducible Android debug builds, and running a focused regression sweep with no P0/P1 blockers.

Sprint 10 is a stabilization sprint. It should not expand gameplay scope; it should make the existing Sprints 1-9 prototype easier to validate, rebuild, and hand off as a credible MVP release candidate.

## Current Status After Sprint 9
- Sprint 1 delivered the playable board, merge, and wave prototype foundation.
- Sprint 2 added progression, reward claim, next-level unlock, retry, and shelter upgrades.
- Sprint 3 added daily reward, daily quests, rewarded mock double/revive, and debug analytics.
- Sprint 4 added local JSON save/load/reset and Android debug APK packaging.
- Sprint 5 added mobile HUD hierarchy, first-run tutorial, feedback prefixes, and Android HUD safety fixes.
- Sprint 6 expanded the prototype level catalog to 30 levels and tuned the level/economy curve.
- Sprint 7 added placeholder visual identity, board/HUD/action polish, and lightweight UI effects.
- Sprint 8 added player-facing objectives, readable enemy labels, grouped wave rosters, planning prompts, and better win/defeat messaging.
- Sprint 9 polished daily quest readability, daily reward state, session loop clarity, and unlocked-level navigation for QA/player testing.
- Current validation baseline after Sprint 9: EditMode tests passed 67/67 and PlayMode tests passed 18/18.
- Current package id: `com.DefaultCompany.mergeshelter`.
- Current Sprint 9 Android APK: `Builds/Android/merge-shelter-sprint9-retention-loop-device-debug.apk`.

## Android Physical Device Validation Baseline
- Primary tested device: Samsung SM-S918B.
- Android version/API: Android 16 / API 36.
- Sprint 9 physical smoke passed install, launch, daily reward, daily quest readability and claim states, Reward Double, reward claim, shelter upgrade, unlocked-level navigation, force-stop/relaunch save/load, Reset Save, readable HUD, and no black-screen regression in the tested paths.
- No P0/P1 blockers were observed during the Sprint 9 physical smoke paths.
- Known coverage gap from Sprint 9: physical Revive was not revalidated because the normal unlocked Level 1-2 empty-board path resolved as victory; PlayMode revive regression still passed.
- Sprint 10 must preserve this Android baseline and explicitly re-run a release-candidate physical smoke before sign-off.
- Android QA security boundary remains unchanged: test only Merge Shelter and do not access personal apps or personal device data.

## MVP Release Candidate Goals

### Stable Install/Launch
- Fresh Android debug APK installs with `adb install -r`.
- Merge Shelter launches through package `com.DefaultCompany.mergeshelter`.
- Foreground verification shows the Unity player activity before any screenshot or input.
- No crash, freeze, black screen, or focus loss during the RC smoke.

### Tutorial And First Session Readable
- New-player Level 1 starts with clear first-tile guidance.
- Tutorial advances through tile placement, merge intent, Start Wave, Claim Reward, and Next Level or optional retention actions.
- First-session HUD text remains readable on Samsung SM-S918B portrait.

### 30-Level Catalog Intact
- Level catalog still contains 30 sequential levels.
- Level 1 remains the tutorial entry.
- Level 10 remains useful for retry/revive regression.
- Level 30 remains present and validated by the strong-board automated assumption.

### Retry/Revive/Reward Double Stable
- Retry returns a defeated level to a playable state.
- Revive is available only after defeat, hides after successful use, cannot be used twice for the same defeat result, and never produces a black screen.
- Reward Double appears only with pending victory reward, doubles once, and then becomes unavailable.

### Daily Reward/Quest/Session Loop Stable
- Daily reward available and claimed states are readable.
- Daily quest titles, progress, READY state, claimed state, and one-time reward grant remain clear.
- Claim-blocked feedback explains why no quest/reward can be claimed.
- Return-session state clearly communicates level, currency, shelter upgrade, daily reward, quest, tutorial, and next useful action.

### Level Navigation Stable
- Previous Level, Replay, and Next Unlocked controls appear only when appropriate.
- Locked future levels remain unavailable in normal player flow.
- Pending reward blocks level navigation until claimed.
- Selected level persists across save/load and force-stop/relaunch.
- Reset Save returns navigation to new-player Level 1.

### Save/Load/Reset Stable
- Local save persists selected level, highest unlocked level, coins, parts, shelter upgrade level, daily reward state, daily quest state, and tutorial state.
- Force-stop/relaunch resumes the same playable state.
- Reset Save clears progression and returns to a clean Level 1 state.

### Reproducible Android Debug Build
- Android debug APK can be built from command line using the documented Unity version and build method.
- APK output path is deterministic and recorded in RC notes.
- Build logs and test result files are easy to locate.

## Hardening Focus

### QA Checklist Cleanup
- Consolidate Sprint 10 RC smoke expectations into a clear checklist.
- Separate automated tests, Android build, physical device smoke, known limitations, and release sign-off notes.
- Keep severity definitions visible and consistent.

### Release-Candidate Smoke Checklist
- Create or update an RC-specific smoke checklist for install, launch, first session, core loop, retention loop, navigation, save/load/reset, rewarded mock flow, revive/retry, and HUD readability.
- Include physical device security boundaries for ADB usage.
- Include exact pass/fail fields for EditMode, PlayMode, APK, device, Android version/API, and blockers.

### Build Documentation
- Confirm Unity version, Android module requirements, package id, build command, APK output path, and troubleshooting notes.
- Document the final MVP RC debug APK path.
- Keep production signing out of scope for this sprint.

### Regression Sweep
- Re-run full EditMode and PlayMode tests.
- Rebuild a fresh Android debug APK.
- Run physical Android smoke on Samsung SM-S918B portrait.
- Re-check Level 10 or another reliable defeat path for Retry/Revive if physically reachable.
- Re-check no HUD overlap, ghosting, or black-screen regression.

### Known Limitations
- Daily reward remains session/local-only and does not use real calendar reset.
- Reward Double and Revive remain mock rewarded-ad flows with no real SDK.
- Local save remains device-local JSON with no cloud save or backend.
- Visuals remain placeholder/prototype art.
- Android build remains debug/development, not production-signed.

### No P0/P1 Blockers
- P0 blockers must be fixed before RC sign-off.
- P1 blockers must be fixed or explicitly deferred with owner approval.
- P2/P3 issues may be documented as known limitations if the MVP loop remains playable.

## Workstreams

### Project Manager
- Define Sprint 10 issues and acceptance criteria around RC readiness.
- Keep scope limited to hardening, documentation, QA, build reproducibility, and bug fixes.
- Track release blockers, coverage gaps, and final sign-off state.
- Confirm PR remains unmerged until QA passes.

### Code
- Fix only regressions or blockers found by the RC sweep.
- Avoid new feature work unless it directly resolves a P0/P1 issue.
- Preserve the existing board, tutorial, progression, retention, level navigation, save/load/reset, and mock rewarded flows.
- Keep any needed fixes focused and covered by tests.

### QA
- Run EditMode and PlayMode tests.
- Build the RC Android debug APK.
- Run physical Android smoke under the Merge Shelter-only device security boundary.
- Add final Sprint 10 RC QA notes.
- Document residual risks, skipped coverage, and P0/P1 blocker status.

### Build/Release
- Confirm the Unity version and Android build modules.
- Verify the command-line Android build path.
- Record final MVP RC APK path, package id, and build log location.
- Confirm no production signing or store submission is attempted.

### UI/UX
- Review first-session readability, HUD stability, action button tappability, and text overlap on portrait phone.
- Confirm retention/session surfaces remain understandable: daily reward, quests, reward claim, upgrade, level navigation, and save/load resume.
- Document any non-blocking readability issues as known limitations.

## Definition Of Done
- RC plan exists.
- RC checklist exists.
- Reproducible Android build documentation exists.
- EditMode tests pass.
- PlayMode tests pass.
- Android physical smoke passes.
- Final RC notes are added.
- No P0/P1 blockers remain.

## Out Of Scope
- Real backend.
- Cloud save.
- Real calendar daily reset.
- Real ad SDK.
- Real IAP.
- Final art.
- Store submission.
- Production signing.
- Push notifications.
- LiveOps calendar.
- Analytics dashboard productionization.

## Exit Criteria
- Sprint 10 QA notes are added.
- Final MVP RC APK path is documented.
- EditMode and PlayMode test result counts are recorded.
- Android physical smoke result is recorded with device and Android version/API.
- Known limitations and residual risks are documented.
- No P0/P1 blockers remain.
- PR remains unmerged until QA passes.
