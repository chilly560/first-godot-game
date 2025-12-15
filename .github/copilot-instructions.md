## Purpose
This repository is a Godot 4 + C# (Mono/.NET 8) game. These instructions give AI coding agents immediate, actionable knowledge to be productive: how the project is built/run, the main architectural patterns, and where to find integration points.

## Quick build & run
- Build C# assemblies: `dotnet build "New Game Project.csproj"` (Task: `dotnet: build` is available in VS Code tasks).
- The game is a Godot project; to run/play use the Godot editor (Mono-enabled) or the Godot CLI that knows about the project.godot file.
- Project targets `net8.0` and uses `Godot.NET.Sdk/4.4.1` (see [New Game Project.csproj](New Game Project.csproj)).

## High-level architecture
- Godot scenes under `scenes/` compose the runtime nodes and UI. C# scripts live in `scripts/` and are compiled into the assembly.
- `GameData` acts as a global Event-BUS / singleton and is frequently accessed via `GetNode<...>("%GameData")` (autoload name `GameData`). See [scripts/GameData.cs](scripts/GameData.cs).
- Weapon subsystem: `WeaponScene` (a Marker2D child under player/enemy nodes) mediates weapons via `IWeapon` and a `WeaponFactory`. See [scripts/WeaponScene.cs](scripts/WeaponScene.cs) and `dotnet_objects/Game/` interfaces.
- Entities: `Player`, `Enemy`, `Bullet` are typical node+script pairs under `scripts/` and `scenes/`. They use Godot signals and `_PhysicsProcess/_Process` for update loops. Examples: [scripts/Player.cs](scripts/Player.cs), [scripts/Bullet.cs](scripts/Bullet.cs).

## Project-specific conventions & patterns
- Autoload & Event Bus: `GameData` is used as the central event bus. Components emit or subscribe to signals through `EmitSignal(SignalName.X)` and by calling `gameData.UpdateAmmoLabel += Handler;` in `_Ready()`.
- Node access: Code relies heavily on explicit node paths (relative and absolute). Examples: `GetNode<WeaponScene>("AnimatedSprite2D/WeaponScene")`, `GetNode<GameData>("%GameData")`, `GetNode<Player>("../Player")`. Maintain exact node names when refactoring scenes.
- Naming & layout: C# classes use PascalCase and files match class names in `scripts/`. Godot scene files (`.tscn`) reference scripts via UID metadata (paired `.uid` files exist next to `.cs` files).
- Physics & timers: Movement and collisions use `_PhysicsProcess(double delta)`, `MoveAndSlide()`, `Area2D` signals (`OnAreaEnteredBullet`, `OnBodyEnteredBullet`), and `Timer` nodes for time-based behavior.
- Weapon flow: `WeaponScene` holds `currentWeapon` and `secondaryWeapon`. Secondary ammo updates propagate through GameData signals to HUD labels. Adding weapons typically uses `Collect(ICollectable)` from player code.

## Integration & external dependencies
- .NET SDK: uses Godot.NET.Sdk and .NET 8. Build inside the project root so the Godot assembly metadata is generated correctly.
- Godot editor: scenes and autoloads are configured in `project.godot` — prefer using the Godot editor to modify autoloads or node paths to avoid corrupting the file.

## Typical developer workflows
- Add new scripts: create `scripts/MyClass.cs` with `public partial class MyClass : NodeType` and add the script to the scene via the Godot editor so UID links are created.
- Run tests / quick iteration: compile with `dotnet build` when changing C# code, then open/run the project in Godot for scene-level testing.
- Debugging: use Godot's debugger for runtime inspection (breakpoints in Mono-enabled Godot are supported if configured). For build errors, inspect `dotnet build` output.

## Files to inspect first (high signal-to-noise)
- [project.godot](project.godot) — project config, main scene, input map, dotnet settings
- [New Game Project.csproj](New Game Project.csproj) — SDK and target framework
- [scripts/GameData.cs](scripts/GameData.cs) — event-bus/autoload pattern
- [scripts/WeaponScene.cs](scripts/WeaponScene.cs) — weapon API and HUD integration
- [scripts/Player.cs](scripts/Player.cs) — player input, health, collectable handling
- [scripts/Bullet.cs](scripts/Bullet.cs) — projectile lifecycle & collision hooks

## Practical guidance for code edits
- When changing node paths or scene structure, update every `GetNode` usage that references those paths. Grep for `GetNode<` to find dependent code.
- Preserve signal names and signatures; many HUD updates and game logic depend on `UpdateAmmoLabel` and `UpdateScoreLabel` signals.
- Prefer modifying behavior by editing C# scripts and recompiling (`dotnet build`) rather than hand-editing `.tscn` text, unless you know the scene file format.

If anything here is unclear or you want me to expand a section (for example: autoload setup, weapon interface docs, or a short contributor workflow), tell me which bit to iterate on.
