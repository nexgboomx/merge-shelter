# Sprint 4 Save System, Build Pipeline, and Android Prototype Packaging

## Sprint Goal
Turn the current session-only prototype into a locally persistent Android-ready prototype package.

Sprint 1 proved the core board/wave loop. Sprint 2 added progression and shelter upgrade. Sprint 3 added retention and monetization mock loops. Sprint 4 focuses on persistence, build reliability, and creating an installable Android prototype for device testing and recording.

## Target Player Experience
1. Player opens the prototype.
2. Player claims daily reward, progresses levels, completes quests, upgrades shelter.
3. Player closes and reopens the app.
4. Progress remains locally saved.
5. QA can reset save data when needed.
6. Developer can build an Android APK/AAB prototype from Unity.
7. GitHub contains clear build and release instructions.

## Sprint 4 Definition of Done
- Local save model exists.
- Progression state can be serialized/deserialized.
- Save data includes coins, parts, highest unlocked level, selected level, shelter upgrade level, daily reward state, and daily quest state.
- Save/load is wired into prototype startup and state changes.
- Reset save action exists for QA.
- Android build settings are documented.
- Build script or CI workflow exists for test/build validation.
- QA smoke notes include persistence and Android packaging status.
- No cloud save, real backend, real ads SDK, or real IAP is added.

## Scope

### Must Have
- Save data DTOs.
- Save service interface and local JSON implementation.
- SessionProgressionState export/import support.
- Save-on-change hooks.
- Reset save button or debug method.
- Tests for save/load.
- Android build notes.
- Release checklist update.

### Should Have
- Editor or CLI build script.
- GitHub Actions workflow for tests or build validation.
- Android APK artifact instructions.
- Version/build number documentation.

### Out of Scope
- Cloud save.
- Account login.
- Server backend.
- Real ads SDK.
- Real IAP.
- Push notifications.
- Store submission.
- Final art.

## Agent Workstreams

### Project Manager Agent
- Keep Sprint 4 focused on persistence and packaging.
- Track build blockers.
- Confirm Android prototype exit criteria.

### Code Agent
- Implement save data and save service.
- Wire save/load/reset into prototype controller.
- Add tests.
- Add build script or workflow support.

### QA Agent
- Validate save persists after restart.
- Validate reset save works.
- Validate Android build or document blockers.

### Data Agent
- Ensure analytics remains debug-only.
- Avoid persisting sensitive data.

### Strategy Agent
- Define what Android build is for: internal testing, gameplay capture, and first ad creative capture.

## Save Data Requirements

Save data should include:
- save version
- coins
- parts
- highest unlocked level
- selected level
- shelter upgrade level
- daily reward claimed state
- daily quest states

Save data can exclude:
- active board layout
- active level run state
- ad mock state
- temporary result screen state

## Build Requirements

Android packaging should target internal prototype testing:
- Development build is acceptable.
- Debug keystore is acceptable.
- APK is acceptable before AAB.
- No store signing required yet.

## Sprint 4 Exit Criteria
- Save/load tests pass.
- Existing EditMode tests pass.
- Existing PlayMode tests pass.
- Local persistence works in Unity PlayMode or documented limitation exists.
- Android packaging instructions are complete.
- No P0/P1 QA blockers remain.
