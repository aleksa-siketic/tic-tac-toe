# Tic-Tac-Toe

A polished Tic-Tac-Toe game built in Unity 6. Two-player local pass-and-play with selectable visual themes, persistent statistics, and full audio.

## Play

[Play in browser](TODO) — itch.io WebGL build

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

This separation keeps data layer (settings, stats, themes) decoupled from gameplay state. The persistent managers are configured once in `PlayScene` and survive scene transitions; `GameManager` is freshly created each time `GameScene` loads.

### Event-driven UI

`GameManager` broadcasts game events (`OnGameStarted`, `OnMarkPlaced`, `OnGameEnded`) via C# events. UI components (`BoardCell`, `StrikeLine`, `GameOverPopup`, `GameSceneUI`) subscribe to react. This keeps the game logic free of UI concerns and lets UI components be added or replaced without touching the core game.

### Popup system

All popups inherit from a `PopupBase` abstract class that handles:

- Show/hide via a `Content` GameObject toggle
- Optional open delay (used to space audio cues, and to show the winning strike before the game-over popup)
- Pop sound on open
- `OnOpened` / `OnClosed` virtual hooks for subclasses

### Theme system

Themes are configured as a serialized array on `ThemeManager`. Each theme references X and O sprites. `BoardCell` queries `ThemeManager.CurrentTheme` for the appropriate sprite when rendering a mark and subscribes to `OnThemeChanged` to react to runtime theme switches.

### Orientation handling

A custom `OrientationManager` script attached to each Canvas dynamically adjusts the `CanvasScaler`'s match value based on aspect ratio. In portrait it matches by width; in landscape it matches by height. This keeps UI sized appropriately to whichever screen dimension is more constrained.

## Project Structure

Assets/_Project/
Scenes/
PlayScene.unity
GameScene.unity
Scripts/
GameManager.cs           Game logic and events
BoardCell.cs             Cell click handling and visual state
StrikeLine.cs            Winning line indicator
PopupBase.cs             Abstract base for all popups
GameOverPopup.cs
SettingsPopup.cs
StatsPopup.cs
ThemePopup.cs
ExitConfirmPopup.cs
SettingsManager.cs       Persistent: music/SFX toggles
StatsManager.cs          Persistent: match statistics
ThemeManager.cs          Persistent: theme selection
AudioManager.cs          Persistent: BGM and SFX playback
PlaySceneUI.cs           Main menu wiring
GameSceneUI.cs           HUD wiring
ButtonClickSound.cs      Helper: plays click sound on Button.onClick
OrientationManager.cs    Helper: adjusts CanvasScaler for orientation
TimeFormatter.cs         Static utility: formats seconds as MM:SS
Sprites/                   Theme sprites, popup background, button sprites
Audio/                     Music and SFX clips

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
