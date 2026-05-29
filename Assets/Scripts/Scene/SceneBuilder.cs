using UnityEngine;

/// <summary>
/// Builds the entire assignment scene procedurally at Start():
///   - 5 obstacles (2 with children, 3 concave)
///   - A shooter object with raycast reflection
///   - Ground, lighting, and skybox color
///
/// To use:
///   1. Create an empty GameObject in your scene named "SceneBuilder".
///   2. Attach this script to it.
///   3. Press Play.
/// </summary>
public class SceneBuilder : MonoBehaviour
{
    [Header("Layout")]
    public float xSpacing = 4f;
    public float yOffset = 0f;

    [Header("Build")]
    public bool buildOnStart = true;
    public bool createShooter = true;
    public bool createGround = true;
    public bool createLighting = true;

    private Shader litShader;

    void Start()
    {
        // Try URP first (most common in Unity 6 new projects), then fall back to Standard.
        litShader = Shader.Find("Universal Render Pipeline/Lit");
        if (litShader == null) litShader = Shader.Find("Standard");
        if (litShader == null) litShader = Shader.Find("Diffuse"); // very old fallback

        if (buildOnStart) BuildScene();
    }

    public void BuildScene()
    {
        // Clear any previous children
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        if (createLighting) SetupLighting();
        if (createGround)   CreateGroundObject();

        BuildObstacle1_PyramidWithCubes();
        BuildObstacle2_SphereWithMoons();
        BuildObstacle3_Torus();
        BuildObstacle4_LShape();
        BuildObstacle5_Star();

        // Colored point lights — one per obstacle for atmosphere
        AddPointLight(new Vector3(-xSpacing * 2f, 3f, 0f), new Color(1f, 0.5f, 0.1f), 6f, 1.8f); // Pyramid – orange
        AddPointLight(new Vector3(-xSpacing,       3f, 0f), new Color(1f, 0.9f, 0.2f), 6f, 1.8f); // Sphere  – gold
        AddPointLight(new Vector3(0f,              4f, 0f), new Color(0.2f, 0.9f, 1f), 6f, 2.0f); // Torus   – cyan
        AddPointLight(new Vector3(xSpacing,        3f, 0f), new Color(0.3f, 1f, 0.4f), 6f, 1.8f); // L-shape – green
        AddPointLight(new Vector3(xSpacing * 2f,   4f, 0f), new Color(1f, 0.9f, 0.1f), 6f, 2.0f); // Star    – yellow

        if (createShooter)  CreateShooterObject();
    }

    // ----------------------------------------------------------------------
    // OBSTACLE 1: Pyramid (convex) with 3 cube children orbiting slantly
    // ----------------------------------------------------------------------
    void BuildObstacle1_PyramidWithCubes()
    {
        GameObject parent = CreateMeshObject(
            "Obstacle1_Pyramid",
            ProceduralShapes.CreatePyramid(2f, 3f),
            ProceduralTextures.CreateCheckerboard(256, 8,
                new Color(0.9f, 0.45f, 0.2f), new Color(0.25f, 0.1f, 0.05f))
        );
        parent.transform.SetParent(transform);
        parent.transform.position = new Vector3(-xSpacing * 2f, yOffset, 0f);
        parent.AddComponent<MeshCollider>();

        for (int i = 0; i < 3; i++)
        {
            GameObject child = CreateMeshObject(
                $"Pyramid_Cube_Child_{i}",
                ProceduralShapes.CreateCube(0.6f),
                ProceduralTextures.CreateStripes(128, 6, Color.white, new Color(0.1f, 0.7f, 0.9f))
            );
            child.transform.SetParent(parent.transform);
            float ang = i * Mathf.PI * 2f / 3f;
            child.transform.localPosition = new Vector3(Mathf.Cos(ang) * 2f, 1.5f, Mathf.Sin(ang) * 2f);
            child.AddComponent<BoxCollider>();

            var orbit = child.AddComponent<SlantedOrbit>();
            orbit.target = parent.transform;
            orbit.orbitSpeed = 30f + i * 10f;
            orbit.orbitAxis = new Vector3(1f, 1f, 0.3f);   // slanted
            orbit.selfRotateSpeed = 80f;

            // Trail renderer — cyan glow streak
            AddTrail(child, new Color(0.2f, 0.9f, 1f), new Color(0.2f, 0.9f, 1f, 0f), 0.6f, 0.07f);
        }
    }

    // ----------------------------------------------------------------------
    // OBSTACLE 2: Sphere (convex) with 4 small sphere children orbiting slantly
    // ----------------------------------------------------------------------
    void BuildObstacle2_SphereWithMoons()
    {
        GameObject parent = CreateMeshObject(
            "Obstacle2_Sphere",
            ProceduralShapes.CreateSphere(1f, 32, 24),
            ProceduralTextures.CreateRadial(256,
                new Color(1f, 0.85f, 0.25f), new Color(0.55f, 0.15f, 0.55f))
        );
        parent.transform.SetParent(transform);
        parent.transform.position = new Vector3(-xSpacing, yOffset + 0.5f, 0f);
        parent.AddComponent<SphereCollider>();

        for (int i = 0; i < 4; i++)
        {
            GameObject child = CreateMeshObject(
                $"Sphere_Moon_Child_{i}",
                ProceduralShapes.CreateSphere(0.25f, 16, 12),
                ProceduralTextures.CreateNoise(128, 8f,
                    new Color(0.3f, 0.3f, 0.35f), new Color(0.9f, 0.9f, 0.95f))
            );
            child.transform.SetParent(parent.transform);
            float ang = i * Mathf.PI * 2f / 4f;
            child.transform.localPosition = new Vector3(Mathf.Cos(ang) * 2f, 0f, Mathf.Sin(ang) * 2f);
            child.AddComponent<SphereCollider>();

            var orbit = child.AddComponent<SlantedOrbit>();
            orbit.target = parent.transform;
            orbit.orbitSpeed = 45f + i * 12f;
            // Each moon gets a slightly different slanted axis
            orbit.orbitAxis = new Vector3(0.5f + 0.1f * i, 1f, 0.3f - 0.05f * i);
            orbit.selfRotateSpeed = 100f;

            // Trail renderer — warm gold/orange streak
            AddTrail(child, new Color(1f, 0.8f, 0.1f), new Color(1f, 0.4f, 0f, 0f), 0.5f, 0.05f);
        }
    }

    // ----------------------------------------------------------------------
    // OBSTACLE 3: Torus (CONCAVE — has a hole through it)
    // ----------------------------------------------------------------------
    void BuildObstacle3_Torus()
    {
        GameObject obj = CreateMeshObject(
            "Obstacle3_Torus_Concave",
            ProceduralShapes.CreateTorus(1.2f, 0.4f, 48, 24),
            ProceduralTextures.CreateGradient(256,
                new Color(0.2f, 0.85f, 1f), new Color(0.85f, 0.25f, 0.85f))
        );
        obj.transform.SetParent(transform);
        obj.transform.position = new Vector3(0f, yOffset + 1f, 0f);
        obj.transform.rotation = Quaternion.Euler(45f, 0f, 30f);

        // Concave torus needs a non-convex MeshCollider
        var mc = obj.AddComponent<MeshCollider>();
        mc.convex = false;
    }

    // ----------------------------------------------------------------------
    // OBSTACLE 4: L-shape (CONCAVE)
    // ----------------------------------------------------------------------
    void BuildObstacle4_LShape()
    {
        GameObject obj = CreateMeshObject(
            "Obstacle4_LShape_Concave",
            ProceduralShapes.CreateLShape(2f, 0.6f, 0.8f),
            ProceduralTextures.CreateCheckerboard(256, 4,
                new Color(0.35f, 0.7f, 0.35f), new Color(0.1f, 0.25f, 0.1f))
        );
        obj.transform.SetParent(transform);
        obj.transform.position = new Vector3(xSpacing, yOffset + 0.2f, 0f);

        var mc = obj.AddComponent<MeshCollider>();
        mc.convex = false;
    }

    // ----------------------------------------------------------------------
    // OBSTACLE 5: Star (CONCAVE — inward points between outer points)
    // ----------------------------------------------------------------------
    void BuildObstacle5_Star()
    {
        GameObject obj = CreateMeshObject(
            "Obstacle5_Star_Concave",
            ProceduralShapes.CreateStar(5, 1.2f, 0.5f, 0.4f),
            ProceduralTextures.CreateRadial(256,
                new Color(1f, 0.95f, 0.2f), new Color(0.85f, 0.4f, 0f))
        );
        obj.transform.SetParent(transform);
        obj.transform.position = new Vector3(xSpacing * 2f, yOffset + 1f, 0f);

        var mc = obj.AddComponent<MeshCollider>();
        mc.convex = false;
    }

    // ----------------------------------------------------------------------
    // SHOOTER OBJECT (procedural: cube body + pyramid barrel)
    // ----------------------------------------------------------------------
    void CreateShooterObject()
    {
        GameObject shooter = new GameObject("Shooter");
        shooter.transform.SetParent(transform);
        shooter.transform.position = new Vector3(0f, yOffset + 1f, -7f);
        shooter.transform.rotation = Quaternion.identity;

        // Body
        GameObject body = CreateMeshObject(
            "Shooter_Body",
            ProceduralShapes.CreateCube(0.7f),
            ProceduralTextures.CreateStripes(128, 4,
                new Color(0.25f, 0.25f, 0.32f), new Color(0.1f, 0.1f, 0.15f))
        );
        body.transform.SetParent(shooter.transform);
        body.transform.localPosition = Vector3.zero;

        // Barrel (pyramid pointing forward)
        GameObject barrel = CreateMeshObject(
            "Shooter_Barrel",
            ProceduralShapes.CreatePyramid(0.35f, 1f),
            ProceduralTextures.CreateGradient(128, new Color(0.9f, 0.1f, 0.1f), new Color(0.4f, 0f, 0f))
        );
        barrel.transform.SetParent(shooter.transform);
        barrel.transform.localPosition = new Vector3(0f, 0f, 0.5f);
        // Pyramid normally points +Y, rotate so it points +Z (forward)
        barrel.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        // LineRenderer + RaycastShooter live on the shooter root
        var lr = shooter.AddComponent<LineRenderer>();
        lr.positionCount = 0;

        var caster = shooter.AddComponent<RaycastShooter>();
        caster.continuous = true;
        caster.maxReflections = 6;
        caster.maxDistance = 60f;
        // Slow auto-rotation makes the reflections visually interesting in demo
        caster.autoRotate = true;
        caster.autoRotateSpeed = new Vector3(0f, 20f, 0f);
    }

    // ----------------------------------------------------------------------
    // GROUND, LIGHTING, DESIGN
    // ----------------------------------------------------------------------
    void CreateGroundObject()
    {
        GameObject ground = CreateMeshObject(
            "Ground",
            ProceduralShapes.CreateCube(1f),
            ProceduralTextures.CreateCheckerboard(512, 16,
                new Color(0.32f, 0.32f, 0.36f), new Color(0.2f, 0.2f, 0.24f))
        );
        ground.transform.SetParent(transform);
        ground.transform.localScale = new Vector3(40f, 0.2f, 40f);
        ground.transform.position = new Vector3(0f, yOffset - 2f, 0f);
        ground.AddComponent<BoxCollider>();
    }

    void SetupLighting()
    {
        // Solid colour skybox (low-key blue/violet)
        Camera.main?.gameObject.GetComponent<Camera>(); // no-op safety
        RenderSettings.skybox = null;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor    = new Color(0.45f, 0.55f, 0.75f);
        RenderSettings.ambientEquatorColor = new Color(0.30f, 0.30f, 0.40f);
        RenderSettings.ambientGroundColor  = new Color(0.10f, 0.10f, 0.15f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.10f, 0.12f, 0.18f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 20f;
        RenderSettings.fogEndDistance = 60f;

        if (Camera.main != null)
            Camera.main.backgroundColor = new Color(0.10f, 0.12f, 0.18f);

        // Key light
        GameObject sunGO = new GameObject("KeyLight");
        sunGO.transform.SetParent(transform);
        Light sun = sunGO.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        sun.intensity = 1.0f;
        sun.color = new Color(1f, 0.96f, 0.85f);

        // Fill light (cool tint, low intensity)
        GameObject fillGO = new GameObject("FillLight");
        fillGO.transform.SetParent(transform);
        Light fill = fillGO.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.transform.rotation = Quaternion.Euler(-30f, 150f, 0f);
        fill.intensity = 0.4f;
        fill.color = new Color(0.6f, 0.75f, 1f);
    }

    // ----------------------------------------------------------------------
    // HELPER: add a glowing TrailRenderer to a child object
    // ----------------------------------------------------------------------
    void AddTrail(GameObject go, Color startCol, Color endCol, float time, float width)
    {
        TrailRenderer trail = go.AddComponent<TrailRenderer>();
        trail.time       = time;
        trail.startWidth = width;
        trail.endWidth   = 0f;
        trail.minVertexDistance = 0.05f;
        trail.generateLightingData = true;

        Shader sh = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                 ?? Shader.Find("Sprites/Default")
                 ?? Shader.Find("Unlit/Color");

        Material mat = new Material(sh);
        trail.material   = mat;
        trail.startColor = startCol;
        trail.endColor   = endCol;
    }

    // ----------------------------------------------------------------------
    // HELPER: add a coloured point light to the scene
    // ----------------------------------------------------------------------
    void AddPointLight(Vector3 position, Color color, float range, float intensity)
    {
        GameObject go = new GameObject("PointLight");
        go.transform.SetParent(transform);
        go.transform.position = position;
        Light light = go.AddComponent<Light>();
        light.type      = LightType.Point;
        light.color     = color;
        light.range     = range;
        light.intensity = intensity;
    }

    // ----------------------------------------------------------------------
    // HELPER: build a GameObject with mesh + textured material
    // ----------------------------------------------------------------------
    GameObject CreateMeshObject(string name, Mesh mesh, Texture2D tex)
    {
        GameObject go = new GameObject(name);
        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        mf.sharedMesh = mesh;

        Material mat = new Material(litShader);
        // Cover both URP and Built-in property names
        if (mat.HasProperty("_BaseMap"))  mat.SetTexture("_BaseMap", tex);
        if (mat.HasProperty("_MainTex"))  mat.SetTexture("_MainTex", tex);
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", Color.white);
        if (mat.HasProperty("_Color"))     mat.SetColor("_Color", Color.white);
        mr.sharedMaterial = mat;
        return go;
    }
}
