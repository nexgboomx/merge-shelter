# Unity Setup Notes

## Current Implementation Status
The repository now contains Sprint 1 prototype logic, but it still needs a Unity scene to be wired manually or generated in a later automation step.

## Required Local Tools
Install these when ready to build and test locally:

1. Unity Hub
2. Unity 2022.3 LTS or Unity 6 LTS
3. Android Build Support module
4. OpenJDK module from Unity installer
5. Android SDK and NDK tools from Unity installer
6. Git LFS, recommended for future large assets

## Optional CI Tools
For automated GitHub Actions builds, use one of these later:

- game-ci/unity-builder GitHub Action
- Unity activation/license secret for CI
- A self-hosted GitHub Actions runner with Unity installed

## Scene Wiring for Sprint 1
Create a scene named:

`Assets/Scenes/PrototypeSprint1.unity`

Recommended hierarchy:

- PrototypeGame
  - PrototypeGameController
- Canvas
  - LevelText
  - TutorialText
  - ShelterHpText
  - NextTileText
  - ResultText
  - WalletText
- BoardRoot
  - 36 prototype cell buttons or sprites

Attach:

- `PrototypeGameController` to `PrototypeGame`
- `PrototypeHudView` to a HUD object under `Canvas`

Wire the HUD text fields in the inspector.

## Temporary Input Method
Until the board visual controller is implemented, test placement by adding UI buttons or debug calls that invoke:

`PrototypeGameController.TryPlaceNextTile(x, y)`

Then add a button to call:

`PrototypeGameController.StartWave()`

## Next Engineering Step
Implement a `PrototypeBoardView` MonoBehaviour that:

- Creates 36 cell buttons at runtime
- Maps each button to board coordinates
- Calls `TryPlaceNextTile(x, y)`
- Updates visible tile labels after placement and merge

## Important Note
The current scripts are prototype-first. They are intentionally simple and should be refactored after the core loop is validated.
