# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

**Build C# scripts:**
```
dotnet build
```
Always run after any C# file change. Godot picks up the compiled assembly on next editor focus or run.

**Run the project:**
Open in Godot 4.7 editor and press F5, or use `godot --path .` from the project root.

## Architecture

**Engine & language:** Godot 4.7, C# (.NET 8), Jolt Physics 3D, Forward Plus renderer.

**C# namespace layout:**
- `RPGFramework.Core` — reusable Godot node extensions (`TPSSpringArm3D`, `RotateDirection`)
- `RPGFramework.Entitys` — character/entity classes (`Pawn`)
- `RPGFramework.Resources` — `[GlobalClass]` Resource subclasses (`PawnStats`)

All runtime C# files live under `content/source/`. Every public node class uses `[GlobalClass]` to expose it in the Godot editor.

**Key classes:**
- `Pawn` (`CharacterBody3D`) — player controller; delegates movement parameters to an attached `PawnStats` resource. Movement direction is derived from `SpringArm.RotationDegrees.Y` so the character moves relative to the camera.
- `TPSSpringArm3D` (`SpringArm3D`) — third-person camera. Captures mouse input to drive pitch/yaw targets; optionally smooths them via exponential lerp each frame. ESC toggles mouse capture.
- `PawnStats` (`Resource`) — data-only resource holding speed, acceleration, friction, jump force, gravity scale, and push force. Attach one per Pawn in the inspector.

**Scene layout (`content/scenes/`):**
- `application/execute/execute.tscn` — main scene (project entry point)
- `entitys/player/pawn.tscn` — player Pawn scene
- `maps/dev_map.tscn` — dev sandbox; instantiates the player pawn and provides a flat ground plane with loose `RigidBody3D` props for testing push behaviour

**Input actions (defined in `project.godot`):**
`move_forward`, `move_back`, `move_left`, `move_right`, `jump`, `run`, `attack` (LMB), `block` (RMB), `dodge`

**Physics collision layers:**
- Layer 1 `world` — static environment
- Layer 2 `hurtbox` — areas that can receive damage
- Layer 3 `hitbox` — areas that deal damage

**AI:** LimboAI behaviour trees; task scripts go under `content/source/ai/tasks/` (actions / conditions sub-folders).

**Assets:**
- Kenney prototype textures under `assets/`
- Quaternius CC0 IK-rigged humanoid character addon (see `README.md` for scene structure and animation list)
- `addons/ultimate_placer/` — editor plugin for scene object placement
