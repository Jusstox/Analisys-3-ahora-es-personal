# 3D Game Analytics & Visualization Tool

An integrated analytics pipeline for the *3D Game Kit Lite* that captures gameplay telemetry and renders it as **interactive 3D spatial visualizations** directly within the Unity Editor.

## Overview

This tool bridges the gap between raw data and level design. Instead of analyzing spreadsheets, designers can view player behavior (paths, deaths, item interactions) overlaid on the actual game geometry. The system uses a **normalized MySQL database** for storage and a custom **Unity Editor Dashboard** for visualization.

### Key Features
* **3D Volumetric Heatmaps:** Visualizes event density (e.g., death clusters) using voxel-based rendering to preserve vertical context.
* **Hybrid Data Sampling:** Captures high-fidelity movement paths using spatial delta checks (0.1m threshold) to minimize database bloat.
* **Interactive Filtering:** Filter runs by **Outcome** (Win/Loss), **State** (Restart/Normal), or **Playstyle** (Pacifist/Speedrun).
* **Event Layering:** Toggle visibility for 10+ distinct event types, including Deaths, Kills, Jumps, Keys, and Breakables.

---

## Project Structure

This project is divided into two main scenes found in `Assets/Scenes/`:

### 1. `GameplayScene` (Data Collection)
* **Purpose:** The playable level where data is recorded.
* **How it works:**
    * Play the game normally.
    * The `AnalyticsManager` automatically starts a session and records your movement, combat, and interactions.
    * Data is sent to the server in real-time via PHP endpoints.

### 2. `Analytics_Visualization` (Data Inspection)
* **Purpose:** The empty scene containing the **Visualization Manager** tool.
* **How it works:**
    * Open this scene to view the Dashboard.
    * Click **"Download & Parse Data"** to fetch the latest telemetry from the SQL server.
    * Use the Inspector controls to generate heatmaps and paths.

---

## How to Use the Editor Dashboard

Open the `Analytics_Visualization` scene and select the **`VizManager`** object in the hierarchy. The custom inspector provides the following controls:

### 1. Visualization Modes
* **Exact Points:** Draws precise spheres/wireframes where events occurred. Good for tracking specific item locations.
* **Heatmap:** Aggregates data into 3D Voxels.
    * **Grid Resolution:** Slider (1m - 20m) to adjust the granularity of the heatmap.
    * **Density Opacity:** Slider to adjust transparency based on event frequency.

### 2. Data Filters
Use these dropdowns to slice the dataset:
* **Win Status:** Compare paths of winners vs. losers.
* **Restarts:** Isolate fresh runs vs. retries.
* **Exclusions:** Quickly find "Pacifist" (No Kills) or "Perfect" (No Damage) runs.

### 3. Event Layers
Toggle specific data types on/off to reduce clutter. Each of the **10 layers** has a **customizable color picker**:

**Movement & Progression**
* **Path:** Trajectory of player movement.
* **Jumps:** Locations where the jump input was pressed.
* **Checkpoints:** Locations where players triggered a save point.

**Combat**
* **Deaths:** Locations where the player died (Solid Spheres / Red Voxels).
* **Kills:** Locations where an enemy was defeated.
* **Player Hits:** Locations where the player took non-lethal damage (Wireframe Spheres).

**Economy & Interaction**
* **Keys:** Key pickup locations.
* **Boxes:** Breakable crate locations.
* **Buttons:** Floor switch interactions.
* **Heals:** Health cube pickups.

### 4. Scene Control
* **Hide Level Geometry:** A button at the bottom allows you to toggle the visibility of the level meshes (`ExampleLevel`) to see the raw data cloud clearly without obstruction.

---

## Technical Architecture

### Sampling Strategy
To ensure performance and storage efficiency, we use a distributed sampling logic:
1.  **Continuous Movement:** Uses `PlayerTracker.cs`. Polled at 2Hz but restricted by a **0.1m Spatial Delta** (only records if the player moves).
2.  **Combat Events:** Uses `AnalyticsDamageListener.cs`. Event-driven (hooks into Unity's Message system) to capture exact frames of damage/death.
3.  **World Interaction:** Uses `AnalyticsEventTrigger.cs`. Observer pattern scripts attached to interactive objects (Keys, Switches).

### Backend Pipeline
1.  **Unity (C#):** Serializes data to JSON.
2.  **PHP API:** Receives POST requests, validates input, and executes Prepared SQL Statements.
3.  **MySQL:** Stores data in a relational schema with `ON DELETE CASCADE` for automatic cleanup.
4.  **Get Data:** `get_data.php` aggregates all 14 tables into a single JSON payload for the visualizer.

---

## Credits
**Delivery 3: In-Editor Visualization**
* **Core Systems:** [Marc Avante, Pol Celaya, Andrea Doña, Joan Marques and Justo Tiscornia]
* **Visualization & Editor Tool:** [Marc Avante]
* **Backend & SQL:** [Pol Celaya and Justo Tiscornia]
* **Sampling Scripts:** [Marc Avante, Andrea Doña and Joan Marques]
