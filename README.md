# Orange-Games Project Overview

This README provides context on the structure and layout of the "Orange-Games" Unity project. It is intended to assist LLMs and developers in understanding the codebase.

## Project Structure
 The project follows a standard Unity project layout with some custom organization for team collaboration.

### Root Directory
- **"Orange-Games.sln"**: Visual Studio Solution file.
- **"Assets/"**: Contains all game assets, scripts, and scenes.
- **"docs/"**: Documentation and team-specific folders.
- **"Library/", "Logs/", "Packages/", "ProjectSettings/"**: Standard Unity generated/configuration folders.

### Assets Directory ("Assets/")

#### Key Folders
- **"Scripts/"**: The core C# scripts for the game.
- **"Scenes/"**: Unity scene files (e.g., "Level2.unity", "SampleScene.unity").
- **"src/"**: Intended for source code organized by team leads (currently mostly placeholders).
- **"tst/"**: Intended for tests organized by team leads.
- **"Levels/", "Materials/", "Prefabs/", "Sprites/"**: Standard asset organization.

---

## Code Architecture

The codebase is primarily located in "Assets/Scripts/".

### Player System ("Assets/Scripts/TL_4_Player/")
The player logic is modularized into several controllers, orchestrated by a central manager.

- **"PlayerController.cs"**: The central manager. It initializes other components ("Movement", "Combat", "Health") and tracks win conditions (e.g., defeating all enemies).
- **"PlayerMovementController.cs"**: Handles physics-based movement.
- **"PlayerCombatController.cs"**: Manages attacking and combat interactions.
- **"PlayerHealth.cs"**: Manages player health and damage taking.
- **"IDamageable.cs"**: Interface for any entity that can take damage.

### Enemy System ("Assets/Scripts/")
Enemy logic is currently located in the root of the Scripts folder.

- **"EnemyController.cs"**: Handles basic enemy behavior, specifically patrolling left/right within defined offsets.
- **"EnemyShooter.cs"**: Handles enemy attack logic (shooting).
- **"EnemyHealthSimple.cs"**: Manages enemy health.

### Game Logic
- **"nextLevel.cs"**: Handles level transitions.
- **"HealthPickup.cs"**: Logic for health restoration items.

---

## Team Organization
The project appears to be structured for a team-based workflow, with specific folders allocated for different team leads (though currently "src" and "tst" folders for leads 1-6 are mostly empty).

- **TL_4 (Team Lead 4)**: Seems to be responsible for the Player System ("TL_4_Player").

## Getting Started for LLMs
When analyzing or modifying this project:
1.  **Player Logic**: Look in "Assets/Scripts/TL_4_Player/".
2.  **Enemy Logic**: Look in "Assets/Scripts/".
3.  **Scene Context**: "Level2" appears to be a primary gameplay scene.
