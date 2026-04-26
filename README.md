# Tic-Tac-Toe

A polished Tic-Tac-Toe game built in Unity 6. Two-player local pass-and-play with selectable visual themes, persistent statistics, and full audio.

## Play

[Play in browser](https://aleksa-siketic.github.io/tic-tac-toe/)

> Note: Background music starts after the first user interaction due to browser autoplay policies. This is standard behavior for web games.

## Features

- Two-player local Tic-Tac-Toe (3×3, alternating turns)
- Win detection across rows, columns, and diagonals with animated strike line
- Match timer and per-player move counters
- Three selectable visual themes for X and O sprites
- Persistent statistics (games played, wins per player, draws, average match duration)
- Background music and sound effects with independent toggles
- Settings persist across sessions via PlayerPrefs
- Responsive UI supporting both portrait and landscape orientations
- Built for WebGL with mobile-first design

## Architecture

### Scene structure

The game uses two scenes:

- **PlayScene** — main menu with Play / Stats / Settings / Exit options. Hosts the persistent singletons.
- **GameScene** — the actual gameplay with board, HUD, and game-over flow.

### Manager pattern

Singletons are split by lifetime:

- **Persistent** (live across all scenes via `DontDestroyOnLoad`):
  - `SettingsManager` — music/SFX preferences with PlayerPrefs persistence
  - `StatsManager` — match statistics with PlayerPrefs persistence
  - `ThemeManager` — selected visual theme with PlayerPrefs persistence
  - `AudioManager` — BGM and SFX playback, reads from `SettingsManager`
- **Scene-scoped** (live only in `GameScene`):
  - `GameManager` — board state, turn logic, win detection, match events

This separation keeps the data layer (settings, stats, themes) decoupled from gameplay state. The persistent managers are configured once in `PlayScene` and survive scene transitions; `GameManager` is freshly created each time `GameScene` loads.

### Event-driven UI

`GameManager` broadcasts game events (`OnGameStarted`, `OnMarkPlaced`, `OnGameEnded`) via C# events. UI components (`BoardCell`, `StrikeLine`, `GameOverPopup`, `GameSceneUI`) subscribe to react. This keeps the game logic free of UI concerns and lets UI components be added or replaced without touching the core game.

### Popup system

All popups inherit from a `PopupBase` abstract class that handles:

- Show/hide via a `Content` GameObject toggle
- Optional open delay (used to space audio cues, and to show the winning strike before the game-over popup)
- Pop sound on open
- Scale-in animation with subtle overshoot
- `OnOpened` / `OnClosed` virtual hooks for subclasses

### Theme system

Themes are configured as a serialized array on `ThemeManager`. Each theme references X and O sprites. `BoardCell` queries `ThemeManager.CurrentTheme` for the appropriate sprite when rendering a mark and subscribes to `OnThemeChanged` to react to runtime theme switches.

### Orientation handling

A custom `OrientationManager` script attached to each Canvas dynamically adjusts the `CanvasScaler`'s match value based on aspect ratio. In portrait it matches by width; in landscape it matches by height. This keeps UI sized appropriately to whichever screen dimension is more constrained.

### Win celebration

When a game ends in a win, the strike line animates drawing across the winning cells, followed by a particle burst once the strike completes. The game-over popup then appears after a short delay so the player has time to see what happened.

## Tech Stack

- Unity 6.4 (6000.4.3f1)
- C# / .NET Standard 2.1
- TextMeshPro for text rendering
- Universal Render Pipeline (2D)

## Running Locally

Clone and open in Unity 6.4 or later. Open `Assets/_Project/Scenes/PlayScene.unity` and press Play.

To produce a WebGL build:

1. Open `File → Build Profiles`
2. Select the Web profile (Mobile / Release)
3. Click `Build` and choose an output folder

## Credits

UI sprites and audio assets provided as part of the project brief.