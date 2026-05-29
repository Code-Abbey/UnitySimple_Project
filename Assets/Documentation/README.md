# Engine-based Cross Reality Development — Assignment 2

**Student:** Abiodun Adefusi  
**Student ID:** cc241033  
**Unity Version:** 6.3 LTS (6000.3.6f1)

---

## How to Run the Project

### Prerequisites
- Unity 6.3 LTS (6000.3.6f1) installed via Unity Hub
- Universal Render Pipeline (URP) project template (the scripts auto-detect URP or Built-in)

### Step 1 — Open the Project
1. Open **Unity Hub**
2. Click **Open → Add project from disk**
3. Navigate to the project folder and select it
4. Unity will import and compile all assets automatically — wait for the progress bar to finish

### Step 2 — Open the Scene
1. In the **Project window**, go to `Assets → Scenes`
2. Double-click **SampleScene** (or your main scene) to open it

### Step 3 — Set Up the Scene (first time only)
If the Hierarchy is empty or missing the SceneBuilder:
1. **GameObject → Create Empty** → rename it `SceneBuilder`
2. In the **Inspector**, click **Add Component** → search `SceneBuilder` → add it
3. Select the **Main Camera** in the Hierarchy
4. Set its **Position** to `X: 0, Y: 5, Z: -15` and **Rotation** to `X: 15, Y: 0, Z: 0`
5. Add Component → `CameraController`

### Step 4 — Press Play ▶
Everything is generated procedurally at runtime. The full scene — obstacles, shooter, lighting, ground — builds itself when you press Play.

---

## Controls

| Input | Action |
|-------|--------|
| W / A / S / D | Move camera forward / left / back / right |
| Q / E | Move camera down / up |
| Mouse | Look around |
| Escape | Unlock / re-lock cursor |

> The Shooter Object rotates automatically and fires a reflecting raycast continuously — no input needed to activate it.

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── MeshGeneration/
│   │   └── ProceduralShapes.cs
│   ├── Textures/
│   │   └── ProceduralTextures.cs
│   ├── Behaviours/
│   │   ├── SlantedOrbit.cs
│   │   └── CameraController.cs
│   ├── Raycasting/
│   │   └── RaycastShooter.cs
│   └── Scene/
│       └── SceneBuilder.cs
├── Documentation/
│   ├── README.md
│   └── report.md
└── Scenes/
```

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| Everything is pink/magenta | Shader not found. Go to **Edit → Project Settings → Graphics** and make sure a URP asset is assigned |
| Controls not working | Confirm `CameraController` is on the Main Camera and the project uses the **Input System Package** (not Legacy Input) |
| Scene is empty on Play | Make sure `SceneBuilder` component is attached to an empty GameObject in the Hierarchy |
| Ray passes through the torus hole | That is correct — the hole is empty space. Aim at the solid ring |
