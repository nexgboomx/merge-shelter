# Sprint 4 Release Checklist

Use this checklist before sharing an internal Android prototype APK for Sprint 4.

## Branch And PR Status

- Working branch is `sprint-4-save-build-android`.
- Branch is up to date with `origin/sprint-4-save-build-android`.
- Main branch is not modified directly.
- PR #26 is not merged as part of this checklist unless separately approved.
- Working tree is clean after commit.
- Release notes or QA notes include current test/build status.

## Tests

- Unity project opens with no C# compile errors.
- EditMode tests pass.
- PlayMode tests pass.
- Any skipped tests are documented with reason.
- `git diff --check` passes before commit.

## Save And Load

- New player starts on Level 1.
- Coins persist after restart.
- Parts persist after restart.
- Highest unlocked level persists after restart.
- Selected level persists after restart.
- Shelter upgrade level persists after restart.
- Daily reward claimed state persists after restart.
- Daily quest progress persists after restart.
- Daily quest claimed state persists after restart.
- Active board layout is not expected to persist.
- Active wave/result/ad mock state is not expected to persist.

## Reset Save

- Reset Save button is visible in the prototype UI.
- Reset Save deletes the local JSON save.
- Reset Save returns coins and parts to zero.
- Reset Save returns highest unlocked level and selected level to Level 1.
- Reset Save returns shelter upgrade to Level 1.
- Reset Save makes the daily reward available again.
- Reset Save clears daily quest progress and claimed state.
- Reset Save clears the board and updates the HUD.

## Android Build

- Unity version is `6000.3.16f1`.
- Android Build Support is installed.
- Android SDK and NDK Tools are installed.
- OpenJDK is installed.
- Platform is switched to Android.
- `Assets/Scenes/PrototypeSprint1.unity` is enabled in build scenes.
- APK build is selected for prototype distribution.
- Development build is acceptable.
- Debug signing is used.
- No production keystore is added.
- Artifact follows naming convention:

```text
merge-shelter-sprint4-prototype-YYYYMMDD-b001-debug.apk
```

## Device Install

- Android test device has USB debugging enabled.
- `adb devices` shows the target device.
- APK installs with `adb install -r`.
- App launches from device launcher.
- No startup crash occurs.
- Device model, Android version, APK filename, and tester are recorded in QA notes.

## Manual Device Smoke

- Board cell tap places a tile.
- Start Wave resolves the current level.
- Weak board and strong board produce different outcomes.
- Reward claim flow works after victory.
- Retry flow works after defeat.
- Daily reward claim works and double claim is blocked.
- Daily quest progress and claim work.
- Reward Double mock works once after victory.
- Revive mock works once after defeat.
- Upgrade Shelter shows cost/level and works when affordable.
- Upgrade Shelter is blocked when coins are insufficient.
- Save persists after closing and relaunching the app.
- Reset Save returns the app to new-player state.

## Known Out Of Scope

- Cloud save.
- Account login.
- Save encryption.
- Store signing.
- Play Console upload.
- Real ads SDK.
- Real IAP.
- Backend services.
- Remote config.
- Push notifications.
- Crash reporting.
- Final art, final icons, and store screenshots.

## Go Or No-Go

- P0 bugs: 0 required.
- P1 bugs: 0 required or explicitly accepted.
- Compile errors: 0 required.
- EditMode and PlayMode failures: 0 required unless documented as unrelated infrastructure failure.
- Device install blocker: 0 required before sharing APK.
