# UnitySimple_Project

Unity coursework project for **Engine-based Cross Reality Development**. The scene is assembled entirely at runtime using C# and demonstrates procedural mesh generation, procedural texturing, slanted orbital motion, and raycasting with reflections in Unity 6.

## Project Summary

This project builds a complete 3D scene from code when Play mode starts. It includes:

- five procedural obstacles, including convex and concave meshes
- runtime-generated textures with no external texture assets
- orbiting child objects moving on tilted axes
- a rotating shooter that casts a reflecting ray across the scene
- custom lighting, fog, trails, and hit markers for presentation

## Implemented Features

### Procedural Geometry

The scene includes five main obstacles created in code:

- Pyramid
- Sphere
- Torus
- L-shape
- Star

These are generated in `ProceduralShapes.cs` by manually defining mesh data such as vertices, triangles, and UVs.

### Procedural Textures

Textures are generated at runtime in `ProceduralTextures.cs`. Implemented texture types include:

- checkerboard
- linear gradient
- radial gradient
- stripes
- noise

### Motion and Interaction

- Pyramid children orbit their parent on a slanted axis
- Sphere children orbit with varied axes and speeds
- A shooter object rotates continuously and casts a reflective ray
- Bounce points are visualized with markers and a `LineRenderer`

### Scene Design

The scene also includes:

- directional and point lighting
- fog for depth
- trail renderers on orbiting objects
- a generated ground plane

## Tech Stack

- Unity 6.3 LTS (`6000.3.6f1`)
- C#
- Universal Render Pipeline support, with fallback-friendly material setup

## Repository Structure

```text
Assets/
├── Documentation/
├── Scenes/
├── Scripts/
│   ├── Behaviours/
│   ├── MeshGeneration/
│   ├── Raycasting/
│   ├── Scene/
│   └── Textures/
Packages/
ProjectSettings/
demo.mp4
```

## Scripts

- `Assets/Scripts/MeshGeneration/ProceduralShapes.cs` builds the procedural meshes
- `Assets/Scripts/Textures/ProceduralTextures.cs` generates runtime textures
- `Assets/Scripts/Scene/SceneBuilder.cs` assembles the scene at startup
- `Assets/Scripts/Behaviours/SlantedOrbit.cs` handles tilted orbital motion
- `Assets/Scripts/Raycasting/RaycastShooter.cs` handles reflective raycasting
- `Assets/Scripts/Behaviours/CameraController.cs` handles movement and look controls

## How To Run

1. Open the project in Unity Hub using Unity `6000.3.6f1`.
2. Open `Assets/Scenes/SampleScene`.
3. Press Play.

The scene content is generated at runtime.

## Controls

- `W/A/S/D` move the camera
- `Q/E` move down and up
- `Mouse` looks around
- `Escape` unlocks or re-locks the cursor

## Documentation

More detailed assignment documentation is included here:

- `Assets/Documentation/README.md`
- `Assets/Documentation/report.md`

## Demo

The repository includes `demo.mp4`, showing the project in motion.

## Author

- **Abiodun Adefusi**
- **Student ID:** `cc241033`
