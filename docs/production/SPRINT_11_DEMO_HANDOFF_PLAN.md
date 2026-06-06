# Sprint 11 Demo Handoff Plan

## Sprint Goal
Prepare Merge Shelter for demo handoff by packaging the current MVP release-candidate state into clear documentation, a practical playtest script, final post-fix QA notes, and handoff-ready release notes without adding new gameplay scope.

Sprint 11 is a documentation, QA, and handoff sprint. The goal is to make the project understandable to another developer, tester, stakeholder, or playtester who needs to install, build, run, test, and evaluate the current prototype.

## Current Status After Sprint 10 And PR #65 Victory-Copy Fix
- Sprint 1 delivered the playable board, merge, and wave prototype foundation.
- Sprint 2 added progression, reward claim, next-level unlock, retry, and shelter upgrades.
- Sprint 3 added daily reward, daily quests, rewarded mock double/revive, and debug analytics.
- Sprint 4 added local JSON save/load/reset and Android debug APK packaging.
- Sprint 5 added mobile HUD hierarchy, first-run tutorial, feedback prefixes, and Android HUD safety fixes.
- Sprint 6 expanded the prototype level catalog to 30 levels and tuned the level/economy curve.
- Sprint 7 added placeholder visual identity, board/HUD/action polish, and lightweight UI effects.
- Sprint 8 added player-facing objectives, readable enemy labels, grouped wave rosters, planning prompts, and better win/defeat messaging.
- Sprint 9 polished daily quest readability, daily reward state, session loop clarity, and unlocked-level navigation for QA/player testing.
- Sprint 10 added MVP RC planning, a release-candidate checklist, a reproducible Android debug build runbook, and final RC QA notes.
- Sprint 10 MVP RC QA passed with no P0/P1 blockers and one P2 copy issue: a survivable-damage victory could show defeat explanation text.
- PR #65 fixed the victory-copy issue so victory results use victory/objective/reward language and do not include defeat wording or defeat fail-reason hints.
- Post-fix automated validation on `fix-victory-defeat-copy` passed EditMode 67/67 and PlayMode 19/19 before the fix was merged into `main`.
- Latest `main` includes the victory-copy fix commit `7b4720a`.

## MVP RC Baseline
- EditMode tests passed.
- PlayMode tests passed.
- Android physical smoke passed.
- No P0/P1 blockers were observed in the Sprint 10 MVP RC physical smoke.
- Sprint 10 final MVP RC APK path: `Builds/Android/merge-shelter-mvp-rc-debug.apk`.
- Primary Android physical smoke baseline: Samsung SM-S918B, Android 16 / API 36.
- Current package id: `com.DefaultCompany.mergeshelter`.
- Final post-fix Android RC smoke notes still need to be added after rebuilding or reinstalling the post-fix APK.

## Demo And Handoff Goals

### Clear README For Repo And Project
- Explain what Merge Shelter is, what the current MVP includes, and how to open the Unity project.
- Document required Unity version, Android module expectations, package id, and core repository structure.
- Link to production plans, QA checklists, build runbook, and release notes.

### Clear Demo Script For Playtest
- Provide a short first-session walkthrough for Level 1.
- Cover tile placement, merging 3 matching tiles, Start Wave, Claim Reward, Next Level, daily reward, daily quest, upgrade, reward double, retry/revive, save/load, and reset.
- Include expected player-facing prompts and what a tester should observe.

### Clear Install, Build, And Test Instructions
- Reference the Android debug build runbook.
- Document exact APK paths for MVP RC and post-fix RC validation.
- Include Unity EditMode and PlayMode test commands or links to the runbook/checklist.
- Keep Android phone security boundaries visible for physical-device QA.

### Release Notes For Current MVP
- Summarize the shipped prototype loop and sprint-by-sprint capability growth.
- Call out the 30-level catalog, tutorial, objectives, wave roster, retention/session loop, visual polish, and Android debug APK support.
- Include the PR #65 victory-copy fix as a post-RC polish fix.

### Known Limitations And Future Backlog
- Document prototype limitations clearly so demo reviewers do not mistake them for bugs.
- Separate known limitations from future backlog candidates.
- Preserve out-of-scope boundaries for backend, cloud save, real ad SDK, real IAP, final art, production signing, and store submission.

### Final Post-Fix RC QA Notes
- Re-run automated tests on the post-fix branch or latest `main` as appropriate.
- Build or identify the final post-fix Android debug APK.
- Run targeted Android physical smoke for install, launch, tutorial, Level 2 victory-copy behavior, core loop, save/load, reset, and no P0/P1 blockers.
- Add final Sprint 11 QA notes with exact test counts, APK path, device, Android version/API, and blocker status.

## Workstreams

### Project Manager
- Keep Sprint 11 scope limited to handoff docs, release notes, QA notes, and build/test clarity.
- Track remaining release-candidate sign-off tasks and known limitations.
- Confirm PR remains unmerged until QA passes.
- Ensure no new gameplay, economy, art, or monetization scope enters this sprint.

### Docs
- Create or update the project README, demo script, MVP release notes, and known limitations/backlog documentation.
- Link related docs so a new contributor can find the plan, runbook, QA checklist, and final smoke notes quickly.
- Keep wording concise and handoff-oriented.

### QA
- Re-run automated tests as needed and record exact pass/fail counts.
- Run Android physical smoke or targeted post-fix smoke under the Merge Shelter-only security boundary.
- Verify the victory-copy fix no longer shows defeat wording on victories.
- Add final post-fix RC smoke notes to the QA checklist.
- Record P0/P1 blocker status and any remaining P2/P3 issues.

### Build/Release
- Confirm the final post-fix Android debug APK path.
- Ensure build instructions remain reproducible from the documented Unity version.
- Confirm generated APKs, logs, and test results are not committed unless the release process explicitly asks for them.
- Keep production signing and store submission out of scope.

## Definition Of Done
- Sprint 11 plan exists.
- Demo README/script exists.
- MVP release notes exist.
- Backlog/known limitations documentation exists.
- Final post-fix RC smoke notes are added.
- Final post-fix APK path is documented.
- Automated tests pass.
- Android physical smoke or targeted post-fix smoke passes.
- No P0/P1 blockers remain.

## Out Of Scope
- New gameplay features.
- New art.
- Real ads SDK.
- Real IAP.
- Backend.
- Cloud save.
- Real calendar daily reset.
- Production signing.
- Store submission.
- Analytics productionization.
- LiveOps calendar.

## Exit Criteria
- Handoff documentation is complete enough for a new tester or developer to install, build, run, and evaluate the current MVP.
- Final post-fix APK path is documented.
- EditMode and PlayMode tests pass with exact counts recorded.
- Android physical smoke or targeted post-fix smoke passes with device and Android version/API recorded.
- Victory-copy fix is validated in automated tests and, where practical, on Android physical device.
- Known limitations and future backlog are documented.
- No P0/P1 blockers remain.
- PR remains unmerged until QA passes.
